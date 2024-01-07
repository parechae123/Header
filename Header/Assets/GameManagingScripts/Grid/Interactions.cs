using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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
        if (!Managers.instance.UI.DialogCall.DialogueBackGround.gameObject.activeSelf)
        {
            Managers.instance.UI.TargetUIOnOff(Managers.instance.UI.DialogCall.DialogueBackGround.rectTransform, true);
            Managers.instance.UI.DialogCall.SetDialogueData(interactionKeyNumber);
            InteractionKeyUI(false);
        }
        else
        {
            if (detail.interactionRemoveSelf)
            {
                Managers.instance.UI.DialogCall.DialogTextChanger(detail.installedInteractionPosition);
            }
            else
            {
                Managers.instance.UI.DialogCall.DialogTextChanger();
            }
        }
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
