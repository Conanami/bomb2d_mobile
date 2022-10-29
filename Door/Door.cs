using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    Animator anim;
    BoxCollider2D coll;

    private void Start()
    {
        anim = GetComponent<Animator>();
        coll = GetComponent<BoxCollider2D>();
        coll.enabled = false;
        GameManager.instance.IsDoorExit(this);
    }

    public void OpenDoor()  //Game Manager内调用
    {
        anim.Play("open");
        coll.enabled = true;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            GameManager.instance.NextLevel();
    }
}
