using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
public class PlayerController : MonoBehaviourPun,IDamageable
{
    private Rigidbody2D rb;
    private Animator anim;
    private FixedJoystick joystick;
    private Button jumpButton;
    private Button attackButton;
    public Text nameText;

    private SpriteRenderer renderer;
    public float speed;
    public float jumpForce;
    [Header("Player State")]
    public float health;
    public bool isDead;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float checkRadius;
    public LayerMask groundLayer;

    [Header("States Check")]
    public bool isGround;
    [SerializeField]
    private bool canJump;
    public bool isJump;

    [Header("Jump FX")]
    public GameObject jumpFX;
    public GameObject landFX;

    [Header("Attack Settings")]
    public GameObject bombPrefab;
    public float nextAttack;   //下次攻击时间，是一个计数器
    public float attackRate;    //攻击间隔时间，是一个固定值

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        joystick = FindObjectOfType<FixedJoystick>();
        renderer = GetComponent<SpriteRenderer>();

        if(photonView.IsMine)
        {
            nameText.text = PhotonNetwork.NickName;
        }
        else
        {
            nameText.text = photonView.Owner.NickName;
        }
        health = GameManager.instance.LoadHealth();
        GameManager.instance.IsPlayer(this);
        jumpButton = joystick.GetComponentsInChildren<Button>()[0];
        attackButton = joystick.GetComponentsInChildren<Button>()[1];
        jumpButton.onClick.AddListener(ButtonJump);
        attackButton.onClick.AddListener(Attack);
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine && PhotonNetwork.IsConnected)
            return;
        anim.SetBool("dead", isDead);
        if (isDead)
            return;
        CheckInput();
        
    }

    private void CheckInput()
    {
        if (Input.GetButtonDown("Jump") && isGround)
            canJump = true;
        if (Input.GetKeyDown(KeyCode.J))
            Attack();
    }

    private void FixedUpdate()
    {
        if (!photonView.IsMine && PhotonNetwork.IsConnected)
            return;

        if(isDead)
        {
            rb.velocity = Vector2.zero;
            return;
        }
        PhysicsCheck();
        Movement();
        Jump();
        
    }
    void Movement()
    {
        //float horizontalInput = Input.GetAxisRaw("Horizontal");
        float horizontalInput = joystick.Horizontal;
        rb.velocity = new Vector2(horizontalInput * speed, rb.velocity.y);
        //面孔朝向的翻转
        //if (horizontalInput != 0)
        //    transform.localScale = new Vector3(horizontalInput, 1, 1);
        if (horizontalInput > 0)
            renderer.flipX = false;//transform.eulerAngles = new Vector3(0, 0, 0);
        else
            renderer.flipX = true;
    }
    void Jump()
    {
        if (canJump)
        {
            isJump = true;
            jumpFX.SetActive(true);
            jumpFX.transform.position = transform.position + new Vector3(0, -0.45f, 0);
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            rb.gravityScale = 4;
            canJump = false;
        }
    }

    public void ButtonJump()
    {
        if(isGround)
            canJump = true;
    }

    public void Attack()
    {
        if(Time.time>nextAttack)
        {
            Instantiate(bombPrefab, transform.position+Vector3.up*0.1f, bombPrefab.transform.rotation);
            nextAttack = Time.time + attackRate;
        }
    }

    void PhysicsCheck()
    {
        isGround = Physics2D.OverlapCircle(groundCheck.position, checkRadius,groundLayer);
        if (isGround)
        {
            rb.gravityScale = 1;
            isJump = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
    }

    public void LandFX() //Animation Event
    {
        landFX.SetActive(true);
        landFX.transform.position = transform.position + new Vector3(0, -0.7f, 0);
    }

    public void GetHit(float damage)
    {
        if (!photonView.IsMine && PhotonNetwork.IsConnected)
            return;
        //如果还在播放受伤动画，就短暂无敌
        if(!anim.GetCurrentAnimatorStateInfo(1).IsName("player_hit"))
        {
            health -= damage;
            if (health <= 0)
            {
                health = 0;
                isDead = true;
            }
            anim.SetTrigger("hit");
            UIManager.instance.UpdateHealth(health);
        }
        
       
    }
}
