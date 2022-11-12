using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, Hittable
{
    public float runSpeed = 0.5f;
    [SerializeField]
    private float health = 100f;
    
   

    //Attack
    public Transform attackSensor;
    public float attackDelayTime = 3f;
    public float attackRange = 5f;
    public LayerMask playerLayers;
    public float damage = 20f;

    //Detect & follow
    public Transform detectionSensor;
    public float detectRange = 12f;
    private bool keepFollow = true;
    private bool canAttack = true;
    //Flip
    private float faceing = 1; //Starts facing right



    //Components
    private Animator myAnimator;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        myAnimator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        FollowHero();
        AttackHero();
    }

    private void FollowHero()
    {
       
        Collider2D[] playersColliders = Physics2D.OverlapCircleAll(detectionSensor.position, detectRange, playerLayers);
        if (playersColliders.Length > 0 && keepFollow)
        {
            Transform targetTranform = playersColliders[0].GetComponent<Transform>();
            if (faceing != Mathf.Sign(targetTranform.position.x - transform.position.x))
            {
                faceing = Mathf.Sign(targetTranform.position.x - transform.position.x);
                transform.Rotate(0, 180f, 0);
            }

            transform.position = Vector2.MoveTowards(transform.position, targetTranform.position, runSpeed * Time.deltaTime);
            myAnimator.SetBool("IsWalking", true);
        }
        else
            myAnimator.SetBool("IsWalking", false);
       
    }


    private void AttackHero()
    {
        Collider2D[] playersColliders = Physics2D.OverlapCircleAll(attackSensor.position, attackRange, playerLayers);
        if (playersColliders.Length > 0 )
        {
            keepFollow = false;
           

            if (canAttack)
            {
                canAttack = false;
                FindObjectOfType<AudioManager>().Play("Sword");

                spriteRenderer.color = Color.black;
                StartCoroutine(ChangeColor());

                playersColliders[0].GetComponent<Hittable>().TakeHit(damage);

                int ran = Random.Range(1, 3);
                myAnimator.SetTrigger("Attack" + ran);
                StartCoroutine(DelayBySecond(attackDelayTime));
            }
        }
        else
            keepFollow = true;

        
    }




    IEnumerator DelayBySecond(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        canAttack = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(detectionSensor.position, detectRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackSensor.position, attackRange);
    }

    public void TakeHit(float damage)
    {
        keepFollow = false;
        canAttack = false;
        StartCoroutine(DelayBySecond(attackDelayTime));
        health -= damage;
        myAnimator.SetTrigger("TakeHit");

        if (health <= 0)
        {
            myAnimator.SetBool("Death", true);
            Destroy(gameObject, 0.5f);
        }
        
    }

    IEnumerator ChangeColor()
    {
        while (spriteRenderer.color != Color.white)
        {
            yield return null;
            spriteRenderer.color = Color.Lerp(spriteRenderer.color, Color.white, Time.deltaTime * 2);
        }

    }
}
