using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class TorchScript : MonoBehaviour
{
    // Start is called before the first frame update
    private Light2D torchLight;
    private SpriteRenderer torchSR;
    private float fireValue = 0;
    private float lightForce = 0;
    public float maxOuterRadius = 2;
    public float minRadious = 4;
    private bool isSetted = false;
    public void SetStart()
    {
        torchLight = gameObject.AddComponent<Light2D>();
        torchLight.lightType = Light2D.LightType.Point;
        torchLight.pointLightInnerAngle = 360;
        torchLight.pointLightOuterAngle = 360;
        torchLight.pointLightInnerRadius = 0;
        torchLight.pointLightOuterRadius = 1;
        torchLight.color = new Color(0.3018868f, 0.3018868f, 0.3018868f, 1);
        torchLight.intensity = 15.44f;
        torchSR = gameObject.AddComponent<SpriteRenderer>();
        torchSR.sprite = Managers.instance.Resource.Load<Sprite>("torch_bulb");
        torchSR.sortingLayerName = "Ball";
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
