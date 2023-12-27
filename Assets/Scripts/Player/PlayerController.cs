using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private static float TIME_BETWEEN_ACTIONS = 0.3f;
    private static float DASH_TIME = 0.3f;
    private static float JUMP_TIME = 0.3f;

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
    private bool dashing;
    private bool canDash;
    private bool canDoAction;
    private float jumpTimer;

    private Rigidbody2D rb;
    private Animator anim;

    public void Start() {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        dashing = false;
        canDash = true;
        canDoAction = true;

        jumpTimer = JUMP_TIME;
    }

    public void Update() {
        UpdateMovement();
        CheckDash();
        UpdateAnimations();
    }

    private void UpdateMovement() {
        if(dashing) return;

        moveDir = rb.velocity;
        xAxis = Input.GetAxisRaw("Horizontal");
        if (xAxis != 0) {
            transform.localScale = new Vector3(xAxis, 1, 1);
        }

        onGround = Physics2D.OverlapCircle(groundDetector.position, 0.1f, groundMask);
        if(onGround) {
            jumpTimer = JUMP_TIME;
            canDash = true;
            if(Input.GetKey(KeyCode.Space)) {
            moveDir.y = jumpForce;
            onGround = false;
            }
        } else {
            moveDir.y -= GlobalVariables.GRAVITY * Time.deltaTime;
            if(rb.velocity.y < 0) {
                if (jumpTimer > 0) jumpTimer = 0;
            } else {
                if(Input.GetKey(KeyCode.Space) && jumpTimer > 0) {
                    moveDir.y = jumpForce;
                    jumpTimer -= Time.deltaTime;
                }
                if(Input.GetKeyUp(KeyCode.Space)) {
                    jumpTimer = 0;
                    moveDir.y = 0;
                }
            }
        }

        moveDir.x = xAxis * moveSpeed;
        rb.velocity = moveDir;
        }

        private void CheckDash() {
        if(dashing) {
            rb.velocity = new Vector2(transform.localScale.x * (moveSpeed * 2), 0);
        }

        if(!dashing && Input.GetKey(KeyCode.K) && canDoAction && canDash) {
            StartCoroutine(CoDash());
        }
    }

    private IEnumerator CoDash() {
        dashing = true;
        canDash = false;
        canDoAction = false;
        yield return new WaitForSeconds(DASH_TIME);
        dashing = false;
        StartCoroutine(CoActionTime());
    }

    private IEnumerator CoActionTime() {
        yield return new WaitForSeconds(TIME_BETWEEN_ACTIONS);
        canDoAction = true;
    }

    private void UpdateAnimations() {
        anim.SetBool("moving", xAxis != 0);
        anim.SetBool("onGround", onGround);
        anim.SetFloat("yAxis", rb.velocity.y);
        anim.SetBool("dashing", dashing);
    }
}