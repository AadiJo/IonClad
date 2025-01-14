using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{

    public Animator animator;
    bool isAttacking = false;
    public Transform attackPoint;
    public float attackRange = 0.5f;

    private Rigidbody2D rb;

    public LayerMask enemyLayers;
    public LayerMask bossLayers;

    public PlayerMovement playerMovement;
    private GameManager gameManager;

    public int attackDamage = 10;
    private PlayerHealth health;

    void Start()
    {

        PauseMenu.GameisPaused = false;
        health = GetComponent<PlayerHealth>();
        rb = GetComponent<Rigidbody2D>();
        gameManager = FindObjectOfType<GameManager>();

    }

    void Update()
    {

        if (Input.GetButtonDown("Fire1"))
        {

            Attack();

        }

        if (isAttacking)
        {

            playerMovement.canMove = false;

        }
        else
        {

            playerMovement.canMove = true;

        }


    }

    private void Attack()
    {

        if (playerMovement.canMove && !PauseMenu.GameisPaused)
        {

            if (!animator.GetBool("isJumping") && !animator.GetBool("isFalling"))
            {
                //Play animation
                int attackRandomizer = Random.Range(0, 2);
                animator.SetInteger("attackRandomizer", attackRandomizer);
                if (attackRandomizer == 0)
                {

                    attackPoint.transform.position = new Vector2(attackPoint.position.x, transform.position.y - 0.2f);

                }
                else if (attackRandomizer == 1)
                {

                    attackPoint.transform.position = new Vector2(attackPoint.position.x, transform.position.y - 0.8f);

                }
                FindObjectOfType<AudioManager>().Play("PlayerAttack");
                animator.SetTrigger("attack");
                StartCoroutine(stopMovement());

                // Detect enemies in range

                Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
                Collider2D[] hitBosses = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, bossLayers);

                // Damage
                foreach (Collider2D enemy in hitEnemies)
                {

                    enemy.GetComponent<Enemy>().TakeDamage(attackDamage);

                }

                foreach (Collider2D boss in hitBosses)
                {
                    if (boss.GetComponent<Boss>() != null)
                    {

                        boss.GetComponent<Boss>().TakeDamage(attackDamage);

                    }
                    else
                    {

                        boss.GetComponent<BigBoss>().TakeDamage(attackDamage);

                    }


                }
            }


        }



    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
        {

            return;

        }

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);

    }

    IEnumerator stopMovement()
    {
        isAttacking = true;
        yield return new WaitForSeconds(0.5f);
        isAttacking = false;

    }

    private void OnCollisionEnter2D(Collision2D other)
    {

        if (other.gameObject.layer == 9)
        {

            health.TakeDamage(1, new float[] { 2000, 200 });
            if (health.currentHealth <= 0)
            {

                gameManager.killerName = "SPIKE";
                //Debug.Log("Death by SPIKE");

            }

        }

    }
}
