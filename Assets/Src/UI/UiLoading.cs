using UnityEngine;
using UnityEngine.UI.ProceduralImage;
using TMPro;

public class UiLoading : UiWidget
{
    public TextMeshProUGUI loadingLabelText;
    public ProceduralImage loadingBarImage;
    public TextMeshProUGUI tipLabelText;

    private GameManager gameManager;
    private GameModeRunner gameRunner;

    public override void Start()
    {
        base.Start();

        this.loadingBarImage.rectTransform.sizeDelta = Vector2.zero;
    }

    public void Update()
    {
        this.loadingBarImage.rectTransform.sizeDelta = new Vector2(320f * TransitionManager.Instance.GetLoadingProgress(), 10f);
    }
}
