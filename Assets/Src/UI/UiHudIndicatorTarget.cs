using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UiHudIndicatorTarget : MonoBehaviour
{
    public Sprite indicatorIcon;

    private Camera mainCamera;
    private UiHudIndicators hudIndicators;
    private GameObject hudIndicator;
    private Image hudIndicatorIcon;

    public void Start()
    {
        this.mainCamera = Camera.main;
        this.hudIndicators = Object.FindObjectOfType(typeof(UiHudIndicators)) as UiHudIndicators;

        GameObject hudIndicator = Instantiate(Resources.Load<GameObject>("Prefabs/HUD/IndicatorTarget"), this.hudIndicators.transform);
        Image hudIndicatorIcon = hudIndicator.transform.Find("IndicatorIcon").GetComponent<Image>();

        hudIndicatorIcon.sprite = this.indicatorIcon;

        this.hudIndicator = hudIndicator;
        this.hudIndicatorIcon = hudIndicatorIcon;

        this.hudIndicator.SetActive(false);
    }

    public void OnDestroy()
    {
        Destroy(this.hudIndicator);
    }

    public void Update()
    {
        Vector3 newPosition = this.mainCamera.WorldToViewportPoint(this.transform.position);

        if (newPosition.x > 0 && newPosition.y > 0 && newPosition.x < 1 && newPosition.y < 1) {
            this.hudIndicator.SetActive(false);
            return;
        }

        this.hudIndicator.SetActive(true);

        if (newPosition.z < 0) {
            newPosition.x = 1f - newPosition.x;
            newPosition.y = 1f - newPosition.y;
            newPosition.z = 0;
            newPosition = newPosition.normalized;
        }

        newPosition = this.mainCamera.ViewportToScreenPoint(newPosition);

        newPosition.x = Mathf.Clamp(newPosition.x, 32f, Screen.width - 32f);
        newPosition.y = Mathf.Clamp(newPosition.y, 32f, Screen.height - 32f);

        Vector3 targetPositionLocal = this.mainCamera.transform.InverseTransformPoint(this.transform.position);
        float targetAngle = -Mathf.Atan2(targetPositionLocal.x, targetPositionLocal.y) * Mathf.Rad2Deg - 180f;

        this.hudIndicator.transform.position = newPosition;
        this.hudIndicator.transform.eulerAngles = new Vector3(0, 0, targetAngle);

        this.hudIndicatorIcon.transform.eulerAngles = Vector3.zero;
    }
}
