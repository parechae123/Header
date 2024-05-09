using HeaderPadDefines;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using System;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Reflection;

public class ShoterController : MonoBehaviour
{
    public string nextSceneName = "";
    public bool isReadyFire = false;
    public bool isParabolaOn;
    public LineRenderer lineRenderer;
    private int numPoints = 50;
    private float timeInterval = 0.1f;
    private float gravity = -9.8f;
    public float shootBallColideTime = 0;
    [SerializeField] public Vector2 normalizedRelValue;
    [SerializeField] public UnityEngine.Transform collPointPreview;
    [SerializeField] LayerMask layerBallCollision;
    public static ShoterController Instance;
    [SerializeField]private float fireForce = 0;
    public BallScript targetBall;
    public float regionalDamage = 0;
    public float targetDamage = 0;
    private short forceDirrection = 1;
    private Transform targetMonsterTR = null;

    public Transform TargetMonsterTR
    {
        get 
        { 
            if (targetMonsterTR == null) 
            {
                targetMonsterTR = MonsterManager.MonsterManagerInstance.SetTargetMonsters;
                    //TODO : 타겟몬스터 표시하는 UI 추가필요
            }
            else
            {
                if (!targetMonsterTR.gameObject.activeSelf)
                {
                    targetMonsterTR = MonsterManager.MonsterManagerInstance.SetTargetMonsters;
                    //TODO : 타겟몬스터 표시하는 UI 추가필요
                }
            }
            if (targetMonsterTR != null)
            {
                Managers.instance.UI.BattleUICall.SetTargetUI(targetMonsterTR, MonsterManager.MonsterManagerInstance.ReturnMonsterSpriteSize(targetMonsterTR));
            }
            else
            {
                Managers.instance.UI.BattleUICall.SetTargetUI(null, Vector2.zero);
            }

            return targetMonsterTR; 
        }
    }
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
    [SerializeField]private ExtraBallStat nowBallStat;
    public ExtraBallStat NowBallStat
    {
        get 
        { 
            return nowBallStat;
        }
        set 
        {
            if (value != null)
            {
                TargetBall.ballNowHP = value.ballHealth;
                if (value.amount <= 0)
                {
                    Managers.instance.UI.BattleUICall.ChangeWeaponUI(string.Empty, "전구가 없어!!");
                }
                else
                {
                    Managers.instance.UI.BattleUICall.ChangeWeaponUI(value.ballName, value.ballKoreanName);
                    SetBulbSkills(value.ballName);
                }
            }
            else
            {
                if (Managers.instance.PlayerDataManager.playerOwnBalls.Count<=0)
                {
                    Managers.instance.UI.BattleUICall.ChangeWeaponUI(string.Empty, "전구가 없어!!");
                    Managers.instance.UI.BattleUICall.GameOverBTN.enabled = true;
                    ballStatQueue.Clear();
                }
                else
                {
                    ballStatQueue.Clear();
                    for (int i = 0; i < Managers.instance.PlayerDataManager.playerOwnBalls.Count; i++)
                    {
                        ballStatQueue.Enqueue(Managers.instance.PlayerDataManager.playerOwnBalls[i]);
                    }
                }
            }
            nowBallStat = value;
        }
    }
    public Queue<HeaderPadDefines.ExtraBallStat> ballStatQueue = new Queue<HeaderPadDefines.ExtraBallStat>();
    private void Awake()
    {
        refPhyMat = new PhysicsMaterial2D();
        Instance = this;
    }
    private void Start()
    {
        if (Managers.instance.PlayerDataManager.playerOwnBalls == null || Managers.instance.PlayerDataManager.playerOwnBalls.Count <= 0)
        {
            isReadyFire = false;
            Managers.instance.UI.BattleUICall.GameOverBTN.enabled = true;
            return;
        }
        Managers.instance.UI.BattleUICall.SettingPlayerBattleUI();
        Managers.instance.UI.BattleUICall.RegistListener();
        lineRenderer = transform.AddComponent<LineRenderer>();
        isParabolaOn = Managers.instance.PlayerDataManager.isParabolaTurnOn;
        ParabolaOnOFF(isParabolaOn);
        lineRenderer.textureMode = LineTextureMode.Tile;
        List<Material> TempMat = new List<Material>();
        TempMat.Add(Managers.instance.Resource.Load<Material>("ShootLine"));
        lineRenderer.SetMaterials(TempMat);
        lineRenderer.textureScale = new Vector2(3.47f, 1f);
        lineRenderer.startWidth = 0.22f;
        lineRenderer.endWidth = 0.22f;
        lineRenderer.positionCount = numPoints;
        ReloadBalls(Managers.instance.PlayerDataManager.playerOwnBalls);
    }
    private void Update()
    {
        if (isReadyFire)
        {
           GetBulbPoints();
           ShotBall();
        }
        if (Input.GetMouseButtonDown(1))
        {
            Debug.LogError("클릭은 됬는디");
            BaseInputModule tempModule = EventSystem.current.currentInputModule;
            Type tempType = tempModule.GetType();
            FieldInfo focusedObjectField = tempType.GetField("m_CurrentFocusedGameObject", BindingFlags.Instance | BindingFlags.NonPublic);
            GameObject tempGOBJ = focusedObjectField.GetValue(tempModule) as GameObject;
            if (tempGOBJ == null)
            {
                return;
            }
            else
            {
                Debug.Log(tempGOBJ);
                if (tempGOBJ.name.Contains("weaponIMG"))
                {
                    Managers.instance.UI.BattleUICall.GirlBulbExplane = "'" + NowBallStat.flavorText + " ' " + "라고 하네요";
                }
            }

        }
        if (TargetBall.transform.position.y < -10)
        {
            MonsterManager.MonsterManagerInstance.NextTurnFunctions(regionalDamage, targetDamage, TargetMonsterTR, () =>
            {
                if (Managers.instance.PlayerDataManager.RemoveBall(Instance.NowBallStat))
                {
                    SetBallOnNext();
                }
                TargetBall.BallPause();
                TargetBall.ResetBall();
                regionalDamage = 0;
                targetDamage = 0;
            });
        }
    }
    private void GetBulbPoints()
    {
        if (isParabolaOn)
        {
            if (fireForce != 0)
            {
                normalizedRelValue = pos();
                TargetBall.transform.rotation = Quaternion.Euler(0, 0, Rot());
                for (int i = 0; i < numPoints; i++)
                {
                    float time = i * timeInterval;
                    float time2 = (i + 1) * timeInterval;
                    Vector2 tempVec = normalizedRelValue * fireForce;
                    float x = tempVec.x * time;
                    float y = (tempVec.y * time) + (0.5f * (gravity * NowBallStat.weight) * time * time);
                    float x2 = tempVec.x * time2;
                    float y2 = (tempVec.y * time2) + (0.5f * (gravity * NowBallStat.weight) * time2 * time2);

                    Vector3 firstPos = new Vector3(x, y) + transform.position;
                    Vector3 secondPos = new Vector3(x2, y2) + transform.position;
                    float nowAndNextDistance = Vector2.Distance(firstPos, secondPos);
                    lineRenderer.SetPosition(i, firstPos);
                    RaycastHit2D colInfo = Physics2D.Raycast(firstPos, secondPos - firstPos, nowAndNextDistance, layerBallCollision);
                    if (colInfo)
                    {
                        if(colInfo.collider != null)shootBallColideTime = time;
                        if(collPointPreview != null)collPointPreview.position = colInfo.point;
                        lineRenderer.SetPosition(i + 1, colInfo.point);
                        float reflectedBallVelocity = NowBallStat.ballBouncienss * (Vector2.Distance(firstPos, secondPos) / timeInterval);
                        Vector2 normalizedRefVector = (Vector2.Reflect(secondPos - firstPos, colInfo.normal)).normalized;
                        Vector2 refTempVec = reflectedBallVelocity * normalizedRefVector;
                        for (int E = i + 2; E < lineRenderer.positionCount; E++)
                        {
                            float timeScaler = (E - i) * timeInterval;

                            if (E < lineRenderer.positionCount)
                            {

                                float reflectedX = refTempVec.x * timeScaler;
                                float reflectedY = (refTempVec.y * timeScaler) + (0.5f * (gravity * NowBallStat.weight) * timeScaler * timeScaler);
                                lineRenderer.SetPosition(E, colInfo.point + new Vector2(reflectedX, reflectedY));
                                if (E - 1 != i)
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
            }
            else
            {
                normalizedRelValue = pos();
                TargetBall.transform.rotation = Quaternion.Euler(0, 0, Rot());
                lineRenderer.SetPosition(0, (Vector2)transform.position);
                for (int i = 1; i < lineRenderer.positionCount; i++)
                {
                    lineRenderer.SetPosition(i, (normalizedRelValue * 4) + (Vector2)transform.position);
                }

            } 
        }
        else
        {
            normalizedRelValue = pos();
            TargetBall.transform.rotation = Quaternion.Euler(0, 0, Rot());
            
        }
    }

    private void ShotBall()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Ray2D ray = new Ray2D(mousePosition, Vector2.zero);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction,Mathf.Infinity,64);
        if (!hit)
        {
            if (Input.GetMouseButtonUp(0) && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject() && fireForce != 0)
            {
                TargetBall.BallFire(normalizedRelValue, fireForce);
                for (int i = 0; i < lineRenderer.positionCount; i++)
                {
                    lineRenderer.SetPosition(i, transform.position);
                }
                fireForce = 0;
                Managers.instance.UI.BattleUICall.SetBallSliderPos(transform.position, false);
            }
            else if (Input.GetMouseButtonDown(0) && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                if(collPointPreview != null)collPointPreview.gameObject.SetActive(true);
                Managers.instance.UI.BattleUICall.SetBallSliderPos(transform.position, true);
            }
            else if (Input.GetMouseButton(0) && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                if (NowBallStat.ballStartForce <= fireForce || fireForce <= 0)
                {
                    if (forceDirrection == 1)
                    {
                        forceDirrection = -1;
                    }
                    else
                    {
                        forceDirrection = 1;
                    }
                }
                fireForce += (Time.deltaTime * NowBallStat.ballStartForce) * forceDirrection;
                Managers.instance.UI.BattleUICall.UpdateBallForce( fireForce/ NowBallStat.ballStartForce);
            }

            else if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                if (fireForce != 0)
                {
                    fireForce = 0;
                    Managers.instance.UI.BattleUICall.SetBallSliderPos(transform.position, false);
                }
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (hit.collider.transform!=null)
                {
                    targetMonsterTR = hit.collider.transform;
                    Managers.instance.UI.BattleUICall.SetTargetUI(targetMonsterTR, MonsterManager.MonsterManagerInstance.ReturnMonsterSpriteSize(TargetMonsterTR));
                }
                fireForce = 0;
                Managers.instance.UI.BattleUICall.SetBallSliderPos(transform.position, false);
            }
            else if (Managers.instance.UI.BattleUICall.BallForceSliderParent == null|| Managers.instance.UI.BattleUICall.BallForceSliderParent.gameObject.activeSelf)
            {
                if (Input.GetMouseButtonUp(0) && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject() && fireForce != 0)
                {

                    TargetBall.BallFire(normalizedRelValue, fireForce);
                    for (int i = 0; i < lineRenderer.positionCount; i++)
                    {
                        lineRenderer.SetPosition(i, transform.position);
                    }
                    fireForce = 0;
                    Managers.instance.UI.BattleUICall.SetBallSliderPos(transform.position, false);
                }
                else if (Input.GetMouseButton(0) && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
                {
                    if (NowBallStat.ballStartForce > fireForce)
                    {
                        fireForce += Time.deltaTime * NowBallStat.ballStartForce;
                    }
                    else
                    {
                        fireForce = NowBallStat.ballStartForce;
                    }
                    Managers.instance.UI.BattleUICall.UpdateBallForce(fireForce / NowBallStat.ballStartForce);
                }
            }
        }
    }
    public void ReloadBalls(List<ExtraBallStat> EquipedBalls)
    {
        Managers.instance.PlayerDataManager.CheckWeaponNextBeforeButton();
        if (EquipedBalls.Count> 0)
        {
            for (int i = 0; i < EquipedBalls.Count; i++)
            {
                ballStatQueue.Enqueue(EquipedBalls[i]);
            }
            SetBall();
        }

        
    }

    public void SetBallOnNext()
    {
        string tempName = NowBallStat.ballName;
        if (Managers.instance.PlayerDataManager.playerOwnBalls.Count<=0&& NowBallStat.amount <= 0)
        {
            ballStatQueue.Clear();
            NowBallStat = null;
            isReadyFire = false;
            Managers.instance.UI.BattleUICall.WeaponAnim(true, "Bulb_Empty", "전구가 없어!!", tempName);
            TargetBall.ChangeBallSprite("Bulb_Empty");
            return;
        }
        else
        {
            if (NowBallStat.amount > 0)
            {
                ballStatQueue.Enqueue(NowBallStat);
            }
            else if(NowBallStat.amount <= 0)
            {
                
                ExtraBallStat[] tempEx = new ExtraBallStat[0];
                Queue<ExtraBallStat> tempQueue = new Queue<ExtraBallStat>();
                Array.Resize<ExtraBallStat>(ref tempEx, Managers.instance.PlayerDataManager.playerOwnBalls.Count);
                tempEx = ballStatQueue.ToArray();
                for (int i = 0; i < tempEx.Length; i++)
                {
                    if (tempEx[i].amount > 0)
                    {
                        tempQueue.Enqueue(tempEx[i]);
                    }
                }
                ballStatQueue.Clear();
                ballStatQueue = tempQueue;
            }

            NowBallStat = ballStatQueue.Dequeue();
            TargetBall.Ballsetting(SettingValue(NowBallStat.ballBouncienss, NowBallStat.ballFriction), NowBallStat.weight);
            Managers.instance.UI.BattleUICall.WeaponAnim(true, NowBallStat.ballName, NowBallStat.ballKoreanName , tempName);

            TargetBall.ChangeBallSprite(NowBallStat.ballName);
        }
    }
    public void SetBallOnBehind()
    {
        
        string tempName = NowBallStat.ballName;
        ExtraBallStat[] TempBallStat = ballStatQueue.ToArray();
        ballStatQueue.Clear();
        if (NowBallStat.amount >0)
        {
            ballStatQueue.Enqueue(NowBallStat);
        }
        for (int i = 0; i < TempBallStat.Length-1; i++)
        {
            ballStatQueue.Enqueue(TempBallStat[i]);
        }
        Debug.Log(TempBallStat[TempBallStat.Length-1].ballName);
        Debug.Log(TempBallStat.Length);

        NowBallStat = TempBallStat[TempBallStat.Length - 1];
        TargetBall.Ballsetting(SettingValue(NowBallStat.ballBouncienss, NowBallStat.ballFriction), NowBallStat.weight);
        Managers.instance.UI.BattleUICall.WeaponAnim(false, NowBallStat.ballName, NowBallStat.ballKoreanName, tempName);
        TargetBall.ChangeBallSprite(NowBallStat.ballName);
    }
    public void ResetBallQueue(string NowBallName)
    {
        ballStatQueue.Clear();
        for (int i = 0; i < Managers.instance.PlayerDataManager.playerOwnBalls.Count; i++)
        {
            if (Managers.instance.PlayerDataManager.playerOwnBalls[i].amount != 0 && Managers.instance.PlayerDataManager.playerOwnBalls[i].ballName != NowBallName)
            {
                ballStatQueue.Enqueue(Managers.instance.PlayerDataManager.playerOwnBalls[i]);
            }
            else if (Managers.instance.PlayerDataManager.playerOwnBalls[i].amount != 0 && Managers.instance.PlayerDataManager.playerOwnBalls[i].ballName == NowBallName)
            {
                NowBallStat = Managers.instance.PlayerDataManager.playerOwnBalls[i];
            }
        }
    }
    public void SetBall()
    {
        if (ballStatQueue.Count <= 0 && Managers.instance.PlayerDataManager.playerOwnBalls.Count <= 0 && NowBallStat == null)
        {

            Managers.instance.PlayerDataManager.CheckWeaponNextBeforeButton();
            isReadyFire = false;
            Managers.instance.UI.BattleUICall.GameOverBTN.enabled = true;
            TargetBall.BulbLight.intensity = 0;
            return;
        }
        else if (ballStatQueue.Count>0)
        {
            Managers.instance.PlayerDataManager.CheckWeaponNextBeforeButton();
            if (NowBallStat.ballName == string.Empty)
            {
                NowBallStat = ballStatQueue.Dequeue();
            }
            
            TargetBall.Ballsetting(SettingValue(NowBallStat.ballBouncienss, NowBallStat.ballFriction), NowBallStat.weight);
            TargetBall.ChangeBallSprite(NowBallStat.ballName);
            TargetBall.ballNowHP = NowBallStat.ballHealth;

        }
        else if (ballStatQueue.Count <= 0 && NowBallStat.amount > 0)
        {
            Managers.instance.PlayerDataManager.CheckWeaponNextBeforeButton();
            TargetBall.Ballsetting(SettingValue(NowBallStat.ballBouncienss, NowBallStat.ballFriction), NowBallStat.weight);
            TargetBall.ChangeBallSprite(NowBallStat.ballName);
            TargetBall.ballNowHP = NowBallStat.ballHealth;
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
    public void SetBulbSkills(string bulblName)
    {
        if (TargetBall.bulbSkills !=null)
        {
            TargetBall.bulbSkills.Reset();
            TargetBall.bulbSkills = null;
        }
        switch (bulblName)
        {
            case "deco_bulb":
                TargetBall.bulbSkills = new DecoBulbSkill();
                TargetBall.bulbSkills.InitializeSetting();
                break;

            case "hp_bulb":
                //TODO : 전구 스킬구현시 해당위치에 스크립트 넣어주면됨
                TargetBall.bulbSkills = new HPBulbSkill();
                break;

            case "metal_bulb":
                TargetBall.bulbSkills = new metalBulbSkill();
                break;
            case "torch_bulb":
                TargetBall.bulbSkills = new TorchBulbSKill();
                TargetBall.bulbSkills.InitializeSetting();
                break;
            case "shine_bulb":
                TargetBall.bulbSkills = new ShineBulbSkill();
                TargetBall.bulbSkills.InitializeSetting();
                break;
            case "aim_bulb":
                TargetBall.bulbSkills = new AimBulbSkill();
                TargetBall.bulbSkills.InitializeSetting();
                break;
            case "crash_bulb":
                TargetBall.bulbSkills = new crashBulbSkill();
                TargetBall.bulbSkills.InitializeSetting();
                break;
            default:
                Debug.Log("스킬이 없어용");
                TargetBall.bulbSkills = null;
                break;
        }
    }
    public void ParabolaOnOFF(bool value)
    {
        lineRenderer.enabled = value;
    }
}