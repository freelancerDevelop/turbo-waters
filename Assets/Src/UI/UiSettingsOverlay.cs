using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UiSettingsOverlay : UiLayer
{
    public Slider effectVolumeSlider;

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
        float effectVolume = StorageManager.Instance.GetFloat(StorageKeys.EffectVolume, 1f);

        this.effectVolumeSlider.value = effectVolume;
    }

    public void OnTriggerEffectVolumeUpdate()
    {
        AudioManager.Instance.SetVolumeForAudioType(AudioType.Effect, this.effectVolumeSlider.value);

        StorageManager.Instance.SetFloat(StorageKeys.EffectVolume, this.effectVolumeSlider.value);
    }

    public void OnTriggerRestorePurchases()
    {
        ShopManager.Instance.RestoreIapPurchases();
    }
}
