using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//MAINTANCE : ���ͷ��� �߰� �� Ŭ���� �۾��ʿ�
public abstract class Interactions
{
    public InteractionDefines.InteractionDetailPosition detail;
    public abstract void Init();
    public abstract void Interaction();
}
public class DialogInteraction : Interactions
{
    public override void Init()
    {
        
    }
    public override void Interaction()
    {
        
    }
}
public class ToBattleSceneInteraction : Interactions
{
    public override void Init()
    {

    }
    public override void Interaction()
    {

    }
}
