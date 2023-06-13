using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using Unity.VisualScripting;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D myRigidBody;
    Transform myTransform;
    Animator myAnimator;
    TrailRenderer myTrailRenderer;

    [SerializeField] int heroHealt = 200;
    [SerializeField]  int currentHP;

    // run and jump
    [SerializeField] float speed = 7, jumpSpeed=700f,groundPointRange=0.1f,jumpFrequency=1f,nextJumpTime;
    [SerializeField] Transform groundCheckPoint;
    [SerializeField] LayerMask groundCheckLayer;
    bool isGrounded;
    string yon;
    float yatay;

    //attack
    [SerializeField] float attackRange = 0.5f, attackRate = 0.5f;
    [SerializeField] Transform attackPoint;
    [SerializeField] LayerMask enemyLayer;
    float  nextAttack = 0f;
    [SerializeField] int hitAmount = 25;

    //dash
    private bool isDashing, canDash = true;
    private float dashingTime=1f, dashingCooldown=0.2f, dashingPower=24f;

    //door open
    [SerializeField] GameObject closedButton,openedButton,enterDoor;

    //Chest open keys & golds
    [SerializeField] GameObject keyChest, goldChest, keyImage;
    [SerializeField] TextMeshProUGUI topGold;
    int goldAmount=0;
    bool keyTaken=false;

    //teleport
    GameObject currentTeleporter;

    //key warning
    [SerializeField] GameObject keyWarning;


    private void Awake()
    {
        myAnimator = GetComponent<Animator>();
        myTransform = GetComponent<Transform>();
        myRigidBody = GetComponent<Rigidbody2D>();
        myTrailRenderer = GetComponent<TrailRenderer>();
    }


    void Start()
    {
        
        currentHP = heroHealt;
        topGold.text=goldAmount.ToString();
        keyImage.SetActive(false);
        yon = "right";
    }


    void Update()
    {
        if (isDashing)
        {
            return;
        }
        yatay = Input.GetAxis("Horizontal");
        PlayerMovement(yatay);
        InputManager();
        OnGroundCheck();
        Teleporter();
    }
    private void FixedUpdate()
    {
        if (isDashing)
        {
            return;
        }
        myRigidBody.velocity = new Vector2(yatay * speed * Time.deltaTime, myRigidBody.velocity.y);
    }

    void InputManager()
    {
        
        if (Time.time >= nextAttack)
        { 
            if (Input.GetMouseButtonDown(0))
            {
                Attack();
                nextAttack = Time.time+1f/attackRate;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space)&& isGrounded&&(nextJumpTime<=Time.time))
        {
            nextJumpTime= Time.time+jumpFrequency;
           Jump();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift)&&canDash)
        {
            StartCoroutine(Dash());
        }
    }

    void PlayerMovement(float yatay)
    {
        #region yön belirleme
        if (yatay > 0 && yon == "left")
        {
            yon = "right";
            myTransform.localScale = new Vector2(myTransform.localScale.x * -1, myTransform.localScale.y);
        }
        else if (yatay < 0 && yon == "right")
        {
            yon = "left";
            myTransform.localScale = new Vector2(myTransform.localScale.x * -1, myTransform.localScale.y);

        }
        myAnimator.SetFloat("Speed", Mathf.Abs(yatay));
        #endregion    
    }

    void Attack()
    {
        myAnimator.SetTrigger("Attack");

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange,enemyLayer);

        foreach (Collider2D enemy in hitEnemies)
        {
            Debug.Log("i hit to"+enemy.name);
            enemy.GetComponent<EnemyController>().TakeDamage(hitAmount);
        }
    }

    public void TakenDamage(int hitAmount)
    {
        currentHP-=hitAmount;
        HeroDeath(currentHP);

    }

    void HeroDeath(int hp)
    {
        if (hp<=0)
        {
            myAnimator.SetBool("isDie", true);
        }
        else { return; }
    }

    void Jump()
    {
        myRigidBody.velocity=new Vector2(myRigidBody.velocity.x,jumpSpeed);
        
    }

    void OnGroundCheck()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheckPoint.position, groundPointRange,groundCheckLayer);
        myAnimator.SetBool("isGround", isGrounded);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Coin")
        {

            goldAmount += 20;
            topGold.text=goldAmount.ToString();
            FindObjectOfType<AudioManager>().Play("gold");
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.tag=="Door")
        {
            
            if (SceneManager.GetActiveScene().buildIndex + 1<=2 && keyTaken)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

            }
            else
            {
                
                keyWarning.GetComponent<Animator>().SetTrigger("noKey");
                

            }
            

        }

        if (collision.gameObject.tag=="Reloader")
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        if (collision.gameObject.tag=="OpenButton")
        {
            closedButton.SetActive(false);
            openedButton.SetActive(true);
            enterDoor.gameObject.GetComponent<Animator>().SetBool("isOpenEnterDoor", true);
            
        }
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name=="KeyChest")
        {
            keyChest.GetComponent<Animator>().SetBool("isOpenKeyChest", true);
            keyImage.SetActive(true);
            FindObjectOfType<AudioManager>().Play("KeyChest");
            keyTaken = true;
        }
        if (collision.name == "GoldChest")
        {
            goldChest.GetComponent<Animator>().SetBool("isOpenGoldChest", true);
            goldAmount += 100;
            FindObjectOfType<AudioManager>().Play("GoldChest");
            topGold.text = goldAmount.ToString();
        }
        if (collision.CompareTag("Teleporter"))
        {
            currentTeleporter = collision.gameObject;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Teleporter"))
        {
            if (collision.gameObject == currentTeleporter)
            {
                currentTeleporter = null;
            }
        }
    }

    void Teleporter()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (currentTeleporter != null)
            {
                transform.position = currentTeleporter.GetComponent<Teleporter>().GetDestination().position;
            }
        }
    }

    IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        float originalGravity = myRigidBody.gravityScale;
        myRigidBody.gravityScale = 0f;
        myRigidBody.velocity = new Vector2(transform.localScale.x * dashingPower, 0f);
        myTrailRenderer.emitting = true;
        yield return new WaitForSeconds(dashingTime);
        myTrailRenderer.emitting = false;
        myRigidBody.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }

}
