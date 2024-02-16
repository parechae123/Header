using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using MonsterDefines;
using System;

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
    
    public (MonsterStats, SpriteRenderer, Animator)[] Monsters = new (MonsterStats, SpriteRenderer, Animator)[0];
    public Vector3 PlayerPos;
    public Vector3 MonsterSpawnPos;
    private void Awake()
    {
        Array.Resize(ref Monsters, MonsterSpawnOrder.Length);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            Spawn();
        }
    }
    public void DamageToAllMonsters(int damage)
    {

    }
    public void DamageToTargetMonster(int damage,int targetMonsterArray)
    {

    }
    public void Spawn()
    {
        int spawnArray = -1;
        for (int i = 0; i < MonsterSpawnOrder.Length; i++) 
        {
            if (Monsters[i].Item1 == null)
            {
                spawnArray = i;
                break;
            }
        }
        if (spawnArray != -1)
        {
            SpawnMonsters(MonsterSpawnOrder[spawnArray], spawnArray);
        }
    }
    public void SpawnMonsters(int prefabNum,int arrayOrder)
    {
        GameObject tempGOBJ = Instantiate(monsterPrefabs[prefabNum].prefab, MonsterSpawnPos, Quaternion.identity, null);
        //TODO : 몬스터 이름을 해당 배열로 바꿔서 클릭시 이름을 가져오고 해당 배열이 타겟이 되도록
        tempGOBJ.name = arrayOrder.ToString();
        Monsters[arrayOrder].Item1 = new MonsterStats(monsterPrefabs[prefabNum].stat);
        (SpriteRenderer,Animator) tempComponents = Monsters[arrayOrder].Item1.monsterAnimSprite(tempGOBJ);
        Monsters[arrayOrder].Item2 = tempComponents.Item1;
        Monsters[arrayOrder].Item3 = tempComponents.Item2;
    }
}
