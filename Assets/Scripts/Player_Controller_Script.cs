using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player_Controller_Script : MonoBehaviour
{
    [Header("Movement")]
    public float normal_movement_speed;
    public float boosted_movement_speed;
    public float turn_speed;
    public float reverse_speed;
    public float ground_drag;
    public float jump_force;
    public float jump_cooldown;
    public float air_multiplier;
    public float gravityScale;

    float current_speed;
    bool ready_to_jump;
    float start_jump;

    [Header("Ground Check")]
    public float player_height;
    public LayerMask ground_layer;
    RaycastHit ground_hit;
    bool grounded;

    [Header("Slope Handling")]
    public float max_slope_angle;
    public float rotation_speed;
    RaycastHit slope_hit;
    private bool exiting_slope;

    [Header("Keybinds")]
    public KeyCode jump_key = KeyCode.Space;
    public KeyCode boost_key = KeyCode.LeftShift;

    [Header("Configs")]
    public Transform spawn_point;

    // public Transform orientation;
    float horizontalInput;
    float verticalInput;

    Vector3 move_direction;
    float current_velocity;
    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        ready_to_jump = true;
        start_jump = 0;
    }

    private void Update()
    {
        Inputs();
        SpeedControl();

        current_velocity = rb.velocity.magnitude;
        // Debug.Log(current_velocity);

        if (Input.GetKey(KeyCode.W))
            start_jump += 1f * Time.deltaTime;

        // ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, out ground_hit, player_height * 1f, ground_layer);

        // handle drag
        if (grounded)
            rb.drag = ground_drag;
        else
            rb.drag = 0;

        // rotate player based on slope
        // create a rotation that aligns the player's up vector with the slope's normal vector
        Quaternion slopeRotation = Quaternion.FromToRotation(transform.up, slope_hit.normal) * transform.rotation;

        // interpolate the player's rotation towards the slope rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, slopeRotation, rotation_speed * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void Inputs()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(jump_key) && ready_to_jump && grounded && start_jump > 1.5f)
        {
            ready_to_jump = false;
            start_jump = 0f;
            Jump();
            Invoke(nameof(ResetJump), jump_cooldown);
        }
    }

    private void MovePlayer()
    {
        // calculate movement direction
        if (verticalInput < 0)
            move_direction = reverse_speed * verticalInput * transform.right;
        else
            move_direction = transform.right * verticalInput;

        transform.Rotate(10f * horizontalInput * Time.deltaTime * turn_speed * Vector3.up);

        // if on ground
        if (grounded)
        {
            if (Input.GetKey(boost_key))
                current_speed = boosted_movement_speed;
            else
                current_speed = normal_movement_speed;

            rb.AddForce(10f * current_speed * move_direction, ForceMode.Force);
        }
        // if on air apply air drag
        else
        {
            // prevents movement keys to break jump
            move_direction = 0 * verticalInput * transform.right;
            rb.AddForce(10f * air_multiplier * current_velocity * move_direction.normalized, ForceMode.Force);
        }

        // if on slope
        if (OnSlope() && !exiting_slope)
        {
            rb.AddForce(20f * current_speed * GetSlopeMoveDirection(), ForceMode.Force);

            if (rb.velocity.y > 0)
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
        }

        // add gravity to the player
        rb.AddForce(gravityScale * rb.mass * Physics.gravity, ForceMode.Force);
        rb.useGravity = !OnSlope();
    }

    private void SpeedControl()
    {
        // limit speed on slope
        if (OnSlope() && !exiting_slope)
        {
            if (rb.velocity.magnitude > current_speed)
                rb.velocity = rb.velocity.normalized * current_speed;
        }
        else
        {
            Vector3 flatVel = new(rb.velocity.x, 0f, rb.velocity.z);

            // limit velocity if needed
            if (flatVel.magnitude > current_speed)
            {
                Vector3 limitedVel = flatVel.normalized * current_speed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }

            // checks if player is moving
            if (flatVel == Vector3.zero)
                start_jump = 0f;
        }
    }

    private void Jump()
    {
        exiting_slope = true;

        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jump_force, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        ready_to_jump = true;
        exiting_slope = false;
    }

    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slope_hit, player_height * 1.2f) && grounded)
        {
            float angle = Vector3.Angle(Vector3.up, slope_hit.normal);
            return angle < max_slope_angle && angle != 0;
        }
        return false;
    }

    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(move_direction, slope_hit.normal).normalized;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bounds"))
        {
            Debug.Log("Player out of bounds");
            gameObject.transform.position = spawn_point.position;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}
