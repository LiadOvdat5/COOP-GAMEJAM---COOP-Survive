using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroKnightController : MonoBehaviour, Hittable
{
    public float runSpeed = 4f;
    public float jumpSpeed = 8f;
    public float rollSpeed = 6f;

    //Health & Damage
    [SerializeField] private float health = 100;
    public float damage = 20;
    public Bar healthBar;

    //Attack
    public float attackDelayTime = .5f;
    public Transform attackSensor;
    public float attackRange = 5f;
    public LayerMask enemysLayers;
    private bool blocking = false;

    //Ground Check
    public bool isGrounded = false;
    public Transform groundSensor;
    private float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    //Flip
    private float faceing = 1; //Starts facing right
    
    //Roll
    public float rollDelayTime = .7f;
    //Delay
    public bool canDo = true;


    //Components
    private ControlsMap controlsMap;
    private Animator myAnimator;
    private Rigidbody2D rb;
    private SceneLoader sceneLoader;
    private AudioManager audioManager;

    private void Awake()
    {
        controlsMap = new ControlsMap();
        myAnimator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        sceneLoader = FindObjectOfType<SceneLoader>();
        audioManager = FindObjectOfType<AudioManager>();
    }

    private void OnEnable()
    {
        controlsMap.Enable();
    }

    private void OnDisable()
    {
        controlsMap.Disable();
    }

    void Start()
    {
        controlsMap.HeroKnightMap.Jump.performed += _ => Jump(); // "_" for not passing any value
        controlsMap.HeroKnightMap.Roll.performed += _ => Roll();
        controlsMap.HeroKnightMap.Attack.performed += _ => Attack();
        controlsMap.HeroKnightMap.Block.performed += _ => Block();
        

        healthBar.SetMaxValue((int)health);
    }

    void Update()
    {
        GroundedCheck();
        SideMove();
        

        //set animator falling float to Y velocity
        myAnimator.SetFloat("AirSpeedY", rb.velocity.y);
    }

    private void SideMove()
    {
        if (canDo)
        {
            myAnimator.SetBool("Grounded", isGrounded);
            float movementInput = controlsMap.HeroKnightMap.Move.ReadValue<float>();
            Vector3 currentPos = transform.position;
            currentPos.x += movementInput * runSpeed * Time.deltaTime;
            transform.position = currentPos;

            if (isGrounded && movementInput != 0)
            {
                myAnimator.SetInteger("AnimState", 1);
            }
            else
            {
                myAnimator.SetInteger("AnimState", 0);
            }
            if (faceing != movementInput && movementInput != 0) //Flip
            {
                faceing = movementInput;
                transform.Rotate(0, 180f, 0);
            }
        }
    }

    private void GroundedCheck()
    {
        isGrounded = false;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundSensor.position, groundCheckRadius, groundLayer);
        isGrounded = colliders.Length > 0;
    }

    private void Jump()
    {
        if (isGrounded)
        {
            rb.AddForce(new Vector2(0, jumpSpeed), ForceMode2D.Impulse);
            myAnimator.SetTrigger("Jump");
            audioManager.Play("Jump");
        } 
    }
  
    private void Roll()
    {
        if (isGrounded && canDo)
        {
            canDo = false;
            blocking = true;
            audioManager.Play("Roll");

            rb.AddForce(new Vector2(rollSpeed*faceing, 0), ForceMode2D.Impulse);
            myAnimator.SetTrigger("Roll");

            StartCoroutine(DelayBySecond(rollDelayTime));
            StartCoroutine(DelayBlock());
        }
    }

    private void Block()
    {
        if (canDo && !blocking)
        {
            
            blocking = true;
            myAnimator.SetTrigger("Block");
            myAnimator.SetBool("IdleBlock", true);
            StartCoroutine(DelayBlock());
            canDo = false;
            StartCoroutine(DelayBySecond(1.5f));
            

        }
    }
    IEnumerator DelayBlock()
    {
        yield return new WaitForSeconds(1);
        myAnimator.SetBool("IdleBlock", false);
        yield return new WaitForSeconds(2);
        blocking = false;
    }

    IEnumerator DelayBySecond(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        canDo = true;
    }


    private void Attack()
    {
        if (isGrounded && canDo)
        {
            canDo = false;
            
            int ran = Random.Range(1, 4);
            myAnimator.SetTrigger("Attack" + ran);

            audioManager.Play("Sword");
            
            Collider2D[] colliders = Physics2D.OverlapCircleAll(attackSensor.position, attackRange, enemysLayers);
            
            foreach(Collider2D enemy in colliders)
            {
                enemy.GetComponent<Enemy>().TakeHit(damage);
            }

            StartCoroutine(DelayBySecond(attackDelayTime));
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(attackSensor.position, attackRange);
    }

    public void TakeHit(float damage)
    {
        if (!blocking)
        {
            health -= damage;
            myAnimator.SetTrigger("Hurt");
            healthBar.SetValue((int)health);

            if (health <= 0)
            {
                canDo = false;
                myAnimator.SetTrigger("Death");
                sceneLoader.someoneDied = true;
                Destroy(gameObject, 1.5f);

            }
        }
    }

   
}
