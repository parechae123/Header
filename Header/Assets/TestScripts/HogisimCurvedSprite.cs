using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class HogisimCurvedSprite : MonoBehaviour
{
    // Start is called before the first frame update
    SpriteShapeController SSC;
    void Start()
    {
        SSC = new GameObject("spriteShape").AddComponent<SpriteShapeController>();
        SSC.splineDetail = 2;
        SSC.spline.InsertPointAt(SSC.spline.GetPointCount(), Vector3.zero);
        SSC.spline.InsertPointAt(SSC.spline.GetPointCount(), Vector3.one);
        SSC.spline.InsertPointAt(SSC.spline.GetPointCount(), Vector3.one*3);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {

        }
    }
}
