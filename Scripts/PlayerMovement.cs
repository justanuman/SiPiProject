using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")] [SerializeField] 
    private float moveSpeed;

    [SerializeField]
    private float groundDrag;

    [SerializeField]
    private float jumpForce;

    [SerializeField]
    private float jumpCooldown;

    [SerializeField]
    private float airMultiplier;
    bool readyToJump;

    [Header("Keybinds")] [SerializeField]
    private KeyCode jumpKey = KeyCode.Space;

    [Header("Ground Check")] [SerializeField]
    private float playerHeight;

    [SerializeField]
    private LayerMask linoleum;
    bool grounded;

    [SerializeField]
    private Transform orientation;

    float horizontalInput;
    float vertivalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyToJump = true;
    }

    // Update is called once per frame
    void Update()
    {
        // ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, linoleum);

        MyInput();
        SpeedControl();

        // handle drag
        if (grounded) 
        {
            rb.drag = groundDrag;
        }
        else 
        {
            rb.drag = 0;
        }
    }

    private void FixedUpdate() 
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        vertivalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void MovePlayer () 
    {
        // calculate movement dir
        moveDirection = orientation.forward * vertivalInput + orientation.right * horizontalInput;

        if (grounded) 
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }
        else 
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
       
        // limit velocity if needed
        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump() 
    {
        readyToJump = true;
    }
}
