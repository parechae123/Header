using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BallScript : MonoBehaviour
{
    [SerializeField]private Collider2D[] pauseCollector;
    public float ballBouncienss;
    public float ballFriction;
    public float ballRadios;
    private Rigidbody2D ballRB;
    private Rigidbody2D BallRB
    {
        get 
        {
            if (ballRB == null)
            {
                if (transform.TryGetComponent<Rigidbody2D>(out Rigidbody2D tempRB))
                {
                    ballRB = tempRB;
                    ballRB.gravityScale = 0;
                }
                else
                {
                    ballRB = transform.AddComponent<Rigidbody2D>();
                    ballRB.gravityScale = 0;
                }
                
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
                IMG.enabled = true;
                if (transform.TryGetComponent<CircleCollider2D>(out CircleCollider2D temp))
                {
                    ballCol = temp;
                }
                else
                {
                    ballCol = transform.AddComponent<CircleCollider2D>();
                }
                
            }
            return ballCol;
        }
    }
    private SpriteRenderer img;
    public SpriteRenderer IMG
    {
        get
        {
            if (img == null)
            {
                if (transform.TryGetComponent<SpriteRenderer>(out SpriteRenderer temp))
                {
                    img = temp;
                }
                else
                {
                    img = transform.AddComponent<SpriteRenderer>();
                }
                img.sprite = Managers.instance.Resource.Load<Sprite>("Bullet_Basic");
            }
            return img;
        }
    }

    private void Update()
    {
        if (BallRB.velocity == Vector2.zero)
        {
            pauseCollector = Physics2D.OverlapCircleAll(transform.position,BallCol.bounds.size.x,8);
            for (int i = 0; i < pauseCollector.Length; i++)
            {
                Managers.instance.Grid.OnColide(pauseCollector[i].transform.position);
            }
        }
    }
    public void Ballsetting(PhysicsMaterial2D phyMat, float Weight)
    {
        ShoterController.Instance.isReadyFire = true; 
        ShoterController.Instance.lineRenderer.enabled = true;
        BallRB.simulated = false;
        BallCol.enabled = false;
        ballRadios = BallCol.bounds.size.x;
        BallRB.gravityScale = Weight;
        transform.localPosition = Vector3.zero;
        BallRB.velocity = Vector2.zero;
        if (BallRB.sharedMaterial != phyMat)
        {
            BallRB.sharedMaterial = phyMat;
        }
    }
    public void BallFire(Vector2 FireToward,float FireForce)
    {
        ShoterController.Instance.isReadyFire = false;
        ShoterController.Instance.lineRenderer.enabled = false;
        BallRB.simulated = true;
        BallCol.enabled = true;
        Debug.Log(FireToward);
        FireToward = FireToward * FireForce;
        FireToward.y += 0.5f * BallRB.gravityScale * BallRB.mass * Mathf.Pow(Time.fixedDeltaTime, 2);
        BallRB.velocity = FireToward;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 3) 
        {
            Managers.instance.Grid.OnColide(collision.transform.position);
        }
    }
    public void BallToTarget(Vector2 targetPos)
    {
        BallRB.velocity = Vector2.zero;
        BallRB.velocity = CarculateVelocity(targetPos);
    }
    Vector2 CarculateVelocity(Vector2 targetPos)
    {
        return (((targetPos - (Vector2)transform.position).normalized) * (Vector2.Distance(targetPos, transform.position) * (9.8f * BallRB.gravityScale)));
    }
}

