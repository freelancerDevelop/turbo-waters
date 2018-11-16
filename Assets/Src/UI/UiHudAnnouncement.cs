using UnityEngine;
using TMPro;

public class UiHudAnnouncement : UiWidget
{
    public TextMeshProUGUI announcementLabelText;

    private GameManager gameManager;
    private GameModeRunner gameRunner;

    private bool isVisible = false;
    private float visibleTimer = 0;

    public override void Start()
    {
        base.Start();

        this.gameManager = GameManager.Instance;
        this.gameRunner = this.gameManager.GetRunner();

        //EventManager.Instance.AddListener<SceneLoadedEvent>(this.OnSceneLoaded);
        //EventManager.Instance.AddListener<RoundTimeRemainingEvent>(this.OnRoundTimeRemaining);
        EventManager.Instance.AddListener<PlayerDeathEvent>(this.OnPlayerDeath);
        EventManager.Instance.AddListener<PlayerTierUpdatedEvent>(this.OnPlayerTierUpdated);
    }

    public override void Show()
    {
        if (this.isVisible) {
            return;
        }

        this.isVisible = true;

        this.gameObject.SetActive(true);

        AnimationManager.Instance.Show(this.canvasGroup);

        this.canvasGroup.alpha = 0;
        this.canvasGroup.transform.localScale = Vector3.zero;

        AnimationManager.Instance.ScaleTo(this.canvasGroup, Vector3.one, 0.25f, EasingType.EaseOutBack);
        AnimationManager.Instance.FadeIn(this.canvasGroup, 0.25f, EasingType.EaseOutBack);
    }

    public override void Hide()
    {
        if (!this.isVisible) {
            return;
        }

        this.isVisible = false;

        AnimationManager.Instance.ScaleTo(this.canvasGroup, Vector3.zero, 0.25f, EasingType.EaseInBack);
        AnimationManager.Instance.FadeOut(this.canvasGroup, 0.25f, EasingType.EaseInBack);

        AnimationManager.Instance.FireAfterDelay(delegate {
            if (this.isVisible) {
                return;
            }

            this.gameObject.SetActive(false);

            AnimationManager.Instance.Hide(this.canvasGroup);
        }, 0.25f);
    }

    public override bool IsVisible()
    {
        return this.isVisible;
    }

    public void OnDestroy()
    {
        if (EventManager.InstanceExists()) {
            //EventManager.Instance.RemoveListener<SceneLoadedEvent>(this.OnSceneLoaded);
            EventManager.Instance.RemoveListener<RoundTimeRemainingEvent>(this.OnRoundTimeRemaining);
            EventManager.Instance.RemoveListener<PlayerDeathEvent>(this.OnPlayerDeath);
            EventManager.Instance.RemoveListener<PlayerTierUpdatedEvent>(this.OnPlayerTierUpdated);
        }
    }

    public void Update()
    {
        // Check for null game runner
        if (this.gameRunner == null) {
            gameManager = GameManager.Instance;
            this.gameRunner = this.gameManager.GetRunner();
        }

        if (this.gameRunner.GetState() == GameState.Ended) {
            return;
        }

        // Temp removed game countdown
        //if (this.gameRunner.GetState() == GameState.Countdown) {
        //    ITimedMode timedRunner = this.gameRunner as ITimedMode;

        //    this.visibleTimer = timedRunner.GetCountdownRemaining();

        //    this.announcementLabelText.text = "Starting In " + timedRunner.GetCountdownRemaining().ToString("n0");

        //    this.Show();
        //    return;
        //}

        if (!this.IsVisible()) {
            return;
        }

        this.visibleTimer -= Time.deltaTime;

        //if (this.announcementLabelText.text.IndexOf("s Remaining") > -1) {
        //    ITimedMode timedRunner = this.gameRunner as ITimedMode;

        //    this.announcementLabelText.text = ((uint) timedRunner.GetTimeRemaining()) + "s Remaining";
        //}

        if (this.visibleTimer <= 0) {
            this.visibleTimer = 0;

            this.Hide();
            return;
        }
    }

    //private void OnSceneLoaded(SceneLoadedEvent e)
    //{
    //    if (e.name != "GameScene") {
    //        return;
    //    }

    //    if (!(this.gameRunner is ITimedMode)) {
    //        return;
    //    }

    //    ITimedMode timedRunner = this.gameRunner as ITimedMode;

    //    this.visibleTimer = timedRunner.GetCountdownRemaining();

    //    this.announcementLabelText.text = "Starting In " + timedRunner.GetCountdownRemaining().ToString("n0");

    //    this.Show();
    //}

    private void OnRoundTimeRemaining(RoundTimeRemainingEvent e)
    {
        if (!(this.gameRunner is ITimedMode)) {
            return;
        }

        if (e.timeRemaining == 10) {
            this.visibleTimer = 10f;
        } else {
            this.visibleTimer = 3f;
        }

        this.announcementLabelText.text = e.timeRemaining.ToString("n0") + "s Remaining";

        this.Show();
    }

    private void OnPlayerDeath(PlayerDeathEvent e)
    {
        if (e.player.isLocalPlayer) {
            return;
        }

        if (e.eatenBy != this.gameRunner.GetPlayer()) {
            return;
        }

        this.visibleTimer = 1.5f;

        this.announcementLabelText.text = "NICELY DONE!\n<size=18><color=#FFFFFFBB>Washed Away</color> <color=#E31B1BFF>" + e.player.name + "</color><color=#FFFFFFBB>!</color></size>";

        this.Show();
    }

    private void OnPlayerTierUpdated(PlayerTierUpdatedEvent e)
    {
        if (!e.player.isLocalPlayer) {
            return;
        }

        this.visibleTimer = 1.5f;

        this.announcementLabelText.text = "LEVEL UP!\n<size=18><color=#FFFFFFBB>Level " + e.tier + "</color></size>";

        this.Show();
    }
}
