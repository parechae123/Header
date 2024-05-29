using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetalBulbDispenser : MonoBehaviour
{
    [SerializeField] MetalBulbFrag[] frags = new MetalBulbFrag[3];
    public Vector2 refDirrection;
    public float angle;
    private void Reset()
    {
        
        for (int i = 0; i < frags.Length; i++)
        {
            frags[i] = transform.GetChild(i).GetComponent<MetalBulbFrag>();
            frags[i].fragRB = frags[i].gameObject.GetComponent<Rigidbody2D>();
        }
    }
    public void Init(Vector2 ballVel,int fragHP)
    {
        refDirrection = ballVel;
        angle = Mathf.Atan2(refDirrection.y, refDirrection.x);
        angle = (angle * Mathf.Rad2Deg) - 105f;
        //º¸Á¤°ª (-90) + (-15)
        for (int i = 0; i < frags.Length; i++)
        {
            frags[i].gameObject.SetActive(true);
            frags[i].fragHP = fragHP;
            frags[i].transform.localPosition = Vector3.zero;
            frags[i].transform.eulerAngles = Vector3.forward * (angle + (15 * i));
            frags[i].fragRB.velocity = Vector3.zero;
            frags[i].fragRB.AddForce(frags[i].transform.up * 10, ForceMode2D.Impulse);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!frags[0].gameObject.activeSelf && !frags[1].gameObject.activeSelf && !frags[2].gameObject.activeSelf)
        {
            if (gameObject.activeSelf)
            {
                ShoterController.Instance.targetBall.EndPlayerTurn(true);
                gameObject.SetActive(false);
            }
        }
    }
}
