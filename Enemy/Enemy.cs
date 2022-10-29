using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Enemy : MonoBehaviour
{
    //状态机的控制
    EnemyBaseState currentState;
    public PatrolState patrolState = new PatrolState();
    public AttackState attackState = new AttackState();

    //警告标识
    private GameObject alarmSign;

    //动画状态的控制
    public Animator anim;
    public int animState;

    [Header("Base State")]
    public float health;
    public bool isDead;
    public bool hasBomb;
    public bool isBoss;

    [Header("Movement")]
    public float speed;
    public Transform pointA, pointB;
    public Transform targetPoint;

    [Header("Attack Settings")]
    private float nextAttack = 0;
    public float attackRate;
    public float attackRange, skillRange;

    public List<Transform> attackList = new List<Transform>();


    public virtual void Init()
    {
        anim = GetComponent<Animator>();
        alarmSign = transform.GetChild(0).gameObject;
        //GameManager.instance.IsEnemy(this);
    }
    private void Awake()
    {
        Init();
    }
    // Start is called before the first frame update
    void Start()
    {

        TransitionToState(patrolState);
        if (isBoss)
            UIManager.instance.SetBossHealth(health);
        GameManager.instance.IsEnemy(this);
    }

    // Update is called once per frame
    public virtual void Update()
    {
        anim.SetBool("dead", isDead);
        if (isDead)
        {
            if(isBoss)
                UIManager.instance.UpdateBossHealth(health);
            GameManager.instance.EnemyDead(this);
            return;
        }
        currentState.OnUpdate(this);
        anim.SetInteger("state", animState);
        if (isBoss)
            UIManager.instance.UpdateBossHealth(health);
    }

    public void TransitionToState(EnemyBaseState state)
    {
        currentState = state;
        currentState.EnterState(this);
    }
    public void MoveToTarget()
    {
        transform.position = Vector2.MoveTowards(transform.position, targetPoint.position, speed * Time.deltaTime);
        FlipDirection();
    }
    public virtual void ActionAttack()
    {
        //
        if (Vector2.Distance(transform.position, targetPoint.position) < attackRange)
        {
            if (nextAttack < Time.time)
            {
                nextAttack = Time.time + attackRate;
                anim.SetTrigger("attack");
                //播放攻击动画

            }
        }
    }

    public virtual void SkillAttack()
    {
        if (Vector2.Distance(transform.position, targetPoint.position) < skillRange)
        {
            if (nextAttack < Time.time)
            {
                nextAttack = Time.time + attackRate;
                anim.SetTrigger("skill");
                //播放攻击动画

            }
        }

    }
    public void FlipDirection()
    {
        if (transform.position.x < targetPoint.position.x)
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        else
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
    }
    public void SwitchTarget()
    {
        if (Mathf.Abs(transform.position.x - pointA.position.x) > Mathf.Abs(transform.position.x - pointB.position.x))
            targetPoint = pointA;
        else
            targetPoint = pointB;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!attackList.Contains(collision.transform) && !hasBomb && !isDead && !GameManager.instance.gameOver)   //如果手里有炸弹，就不判断？
            attackList.Add(collision.transform);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        attackList.Remove(collision.transform);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isDead && !GameManager.instance.gameOver)
        {
            StartCoroutine(OnAlarm());
        }
    }
    IEnumerator OnAlarm()
    {
        alarmSign.SetActive(true);
        yield return new WaitForSeconds(alarmSign.GetComponent<Animator>().GetCurrentAnimatorClipInfo(0)[0].clip.length);
        alarmSign.SetActive(false);
    }
}
