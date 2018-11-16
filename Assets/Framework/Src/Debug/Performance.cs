using UnityEngine;
using UnityEngine.Profiling;
using System;
using System.Runtime.InteropServices;

public class Performance : MonoBehaviour
{
    private float deltaTime = 0;
    private float framesPerSecond = 0;
    private float nextUpdateIn = 0;
    private uint memoryUsed = 0;

    public float GetFramesPerSecond()
    {
        return 1f / this.deltaTime;
    }

    public float GetMemoryUsage()
    {
        return this.memoryUsed / 1024f / 1024f;
    }

    public void Update()
    {
        this.deltaTime += (Time.unscaledDeltaTime - this.deltaTime) * 0.1f;
        this.nextUpdateIn -= Time.deltaTime;

        if (this.nextUpdateIn <= 0) {
            this.nextUpdateIn = 1f;

            #if UNITY_WEBGL && !UNITY_EDITOR
                this.memoryUsed = GetDynamicMemorySize() + GetStaticMemorySize() + GetTotalStackSize();
            #else
                this.memoryUsed = (uint) GC.GetTotalMemory(false);
            #endif
        }
    }

    #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern uint GetTotalMemorySize();

        [DllImport("__Internal")]
        private static extern uint GetTotalStackSize();

        [DllImport("__Internal")]
        private static extern uint GetStaticMemorySize();

        [DllImport("__Internal")]
        private static extern uint GetDynamicMemorySize();
    #endif
}
