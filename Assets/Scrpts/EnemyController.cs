using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    

    [SerializeField] float attackRange = 0.5f, attackRate = 0.5f;
    [SerializeField] Transform attackPoint;
    [SerializeField] LayerMask playerLayer;
    float nextAttack = 0f;
    [SerializeField] int hitAmount = 30;

    Animator enemyAnimator;

    [SerializeField] AudioSource deathSound;
    [SerializeField] int maxHealt = 100;
    int currentHealt;

    private void Awake()
    {
        enemyAnimator = GetComponent<Animator>();
    }

    void Start()
    {
        currentHealt = maxHealt;   
       
    }

    public void TakeDamage(int damageAmount)
    {
        
        enemyAnimator.SetTrigger("takeHit");
        currentHealt -=damageAmount;
        Die(currentHealt);
    }

    void Die(int healt)
    {
        if (healt<=0)
        {
            //deathSound.Play();
            enemyAnimator.SetBool("isDie", true);
            GetComponent<Collider2D>().enabled = false;
            this.enabled = false;
        }
        else
        {
            //enemyAnimator.SetTrigger("TakeDamage");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag=="Reloader")
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            CanAttack();
            enemyAnimator.SetTrigger("Attack");
            
        }
    }

    void CanAttack()
    {
        if (Time.time >= nextAttack)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Attack();
                nextAttack = Time.time + 1f / attackRate;
            }
        }
    }

    void Attack()
    {
        Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, playerLayer);

        foreach (Collider2D hero in hitPlayer)
        {
            Debug.Log("i hit to" + hero.name);
            hero.GetComponent<PlayerController>().TakenDamage(hitAmount);
        }
    }

    private void OnDrawGizmos()
    {
           Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
