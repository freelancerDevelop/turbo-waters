using UnityEngine;
using GameAnalyticsSDK;
using GameAnalyticsSDK.Events;
using MoreMountains.NiceVibrations;

public class Bootstrap : MonoBehaviour
{
    private static bool hasLoaded = false;

    private GameManager gameManager;
    private TransitionManager transitionManager;
    private AudioManager audioManager;

    public void Start()
    {
        if (hasLoaded) {
            return;
        }

        #if PLATFORM_MOBILE
            Application.targetFrameRate = 60;

            Physics.defaultSolverIterations = 1;
        #endif

        GameObject gameAnalyticsObject = new GameObject("GameAnalytics");

        gameAnalyticsObject.AddComponent<GA_SpecialEvents>();
        gameAnalyticsObject.AddComponent<GameAnalytics>();

        GameAnalytics.Initialize();

        MMVibrationManager.iOSInitializeHaptics();

        AudioListener.volume = 0.5f;

        this.gameManager = GameManager.Instance;

        this.audioManager = AudioManager.Instance;
        this.audioManager.SetVolumeForAudioType(AudioType.Effect, StorageManager.Instance.GetFloat(StorageKeys.EffectVolume, 1f));
        this.audioManager.Preload("Sounds/level_up");
        this.audioManager.Preload("Sounds/points_gained");

        this.transitionManager = TransitionManager.Instance;
        this.transitionManager.LoadScene("MainMenuScene");

        hasLoaded = true;
    }
}
