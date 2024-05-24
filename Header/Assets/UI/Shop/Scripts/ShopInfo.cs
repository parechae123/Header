using HeaderPadDefines;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ShopInfo : MonoBehaviour
{
    // Start is called before the first frame update
    public string[] sellableBallNames = new string[0];
    [SerializeField]private BallStat[] shopBalls;

    public BallStat[] CallBallList()
    {
        if (sellableBallNames.Length != shopBalls.Length||sellableBallNames.Length == 0)
        {
            SetShopList();
        }
        return shopBalls;
    }
    
    void Start()
    {
        Managers.instance.UI.shopUICall.ShopUISetting();
        CallBallList();
    }

    // Update is called once per frame
    public void SetShopList()
    {
        if (Managers.instance.Resource._weaponDictionary.Count <= 0)
        {
            JObject tempJson = JObject.Parse(Managers.instance.Resource.Load<TextAsset>("Weapon_Table").text);
            JToken tempJToken = tempJson["Weapon_Table"];
            ExtraBallStat[] tempBallTable = tempJToken.ToObject<ExtraBallStat[]>();
            for (int i = 0; i < tempBallTable.Length; i++)
            {
                Managers.instance.Resource._weaponDictionary.Add(tempBallTable[i].ballName, tempBallTable[i]);
            }
        }

        if (sellableBallNames.Length == 0)
        {
            ExtraBallStat[] tempStatArray = Managers.instance.Resource._weaponDictionary.Values.ToArray<ExtraBallStat>();
            for (int i = 0; i < 4; i++)
            {
                Array.Resize<BallStat>(ref shopBalls, i + 1);
                int tempRandomNumber = UnityEngine.Random.Range(0, tempStatArray.Length);
                shopBalls[i] = tempStatArray[tempRandomNumber];
                if(Managers.instance.PlayerDataManager.isChallengeMode)shopBalls[i].ballPrice = 0;
                Managers.instance.UI.shopUICall.CreateWeaponBuyButtons(tempStatArray[tempRandomNumber], i);
            }
        }
        else
        {
            for (int i = 0; i < sellableBallNames.Length; i++)
            {
                Array.Resize<BallStat>(ref shopBalls, i+1);
                if (Managers.instance.Resource._weaponDictionary.TryGetValue(sellableBallNames[i],out ExtraBallStat targetStat))
                {
                    shopBalls[i] = targetStat;
                    if (Managers.instance.PlayerDataManager.isChallengeMode) shopBalls[i].ballPrice = 0;
                    Managers.instance.UI.shopUICall.CreateWeaponBuyButtons(targetStat, i);
                }
                else
                {
                    Debug.LogError("이름에 해당하는 공이 없습니다 입력명 : "+ shopBalls[i]);
                }
            }
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Managers.instance.PlayerDataManager.PlayerMoney += 1000;
            
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            Managers.instance.UI.shopUICall.IsShopActivate = Managers.instance.UI.shopUICall.IsShopActivate == true ? false : true;
        }
    }
}
