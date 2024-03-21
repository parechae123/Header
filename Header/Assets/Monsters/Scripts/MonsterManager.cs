using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using MonsterDefines;
using System;
using Newtonsoft.Json.Linq;
using UnityEditor;
using DG.Tweening;
using static UnityEngine.GraphicsBuffer;

public class MonsterManager : MonoBehaviour
{
    private static MonsterManager monsterManagerInstance;
    public static MonsterManager MonsterManagerInstance
    {
        get
        {
            if (monsterManagerInstance == null)
            {
                GameObject tempOBJ = GameObject.Find("MonsterManager");
                if (tempOBJ != null)
                {
                    if (tempOBJ.TryGetComponent<MonsterManager>(out MonsterManager TempMC))
                    {
                        monsterManagerInstance = TempMC;
                    }
                    else
                    {
                        tempOBJ.AddComponent<MonsterManager>();
                    }
                }
                else
                {
                    monsterManagerInstance = new GameObject("MonsterManager").AddComponent<MonsterManager>();
                }
            }
            return monsterManagerInstance;
        }
    }

    [Header("몬스터 스폰(프리팹 배열)")]
    public int[] MonsterSpawnOrder = new int[0];
    //인스펙터에서 수정
    public MonsterPrefab[] monsterPrefabs = new MonsterPrefab[0];
    //인스팩터창에서 수정
    public Transform SetTargetMonsters
    {
        get
        {
            Transform target = null;
            for (int i = 0; i < moveSlots.Length; i++)
            {
                if (moveSlots[i].MonsterTR != null)
                {
                    target = moveSlots[i].MonsterTR;
                }
            }
            return target;
        }
    }
    public Vector2 ReturnMonsterSpriteSize(Transform TR)
    {
        if (TR != null)
        {
            if (int.TryParse(TR.name, out int tempNum))
            {
                return Monsters[tempNum].Item2.sprite.bounds.extents;
            }
            else
            {
                return Vector2.one;
            }
        }
        return Vector2.zero;

    }
    private SpriteRenderer playerSprite;
    private SpriteRenderer PlayerSprite
    {
        get 
        { 
            if (playerSprite == null)
            {
                playerSprite = new GameObject("PlayerBattleSceneCharactor").AddComponent<SpriteRenderer>();
                playerSprite.sprite = Managers.instance.Resource.Load<Sprite>("PlayerTopViewWalkR_1");
                playerSprite.transform.position = playerPos;
            }
            return playerSprite;
        }
    }
    [SerializeField]public (MonsterStats, SpriteRenderer)[] Monsters = new (MonsterStats, SpriteRenderer)[0];
    //array.resize로 해당 상황에 맞춰 배열 추가
    public Vector3 playerPos;
    public Vector3 monsterSpawnPos;
    public MonsterMoveSlot[] moveSlots = new MonsterMoveSlot[0];
    //array.resize로 해당 상황에 맞춰 배열 추가
    [SerializeField] private int monsterSlotCount;
    private SpriteRenderer attackBulb;
    private SpriteRenderer AttackBulb
    {
        get
        {
            if (attackBulb == null)
            {
                attackBulb = new GameObject("AttackBulb").AddComponent<SpriteRenderer>();
            }
            attackBulb.sprite = Managers.instance.Resource.Load<Sprite>(ShoterController.Instance.NowBallStat == null ? string.Empty : ShoterController.Instance.NowBallStat.ballName);
            return attackBulb;
        }
    }
    private SpriteRenderer bombAttackBulb;
    private SpriteRenderer BombAttackBulb
    {
        get
        {
            if (bombAttackBulb == null)
            {
                bombAttackBulb = new GameObject("BombAttackBulb").AddComponent<SpriteRenderer>();
                bombAttackBulb.sprite = Managers.instance.Resource.Load<Sprite>("bomb");
            }
            return bombAttackBulb;
        }
    }
    private Transform bombTR
    {
        get 
        {
            return BombAttackBulb.transform;
        }
    }
    [Header("폭탄 연출값")]
    private float bounceCount=3;
    private float bounceForce=1.6f;
    private Vector3 targetPosition;
    private float movementTime = 1.2f;


    private void Awake()
    {
        Managers.instance.UI.BattleUICall.InstallMonsterHPBar(monsterSlotCount);
        (float, float) TempDoubleFloat = CarculateMonsterFullHP;
        Managers.instance.UI.BattleUICall.HPBarSetting(false, TempDoubleFloat.Item1, TempDoubleFloat.Item2);
        Array.Resize(ref Monsters, MonsterSpawnOrder.Length);
        Array.Resize(ref moveSlots, monsterSlotCount);
        PlayerSprite.color = Color.white;
        Managers.instance.UI.BattleUICall.HPBarActivate(Managers.instance.PlayerDataManager.SetPlayerHP.Item1, Managers.instance.PlayerDataManager.SetPlayerHP.Item2);
        float slotByXSize = Vector3.Distance(playerPos, monsterSpawnPos) / monsterSlotCount;
        for (int i = 0; i < moveSlots.Length; i++)
        {
            moveSlots[i] = new MonsterMoveSlot();
            moveSlots[i].slotPosition = playerPos + (Vector3.right * slotByXSize) + (Vector3.right * (slotByXSize * i));
        }
        NextTurn();

        targetPosition = playerPos+Vector3.right*4;
/*        Queue<Sprite> monsterQueue = new Queue<Sprite>();
        for (int i = 0; i < MonsterSpawnOrder.Length; i++)
        {
            monsterQueue.Enqueue(monsterPrefabs[MonsterSpawnOrder[i]].prefab.GetComponent<SpriteRenderer>().sprite);
        }*/
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            NextTurn();
        }
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            DamageToAllMonsters(100, (total, count) =>
            {
                if (total == count)
                {

                }
            });
        }

    }
    public void NextTurnFunctions(float regionalDamage, float targetDamage, Transform TargetMonsterTR, Action isDone)
    {
        DamageToAllMonsters(regionalDamage, (total, count) =>
        {

            Debug.Log("토탈" + total + '\n' + "카운트" + count);
            Debug.Log("regionalDamage" + regionalDamage + '\n' + "targetDamage" + targetDamage);
            DamageToTargetMonster(targetDamage, TargetMonsterTR, () =>
            {
                NextTurn(() =>
                {
                    
                    isDone.Invoke();
                });
            });

        });
    }
    public void NextTurn(Action isDone = null)
    {
        AttackPlayer(() =>
        {
            SpawnAndMove();
            if (isDone != null)
            {
                isDone.Invoke();
            }
        });

    }
    public void DamageToAllMonsters(float damage, Action<int, int> isDamageDone,bool isGirlThrowing = false)
    {
        int total = 0;
        int count = 0;
        int actionTime = 0;
        if (damage <= 0)
        {
            isDamageDone.Invoke(total, count);
            return;
        }
        else
        {
            for (int i = 0; i < Monsters.Length; i++)
            {
                if (Monsters[i].Item2 != null && !Monsters[i].Item1.isMonsterDie)
                {
                    total++;
                }
            }
            bombTR.position = playerPos;
            BombAttackBulb.gameObject.SetActive(true);

            if (!isGirlThrowing)
            {
                //헤더가 던지는 로직
                StartCoroutine(MoveBalls(GetBounceVectors(),false, () =>
                    {
                        for (int i = 0; i < Monsters.Length; i++)
                        {
                            if (Monsters[i].Item2 != null && !Monsters[i].Item1.isMonsterDie)
                            {
                                Monsters[i].Item1.GetDamage(damage);
                                Managers.instance.UI.BattleUICall.SetMonsterHPBar(Monsters[i].Item2.transform.position,ReturnMonstersToSlotArray(Monsters[i].Item2.transform), Monsters[i].Item1.monsterHPMax, Monsters[i].Item1.monsterHPNow);
                                StartCoroutine(DamagedAnim(i, false, () =>
                                {
                                    count++;
                                    Debug.Log("카운트" + count + "토탈" + total);
                                    if (actionTime == 0)
                                    {
                                        BombAttackBulb.gameObject.SetActive(false);
                                        Managers.instance.UI.BattleUICall.SetTargetUI(ShoterController.Instance.TargetMonsterTR, MonsterManager.MonsterManagerInstance.ReturnMonsterSpriteSize(ShoterController.Instance.TargetMonsterTR));
                                        isDamageDone.Invoke(total, count);
                                    }
                                    actionTime++;
                                }));
                            }
                        }
                        if (total == 0)
                        {
                            isDamageDone.Invoke(total, count);
                            return;
                        }
                    })); 
            }
            else
            {
                //소녀가 던지는 로직
                StartCoroutine(MoveBalls(GetVectorGirlBomb(),true, () =>
                {
                    for (int i = 0; i < Monsters.Length; i++)
                    {
                        if (Monsters[i].Item2 != null && !Monsters[i].Item1.isMonsterDie)
                        {
                            Monsters[i].Item1.GetDamage(damage);
                            Managers.instance.UI.BattleUICall.SetMonsterHPBar( Monsters[i].Item2.transform.position, ReturnMonstersToSlotArray(Monsters[i].Item2.transform), Monsters[i].Item1.monsterHPMax, Monsters[i].Item1.monsterHPNow);
                            StartCoroutine(DamagedAnim(i, false, () =>
                            {
                                count++;
                                Debug.Log("카운트" + count + "토탈" + total);    
                                if (actionTime == 0)
                                {
                                    BombAttackBulb.gameObject.SetActive(false);
                                    Managers.instance.UI.BattleUICall.GirlBomb.gameObject.SetActive(false);
                                    Managers.instance.UI.BattleUICall.SetTargetUI(ShoterController.Instance.TargetMonsterTR, MonsterManager.MonsterManagerInstance.ReturnMonsterSpriteSize(ShoterController.Instance.TargetMonsterTR));
                                    isDamageDone.Invoke(total, count);
                                }
                                actionTime++;
                            }));
                        }
                    }
                    if (total == 0)
                    {
                        isDamageDone.Invoke(total, count);
                        return;
                    }
                }));
            }

        }


    }
    public void AttackPlayer(Action isDone)
    {
        if (moveSlots[0].MonsterTR != null)
        {
            int tempArray = ReturnSlotToMonsters(moveSlots[0].MonsterTR);
            if (Monsters[tempArray].Item1 != null)
            {
                if (!Monsters[tempArray].Item1.isMonsterDie)
                {
                    // TODO : Monsters[i].Item1.monsterAD; 이용하여 플레이어 피격처리
                    StartCoroutine(PlayerDamagedAnim(Monsters[tempArray].Item2.transform, () =>
                    {
                        Debug.Log("데미지");
                        Managers.instance.PlayerDataManager.PlayerGetDamage(Monsters[tempArray].Item1.monsterAD);
                        isDone.Invoke();
                    }));
                }
                else
                {
                    isDone.Invoke();
                }

            }
            else
            {
                isDone.Invoke();
            }
        }
        else
        {
            isDone.Invoke();
            return;
        }
    }
    public void DamageToTargetMonster(float damage, Transform targetTR, Action isDone)
    {
        if (targetTR != null)
        {
            int tempArray = int.Parse(targetTR.name);

            if (damage <= 0 || !targetTR.gameObject.activeSelf)
            {
                isDone.Invoke();
                return;
            }

            AttackBulb.gameObject.SetActive(true);
            AttackBulb.transform.position = playerPos;
            if (targetTR.gameObject.activeSelf)
            {
                StartCoroutine(DamagedAnim(tempArray,true, () =>
                {
                    int tempNun = int.Parse(targetTR.name);
                    Monsters[tempNun].Item1.GetDamage(damage);
                    Managers.instance.UI.BattleUICall.SetMonsterHPBar(Monsters[tempNun].Item2.transform.position, ReturnMonstersToSlotArray(Monsters[tempNun].Item2.transform), Monsters[tempNun].Item1.monsterHPMax, Monsters[tempNun].Item1.monsterHPNow);
                    AttackBulb.transform.position = playerPos;
                    AttackBulb.gameObject.SetActive(false);
                    Managers.instance.UI.BattleUICall.SetTargetUI(ShoterController.Instance.TargetMonsterTR, MonsterManager.MonsterManagerInstance.ReturnMonsterSpriteSize(ShoterController.Instance.TargetMonsterTR));
                    isDone.Invoke();
                }));
            }
            else 
            {
                AttackBulb.gameObject.SetActive(false);
                isDone.Invoke();
            }

        }
        else
        {
            isDone.Invoke();
            return;
        }
    }
    public void SpawnAndMove()
    {
        int spawnArray = -1;
        for (int i = 0; i < MonsterSpawnOrder.Length; i++)
        {

            if (Monsters[i].Item1 == null)
            {
                spawnArray = i;
                break;
            }
            else
            {
                MoveMonsters(i);
            }
        }
        if (moveSlots[moveSlots.Length - 1].MonsterTR == null && spawnArray != -1)
        {
            SpawnMonsters(MonsterSpawnOrder[spawnArray], spawnArray);
        }
    }
    public void MoveMonsters(int array)
    {
        for (int i = 0; i < moveSlots.Length; i++)
        {
            for (int E = 0; E < Monsters.Length; E++)
            {
                if (Monsters[E].Item2 != null)
                {
                    if (moveSlots[i].MonsterTR == Monsters[E].Item2.transform)
                    {
                        if (Monsters[E].Item1.isMonsterDie)
                        {
                            Debug.Log(moveSlots[i].MonsterTR.name+ Monsters[E].Item1.monsterHPNow);
                            Monsters[E].Item2.gameObject.SetActive(false);
//                            Managers.instance.UI.BattleUICall.SetMonsterDeadInQueue(E);
                            Managers.instance.UI.BattleUICall.SetMonsterHPBar(moveSlots[i].slotPosition, i);
                            moveSlots[i].MonsterTR = null;
                        }
                    }
                }
                else
                {
                    break;
                }
            }
        }
        if (!Monsters[array].Item1.isMonsterDie)
        {
            for (int i = 0; i < moveSlots.Length; i++)
            {
                if (moveSlots[i].MonsterTR == Monsters[array].Item2.transform && i != 0)
                {
                    if (moveSlots[i - 1].MonsterTR == null)
                    {
                        moveSlots[i - 1].MonsterTR.DOComplete();
                        moveSlots[i].MonsterTR = null;
                        moveSlots[i - 1].MonsterTR = Monsters[array].Item2.transform;
                        Managers.instance.UI.BattleUICall.SetMonsterHPBar(moveSlots[i - 1].slotPosition, i - 1, Monsters[array].Item1.monsterHPMax, Monsters[array].Item1.monsterHPNow);
                        Managers.instance.UI.BattleUICall.SetMonsterHPBar(moveSlots[i].slotPosition, i);
                        moveSlots[i - 1].MonsterTR.DOMove(moveSlots[i - 1].slotPosition, 0.2f).OnComplete(()=> 
                        {

                            Managers.instance.UI.BattleUICall.SetTargetUI(ShoterController.Instance.TargetMonsterTR,ReturnMonsterSpriteSize(ShoterController.Instance.TargetMonsterTR));
                            moveSlots[i - 1].MonsterTR.DOKill();
                        });
                    }
                    else
                    {
                        continue;
                    }
                }
            }

        }
    }
    public void SpawnMonsters(int prefabNum, int arrayOrder)
    {
        GameObject tempGOBJ = Instantiate(monsterPrefabs[prefabNum].prefab, moveSlots[moveSlots.Length - 1].slotPosition, Quaternion.identity, null);
        //TODO : 몬스터 이름을 해당 배열로 바꿔서 클릭시 이름을 가져오고 해당 배열이 타겟이 되도록
        
        tempGOBJ.name = arrayOrder.ToString();
        Monsters[arrayOrder].Item1 = new MonsterStats(monsterPrefabs[prefabNum].stat);
        SpriteRenderer tempComponent = Monsters[arrayOrder].Item1.SetMonsterSprite(ref tempGOBJ);
        Monsters[arrayOrder].Item2 = tempComponent;
        Monsters[arrayOrder].Item2.enabled = false;

        if (Managers.instance.UI.BattleUICall.isInFeverMode)
        {
            Monsters[arrayOrder].Item2.color = Color.red;
            Monsters[arrayOrder].Item1.monsterAD = monsterPrefabs[prefabNum].stat.monsterAD * 2;
        }
        moveSlots[moveSlots.Length - 1].MonsterTR = tempComponent.transform;
        Managers.instance.UI.BattleUICall.SetUIMonsterImageArray(monsterPrefabs[MonsterSpawnOrder[arrayOrder + 1]].prefab.GetComponent<SpriteRenderer>().sprite, Monsters[arrayOrder].Item2);
        Managers.instance.UI.BattleUICall.SetMonsterHPBar(moveSlots[moveSlots.Length - 1].slotPosition, moveSlots.Length - 1, Monsters[arrayOrder].Item1.monsterHPMax, Monsters[arrayOrder].Item1.monsterHPNow);
    }
    IEnumerator DamagedAnim(int index,bool isTargetAttack, Action isDone)
    {
        Color tempColor = Monsters[index].Item2.color;
        if (Managers.instance.UI.BattleUICall.isInFeverMode)
        {
            tempColor = Color.red;
        }
        if (isTargetAttack)
        {
            Vector3 tempPlrPos = Monsters[index].Item2.transform.position - playerPos;
            
            for (int i = 0; i < 20; i++)
            {
                AttackBulb.transform.position += Vector3.right * (tempPlrPos.x / 20);
                yield return new WaitForSeconds(0.03f);
            }
        }
        for (int i = 0; i < 3; i++)
        {
            
            Monsters[index].Item2.color = new Color(0, 0, 0, 0);
            yield return new WaitForSeconds(0.13f);
            Monsters[index].Item2.color = tempColor;
            yield return new WaitForSeconds(0.13f);

        }
        Debug.Log("여기서 못나가네"+index);
        if (Monsters[index].Item1.isMonsterDie)
        {
/*            Managers.instance.UI.BattleUICall.SetMonsterDeadInQueue(index);*/
            Monsters[index].Item2.gameObject.SetActive(false);
            for (int i = 0; i < monsterSlotCount; i++)
            {
                if (moveSlots[i].MonsterTR == Monsters[index].Item2.transform)
                {
                    moveSlots[i].MonsterTR = null;
                    break;
                }
            }
            Monsters[index].Item2.transform.position = Vector3.up * 10;
        }
        isDone.Invoke();
    }
    public (float, float) CarculateMonsterFullHP
    {
        get
        {
            (float, float) MaxAndNowHP = (0, 0);
            for (int i = 0; i < MonsterSpawnOrder.Length; i++)
            {
                MaxAndNowHP.Item1 += monsterPrefabs[MonsterSpawnOrder[i]].stat.monsterHPMax;
                MaxAndNowHP.Item2 += monsterPrefabs[MonsterSpawnOrder[i]].stat.monsterHPNow;
            }
            return MaxAndNowHP;
        }
    }

    public int ReturnMonstersToSlotArray(Transform MonsterTR)
    {
        if (MonsterTR != null)
        {
            for (int i = 0; i < moveSlots.Length; i++)
            {
                if (moveSlots[i].MonsterTR == MonsterTR)
                {
                    return i;
                }
            } 
        }
        return -1;
    }
    public int ReturnSlotToMonsters(Transform MonsterTR)
    {
        if (MonsterTR != null)
        {
            for (int i = 0; i < Monsters.Length; i++)
            {
                if (Monsters[i].Item2.transform == MonsterTR)
                {
                    return i;
                }
            } 
        }
        return -1;
    }
    public Vector3[] GetBounceVectors()
    {
        float tempY;
        float timeX = 0;
        int counter = 0;
        float tempPos = targetPosition.x - playerPos.x;
        Debug.Log(tempPos);
        tempPos = tempPos / bounceCount;
        tempPos = tempPos / (314f / 30f);
        Debug.Log(tempPos);
        Vector3[] vectorArray = new Vector3[(int)((314f / 30f) * bounceCount) + ((int)bounceCount - 1)];
        Debug.Log(vectorArray.Length);
        for (int i = 0; i < bounceCount; i++)
        {
            for (float p = 0; p < 3.14f;)
            {
                timeX += tempPos;
                p += 0.3f;
                tempY = Mathf.Sin(p);
                Vector3 tempVec = new Vector3(timeX, (tempY * bounceForce) / (float)(i + 1), 0);
                if (counter >= vectorArray.Length)
                {
                    Debug.Log(counter);
                }
                vectorArray[counter] = playerPos + tempVec;
                counter++;
            }
        }
        return vectorArray;
    }
    public Vector3[] GetVectorGirlBomb()
    {
        Vector3[] vectorArray = new Vector3[0];
        //포문을 통해 계속 리사이징
        Vector3 startPos = Managers.instance.UI.BattleUICall.GirlPortrait.rectTransform.position;
        Vector3 endPos = Camera.main.WorldToScreenPoint(moveSlots[0].slotPosition);
        Vector2 tempNomalVal = startPos+endPos;
        tempNomalVal = tempNomalVal / 30;
        for (int i = 0;i <30;i++)
        {
            Array.Resize(ref vectorArray, i+1);
            vectorArray[i] = new Vector2(startPos.x+(tempNomalVal.x*i), startPos.y + (tempNomalVal.y * i));

        }
        Vector3 controlPoint = (startPos + endPos) / 2  +Vector3.up * 100 + Vector3.left * 400;

        for (int i = 0; i < 30; i++)
        {
            Array.Resize(ref vectorArray, i + 1);
            float t = i / (float)(30 - 1);
            vectorArray[i] = CalculateBezierPoint(t, startPos, controlPoint, endPos);
        }


        return vectorArray;
    }
    private static Vector2 CalculateBezierPoint(float t, Vector2 p0, Vector2 p1, Vector2 p2)
    {
        // 베지어 곡선 공식
        return (1 - t) * (1 - t) * p0 + 2 * (1 - t) * t * p1 + t * t * p2;
    }
    IEnumerator MoveBalls(Vector3[] vectors, bool isGirl, Action isDone)
    {
        if (!isGirl)
        {
            float timeDelay = (movementTime / vectors.Length);
            timeDelay = timeDelay <= 0 ? 0.1f : timeDelay;
            for (int i = 0; i < vectors.Length; i++)
            {
                yield return new WaitForSeconds(timeDelay);

                bombTR.position = vectors[i];
            }
            isDone.Invoke();
        }
        else
        {
            Managers.instance.UI.BattleUICall.GirlBomb.gameObject.SetActive(true);
            float timeDelay = (movementTime / vectors.Length);
            timeDelay = timeDelay <= 0 ? 0.1f : timeDelay;
            for (int i = 0; i < vectors.Length; i++)
            {
                yield return new WaitForSeconds(timeDelay);

                Managers.instance.UI.BattleUICall.GirlBomb.rectTransform.position = vectors[i];
            }
            isDone.Invoke();
        }
    }
    IEnumerator PlayerDamagedAnim(Transform monster,Action isDone)
    {
        Vector3 originPos = monster.transform.position;
        yield return monster.DOJump(playerPos, 0.7f, 1, 0.5f).WaitForCompletion();
        for (int i = 0; i < 2; i++)
        {
            PlayerSprite.color = Color.red;
            yield return new WaitForSeconds(0.12f);
            PlayerSprite.color = Color.white;
            yield return new WaitForSeconds(0.12f);
        }
        monster.DOComplete();
        yield return monster.DOJump(originPos, 0.7f, 1, 0.5f).WaitForCompletion();
        isDone.Invoke();

    }
    public void SetFeaverMode()
    {
        if (Managers.instance.UI.BattleUICall.isInFeverMode)
        {
            for (int i = 0; i < Monsters.Length; i++)
            {
                if (Monsters[i].Item1 != null)
                {
                    Monsters[i].Item1.monsterAD = Monsters[i].Item1.monsterAD * 2;
                    Monsters[i].Item2.color = Color.red;
                }
                else
                {
                    return;
                }
            }
        }
    }
}
