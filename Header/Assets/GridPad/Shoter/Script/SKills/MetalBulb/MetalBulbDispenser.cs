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
    void OnEnable()
    {
        angle = Mathf.Atan2(refDirrection.y, refDirrection.x);
        angle = (angle * Mathf.Rad2Deg)-105f;
        //º¸Á¤°ª (-90) + (-15)
        for (int i = 0; i < frags.Length; i++)
        {
            frags[i].transform.localPosition = Vector3.zero;
            frags[i].transform.eulerAngles = Vector3.forward* (angle+(15*i));
            frags[i].fragRB.AddForce(frags[i].transform.up*10,ForceMode2D.Impulse);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
