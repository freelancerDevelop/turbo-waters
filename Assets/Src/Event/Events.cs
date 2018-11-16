using UnityEngine;
using UnityEngine.Events;
using System;

[Serializable]
public class SceneLoadedEvent : GameEvent
{
    public string name;
}

[Serializable]
public class RoundTimeRemainingEvent : GameEvent
{
    public float timeRemaining;
}

[Serializable]
public class PlayerSpawnEvent : GameEvent
{
    public Player player;
}

[Serializable]
public class PlayerDeathEvent : GameEvent
{
    public Player player;
    public Player eatenBy;
    public float deathTimer;
}

[Serializable]
public class PlayerPointsUpdatedEvent : GameEvent
{
    public Player player;
    public int points;
    public float gained;
}

[Serializable]
public class PlayerKillsUpdatedEvent : GameEvent
{
    public Player player;
    public int kills;
}

[Serializable]
public class PlayerTierUpdatedEvent : GameEvent
{
    public Player player;
    public int tier;
}