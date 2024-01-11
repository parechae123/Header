using HeaderPadDefines;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using MoreLinq.Extensions;
using System;

public class ShoterController : MonoBehaviour
{
    public static ShoterController Instance;
    private BallScript targetBall;
    private BallScript TargetBall
    {
        get
        {
            if (targetBall == null)
            {
                targetBall = new GameObject("ShotBall").AddComponent<BallScript>();
                targetBall.transform.parent = transform;
                targetBall.transform.position = Vector3.zero;
            }
            return targetBall;
        }
    }
    private PhysicsMaterial2D refPhyMat;
    Queue<HeaderPadDefines.BallStat> ballStackQueue = new Queue<HeaderPadDefines.BallStat>();




    private void Awake()
    {
        refPhyMat = new PhysicsMaterial2D();
        Instance = this;
        
    }
    private void Start()
    {
        Managers.instance.UI.BattleUICall.PlayerHPBar.value = 10;
    }
    public void ReloadBalls(List<BallStat> EquipedBalls)
    {
        Debug.Log(EquipedBalls[0].ballBouncienss);
        EquipedBalls.Shuffle();
        Debug.Log(EquipedBalls[0].ballBouncienss);

        for (int i = 0; i < EquipedBalls.Count; i++)
        {
            ballStackQueue.Enqueue(EquipedBalls[i]);
        }
        //TODO : 장전 애니메이션 및 행동제약(장전 전 까지 못쏘게 등) 필요
    }
    public void SetBall()
    {
        if (ballStackQueue.Count>0)
        {
            HeaderPadDefines.BallStat tempStat = ballStackQueue.Dequeue();
            TargetBall.Ballsetting(SettingValue(tempStat.ballBouncienss,tempStat.ballFriction),tempStat.weight);
            
        }
        else
        {
            ReloadBalls(Managers.instance.PlayerDataManager.playerOwnBalls);
        }
    }
    private PhysicsMaterial2D SettingValue(float bounce,float friction)
    {
        refPhyMat.bounciness = bounce;
        refPhyMat.friction = friction;
        return refPhyMat;
    }
}
