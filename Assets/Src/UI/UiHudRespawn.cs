using UnityEngine;
using UnityEngine.UI.ProceduralImage;
using TMPro;

public class UiHudRespawn : UiWidget
{
    public bool isDead = false;
    public float deathTimer = 0;

    public TextMeshProUGUI respawnLabelText;
    public TextMeshProUGUI respawnSubtitleText;
    public ProceduralImage timerBarImage;
    public TextMeshProUGUI timerLabelText;

    public override void Start()
    {
        base.Start();

        this.isDead = false;
        this.deathTimer = 0;

        EventManager.Instance.AddListener<PlayerSpawnEvent>(this.OnPlayerSpawn);
        EventManager.Instance.AddListener<PlayerDeathEvent>(this.OnPlayerDeath);

        this.Hide();
    }

    public void OnDestroy()
    {
        if (EventManager.InstanceExists()) {
            EventManager.Instance.RemoveListener<PlayerSpawnEvent>(this.OnPlayerSpawn);
            EventManager.Instance.RemoveListener<PlayerDeathEvent>(this.OnPlayerDeath);
        }
    }

    public void Update()
    {
        if (!this.isDead) {
            return;
        }

        this.deathTimer -= Time.deltaTime;

        if (this.deathTimer <= 0) {
            this.timerLabelText.text = "0";
            this.timerBarImage.fillAmount = 1f;
            return;
        }

        float respawnRatio = 1f - this.deathTimer / 5f;

        this.timerLabelText.text = Mathf.Ceil(this.deathTimer).ToString("n0");
        this.timerBarImage.fillAmount = respawnRatio;
    }

    private void OnPlayerSpawn(PlayerSpawnEvent e)
    {
        if (!e.player.isLocalPlayer) {
            return;
        }

        this.Hide();
    }

    private void OnPlayerDeath(PlayerDeathEvent e)
    {
        if (!e.player.isLocalPlayer) {
            return;
        }

        this.isDead = true;
        this.deathTimer = e.deathTimer;

        this.respawnLabelText.text = "<size=18><color=#FFFFFFBB>WASHED AWAY BY</color></size>\n<color=#E31B1BFF>" + e.eatenBy.name.ToUpper() + "</color>";

        this.Show();
        this.Update();
    }
}
