using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetalBulbFrag : MonoBehaviour
{
    public int fragHP;
    public Rigidbody2D fragRB;
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 3)
        {
            Managers.instance.Grid.OnColide(collision.transform.position);
        }
        else if (collision.gameObject.layer == 7)
        {
            fragHP = 1;
        }
        if (fragHP <= 1)
        {

            fragHP--;
            gameObject.SetActive(false);
        }
        else
        {
            fragHP--;
        }
    }
}
