using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Renderer))]
public class DissolverFadeOverTime : MonoBehaviour {

    [SerializeField] private float FadeLerpSpeed = 0.5f;
    private Material innerMaterial;
    private float transparency = 0.01f;
    public bool destroyOnTransparent = true;

    // Use this for initialization
    void Start() {
        // Get material
        innerMaterial = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update() {
        // Fade out
        transparency = Mathf.Lerp(transparency, 1, FadeLerpSpeed);

        // Destroy on full transparency if enabled
        if (transparency > 0.99f && destroyOnTransparent) {
            Destroy(gameObject);
        }

        // Apply transparency
        if (innerMaterial.HasProperty("_Level")) {
            innerMaterial.SetFloat("_Level", transparency);
        }
    }
}
