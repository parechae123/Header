using HeaderPadDefines;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using MoreLinq.Extensions;
using System;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class ShoterController : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private int numPoints = 50;
    private float timeInterval = 0.1f;
    private float initialVelocity = 10f;
    private float gravity = -9.8f;
    private Vector2 normalizedRelValue;

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
    private BallStat nowBallStat;
    Queue<HeaderPadDefines.BallStat> ballStatQueue = new Queue<HeaderPadDefines.BallStat>();
    private void Awake()
    {
        refPhyMat = new PhysicsMaterial2D();
        Instance = this;
        
    }
    private void Start()
    {
        Managers.instance.UI.BattleUICall.SettingPlayerBattleUI();
        lineRenderer = transform.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.2f;
        lineRenderer.endWidth = 1;
        lineRenderer.positionCount = numPoints;
    }
    private void Update()
    {
        normalizedRelValue = pos();
        for (int i = 0; i < numPoints; i++)
        {
            float time = i * timeInterval;
            Vector2 tempVec = normalizedRelValue * initialVelocity;
            float x = tempVec.x * time;
            float y = (tempVec.y * time) + (0.5f * gravity * time * time);

            lineRenderer.SetPosition(i, new Vector3(x, y, 0f)+transform.position);
        }
    }

    public void ReloadBalls(List<BallStat> EquipedBalls)
    {
        Debug.Log(EquipedBalls[0].ballBouncienss);
        EquipedBalls.Shuffle();
        Debug.Log(EquipedBalls[0].ballBouncienss);

        for (int i = 0; i < EquipedBalls.Count; i++)
        {
            ballStatQueue.Enqueue(EquipedBalls[i]);
        }
        SetBall();
        //TODO : 장전 애니메이션 및 행동제약(장전 전 까지 못쏘게 등) 필요
    }
    public void SetBall()
    {
        if (ballStatQueue.Count>0)
        {
            nowBallStat = ballStatQueue.Dequeue();
            TargetBall.Ballsetting(SettingValue(nowBallStat.ballBouncienss, nowBallStat.ballFriction), nowBallStat.weight);
            
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
    Vector2 pos()
    {
        Vector2 tempVec = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
        tempVec.x = tempVec.x - transform.position.x;
        tempVec.y = tempVec.y - transform.position.y;
        Debug.Log(tempVec.normalized);
        return tempVec.normalized;
    }
}