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
    public MonsterPrefab[] monsterPrefabs = new MonsterPrefab[0];
    public Transform SetTargetMonsters
    {
        get
        {
            Transform target = null;
            for (int i = 0; i < moveSlots.Length; i++)
            {
                if (moveSlots[i].monsterTR != null)
                {
                    target = moveSlots[i].monsterTR;
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
                playerSprite.flipY = true;
            }
            return playerSprite;
        }
    }
    public (MonsterStats, SpriteRenderer, Animator)[] Monsters = new (MonsterStats, SpriteRenderer, Animator)[0];
    public Vector3 playerPos;
    public Vector3 monsterSpawnPos;
    public MonsterMoveSlot[] moveSlots = new MonsterMoveSlot[0];
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
            attackBulb.sprite = Managers.instance.Resource.Load<Sprite>(ShoterController.Instance.NowBallStat.ballName);
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
                bombAttackBulb.flipY = true;
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
        (float, float) TempDoubleFloat = CarculateMonsterFullHP;
        Managers.instance.UI.BattleUICall.HPBarSetting(false, TempDoubleFloat.Item1, TempDoubleFloat.Item2);
        targetPosition = playerPos+Vector3.right*4;
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
    public void DamageToAllMonsters(float damage, Action<int, int> isDamageDone)
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
            for (int i = 0; i < moveSlots.Length; i++)
            {
                if (Monsters[i].Item2 != null && !Monsters[i].Item1.isMonsterDie)
                {
                    total++;
                }
            }
            bombTR.position = playerPos;
            BombAttackBulb.gameObject.SetActive(true);

            StartCoroutine(MoveBalls(GetBounceVectors(), () =>
            {
                for (int i = 0; i < Monsters.Length; i++)
                {
                    if (Monsters[i].Item2 != null && !Monsters[i].Item1.isMonsterDie)
                    {
                        StartCoroutine(DamagedAnim(i,false, () =>
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
            }));

        }


    }
    public void AttackPlayer(Action isDone)
    {
        if (moveSlots[0].monsterTR != null)
        {
            for (int i = 0; i < Monsters.Length; i++)
            {
                if (Monsters[i].Item2.transform == moveSlots[0].monsterTR)
                {
                    // TODO : Monsters[i].Item1.monsterAD; 이용하여 플레이어 피격처리
                    StartCoroutine(PlayerDamagedAnim(Monsters[i].Item2.transform, () =>
                    {
                        Debug.Log("데미지");
                        Managers.instance.PlayerDataManager.PlayerGetDamage(Monsters[i].Item1.monsterAD);
                        isDone.Invoke();
                    }));

                    break;
                }
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

            if (damage <= 0)
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
                    Monsters[int.Parse(targetTR.name)].Item1.GetDamage(damage);
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
        if (moveSlots[moveSlots.Length - 1].monsterTR == null && spawnArray != -1)
        {
            SpawnMonsters(MonsterSpawnOrder[spawnArray], spawnArray);
        }
    }
    public void MoveMonsters(int array)
    {
        if (!Monsters[array].Item1.isMonsterDie)
        {
            for (int i = 0; i < moveSlots.Length; i++)
            {
                if (moveSlots[i].monsterTR == Monsters[array].Item2.transform && i != 0)
                {
                    if (moveSlots[i - 1].monsterTR == null)
                    {
                        moveSlots[i - 1].monsterTR.DOComplete();
                        moveSlots[i].monsterTR = null;
                        moveSlots[i - 1].monsterTR = Monsters[array].Item2.transform;
                        moveSlots[i - 1].monsterTR.DOMove(moveSlots[i - 1].slotPosition, 0.2f).OnComplete(()=> 
                        {
                            Managers.instance.UI.BattleUICall.SetTargetUI(ShoterController.Instance.TargetMonsterTR, MonsterManager.MonsterManagerInstance.ReturnMonsterSpriteSize(ShoterController.Instance.TargetMonsterTR));
                        });
                    }
                    else
                    {
                        return;
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
        (SpriteRenderer, Animator) tempComponents = Monsters[arrayOrder].Item1.monsterAnimSprite(tempGOBJ);
        Monsters[arrayOrder].Item2 = tempComponents.Item1;
        Monsters[arrayOrder].Item3 = tempComponents.Item2;
        moveSlots[moveSlots.Length - 1].monsterTR = tempComponents.Item1.transform;
    }
    IEnumerator DamagedAnim(int index,bool isTargetAttack, Action isDone)
    {
        if (isTargetAttack)
        {
            Vector3 tempPlrPos = Monsters[index].Item2.transform.position - playerPos;
            
            for (int i = 0; i < 20; i++)
            {
                AttackBulb.transform.position += Vector3.right * (tempPlrPos.x / 20);
                yield return null;
            }
        }
        Monsters[index].Item3.Play("Damaged", 0);
        float animTimer = 0;
        while (Monsters[index].Item3.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1 || !Monsters[index].Item3.GetCurrentAnimatorStateInfo(0).IsName("Damaged"))
        {
            /*            if (!Monsters[index].Item3.GetCurrentAnimatorStateInfo(0).IsName("Damaged"))
                        {
                            Monsters[index].Item3.Play("Damaged", 0);
                        }*/
            animTimer += Time.deltaTime;
            if (animTimer> 3)
            {
                break;
            }
            Debug.Log(Monsters[index].Item3.GetCurrentAnimatorStateInfo(0).normalizedTime + "노멀라이즈 타임"+'\n'+ Monsters[index].Item3.GetCurrentAnimatorStateInfo(0).IsName("Damaged"));
            yield return null;
        }
        Debug.Log("여기서 못나가네"+index);
        if (Monsters[index].Item1.isMonsterDie)
        {
            Monsters[index].Item2.gameObject.SetActive(false);
            for (int i = 0; i < monsterSlotCount; i++)
            {
                if (moveSlots[i].monsterTR == Monsters[index].Item2.transform)
                {
                    moveSlots[i].monsterTR = null;
                    break;
                }
            }
            Monsters[index].Item2.transform.position = Vector3.up * 10;
        }
        else
        {
            Monsters[index].Item3.Play("Idle", 0);
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
                Vector3 tempVec = new Vector3(timeX, (tempY * -bounceForce) / (float)(i + 1), 0);
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
    IEnumerator MoveBalls(Vector3[] vectors,Action isDone)
    {
        float timeDelay = (movementTime / vectors.Length);
        timeDelay = timeDelay<=0? 0.1f: timeDelay;
        for (int i = 0; i < vectors.Length; i++)
        {
            yield return new WaitForSeconds(timeDelay);
            bombTR.position = vectors[i];
        }
        
        isDone.Invoke();
    }
    IEnumerator PlayerDamagedAnim(Transform monster,Action isDone)
    {
        Vector3 originPos = monster.transform.position;
        yield return monster.DOJump(playerPos, -0.7f, 1, 0.5f).WaitForCompletion();
        PlayerSprite.color = Color.red;
        yield return new WaitForSeconds(0.12f);
        PlayerSprite.color = Color.white;
        yield return new WaitForSeconds(0.12f);
        PlayerSprite.color = Color.red;
        yield return new WaitForSeconds(0.12f);
        PlayerSprite.color = Color.white;
        monster.DOComplete();
        yield return monster.DOJump(originPos, -0.7f, 1, 0.5f).WaitForCompletion();
        isDone.Invoke();

    }
}
