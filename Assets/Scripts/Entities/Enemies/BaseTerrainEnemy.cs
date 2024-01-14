using System;
using System.Collections;
using System.Collections.Generic;
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

    private Transform playerTransform;
    
    private bool playerInRange;
    private Vector2 originPos;

    public new void Start() {
        base.Start();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        originPos = transform.position;
    }

    public void Update() {
        playerInRange = Math.Abs(transform.position.x - playerTransform.position.x) < detectionRange;
        UpdateMovement();

        if(getRb().velocity.x != 0) {
            transform.localScale = new Vector3(defaultLookDir * ((getRb().velocity.x > 0) ? 1 : -1), 1, 1);
        }

        UpdateAnimations();
    }

    public void UpdateMovement() {
        if(playerInRange) {
            getRb().velocity = new Vector2(((transform.position.x - playerTransform.position.x) < 0) ? moveSpeed : -moveSpeed, 0);
        } else {
            if(Math.Abs(transform.position.x - originPos.x) > 1) {
                getRb().velocity = new Vector2(((transform.position.x - originPos.x) < 0) ? moveSpeed : -moveSpeed, 0);
            }
        }
    }  
}
