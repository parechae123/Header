using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//MAINTANCE : ���ͷ��� �߰� �� Ŭ���� �۾��ʿ�
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
        Debug.Log("����");
        InteractionKeyUI(true);
    }
    public override void Interaction()
    {
        Managers.instance.UI.TargetUIOnOff(Managers.instance.UI.DialogCall.FullDialogPanel,true);
        InteractionKeyUI(false);

    }
    public override void OutIt()
    {
        Debug.Log("�� ������");
        InteractionKeyUI(false);
    }

}
public class ToBattleSceneInteraction : Interactions
{
    public override void Init()
    {
        Debug.Log("����");
        InteractionKeyUI(true);
    }
    public override void Interaction()
    {

    }
    public override void OutIt()
    {
        Debug.Log("�� ������");
        InteractionKeyUI(false);
    }
}
