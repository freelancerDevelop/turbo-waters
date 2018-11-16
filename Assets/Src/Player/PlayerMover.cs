using System;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerMover : MonoBehaviour {

    private Player player;
    private Vector3 mousePosition = Vector3.zero;
    public GameObject DiveTarget { get; private set; }
    private readonly float diveSmoothness = 0.1f;
    private readonly float maximumDiveSpeed = 21;
    private float DiveY = -2;
    private float diveCooldown = 0.4f;
    private bool reachedDiveTarget = false;
    private Rigidbody rb;
    private Vector3 lastTowardsPos;

    private float WaterLevel = .5f;
    private float maximumSurfacingVelocity = 12;
    public float surfacingSmoothness = 0.5f;

    private bool goToSurface = true;
    private bool reachedSurface = true;
    private bool resurfaceCompleted = true;

    private Vector3 lastRotation;
    public float directionEaseSpeed = 0.3f;

    private float currentXRotation = 0.0f;
    private float horizontalRotationSpeed = 0.3f;
    private float pitchSpeed = 0.2f;
    private float currentSpeed = 0;
    private float stopEase = 3;
    public bool moving = false;

    private Quaternion targetRotation;

    void Start() {
        player = GetComponent<Player>();
        rb = player.GetComponent<Rigidbody>();
        this.mousePosition = transform.position;
        this.lastRotation = this.transform.rotation.eulerAngles;
        this.targetRotation = this.transform.rotation;
    }

    public void Update() {
        Vector3 initialPosition = transform.position;

        // Float to surface functionality
        if (player.Type != PlayerType.Passive && goToSurface && !reachedSurface) {
            // Check if reached surface & ended bounced
            if (Mathf.Abs(transform.position.y - WaterLevel) < 0.03f) {
                reachedSurface = true;
            }

            // Set yVel based on position
            float yVel = 0;
            if (transform.position.y < WaterLevel) {
                surfacingSmoothness = 0.1f;
                // Get new Y velocity value
                yVel = Mathf.Lerp(rb.velocity.y, maximumSurfacingVelocity * 2, surfacingSmoothness);
            } else {
                surfacingSmoothness = 0.05f;
                // Get new Y velocity value
                yVel = Mathf.Lerp(rb.velocity.y, -maximumSurfacingVelocity, surfacingSmoothness);
            }
            // Apply new Y velocity
            rb.velocity = new Vector3(rb.velocity.x, yVel, rb.velocity.z);
        }

        if (!player.isDead) {
            // Movement of local player
            if (player.isLocalPlayer) {
                moving = false;
#if MOBILE_INPUT
                float moveX = CrossPlatformInputManager.GetAxis("Horizontal");
                float moveY = CrossPlatformInputManager.GetAxis("Vertical");

                this.mousePosition = this.transform.position - (Quaternion.Euler(0, -135f, 0) * new Vector3(moveX, 0, moveY)).normalized;
                this.mousePosition.y = this.transform.position.y;

                this.MoveTowards(this.mousePosition);
                moving = true;
#else
                if (Input.GetMouseButton(0)) {
                    RaycastHit hit;

                    if (Physics.Raycast(player.GetMainCamera().ScreenPointToRay(Input.mousePosition), out hit, 500f, player.movementMask)) {
                        this.mousePosition = hit.point;
                        this.mousePosition.y = transform.position.y;

                        this.MoveTowards(this.mousePosition);
                        moving = true;
                    }
                }
#endif
            }
        }

        if (player.Type != PlayerType.Passive) {
            // Ease direction back to 0
            if (!moving) {
                player.GetAnimator().SetDirection(Mathf.Lerp(player.GetAnimator().GetDirection(), 0, directionEaseSpeed));
            }

            // Animate
            player.GetAnimator().SetMoving(moving);

            // Disable diving if target is null
            if (player.State == PlayerState.Diving && DiveTarget == null) {
                DisableDive();
            }

            // Diving to targets
            if (player.State == PlayerState.Diving) {
                float diveSpeed = 0;

                // Set up dive speed based on position
                if (transform.position.y > DiveY) {
                    diveSpeed = Mathf.Lerp(rb.velocity.y, -maximumDiveSpeed, diveSmoothness);
                } else {
                    diveSpeed = Mathf.Lerp(rb.velocity.y, (maximumDiveSpeed / 5f), diveSmoothness);
                }

                // Apply dive speed
                rb.velocity = new Vector3(rb.velocity.x, diveSpeed, rb.velocity.z);

                // Calculate dive rotation
                Vector3 diveRotPos = new Vector3(lastTowardsPos.x, DiveTarget.transform.position.y, lastTowardsPos.z);
                Vector3 movementDifference = diveRotPos - transform.position;
                movementDifference = new Vector3(movementDifference.x, 0, movementDifference.z);

                if (movementDifference != Vector3.zero) {
                    Quaternion newRotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(movementDifference), player.rotationSpeed * Time.deltaTime);

                    targetRotation = newRotation;
                }
            }
        }

        // Set gravity based on state
        if (player.Type != PlayerType.Passive) {
            switch (player.State) {
                case PlayerState.Swimming:
                    // Stop vertical movement on surface reached
                    if (reachedSurface) {
                        if (rb.useGravity) {
                            rb.velocity = new Vector3(0, 0, 0);
                            resurfaceCompleted = true;
                        }
                        rb.useGravity = false;
                    } else {
                        rb.useGravity = true;
                    }
                    break;
                case PlayerState.Diving:
                    // Enable gravity
                    rb.useGravity = true;
                    break;
            }
        }

        // Move rotation towards target rotation
        this.transform.rotation = Quaternion.Lerp(this.transform.rotation, targetRotation, horizontalRotationSpeed);

        // Set rotation based on y velocity
        if (player.Type != PlayerType.Passive) {
            float rotationBasedOnSpeed = (-rb.velocity.y * 1.1f);
            float pitchBasedOnSpeed = (rotationBasedOnSpeed / 80f);

            // Ease towards rotation
            currentXRotation += (rotationBasedOnSpeed - currentXRotation) / 5f;

            // Apply newly calculated rotation
            transform.eulerAngles = new Vector3(currentXRotation, transform.eulerAngles.y, transform.eulerAngles.z);

            // Ease towards pitch
            float newPitch = Mathf.Lerp(player.GetAnimator().GetPitch(), pitchBasedOnSpeed, pitchSpeed);

            // Apply newly calculated pitch
            player.GetAnimator().SetPitch(newPitch);
        }

        // Ease speed up or down
        if (!moving) {
            currentSpeed += (0 - currentSpeed) / stopEase;
        } else {
            currentSpeed += (this.GetMovementSpeed() - currentSpeed) / 2f;
        }

        // Move towards rotation
        Vector3 fullSpeed = transform.forward * currentSpeed;
        rb.velocity = new Vector3(fullSpeed.x, rb.velocity.y, fullSpeed.z);
    }

    public void LateUpdate() {
        // Set gravity based on state
        if (player.Type != PlayerType.Passive) {
            switch (player.State) {
                case PlayerState.Swimming:
                    // Stop vertical movement on surface reached
                    if (resurfaceCompleted) {
                        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                    }

                    if (transform.position.y > WaterLevel + 0.1f) {
                        //player.State = PlayerState.Diving;
                        DisableDive();
                        resurfaceCompleted = false;
                        rb.velocity = new Vector3(rb.velocity.x, -3, rb.velocity.z);
                    }
                break;
            }
        }
    }

    public void MoveTowards(Vector3 towardsPosition) {
        lastTowardsPos = towardsPosition;

        float scale = player.GetScale();

        // Set Y based on state
        towardsPosition.y = player.State == PlayerState.Diving ? DiveY : transform.position.y;

        // Set rotation if not diving
        if (player.State != PlayerState.Diving) {
            // TODO: Player Rotation ease
            //if (player.isLocalPlayer && Mathf.Abs(transform.rotation.x) > 0.05f) {
            //    Logger.Message("[PlayerMover] Rotation X: " + transform.rotation.x);
            //}

            Vector3 movementDifference = towardsPosition - transform.position;
            movementDifference = new Vector3(movementDifference.x, 0, movementDifference.z);

            if (movementDifference != Vector3.zero) {
                Quaternion newRotation =
                    Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(movementDifference), player.rotationSpeed * Time.deltaTime);

                // Set direction for player rig based on rotation
                if (player.isLocalPlayer) {
                    // Get rotation difference
                    float rotationDifference = ((targetRotation.eulerAngles.y  - newRotation.eulerAngles.y) * 100);

                    // Determine eased direction
                    float newDirection = player.GetAnimator().GetDirection();//Mathf.Lerp(player.GetAnimator().GetDirection, );
                    float headTiltDivisor = 650;
                    float targetDirection = Mathf.Abs(rotationDifference / headTiltDivisor);
                    float rotationChange = Mathf.Abs(targetDirection);

                    if (Mathf.Abs(rotationDifference) > 10f && Mathf.Abs(rotationDifference) < 900) {
                        if (rotationDifference > 0) {
                            newDirection = Mathf.Lerp(newDirection, -targetDirection, directionEaseSpeed);
                        } else if (rotationDifference < 0) {
                            newDirection = Mathf.Lerp(newDirection, targetDirection, directionEaseSpeed);
                        }
                    } else {
                        newDirection = Mathf.Lerp(newDirection, 0, directionEaseSpeed);
                    }

                    // Set direction based on rotation difference
                    player.GetAnimator().SetDirection(newDirection);
                }

                targetRotation = newRotation;
            }
        }

        // Reset last rotation to current rotation
        lastRotation = transform.rotation.eulerAngles;
    }

    public void StartSwimTo(GameObject target) {
        // Set player state
        player.State = PlayerState.Diving;

        // Set dive Y
        DiveY = target.transform.position.y;

        // Disable buoyancy
        goToSurface = false;
        reachedSurface = false;
        resurfaceCompleted = false;

        // Set DiveTarget
        this.DiveTarget = target;
    }

    public void DisableDive() {
        // Reset state
        player.State = PlayerState.Swimming;

        // Allow float to surface
        goToSurface = true;
        reachedSurface = false;

        // Reset vertical rigidbody velocity
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.useGravity = true;
    }

    public void ReachedSwimTarget() {
        // Play animation
        player.GetAnimator().Attack();

        // Reset movement
        DisableDive();
    }

    public void ReachedSwimTarget(GameObject target) {
        // Destroy target
        target.GetComponent<Player>().AttackedBy(player);

        // Call original function
        ReachedSwimTarget();
    }

    public float GetMovementSpeed() {
        return player.moveSpeed * player.GetSpeedMultiplier() * (player.isDead ? 0.5f : 1f);
    }

    public float GetWaterLevel() {
        return this.WaterLevel;
    }

}
