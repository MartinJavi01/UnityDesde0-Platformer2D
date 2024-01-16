using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemyController : MonoBehaviour
{
    protected Rigidbody2D rb;
    protected EntityHealthController entityHealthController;
    protected Animator anim;

    protected bool flinching;

    [Header("Protected fields")]
    public float defaultLookDir;
    [SerializeField]
    private float flinch_time;
    [SerializeField]
    private float death_time;  

    public void Start() {
        rb = GetComponent<Rigidbody2D>();
        entityHealthController = GetComponent<EntityHealthController>();
        anim = GetComponent<Animator>();

        flinching = false;
    }
    
    public void HurtEnemy(int damage, Vector2 damagePos) {
        if (flinching) return;
        StartCoroutine(CoFlinch(flinch_time));
        entityHealthController.SubstractHealth(damage);
        if (entityHealthController.getCurrentHealth() == 0) {
            anim.SetTrigger("death");
            StartCoroutine(CoDeath(death_time));
        } else {
            anim.SetTrigger("flinch");
            float flinchDir = (damagePos.x - transform.position.x) < 0 ? 1 : -1;
            rb.velocity = new Vector2(flinchDir * 2, 2);
        }
    }

    private IEnumerator CoFlinch(float time) {
        flinching = true;
        yield return new WaitForSeconds(time);
        flinching = false;
    }

    private IEnumerator CoDeath(float time) {
        flinching = true;
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }

    public void UpdateAnimations() {
        anim.SetBool("moving", rb.velocity.x != 0);
    }

    public Rigidbody2D getRb() {
        return rb;
    }
}
