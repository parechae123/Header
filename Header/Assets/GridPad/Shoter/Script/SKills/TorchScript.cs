using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class TorchScript : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]private Light2D torchLight;
    private float fireValue = 0;
    private float lightForce = 0;
    public float maxOuterRadius = 2;
    public float minRadious = 4;
    private bool isSetted = false;
    public void SetStart()
    {
        isSetted = true;
    }
    // Update is called once per frame
    void Update()
    {
        if (isSetted)
        {
            fireValue += Time.deltaTime;
            lightForce = (Mathf.Sin(fireValue) + 1f) / 2f;

            torchLight.pointLightOuterRadius = (lightForce * maxOuterRadius) + minRadious;
        }
    }
}
