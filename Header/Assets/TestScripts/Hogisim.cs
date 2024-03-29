using HeaderPadDefines;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class Hogisim : MonoBehaviour
{
    // Start is called before the first frame update
    public Button[] BT1;
    public Button[] BT2 = new Button[1];
    public TextAsset WeaponTest;
    public Transform targetMonsterTR;
    public LayerMask asdf;
    public List<TestWeaponClass> WeaponArrayTest = new List<TestWeaponClass>();
    public float targetNum;
    public float outPutNum;
    void Start()
    {
        Debug.Log(BT1.Length);
        Debug.Log(BT2.Length);
        Array.Resize<Button>(ref BT2, BT2.Length+1);
        Debug.Log(BT2.Length);
        /*        List<TestWeaponClass> weaponTemp = JsonConvert.DeserializeObject<List<TestWeaponClass>>(WeaponTest.text);
                WeaponArrayTest = weaponTemp;*/

        JObject tempJson = JObject.Parse(WeaponTest.text);
        JToken tempJToken = tempJson["Weapon_Table"];
        WeaponArrayTest = tempJToken.ToObject<List<TestWeaponClass>>();
        Debug.Log(asdf.value);
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Ray2D ray = new Ray2D(mousePosition, Vector2.one);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, asdf);
        if (hit)
        {
            if (Input.GetMouseButtonDown(0))
            {
                targetMonsterTR = hit.collider.transform;
            }

        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            outPutNum = MathF.Sin(targetNum);
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            Light2D tempGOBJ = new GameObject("Torch").AddComponent<Light2D>();
            tempGOBJ.transform.position = Vector2.zero;
            tempGOBJ.lightType = Light2D.LightType.Point;
            tempGOBJ.intensity = 10;
            tempGOBJ.pointLightOuterRadius = 1;
            tempGOBJ.pointLightInnerRadius = 0;
            tempGOBJ.pointLightInnerAngle = 360;
            tempGOBJ.pointLightOuterAngle= 360;
            SpriteRenderer tempSprite = tempGOBJ.gameObject.AddComponent<SpriteRenderer>();
           

        }
    }
}

[System.Serializable]
public class TestWeaponClass
{
    public string ballName;
    public string ballKoreanName;
    public float ballStartForce;
    public float ballBouncienss;
    public float ballfriction;
    public float weight;
    public float skillValueOne;
    public float skillValueTwo;
    public string flavorText;
}
