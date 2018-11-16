using UnityEngine;
using System.Collections;

public class AttractiveObject : MonoBehaviour {

    private Transform Target = null;
    private bool MoveToTarget = false;
    private Material innerCubeMat;
    public bool fadeOut = true;

    // Use this for initialization
    void Start() {
        // Get material
        innerCubeMat = GetComponent<MeshRenderer>().material;
    }

    // Update is called once per frame
    void Update() {
        if (MoveToTarget && Target != null) {
            // Move towards target
            transform.position = Vector3.Lerp(this.transform.position, Target.position, 0.2f);
        }
    }

    public void SetTarget(Transform trans) {
        // Move towards target
        Target = trans;
        MoveToTarget = true;

        // Disable Buoyancy and collision, enable kinematic movement
        GetComponent<Buoyancy>().enabled = false;
        GetComponent<Collider>().enabled = false;
        GetComponent<Rigidbody>().isKinematic = true;

        // Fade out if enabled
        if (fadeOut) {
            if (this.gameObject.GetComponent<DissolverFadeOverTime>() == null) {
                this.gameObject.AddComponent<DissolverFadeOverTime>();
            }
        }
    }
}
