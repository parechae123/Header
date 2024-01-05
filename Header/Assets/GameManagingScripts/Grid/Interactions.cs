using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//MAINTANCE : 인터렉션 추가 시 클래스 작업필요
public abstract class Interactions
{
    public InteractionDefines.InteractionDetailPosition detail;
    public abstract void Init();
    public abstract void Interaction();
    public abstract void OutIt();
    protected virtual void InteractionKeyUI(bool isOnOFF)
    {
        Managers.instance.UI.TopViewSceneUIs.KeyInteractionOnOFF(isOnOFF);
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
        Managers.instance.UI.TargetUIOnOff(Managers.instance.UI.DialogCall.FullDialogPanel,true);
        InteractionKeyUI(false);

    }
    public override void OutIt()
    {
        Debug.Log("넌 나가라");
        InteractionKeyUI(false);
    }

}
public class ToBattleSceneInteraction : Interactions
{
    public override void Init()
    {
        Debug.Log("들어옴");
        InteractionKeyUI(true);
    }
    public override void Interaction()
    {

    }
    public override void OutIt()
    {
        Debug.Log("넌 나가라");
        InteractionKeyUI(false);
    }
}
