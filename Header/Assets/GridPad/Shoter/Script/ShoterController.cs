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
    public bool isReadyFire = false;
    public LineRenderer lineRenderer;
    private int numPoints = 50;
    private float ballRadios;
    private float timeInterval = 0.1f;
    private float gravity = -9.8f;
    [SerializeField] private Vector2 normalizedRelValue;
    [SerializeField] private UnityEngine.Transform testTR;
    [SerializeField] LayerMask layerBallCollision;
    public static ShoterController Instance;
    private BallScript targetBall;
    public BallScript TargetBall
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
        lineRenderer.endWidth = 0.3f;
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
                float time2 = (i + 1) * timeInterval;
                Vector2 tempVec = normalizedRelValue * NowBallStat.ballStartForce;
                float x = tempVec.x * time;
                float y = (tempVec.y * time) + (0.5f * (gravity * NowBallStat.weight) * time * time);
                float x2 = tempVec.x * time2;
                float y2 = (tempVec.y * time2) + (0.5f * (gravity * NowBallStat.weight) * time2 * time2);

                Vector3 firstPos = new Vector3(x, y) + transform.position;
                Vector3 secondPos = new Vector3(x2, y2) + transform.position;
                float nowAndNextDistance = Vector2.Distance(firstPos, secondPos);
                lineRenderer.SetPosition(i, firstPos);
                RaycastHit2D colInfo = Physics2D.Raycast(firstPos,secondPos-firstPos, nowAndNextDistance, layerBallCollision);
                if (colInfo)
                {
                    testTR.position = colInfo.point;
                    Debug.Log("충돌해쪄"+colInfo.point);
                    lineRenderer.SetPosition(i+1, colInfo.point);
                    float reflectedBallVelocity = nowBallStat.ballBouncienss * (Vector2.Distance(firstPos, secondPos)/timeInterval);
                    Vector2 normalizedRefVector = (Vector2.Reflect(secondPos - firstPos, colInfo.normal)).normalized;
                    Vector2 refTempVec = reflectedBallVelocity * normalizedRefVector;
                    for (int E = i+2; E < lineRenderer.positionCount; E++)
                    {
                        float timeScaler = (E-i)*timeInterval;

                        if (E< lineRenderer.positionCount)
                        {
                            Debug.Log("리플렉션 값 노멀라이즈"+Vector2.Reflect(secondPos - firstPos, colInfo.normal).normalized);
                            Debug.Log("거기에 충돌위치를 곁드린"+(Vector2.Reflect(secondPos - firstPos, colInfo.normal) + colInfo.point));
                            float reflectedX = refTempVec.x * timeScaler;
                            float reflectedY = (refTempVec.y * timeScaler) + (0.5f * (gravity * NowBallStat.weight) * timeScaler * timeScaler);
                            lineRenderer.SetPosition(E, colInfo.point+new Vector2(reflectedX,reflectedY));
                            if (E-1!= i)
                            {
                                float reflectedNowNextDistance = Vector2.Distance(lineRenderer.GetPosition(E), lineRenderer.GetPosition(E - 1));
                                RaycastHit2D reflectedColl = Physics2D.Raycast(lineRenderer.GetPosition(E - 1), lineRenderer.GetPosition(E) - lineRenderer.GetPosition(E - 1), reflectedNowNextDistance, layerBallCollision);
                                if (reflectedColl)
                                {
                                    if (reflectedColl.point != colInfo.point)
                                    {
                                        for (int o = E; o < lineRenderer.positionCount; o++)
                                        {
                                            lineRenderer.SetPosition(o, reflectedColl.point);
                                        }
                                        break;

                                    }
                                }
                            }
                        }
                    }
                    break;
                }


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
        if (Input.GetKeyDown(KeyCode.G))
        {
            Managers.instance.UI.BattleUICall.WeaponAnim(true, "Bullet_Basic", "");
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            Managers.instance.UI.BattleUICall.WeaponAnim(false, "Bullet_Basic", "");
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