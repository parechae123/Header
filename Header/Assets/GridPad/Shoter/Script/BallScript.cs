using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BallScript : MonoBehaviour
{
    public float ballBouncienss;
    public float ballFriction;
    private Rigidbody2D ballRB;
    private Rigidbody2D BallRB
    {
        get 
        {
            if (ballRB == null)
            {
                ballRB = transform.AddComponent<Rigidbody2D>();
                ballRB.gravityScale = 0;
            }
            return ballRB;
        }    
    }
    CircleCollider2D ballCol;
    CircleCollider2D BallCol
    {
        get
        { 
            if (ballCol == null)
            {
                ballCol = transform.AddComponent<CircleCollider2D>();
            }
            return ballCol;
        }
    }


    public void Ballsetting(PhysicsMaterial2D phyMat, float Weight)
    {
        BallRB.simulated = false;
        BallCol.enabled = false;
        BallRB.gravityScale = 0;
        if (BallRB.sharedMaterial != phyMat)
        {
            BallRB.sharedMaterial = phyMat;
        }
        BallRB.mass = Weight;
    }
    public void ShotBall()
    {
        BallRB.simulated = true;
        BallCol.enabled = true;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 3) 
        {
            Managers.instance.Grid.OnColide(collision.transform.position);
        }
    }
}

