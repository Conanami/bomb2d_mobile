using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Captain : Enemy,IDamageable
{
    SpriteRenderer sprite;

    public override void Init()
    {
        base.Init();
        sprite = GetComponent<SpriteRenderer>();
    }

    public override void Update()
    {
        base.Update();
        if (animState == 0)
            sprite.flipX = false;
    }
    public void GetHit(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            health = 0;
            isDead = true;
        }
        anim.SetTrigger("hit");
    }

    public override void SkillAttack()
    {
        base.SkillAttack();
        if(anim.GetCurrentAnimatorStateInfo(1).IsName("skill"))
        {
            sprite.flipX = true;
            if (transform.position.x - targetPoint.position.x > 0)  //船长在炸弹右边
            {
                transform.position = Vector2.MoveTowards(transform.position, transform.position + Vector3.right, speed * 2 * Time.deltaTime);
            }
            else
            {
                transform.position = Vector2.MoveTowards(transform.position, transform.position + Vector3.left, speed * 2 * Time.deltaTime);
            }
        }
        else
        {
            sprite.flipX = false;
        }
    }


}
