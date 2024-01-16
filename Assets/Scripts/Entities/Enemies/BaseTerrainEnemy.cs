using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class BaseTerrainEnemy : BaseEnemyController
{
    [Header("Detectors")]
    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private float detectionRange;
    [SerializeField]
    private Transform groundDetector;
    [SerializeField]
    private LayerMask groundMask;

    [Header("Attack fields")]
    [SerializeField]
    private float attackTime;
    [SerializeField]
    private int attackDamage;
    [SerializeField]
    private int attackDistance;

    private Transform playerTransform;
    
    private bool playerInRange;
    private bool attacking;
    private int distanceToPlayer;
    private Vector2 originPos;

    public new void Start() {
        base.Start();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        originPos = transform.position;
        attacking = false;
    }

    public void Update() {
        if(attacking || flinching || entityHealthController.getCurrentHealth() == 0) {
            rb.velocity = new Vector2(0, rb.velocity.y);
        } else {
            distanceToPlayer = (int)Math.Abs(transform.position.x - playerTransform.position.x);
            playerInRange = Enumerable.Range(attackDistance, (int)detectionRange).Contains(distanceToPlayer);
            UpdateMovement();

            if(getRb().velocity.x != 0) {
                transform.localScale = new Vector3(defaultLookDir * ((getRb().velocity.x > 0) ? 1 : -1), 1, 1);
        }

        UpdateAnimations();
        }
    }

    public void UpdateMovement() {
        if(playerInRange) {
            rb.velocity = new Vector2(((transform.position.x - playerTransform.position.x) < 0) ? moveSpeed : -moveSpeed, 0);
            if(!Physics2D.OverlapCircle(groundDetector.transform.position, 0.1f, groundMask)) {
                rb.velocity = Vector2.zero;
            }
        } else {
            if (distanceToPlayer > attackDistance) {
                if( Math.Abs(transform.position.x - originPos.x) > 1) {
                    getRb().velocity = new Vector2(((transform.position.x - originPos.x) < 0) ? moveSpeed : -moveSpeed, 0);
                }
            } else {
                rb.velocity = new Vector2(0, rb.velocity.y);
                if(!attacking) {
                    StartCoroutine(CoAttack());
                }
            }
        }
    }  

    public IEnumerator CoAttack() {
        attacking = true;
        anim.SetTrigger("attack");
        yield return new WaitForSeconds(attackTime);
        attacking = false;
    }

    public void OnTriggerEnter2D(Collider2D col) {
        if(col.CompareTag("Player")) {
            col.GetComponent<PlayerController>().HurtPlayer(attackDamage, transform.position);
        }
    }
}
