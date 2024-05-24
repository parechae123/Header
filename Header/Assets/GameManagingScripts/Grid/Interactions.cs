using HeaderPadDefines;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;


//MAINTANCE : 인터렉션 추가 시 클래스 작업필요
public abstract class Interactions
{
    public InteractionDefines.InteractionDetailPosition detail;

    public short interactionKeyNumber;
    public abstract void Init();
    public abstract void Interaction();
    public abstract void OutIt();
    protected virtual void InteractionKeyUI(bool isOnOFF)
    {
        Managers.instance.UI.topViewSceneUIs.KeyInteractionOnOFF(isOnOFF);
    }
}
public class DialogInteraction : Interactions
{
    public override void Init()
    {
        Debug.Log("들어옴");
        InteractionKeyUI(true);
    }
    public override void Interaction()
    {
        if (!Managers.instance.UI.dialogCall.UntillVideoBackground.gameObject.activeSelf)
        {
            Managers.instance.UI.TargetUIOnOff(Managers.instance.UI.dialogCall.FullDialogPanel,true,false);
            Managers.instance.UI.TargetUIOnOff(Managers.instance.UI.dialogCall.UntillVideoBackground.rectTransform, true, false);
            Managers.instance.UI.dialogCall.SetDialogueData(interactionKeyNumber);
            InteractionKeyUI(false);
        }
        else
        {
            if (detail.interactionRemoveSelf)
            {
                Managers.instance.UI.dialogCall.DialogTextChanger(detail.installedInteractionPosition);
            }
            else
            {
                Managers.instance.UI.dialogCall.DialogTextChanger();
            }
        }
    }
    public override void OutIt()
    {
        Debug.Log("넌 나가라");
        InteractionKeyUI(false);
    }

}
public class MerchantInteraction : Interactions
{
    public string[] sellableBallNames = new string[0];
    [SerializeField] private BallStat[] shopBalls;
    public override void Init()
    {
        Debug.Log("들어옴");
        SetShopList();
        Managers.instance.UI.shopUICall.ShopUISetting();
        InteractionKeyUI(true);
    }
    public override void Interaction()
    {
        Managers.instance.UI.shopUICall.IsShopActivate = Managers.instance.UI.shopUICall.IsShopActivate == true ? false : true;

    }

    public override void OutIt()
    {
        Debug.Log("넌 나가라");
        InteractionKeyUI(false);
    }
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
                Managers.instance.UI.shopUICall.CreateWeaponBuyButtons(tempStatArray[tempRandomNumber],i);
            }
        }
        else
        {
            for (int i = 0; i < sellableBallNames.Length; i++)
            {
                Array.Resize<BallStat>(ref shopBalls, i + 1);
                if (Managers.instance.Resource._weaponDictionary.TryGetValue(sellableBallNames[i], out ExtraBallStat targetStat))
                {
                    shopBalls[i] = targetStat;
                    Managers.instance.UI.shopUICall.CreateWeaponBuyButtons(targetStat, i);
                }
                else
                {
                    Debug.LogError("이름에 해당하는 공이 없습니다 입력명 : " + shopBalls[i]);
                }
            }
        }
    }
}


public class ToBattleSceneInteraction : Interactions
{
    public string sceneName;
    public override void Init()
    {
        Debug.Log("들어옴");
        InteractionKeyUI(true);
    }
    public override void Interaction()
    {
        SceneManager.LoadScene(sceneName);
    }
    public override void OutIt()
    {
        Debug.Log("넌 나가라");
        InteractionKeyUI(false);
    }
}
