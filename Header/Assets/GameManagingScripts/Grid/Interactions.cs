using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//MAINTANCE : 인터렉션 추가 시 클래스 작업필요
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
