using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private float jumpForce;

    [Header("Detectors")]
    [SerializeField]
    private Transform groundDetector;
    [SerializeField]
    private LayerMask groundMask;

    private Vector2 moveDir;
    private float xAxis;
    private bool onGround;

    private Rigidbody2D rb;
    private Animator anim;

    public void Start() {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    public void Update() {
        UpdateMovement();
        UpdateAnimations();
    }

    private void UpdateMovement() {
        moveDir = rb.velocity;
        xAxis = Input.GetAxisRaw("Horizontal");
        if (xAxis != 0) {
            transform.localScale = new Vector3(xAxis, 1, 1);
        }

        onGround = Physics2D.OverlapCircle(groundDetector.position, 0.1f, groundMask);
        if(onGround && Input.GetKey(KeyCode.Space)) {
            moveDir.y = jumpForce;
            onGround = false;
        }

        if(!onGround) {
            moveDir.y -= GlobalVariables.GRAVITY * Time.deltaTime;
        }
        moveDir.x = xAxis * moveSpeed;
        rb.velocity = moveDir;
    }

    private void UpdateAnimations() {
        anim.SetBool("moving", xAxis != 0);
        anim.SetBool("onGround", onGround);
        anim.SetFloat("yAxis", rb.velocity.y);
    }
}
