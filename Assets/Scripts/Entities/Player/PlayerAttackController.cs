using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackController : MonoBehaviour
{
    [SerializeField]
    private int attackValue;

    public void OnTriggerEnter2D(Collider2D col) {
        if(col.CompareTag("Enemy") && !col.isTrigger) {
            col.GetComponent<BaseEnemyController>()
            .HurtEnemy(attackValue, transform.position);
        }
    }
}
