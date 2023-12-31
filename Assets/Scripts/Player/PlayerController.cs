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
    private Transform wallDetector;
    [SerializeField]
    private LayerMask groundMask;

    private Vector2 moveDir;
    private float xAxis;
    private bool onGround;
    private bool dashing;
    private bool wallSliding;
    private bool canDash;
    private bool canDoAction;
    private bool inputBlocked;
    private bool canDoubleJump;
    private float jumpTimer;
    private IEnumerator currentCo;

    private Rigidbody2D rb;
    private Animator anim;

    public void Start() {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        dashing = false;
        canDash = true;
        canDoAction = true;
        canDoubleJump = false;
        inputBlocked = false;

        jumpTimer = JUMP_TIME;
    }

    public void FixedUpdate() {
        if(!inputBlocked) UpdateMovement();
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
        moveDir.x = xAxis * moveSpeed;

        CheckJump();
        CheckWallSlide();

        rb.velocity = moveDir;
    }

    private void CheckJump() {
        onGround = Physics2D.OverlapCircle(groundDetector.position, 0.1f, groundMask);
        if(onGround) {
            jumpTimer = JUMP_TIME;
            canDash = true;
            canDoubleJump = false;
            if(Input.GetKeyDown(KeyCode.Space)) {
                moveDir.y = jumpForce;
                onGround = false;
                canDoubleJump = true;
            }
        } else {
            moveDir.y -= GlobalVariables.GRAVITY * Time.deltaTime;
            if(rb.velocity.y < 0) {
                jumpTimer = 0;
                if(Input.GetKeyDown(KeyCode.Space) && canDoubleJump) {
                    canDoubleJump = false;
                    moveDir.y = jumpForce;
                }
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
    }

    private void CheckWallSlide() {
        if(onGround) {
            wallSliding = false;
            return;
        }

        wallSliding = Physics2D.OverlapCircle(wallDetector.position, 0.1f, groundMask);
        if(wallSliding) {
            moveDir.y = -GlobalVariables.GRAVITY / 2;
            if(Input.GetKey(KeyCode.Space)) {
                jumpTimer = 0;
                moveDir.y = jumpForce;
                moveDir.x = -transform.localScale.x * moveSpeed / 1.5f;
                wallSliding = false;
                currentCo = CoBlockInput(0.15f);
                StartCoroutine(currentCo);
            }
        }
    }

    private void CheckDash() {
        if(dashing) {
            rb.velocity = new Vector2(transform.localScale.x * (moveSpeed * 2), 0);
        }

        if(!dashing && Input.GetKeyDown(KeyCode.K) && canDoAction && canDash) {
            currentCo = CoDash();
            StartCoroutine(currentCo);
        }
    }

    private IEnumerator CoDash() {
        dashing = true;
        canDash = false;
        canDoAction = false;
        yield return new WaitForSeconds(DASH_TIME);
        dashing = false;
        currentCo = CoActionTime();
        StartCoroutine(currentCo);
    }

    private IEnumerator CoActionTime() {
        yield return new WaitForSeconds(TIME_BETWEEN_ACTIONS);
        canDoAction = true;
    }

    private IEnumerator CoBlockInput(float time) {
        inputBlocked = true;
        yield return new WaitForSeconds(time);
        inputBlocked = false;
    }

    private void UpdateAnimations() {
        anim.SetBool("moving", xAxis != 0);
        anim.SetBool("onGround", onGround);
        anim.SetFloat("yAxis", rb.velocity.y);
        anim.SetBool("dashing", dashing);
        anim.SetBool("wallSliding", wallSliding);
    }
}