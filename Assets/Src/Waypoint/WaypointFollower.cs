using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaypointFollower : MonoBehaviour
{
    public WaypointPath waypointPath;
    public int startWaypointIndex = 0;
    public Vector3 rotationOffset = Vector3.zero;

    [Header("Movement Options")]
    [Range(0.1f, 10f)]
    public float acceleration = 1.5f;

    [Range(0.1f, 10f)]
    public float deceleration = 3f;

    [Range(1f, 20f)]
    public float maxMoveSpeed = 5f;

    [Range(1f, 10f)]
    public float rotationSpeed = 5f;

    [Range(1f, 10f)]
    public float distanceBeforeDeceleration = 8f;
    public LayerMask decelerationLayerMask;

    [Header("Generic Options")]
    public bool loop = true;
    public bool lockToPath = false;

    [Header("Vehicle Options")]
    public bool autoDetectWheels = true;
    public List<Transform> wheels = new List<Transform>();

    private Rigidbody rigidBody;
    private Animator animator;
    private List<Transform> waypoints = new List<Transform>();
    private Quaternion inverseRotationOffset = Quaternion.identity;
    private WaypointJunctionLane atJunctionLane;
    private Collider lastWaypointCollider;
    private int waypointIndex = 0;
    private float currentSpeed = 0;
    private bool isAccelerating = true;
    private bool isStopped = false;
    private bool isAtJunction = false;

    public void Start()
    {
        if (this.waypointPath == null) {
            this.isStopped = true;
            return;
        }

        this.rigidBody = this.GetComponent<Rigidbody>();
        this.animator = this.GetComponent<Animator>();
        this.waypoints = this.waypointPath.GetWaypoints();
        this.waypointIndex = this.startWaypointIndex;
        this.inverseRotationOffset = Quaternion.Inverse(Quaternion.Euler(this.rotationOffset));

        if (this.autoDetectWheels) {
            foreach (Transform child in this.transform) {
                if (child.name.IndexOf("wheel") > -1 || child.name.IndexOf("Wheel") > -1) {
                    this.wheels.Add(child);
                }
            }
        }
    }

    public void OnEnable()
    {
        InvokeRepeating("CheckForCollisions", 0, 1f);
    }

    public void OnDisable()
    {
        CancelInvoke("CheckForCollisions");

        if (this.animator != null && this.animator.gameObject.activeSelf) {
            this.animator.SetFloat("Speed_f", 0);
        }
    }

    public void Update()
    {
        if (this.isStopped) {
            if (this.animator != null && this.animator.gameObject.activeSelf) {
                this.animator.SetFloat("Speed_f", 0);
            }

            return;
        }

        if (this.isAccelerating) {
            this.currentSpeed += this.acceleration * Time.deltaTime;
        } else {
            this.currentSpeed -= this.deceleration * Time.deltaTime;
        }

        this.currentSpeed = Mathf.Clamp(this.currentSpeed, 0, this.maxMoveSpeed);

        if (this.animator != null && this.animator.gameObject.activeSelf) {
            this.animator.SetFloat("Speed_f", this.currentSpeed);
        }

        foreach (Transform wheel in wheels) {
            wheel.Rotate(Vector3.right, this.currentSpeed * Time.deltaTime * 90f, Space.Self);
        }
    }

    public void FixedUpdate()
    {
        if (this.isStopped || this.isAtJunction) {
            return;
        }

        Vector3 direction = (this.waypoints[this.waypointIndex].position - this.transform.position).normalized;

        direction.y = 0;

        if (direction != Vector3.zero) {
            Quaternion lookRotation = Quaternion.LookRotation(direction) * this.inverseRotationOffset;

            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, lookRotation, this.rotationSpeed * Time.fixedDeltaTime);
        }

        if (this.lockToPath) {
            Vector3 lastPoint = this.waypointIndex == 0 ? this.waypoints[this.waypoints.Count - 1].position : this.waypoints[this.waypointIndex - 1].position;
            Vector3 pathDirection = this.waypoints[this.waypointIndex].position - lastPoint;
            Vector3 closestPathPoint = lastPoint + pathDirection.normalized * Vector3.Dot(this.transform.position - lastPoint, pathDirection.normalized);

            this.transform.position = Vector3.Lerp(this.transform.position, closestPathPoint, 0.5f * Time.fixedDeltaTime) + Quaternion.Euler(this.rotationOffset) * this.transform.forward * this.currentSpeed * Time.fixedDeltaTime;
        } else {
            this.transform.position = this.transform.position + Quaternion.Euler(this.rotationOffset) * this.transform.forward * this.currentSpeed * Time.fixedDeltaTime;
        }
    }

    public void CheckForCollisions()
    {
        if (this.isStopped || this.isAtJunction) {
            return;
        }

        RaycastHit hit;

        if (Physics.SphereCast(this.rigidBody.position, 0.5f, Quaternion.Euler(this.rotationOffset) * this.transform.forward, out hit, this.distanceBeforeDeceleration, this.decelerationLayerMask)) {
            this.isAccelerating = false;
        } else {
            this.isAccelerating = true;
        }
    }

    private void OnTriggerStay(Collider collider)
    {
        if (this.isStopped) {
            return;
        }

        if (collider.CompareTag("Waypoint") && Vector3.Distance(this.transform.position, this.waypoints[this.waypointIndex].position) < 1f) {
            if (collider == this.lastWaypointCollider) {
                return;
            }

            this.waypointIndex++;

            if (this.waypointIndex >= this.waypoints.Count) {
                if (this.loop) {
                    this.waypointIndex = 0;
                } else {
                    this.isStopped = true;
                }
            }

            this.lastWaypointCollider = collider;
        }

        if (!this.isAccelerating && collider.CompareTag("WaypointJunction") && this.atJunctionLane.isFree) {
            this.isAtJunction = false;
            this.isAccelerating = true;
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (this.isStopped) {
            return;
        }

        if (collider.CompareTag("WaypointJunction")) {
            this.atJunctionLane = collider.GetComponent<WaypointJunctionLane>();

            if (!this.atJunctionLane.isFree) {
                this.isAtJunction = true;
                this.isAccelerating = false;
            }
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (this.isStopped) {
            return;
        }

        if (collider.CompareTag("WaypointJunction")) {
            this.isAtJunction = false;
            this.isAccelerating = true;
        }
    }
}
