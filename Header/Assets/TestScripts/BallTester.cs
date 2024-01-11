using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BallTester : MonoBehaviour
{
    public float ballBouncienss;
    public float ballFriction;
    public Rigidbody2D RB;
    private void Start()
    {
        transform.AddComponent<CircleCollider2D>();
        RB = transform.AddComponent<Rigidbody2D>();
        RB.gravityScale = 0;
        RB.sharedMaterial = new PhysicsMaterial2D {bounciness = ballBouncienss,friction = ballFriction};
        //TODO : 볼 스크립트 수정필요
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 3) 
        {
            Managers.instance.Grid.OnColide(collision.transform.position);
        }
    }
}

