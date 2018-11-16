using UnityEngine;
using TMPro;

public class UiHudPip : MonoBehaviour
{
    private RectTransform rectTransform;
    private TextMeshProUGUI labelText;

    private string labelValue = "+1";
    private float duration = 1f;
    private float remaining = 1f;
    private float speed = 80f;
    private readonly float randomRotationRange = 10f;
    private readonly float positionRandomization = 70f;

    public void Init()
    {
        this.rectTransform = this.GetComponent<RectTransform>();
        this.labelText = this.GetComponent<TextMeshProUGUI>();

        this.rectTransform.anchoredPosition += new Vector2(Random.insideUnitSphere.x, Random.insideUnitSphere.y) * positionRandomization;
        this.rectTransform.eulerAngles = new Vector3(0, 0, Random.Range(-randomRotationRange, randomRotationRange));
    }

    public void Update()
    {
        this.remaining -= Time.deltaTime;

        float durationRatio = 1f - this.remaining / this.duration;

        this.rectTransform.anchoredPosition += new Vector2(0, this.speed * Time.deltaTime);

        this.labelText.faceColor = Color.Lerp(Color.white, new Color32(255, 255, 255, 0), durationRatio);
        this.labelText.outlineColor = Color.Lerp(new Color32(51, 51, 51, 255), new Color32(51, 51, 51, 0), durationRatio);

        if (this.remaining <= 0) {
            Destroy(this.gameObject);
        }
    }

    public void SetText(string labelValue)
    {
        this.labelValue = labelValue;
    }

    public void SetDuration(float duration)
    {
        this.duration = duration;
        this.remaining = duration;
    }

    public void SetSpeed(float speed)
    {
        this.speed = speed;
    }

    public void Render()
    {
        this.labelText.text = this.labelValue;
    }
}
