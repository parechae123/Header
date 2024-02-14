using HeaderPadDefines;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hogisim : MonoBehaviour
{
    // Start is called before the first frame update
    public Button[] BT1;
    public Button[] BT2 = new Button[1];
    public TextAsset WeaponTest;
    public List<TestWeaponClass> WeaponArrayTest = new List<TestWeaponClass>();
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
    }

    // Update is called once per frame
    void Update()
    {
        
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
