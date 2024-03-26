using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class CloneBalls : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 3)
        {
            Debug.Log("≈¨∑–∫º ¥Ÿ»Ï");
            Managers.instance.Grid.OnColide(collision.transform.position);
        }
    }
    public void OnBallCreate(float ColliderRadius,Sprite Image)
    {
        gameObject.AddComponent<Rigidbody2D>();
        SpriteRenderer tempSP =  gameObject.AddComponent<SpriteRenderer>();
        tempSP.sprite = Image;
        tempSP.flipX = true;
        CircleCollider2D tempCC = gameObject.AddComponent<CircleCollider2D>();
        tempCC.radius = ColliderRadius;
    }

}
