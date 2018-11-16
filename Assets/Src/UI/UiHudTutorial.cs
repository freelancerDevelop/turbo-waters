using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.CrossPlatformInput;
using TMPro;

public class UiHudTutorial : UiWidget
{
    public RectTransform handRect;

    public float handSpeed = 1f;
    public float handScaleX = 1f;
    public float handScaleY = 1f;

    private GameManager gameManager;
    private GameModeRunner gameRunner;

    public override void Start()
    {
        base.Start();

        this.gameManager = GameManager.Instance;
        this.gameRunner = this.gameManager.GetRunner();

        EventManager.Instance.AddListener<SceneLoadedEvent>(this.OnSceneLoaded);

        this.Hide();
    }

    public void OnDestroy()
    {
        if (EventManager.InstanceExists()) {
            EventManager.Instance.RemoveListener<SceneLoadedEvent>(this.OnSceneLoaded);
        }
    }

    public void Update()
    {
        this.handRect.anchoredPosition = Vector3.right * Mathf.Sin(Time.timeSinceLevelLoad / 2f * this.handSpeed) * this.handScaleX - Vector3.up * Mathf.Sin(Time.timeSinceLevelLoad * this.handSpeed) * this.handScaleY;

        #if MOBILE_INPUT
            float moveX = CrossPlatformInputManager.GetAxis("Horizontal");
            float moveY = CrossPlatformInputManager.GetAxis("Vertical");

            if (moveX + moveY > 0) {
                this.Hide();
            }
        #else
            if (Input.GetMouseButton(0)) {
                this.Hide();
            }
        #endif
    }

    private void OnSceneLoaded(SceneLoadedEvent e)
    {
        if (e.name != "GameScene") {
            return;
        }

        if (StorageManager.Instance.GetInt(StorageKeys.AllRounds, 0) > 0) {
            return;
        }

        this.Show();
    }
}
