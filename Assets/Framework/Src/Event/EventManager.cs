using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class EventManager : Singleton<EventManager>
{
    private Queue eventQueue = new Queue();
    private bool limitQueueProcesing = false;
    private float queueProcessTime = 0.0f;

    private Dictionary<Type, EventDelegate> delegates = new Dictionary<Type, EventDelegate>();
    private Dictionary<Delegate, EventDelegate> delegateLookup = new Dictionary<Delegate, EventDelegate>();
    private Dictionary<Delegate, Delegate> onceLookups = new Dictionary<Delegate, Delegate>();

    public delegate void EventDelegate<T>(T e) where T : GameEvent;
    public delegate void EventDelegate(GameEvent e);

    public void AddListener<T>(EventDelegate<T> del) where T : GameEvent
    {
        this.AddDelegate<T>(del);
    }

    public void AddListenerOnce<T>(EventDelegate<T> del) where T : GameEvent
    {
        EventDelegate result = this.AddDelegate<T>(del);

        if (result != null) {
            this.onceLookups[result] = del;
        }
    }

    public void RemoveListener<T>(EventDelegate<T> del) where T : GameEvent
    {
        EventDelegate internalDelegate;

        if (this.delegateLookup.TryGetValue(del, out internalDelegate)) {
            EventDelegate tempDel;

            if (this.delegates.TryGetValue(typeof(T), out tempDel)) {
                tempDel -= internalDelegate;

                if (tempDel == null){
                    this.delegates.Remove(typeof(T));
                } else {
                    this.delegates[typeof(T)] = tempDel;
                }
            }

            this.delegateLookup.Remove(del);
        }
    }

    public bool HasListener<T>(EventDelegate<T> del) where T : GameEvent
    {
        return this.delegateLookup.ContainsKey(del);
    }

    public void Trigger(GameEvent e)
    {
        EventDelegate del;

        if (this.delegates.TryGetValue(e.GetType(), out del)) {
            del.Invoke(e);

            foreach (EventDelegate k in this.delegates[e.GetType()].GetInvocationList()) {
                if (this.onceLookups.ContainsKey(k)) {
                    this.delegates[e.GetType()] -= k;

                    if (this.delegates[e.GetType()] == null) {
                        this.delegates.Remove(e.GetType());
                    }

                    this.delegateLookup.Remove(onceLookups[k]);
                    this.onceLookups.Remove(k);
                }
            }
        } else {
            Logger.MessageFormat("Event '{0}' has no listeners...", e.GetType());
        }
    }

    public bool QueueEvent(GameEvent e)
    {
        if (!delegates.ContainsKey(e.GetType())) {
            Logger.MessageFormat("QueueEvent failed due to no listeners for event: {0}", e.GetType());

            return false;
        }

        this.eventQueue.Enqueue(e);

        return true;
    }

    public void Update()
    {
        float timer = 0.0f;

        while (this.eventQueue.Count > 0) {
            if (this.limitQueueProcesing) {
                if (timer > this.queueProcessTime) {
                    return;
                }
            }

            GameEvent e = this.eventQueue.Dequeue() as GameEvent;

            this.Trigger(e);

            if (this.limitQueueProcesing) {
                timer += Time.deltaTime;
            }
        }
    }

    private EventDelegate AddDelegate<T>(EventDelegate<T> del) where T : GameEvent
    {
        if (this.delegateLookup.ContainsKey(del)) {
            return null;
        }

        EventDelegate tempDel;
        EventDelegate internalDelegate = (e) => del((T) e);

        this.delegateLookup[del] = internalDelegate;

        if (this.delegates.TryGetValue(typeof(T), out tempDel)) {
            this.delegates[typeof(T)] = tempDel += internalDelegate;
        } else {
            this.delegates[typeof(T)] = internalDelegate;
        }

        return internalDelegate;
    }
}
