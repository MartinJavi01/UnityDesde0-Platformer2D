using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField]
    private float moveSpeed;

    private Vector2 moveDir;
    private float xAxis;
    private Rigidbody2D rb;

    public void Start() {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Update() {
        UpdateMovement();
    }

    private void UpdateMovement() {
        moveDir = rb.velocity;
        xAxis = Input.GetAxisRaw("Horizontal");
        moveDir.y -= GlobalVariables.GRAVITY * Time.deltaTime;
        moveDir.x = xAxis * moveSpeed;
        rb.velocity = moveDir;
    }
}
