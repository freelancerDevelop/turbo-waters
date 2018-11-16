using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(Player))]
public class PlayerCollision : MonoBehaviour {

    //public Player player;
    private PlayerMover mover;
    private Player player;

    // Use this for initialization
    void Start() {
        mover = GetComponent<PlayerMover>();
        player = GetComponent<Player>();
    }

    private void OnCollisionEnter(Collision collision) {

        foreach (ContactPoint c in collision.contacts) {

            // Check if other player/AI
            if (c.thisCollider.gameObject.transform.root.tag == "Player") {
                Logger.Message("Collided w/ a player");
                return;
            }

            // Do logic if mouth touches object, and player is an Active type
            if (c.thisCollider.gameObject.transform.root.GetComponent<Player>().Type == PlayerType.Active
            //&& c.otherCollider.GetComponent<FishBot>() != null) {
            && c.otherCollider.GetComponent<CreatureCollision>() != null) {
                // Return if object is dead already
                if (c.otherCollider.gameObject.GetComponent<Player>().isDead) {
                    return;
                }

                // Eat object
                GetComponent<PlayerMover>().ReachedSwimTarget(c.otherCollider.gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        // Check if object is an obstacle
        if (other.gameObject.tag == "Obstacle") {
            // Call obstacle destruction method
            other.gameObject.GetComponent<Obstacle>().Destruct();
        } else if (other.gameObject.tag == "ScoreBlock") {
            // Add points to player
            player.AddPoints(5);
        }

        // Check if object is an AttractiveObject
        if (other.gameObject.GetComponent<AttractiveObject>() != null) {
            other.gameObject.GetComponent<AttractiveObject>().SetTarget(this.transform);
        }
    }

    private void OnTriggerExit(Collider other) {
        // Dive Target
        if (mover.DiveTarget != null) {
            // Disable dive if exiting the current target
            if (other.transform.root.gameObject == mover.DiveTarget.transform.root.gameObject) {
                mover.DisableDive();
            }
        }
    }
}
