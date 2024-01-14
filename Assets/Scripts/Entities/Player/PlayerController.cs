using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private static float TIME_BETWEEN_ACTIONS = 0.3f;
    private static float DASH_TIME = 0.3f;
    private static float JUMP_TIME = 0.3f;
    private static float FLINCH_TIME = 0.5f;

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
    private Transform ledgeDetector;
    [SerializeField]
    private Transform upperLedgeDetector;
    [SerializeField]
    private LayerMask groundMask;

    private Vector2 moveDir;
    private float xAxis;
    private bool onGround;
    private bool dashing;
    private bool wallSliding;
    private bool grabedToLedge;
    private bool flinching;
    private bool canDash;
    private bool canDoAction;
    private bool inputBlocked;
    private bool canDoubleJump;
    private float jumpTimer;
    private IEnumerator currentCo;

    private Rigidbody2D rb;
    private Animator anim;
    private EntityHealthController entityHealthController;

    public void Start() {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        entityHealthController = GetComponent<EntityHealthController>();

        dashing = false;
        canDash = true;
        canDoAction = true;
        canDoubleJump = false;
        inputBlocked = false;
        grabedToLedge = false;
        flinching = false;

        jumpTimer = JUMP_TIME;
    }

    public void Update() {
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

        rb.velocity = moveDir;
        if(grabedToLedge) rb.velocity = new Vector2(rb.velocity.x, 0);
    }

    private void CheckJump() {
        onGround = Physics2D.OverlapCircle(groundDetector.position, 0.1f, groundMask);
        if(onGround) {
            jumpTimer = JUMP_TIME;
            canDash = true;
            canDoubleJump = true;
            if(Input.GetKeyDown(KeyCode.Space)) {
                moveDir.y = jumpForce;
                onGround = false;
            }
            CheckAttack();
        } else {
            if(!grabedToLedge) moveDir.y -= GlobalVariables.GRAVITY * Time.deltaTime;
            if(rb.velocity.y < 0) {
                jumpTimer = 0;
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
            if(Input.GetKeyDown(KeyCode.Space) && canDoubleJump) {
                    canDoubleJump = false;
                    moveDir.y = jumpForce;
                }
        }
        CheckLedge();
        CheckWallSlide();
    }

    private void CheckWallSlide() {
        if(onGround) {
            wallSliding = false;
            return;
        }

        wallSliding = Physics2D.OverlapCircle(wallDetector.position, 0.1f, groundMask) && !grabedToLedge && !onGround;
        if(wallSliding) {
            moveDir.y = -GlobalVariables.GRAVITY / 3;
            if(Input.GetKeyDown(KeyCode.Space)) {
                jumpTimer = 0;
                moveDir.y = jumpForce;
                moveDir.x = -transform.localScale.x * moveSpeed / 1.5f;
                wallSliding = false;
                currentCo = CoBlockInput(0.15f);
                StartCoroutine(currentCo);
            }
        }
    }

    private void CheckLedge() {
        grabedToLedge = Physics2D.OverlapCircle(ledgeDetector.position, 0.1f, groundMask) &&
                            !Physics2D.OverlapCircle(upperLedgeDetector.position, 0.1f, groundMask) && !onGround;

        if(grabedToLedge) {
            rb.gravityScale = 0;
            if(Input.GetKeyDown(KeyCode.Space)) {
                rb.gravityScale = 1;
                moveDir.y = jumpForce * 2;
                grabedToLedge = false;
            }
        } else {
            rb.gravityScale = 1;
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

    private void CheckAttack() {
        if(Input.GetKeyDown(KeyCode.J)) {
            moveDir.x = 0;
            xAxis = 0;
            anim.SetTrigger("attack");
            currentCo = CoBlockInput(0.6f);
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
        flinching = false;
    }

    private void UpdateAnimations() {
        anim.SetBool("moving", xAxis != 0);
        anim.SetBool("onGround", onGround);
        anim.SetFloat("yAxis", rb.velocity.y);
        anim.SetBool("dashing", dashing);
        anim.SetBool("wallSliding", wallSliding);
        anim.SetBool("grabedToLedge", grabedToLedge);
    }

    public void HurtPlayer(int damage, Vector2 damagePos) {
        if(flinching) return;
        else flinching = true;
        float flinchDir = (damagePos.x - transform.position.x) < 0 ? 1 : -1;

        entityHealthController.SubstractHealth(damage);
        currentCo = CoBlockInput(FLINCH_TIME);
        anim.SetTrigger("flinch");
        rb.velocity = new Vector2(flinchDir * moveSpeed, 2);
        
        StartCoroutine(currentCo);
    }
}