using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using MonsterDefines;
using System;
using Newtonsoft.Json.Linq;
using UnityEditor;
using DG.Tweening;

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
    public (MonsterStats, SpriteRenderer, Animator)[] Monsters = new (MonsterStats, SpriteRenderer, Animator)[0];
    public Vector3 PlayerPos;
    public Vector3 MonsterSpawnPos;
    public MonsterMoveSlot[] moveSlots = new MonsterMoveSlot[0];
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
            }
            return bombAttackBulb;
        }
    }

    [SerializeField] private int monsterSlotCount;
    private void Awake()
    {
        Array.Resize(ref Monsters, MonsterSpawnOrder.Length);
        Array.Resize(ref moveSlots, monsterSlotCount);

        float slotByXSize = Vector3.Distance(PlayerPos, MonsterSpawnPos) / monsterSlotCount;
        for (int i = 0; i < moveSlots.Length; i++)
        {
            moveSlots[i] = new MonsterMoveSlot();
            moveSlots[i].slotPosition = PlayerPos + (Vector3.right * slotByXSize) + (Vector3.right * (slotByXSize * i));
        }
        NextTurn();
        (float, float) TempDoubleFloat = CarculateMonsterFullHP;
        Managers.instance.UI.BattleUICall.HPBarSetting(false, TempDoubleFloat.Item1, TempDoubleFloat.Item2);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            NextTurn();
        }
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            DamageToAllMonsters(10, (total, count) =>
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
            if (total == count)
            {
                DamageToTargetMonster(targetDamage, TargetMonsterTR, () =>
                {
                    NextTurn();
                    isDone.Invoke();
                });
            }

        });
    }
    public void NextTurn()
    {
        SpawnAndMove();
        AttackPlayer();

    }
    public void DamageToAllMonsters(float damage, Action<int, int> isDamageDone)
    {
        int total = 0;
        int count = 0;
        if (damage <= 0)
        {
            isDamageDone.Invoke(total, count);
            return;
        }
        else
        {
            BombAttackBulb.transform.position = PlayerPos;
            BombAttackBulb.gameObject.SetActive(true);

            BombAttackBulb.transform.DOJump(PlayerPos + (Vector3.right * 5), 2, 2, 1.8f).OnComplete(() =>
            {
                for (int i = 0; i < moveSlots.Length; i++)
                {
                    if (moveSlots[i].monsterTR != null)
                    {
                        total++;
                    }
                }
                for (int i = 0; i < Monsters.Length; i++)
                {

                    if (Monsters[i].Item2 != null && Monsters[i].Item1.isMonsterDie)
                    {
                        continue;
                    }
                    else if (Monsters[i].Item2 != null && !Monsters[i].Item1.isMonsterDie)
                    {
                        Monsters[i].Item1.GetDamage(damage);
                        StartCoroutine(DamagedAnim(i, () =>
                        {
                            count++;
                            if (count == total)
                            {
                                BombAttackBulb.gameObject.SetActive(false);
                                isDamageDone.Invoke(total, count);
                            }
                        }));
                    }
                }

            });
        }


    }
    public void AttackPlayer()
    {
        if (moveSlots[0].monsterTR != null)
        {
            for (int i = 0; i < Monsters.Length; i++)
            {
                if (Monsters[i].Item2.transform == moveSlots[0].monsterTR)
                {
                    // TODO : Monsters[i].Item1.monsterAD; 이용하여 플레이어 피격처리
                    break;
                }
            }
        }
        else
        {
            return;
        }
    }
    public void DamageToTargetMonster(float damage, Transform targetTR, Action isDone)
    {
        Debug.Log("때림");
        if (targetTR != null)
        {
            int tempArray = int.Parse(targetTR.name);

            if (damage <= 0)
            {
                isDone.Invoke();
                return;
            }

            AttackBulb.gameObject.SetActive(true);
            AttackBulb.transform.position = PlayerPos;
            AttackBulb.transform.DOMove(targetTR.position, 0.3f).OnComplete(() =>
            {
                StartCoroutine(DamagedAnim(tempArray, () =>
                {
                    Monsters[int.Parse(targetTR.name)].Item1.GetDamage(damage);
                    isDone.Invoke();
                }));
                AttackBulb.gameObject.SetActive(false);
                AttackBulb.transform.position = PlayerPos;
            });
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
                        moveSlots[i - 1].monsterTR.DOMove(moveSlots[i - 1].slotPosition, 0.2f);
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
    IEnumerator DamagedAnim(int index, Action isDone)
    {
        Monsters[index].Item3.Play("Damaged", 0);
        while (!Monsters[index].Item3.GetCurrentAnimatorStateInfo(0).IsName("Damaged"))
        {
            yield return null;
            Monsters[index].Item3.Play("Damaged", 0);
            while (Monsters[index].Item3.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
            {
                Debug.Log(Monsters[index].Item3.GetCurrentAnimatorStateInfo(0).normalizedTime);
                if (!Monsters[index].Item3.GetCurrentAnimatorStateInfo(0).IsName("Damaged"))
                {
                    break;
                }
                yield return null;
            }
            break;
        }
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
}
