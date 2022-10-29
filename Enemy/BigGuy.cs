using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigGuy : Enemy,IDamageable
{
    public Transform pickupPoint;
    public float power;          //BigGuy的力量
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

    public void PickupBomb()  //动画事件
    {
        if(targetPoint.CompareTag("Bomb") && !hasBomb)
        {
            targetPoint.gameObject.transform.position = pickupPoint.position;
            targetPoint.SetParent(pickupPoint);
            targetPoint.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;  //改成运动学的，没有重力影响
            hasBomb = true;
        }
    }

    public void ThrowBomb()  //动画事件，扔炸弹
    {
        if(hasBomb)
        {
            targetPoint.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            targetPoint.SetParent(transform.parent.parent);  //往上两级，变成跟着整个地图走
            if (FindObjectOfType<PlayerController>().gameObject.transform.position.x < targetPoint.position.x)
                targetPoint.GetComponent<Rigidbody2D>().AddForce(new Vector2(-1, 1) * power, ForceMode2D.Impulse);
            else
                targetPoint.GetComponent<Rigidbody2D>().AddForce(new Vector2(1, 1) * power, ForceMode2D.Impulse);
            
        }
        hasBomb = false;
    }
}
