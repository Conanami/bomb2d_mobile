using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitPoint : MonoBehaviour
{
    int dir;
    public bool bombAvailable;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (transform.position.x - other.transform.position.x > 0)
            dir = -1;
        else
            dir = 1;

        if(other.CompareTag("Player"))
        {
            //Debug.Log("Player get Hurt");
            other.GetComponent<IDamageable>().GetHit(1);
            //other.GetComponent<Rigidbody2D>().AddForce(new Vector2(dir, 1) * 10, ForceMode2D.Impulse);
        }
        if(other.CompareTag("Bomb") && bombAvailable)
        {
            other.GetComponent<Rigidbody2D>().AddForce(new Vector2(dir, 1)*10, ForceMode2D.Impulse);
        }
    }
}
