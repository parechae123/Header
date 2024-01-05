using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopViewCamera : MonoBehaviour
{
    // Start is called before the first frame update
    private Transform plrTR;
    void Start()
    {
        plrTR = TopViewPlayer.Instance.transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = plrTR.position-(Vector3.forward*10);
    }
}
