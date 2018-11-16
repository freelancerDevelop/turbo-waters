using System;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    private GameMode mode = GameMode.Vanilla;
    private GameModeRunner runner = null;

    public GameMode GetMode()
    {
        return this.mode;
    }

    //public void Start() {
    //    StartRound();
    //}

    public void SetMode(GameMode mode)
    {
        this.mode = mode;
    }

    public GameModeRunner GetRunner()
    {
        return this.runner;
    }

    public void StartRound()
    {
        Logger.Message("Round started!");

        if (this.runner != null) {
            this.runner.Destroy();
        }

        if (this.mode == GameMode.Vanilla) {
            this.runner = this.gameObject.AddComponent<VanillaRunner>();
        }

        this.runner.Init();
    }

}
