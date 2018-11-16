using UnityEngine;
using TMPro;

public class UiMainMenu : UiLayer
{
    public UiSettingsOverlay settingsOverlay;
    public TextMeshProUGUI progressLevelLabelText;
    public TextMeshProUGUI progressXpLabelText;
    public RectTransform progressBarRect;
    public TMP_InputField nameInput;
    public GameObject highScore;
    public TextMeshProUGUI highScoreLabelText;
    public GameObject noAds;

    private GameManager gameManager;
    private GameModeRunner gameRunner;

    public override void Start()
    {
        base.Start();

        this.gameManager = GameManager.Instance;
        this.gameRunner = this.gameManager.GetRunner();

        this.Render();
    }

    public void Render()
    {
        int playerXp = StorageManager.Instance.GetInt(StorageKeys.PlayerXP, 0);
        int playerLevel = StorageManager.Instance.GetInt(StorageKeys.PlayerLevel, 1);
        int nextLevelXpRequired = 100;

        this.progressBarRect.sizeDelta = new Vector2(144f * (playerXp % 100) / nextLevelXpRequired, 26f);
        this.progressLevelLabelText.text = playerLevel.ToString("n0");
        this.progressXpLabelText.text = playerXp.ToString("n0") + "/" + nextLevelXpRequired;

        string playerName = StorageManager.Instance.GetString(StorageKeys.PlayerName);

        this.nameInput.text = playerName;

        int score = StorageManager.Instance.GetInt(StorageKeys.VanillaHighScore, 0);

        if (score == 0) {
            this.highScore.SetActive(false);
        } else {
            this.highScore.SetActive(true);
            this.highScoreLabelText.text = "<color=#FFFFFF99>Best:</color> " + score.ToString("n0");
        }

        if (ShopManager.Instance.IsIapOwned("noads.permanent")) {
            //this.noAds.SetActive(false);
        } else {
            //this.noAds.SetActive(true);
        }
    }

    public void OnTriggerPlay()
    {
        TransitionManager.Instance.LoadScene("GameScene");

        this.gameManager.StartRound();
    }

    public void OnTriggerSettings()
    {
        this.settingsOverlay.Show();
    }

    public void OnTriggerNoAds()
    {
        ShopManager.Instance.PurchaseIapProduct("noads.permanent");
    }

    public void OnTriggerNameUpdate()
    {
        StorageManager.Instance.SetString(StorageKeys.PlayerName, this.nameInput.text);
    }
}
