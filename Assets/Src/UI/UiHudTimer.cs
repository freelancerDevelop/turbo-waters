using UnityEngine;
using TMPro;

#if UNITY_IOS
    using UnityEngine.iOS;
#endif

public class UiHudTimer : UiWidget
{
    public RectTransform rectTransform;
    public TextMeshProUGUI timerLabelText;

    private GameManager gameManager;
    private GameModeRunner gameRunner;

    public override void Start()
    {
        base.Start();

        this.gameManager = GameManager.Instance;
        this.gameRunner = this.gameManager.GetRunner();

        if (this.gameRunner is ITimedMode) {
            this.Show();
        } else {
            this.Hide();
        }

        #if UNITY_IOS
            if (Device.generation == DeviceGeneration.iPhoneX) {
                this.rectTransform.anchoredPosition = new Vector2(0, -50f);
            }
        #endif
    }

    public void Update()
    {
        if (this.gameRunner.GetState() == GameState.Ended) {
            return;
        }

        ITimedMode timedRunner = this.gameRunner as ITimedMode;

        //this.timerLabelText.text = Formatter.Instance.DurationToClock(timedRunner.GetTimeRemaining());

        //float timeRemaining = timedRunner.GetTimeRemaining();
        float timeRemaining = 11f;

        if (timeRemaining > 10f) {
            return;
        }

        Color labelColor = Color.white;
        float timeRemainingRemainder = timeRemaining % 1f;

        if (timeRemainingRemainder > 0.5f) {
            labelColor = Color.Lerp(Color.white, Color.red, timeRemainingRemainder - 0.5f);
        }

        this.timerLabelText.color = labelColor;
    }
}
