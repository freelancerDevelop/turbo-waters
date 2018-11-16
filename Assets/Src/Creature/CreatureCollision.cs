using UnityEngine;
using System.Collections;

public class CreatureCollision : MonoBehaviour {
    private void OnTriggerEnter(Collider other) {
        // Check if player entered radius
        if (other.transform.root.gameObject.tag == "Player") {
            GameObject rootObject = other.transform.root.gameObject;
            // Start swim to this as a target
            rootObject.GetComponent<PlayerMover>().StartSwimTo(this.gameObject);
        }
    }
}
