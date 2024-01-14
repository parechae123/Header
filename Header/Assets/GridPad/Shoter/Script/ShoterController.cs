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
    public bool isReadyFire = false;
    public LineRenderer lineRenderer;
    private int numPoints = 50;
    private float timeInterval = 0.1f;
    private float gravity = -9.8f;
    [SerializeField]private Vector2 normalizedRelValue;

    public static ShoterController Instance;
    private BallScript targetBall;
    private BallScript TargetBall
    {
        get
        {
            if (targetBall == null)
            {
                BallScript tempTarget = GameObject.Find("Bullet").GetComponent<BallScript>();
                if (tempTarget != null )
                {
                    targetBall = tempTarget;
                }
                else
                {
                    targetBall = new GameObject("Bullet").AddComponent<BallScript>();
                }
            }
            return targetBall;
        }
    }
    private PhysicsMaterial2D refPhyMat;
    [SerializeField]private BallStat nowBallStat;
    private BallStat NowBallStat
    {
        get { return nowBallStat; }
        set 
        {
            Managers.instance.UI.BattleUICall.ChangeWeaponUI(true, value.ballName, value.ballKoreanName);
            nowBallStat = value;
        }
    }
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
        ReloadBalls(Managers.instance.PlayerDataManager.playerOwnBalls);
    }
    private void Update()
    {
        if (isReadyFire)
        {
            normalizedRelValue = pos();
            TargetBall.transform.rotation = Quaternion.Euler(0,0,Rot());
            for (int i = 0; i < numPoints; i++)
            {
                float time = i * timeInterval;
                Vector2 tempVec = normalizedRelValue * NowBallStat.ballStartForce;
                float x = tempVec.x * time;
                float y = (tempVec.y * time) + (0.5f * (gravity * NowBallStat.weight) * time * time);

                lineRenderer.SetPosition(i, new Vector3(x, y, 0f) + transform.position);
            }
            if (Input.GetMouseButtonDown(0)&& !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                TargetBall.BallFire(normalizedRelValue, NowBallStat.ballStartForce);
            }
        }
        if (TargetBall.transform.position.y < -10)
        {
            SetBall();
        }
    }

    public void ReloadBalls(List<BallStat> EquipedBalls)
    {
        Managers.instance.PlayerDataManager.CheckWeaponNextBeforeButton();
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
            NowBallStat = ballStatQueue.Dequeue();
            TargetBall.Ballsetting(SettingValue(NowBallStat.ballBouncienss, NowBallStat.ballFriction), NowBallStat.weight);
            
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
    float Rot()
    {
        float tempF = MathF.Atan2(normalizedRelValue.y, normalizedRelValue.x) * Mathf.Rad2Deg;
        return tempF;
    }
    Vector2 pos()
    {
        Vector2 tempVec = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
        tempVec.x = tempVec.x - transform.position.x;
        tempVec.y = tempVec.y - transform.position.y;
        return tempVec.normalized;
    }
}