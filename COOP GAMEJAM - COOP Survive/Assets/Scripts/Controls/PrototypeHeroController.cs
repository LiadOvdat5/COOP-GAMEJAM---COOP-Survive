using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrototypeHeroController : MonoBehaviour, Hittable
{
    public float runSpeed = 2f;
    public float jumpSpeed = 8f;

    //Health & Damage
    [SerializeField] 
    private float health = 100;
    public float damage = 20;
    public Bar healthBar; 

    //Attack
    private bool slowMoOn = false;
    private float slowMoSpent = 0.5f;
    private float slowMoLoad = 0.25f;
    private bool canSlow = true;
    [SerializeField] private float slowMoTimeLeft = 1000;
    [SerializeField] private float timeScale = 0.5f;
    [SerializeField] private float slowCoolDownSec = 3f;
    public Bar slowBar;

    private HeroKnightController heroKnight;


    //Ground Check
    public bool isGrounded = false;
    public Transform groundSensor;
    private float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    //Move
    private float mousePosX;
    private bool moving = false;

    //Flip
    private float faceing = 1; //Starts facing right


    //Sound
    private bool playSound = true;

    //Components
    private ControlsMap controlsMap;
    private Animator myAnimator;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private SceneLoader sceneLoader;
    private AudioManager audioManager;



    private void Awake()
    {
        controlsMap = new ControlsMap();
        myAnimator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        heroKnight = FindObjectOfType<HeroKnightController>();
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
        controlsMap.PrototypeMap.Jump.performed += _ => Jump(); // "_" for not passing any value
        controlsMap.PrototypeMap.Move.performed += _ => moving = true;
        controlsMap.PrototypeMap.Move.performed += _ => MouseInput();

        healthBar.SetMaxValue((int)health);
        slowBar.SetMaxValue((int)slowMoTimeLeft);
        
    }

    private void Update()
    {
        GroundedCheck();
        SideMove();
        SlowMoBar();
        Attack();

        //set animator falling float to Y velocity
        myAnimator.SetFloat("AirSpeedY", rb.velocity.y);


    }

    private void GroundedCheck()
    {
        isGrounded = false;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundSensor.position, groundCheckRadius, groundLayer);
        isGrounded = colliders.Length > 0;
        myAnimator.SetBool("Grounded", isGrounded);
    }

    private void SideMove()
    {
        if (moving)
        {

            //impliment the running with Animation
            Vector2 targetPos = new Vector2(mousePosX, transform.position.y);

            if (isGrounded && targetPos != (Vector2)transform.position)
            {
                transform.position = Vector2.MoveTowards(transform.position, targetPos, runSpeed * Time.deltaTime);
                myAnimator.SetInteger("AnimState", 1);
            }
            else
            {
                myAnimator.SetInteger("AnimState", 0);
                moving = false;
            }

            if (faceing != Mathf.Sign(mousePosX - transform.position.x) && targetPos != (Vector2)transform.position) //Flip
            {
                faceing = Mathf.Sign(mousePosX - transform.position.x);
                transform.Rotate(0, 180f, 0);
            }
        }
    }

    private void MouseInput()
    {
        //taking the X position of the mouse
        Vector2 mousePos = controlsMap.PrototypeMap.MouseInput.ReadValue<Vector2>();
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);
        mousePosX = mousePos.x;
    }


    private void Jump()
    {
        if (isGrounded)
        {
            audioManager.Play("Jump");
            rb.AddForce(new Vector2(faceing*3, jumpSpeed), ForceMode2D.Impulse);
            myAnimator.SetTrigger("Jump");
        }
    }

    private void Attack()
    {
        if (slowMoTimeLeft > 2 && canSlow)
            slowMoOn = controlsMap.PrototypeMap.Attack.ReadValue<float>() == 1;
        else if (slowMoTimeLeft <= 2)
        {
            canSlow = false;
            slowMoOn = false;
            StartCoroutine(DelayBySecond(slowCoolDownSec));
        }

        if(!slowMoOn)
        {
            Time.timeScale = 1;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            playSound = true;

        }
        else if(slowMoOn) 
        {                            
            Time.timeScale = timeScale;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;

            if(playSound == true)
            {
                playSound = false;
                audioManager.Play("Slow Down");

            }
            


        }



    }

    private void SlowMoBar()
    {
        if (slowMoOn && slowMoTimeLeft > 0)
            slowMoTimeLeft -= slowMoSpent;
        else if(slowMoTimeLeft < 1000)
            slowMoTimeLeft += slowMoLoad;

        slowBar.SetValue((int)slowMoTimeLeft);

    }

    public void TakeHit(float damage)
    {
        health -= damage;
        healthBar.SetValue((int)health);

        spriteRenderer.color = Color.red;
        StartCoroutine(GetHit());

        if (health <= 0)
        {
            isGrounded = false;
            canSlow = false;

            spriteRenderer.color = Color.black;
            StartCoroutine(GetHit());

            sceneLoader.someoneDied = true;
            Destroy(gameObject, 1.5f);
            

        }
    }

    IEnumerator DelayBySecond(float delayTime)
    {

        yield return new WaitForSeconds(delayTime);
        canSlow = true;
    }

    IEnumerator GetHit()
    {
        while(spriteRenderer.color != Color.white)
        {
            yield return null;
            spriteRenderer.color = Color.Lerp(spriteRenderer.color, Color.white, Time.deltaTime * 2);
        }
        
    }

    
}
