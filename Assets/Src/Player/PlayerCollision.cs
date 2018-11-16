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
            // Ensure it's not colliding with itself
            if (c.otherCollider.gameObject.transform.root.gameObject == this.transform.root.gameObject) {
                return;
            }

            // Check if colliding with other player object
            if (c.otherCollider.gameObject.transform.root.GetComponent<Player>() != null) {
                Player otherPlayer = c.otherCollider.gameObject.transform.root.GetComponent<Player>();

                // Check if colliding with an Active player
                if (player.Type == PlayerType.Active
                && otherPlayer.Type == PlayerType.Active) {
                    // Return if other player is dead already
                    if (otherPlayer.isDead) {
                        return;
                    }
                    // Attack
                    player.AttackedBy(c.otherCollider.gameObject.transform.root.GetComponent<Player>());
                }


                // Check if colliding with a Passive player
                if (player.Type == PlayerType.Active
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
    }

    private void OnTriggerEnter(Collider other) {

        // Check if object is an obstacle
        if (other.gameObject.tag == "Obstacle") {
            // Call obstacle destruction method
            other.gameObject.GetComponent<Obstacle>().Destruct();
        }

        // Check if object is a ScoreBlock
        if (other.gameObject.tag == "ScoreBlock") {
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
