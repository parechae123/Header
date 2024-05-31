using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Newtonsoft.Json;
using System;
using UnityEngine.UI;
using Unity.VisualScripting;

public class Defines
{

}
namespace DataDefines
{
    [System.Serializable]
    public class ResourceDefine
    {
        public string LabelName;
        public ResourceType Type;
    }
    [System.Serializable]
    public enum ResourceType
    {
        // MAINTANCE : 리소스 타입 추가할때 여기서 Enum 추가작업 필요
        GameObject, Sprites,DataSheets,Fonts,RenderTexture,Video,Texture2D,Materials,AudioClip
    }
    [System.Serializable]
    public class DialogDatas
    {
        public int EventName;
        public string Portrait;
        public string Background;
        public string Name;
        public string sound;
        public string dialogue;
    }
}
namespace InteractionDefines
{
    [System.Serializable]
    public class InteractionDetailPosition
    {
        public Vector2Int installedInteractionPosition = Vector2Int.zero;
        public bool interactionRemoveSelf = false;
    }

    [System.Serializable]
    public class InteractionInstallerProps
    {
        public Vector2Int interactionPosition;
        public InteractionDetailPosition detail;
        
        public InteractionTypes interactionTypes;
        public short keyNumber;
        public string SceneName;
        [Header("merchant만 해당")]
        public string[] weaponNameList;
    }

    public enum InteractionTypes
    {
        //MAINTANCE : 인터렉션 종류 추가 시 여기에 정의 필요
        None, DialogInteraction,MerchantInteraction,ToBattleScene
    }
}
namespace HeaderPadDefines
{
    [System.Serializable]
    public class BlockObjects
    {
        public BlockStatus blockCondition;
        public BlockStatus SettedBlockCondition;
        public SpriteRenderer targetIMG;
        public short BlockHP;
        public delegate void blockEvent();
        public event blockEvent BE;
        public float originBlockValue;
        public float blockValue;
        public void OnColideBlock()
        {
            switch (blockCondition)
            {
                case BlockStatus.Destroyed:
                    break;
                case BlockStatus.Emptied:
                    Managers.instance.SoundManager.SFXPlayOneshot(Managers.instance.Resource.Load<AudioClip>("EmptyBlock"),true);
                    blockCondition = BlockStatus.Destroyed;
                    targetIMG.transform.GetComponent<PolygonCollider2D>().enabled = false;
                    targetIMG.sprite = Managers.instance.Resource.Load<Sprite>("HeaderBlock_Destroyed");
                    ShoterController.Instance.targetDamage += blockValue;
                    Managers.instance.UI.battleUICall.SetComboNumber(true);
                    //TODO : 볼 충돌시 갱신되는 모든걸 여기에 넣으면될듯,EX : 데미지
                    break;
                case BlockStatus.FIlled:
                    Managers.instance.SoundManager.SFXPlayOneshot(Managers.instance.Resource.Load<AudioClip>("FilledBlock"),true);
                    blockCondition = BlockStatus.Emptied;
                    targetIMG.sprite = Managers.instance.Resource.Load<Sprite>("HeaderBlock_Emptied");
                    ShoterController.Instance.targetDamage += blockValue;
                    Managers.instance.UI.battleUICall.SetComboNumber(true);
                    break;
                case BlockStatus.FilledCoin:
                    Managers.instance.SoundManager.SFXPlayOneshot(Managers.instance.Resource.Load<AudioClip>("AllBlocks"), true);
                    blockCondition = BlockStatus.Emptied;
                    targetIMG.sprite = Managers.instance.Resource.Load<Sprite>("HeaderBlock_Emptied");
                    Managers.instance.PlayerDataManager.PlayerMoney += (int)blockValue;
                    Managers.instance.UI.battleUICall.SetComboNumber(true);
                    break;
                case BlockStatus.BoombBlock:
                    if (BlockHP>= 0)
                    {
                        Managers.instance.SoundManager.SFXPlayOneshot(Managers.instance.Resource.Load<AudioClip>("AllBlocks"), true);
                        //targetIMG.sprite = Managers.instance.Resource.Load<Sprite>("HeaderBlock_Emptied"+BlockHP);
                        BlockHP--;
                    }
                    else
                    {
                        Managers.instance.SoundManager.SFXPlayOneshot(Managers.instance.Resource.Load<AudioClip>("AllBlocks"), true);
                        ShoterController.Instance.regionalDamage += blockValue;
                        blockCondition = BlockStatus.Emptied;
                        targetIMG.transform.GetComponent<PolygonCollider2D>().enabled = false;
                        Managers.instance.UI.battleUICall.SetComboNumber(true);
                        targetIMG.sprite = Managers.instance.Resource.Load<Sprite>("HeaderBlock_Destroyed");
                    }
                    break;
                case BlockStatus.reroll:
                    Managers.instance.SoundManager.SFXPlayOneshot(Managers.instance.Resource.Load<AudioClip>("AllBlocks"),true);
                    Managers.instance.Grid.OnReset();
                    blockCondition = BlockStatus.Emptied;
                    targetIMG.sprite = Managers.instance.Resource.Load<Sprite>("HeaderBlock_Emptied");
                    Managers.instance.UI.battleUICall.SetComboNumber(true);
                    break;
            }
            Managers.instance.UI.battleUICall.GirlBulbExplane = "폭탄 데미지 : " + ShoterController.Instance.regionalDamage + '\n' + "단일 데미지 : " + ShoterController.Instance.targetDamage;
            if (BE != null)
            {
                BE();
            }
        }
        public void OnResetBlocks()
        {
            targetIMG.color = Color.white;
            blockValue = originBlockValue;
            if (SettedBlockCondition != BlockStatus.Destroyed)
            {
                switch (SettedBlockCondition)
                {
                    case BlockStatus.Emptied:
                        targetIMG.sprite = Managers.instance.Resource.Load<Sprite>("HeaderBlock_Emptied");
                        break;
                    case BlockStatus.FIlled:
                        targetIMG.sprite = Managers.instance.Resource.Load<Sprite>("HeaderBlock_FIlled");
                        break;
                    case BlockStatus.FilledCoin:
                        targetIMG.sprite = Managers.instance.Resource.Load<Sprite>("HeaderBlock_FilledCoin");
                        break;
                    case BlockStatus.BoombBlock:
                        targetIMG.sprite = Managers.instance.Resource.Load<Sprite>("HeaderBlock_BoombBlock");
                        BlockHP = 2;
                        break;
                    case BlockStatus.reroll:
                        targetIMG.sprite = Managers.instance.Resource.Load<Sprite>("HeaderBlock_Reroll");
                        break;
                }
                blockCondition = SettedBlockCondition;
                targetIMG.transform.GetComponent<PolygonCollider2D>().enabled = true;
            }
        }

        public void ChangeColor(float minusColorValue)
        {
            targetIMG.color = targetIMG.color - new Color(minusColorValue, minusColorValue, minusColorValue, 0);
            blockValue += originBlockValue;
        }
    }
    [System.Serializable]
    public class BallStat
    {
        public string ballName;
        public string ballKoreanName;
        public float ballStartForce;
        public float ballBouncienss;
        public float ballFriction;
        public float weight; //무게 mass에 넣어줘야함
        public int amount;
        public int ballPrice;
        public int ballHealth;
        public string flavorText;
    }
    [System.Serializable]
    public class ExtraBallStat : BallStat
    {
        public float SkillValueOne;
        public float SkillValueTwo;
    }
    public enum BlockStatus
    {
        Destroyed,Emptied,FIlled,FilledCoin,BoombBlock,MovePlatform,reroll
    }
}
namespace MonsterDefines
{
    [System.Serializable]
    public class MonsterStats
    {
        public MonsterStats(MonsterStats monsterStat = null) 
        {
            if (monsterStat == null)
            {
                return;
            }
            else
            {
                monsterHPMax = monsterStat.monsterHPMax;
                monsterHPNow = monsterStat.monsterHPNow;
                monsterAD = monsterStat.monsterAD;
            }
        }
        public float monsterHPMax;
        public float monsterHPNow;
        public float MonsterHP
        {
            get 
            {
                if (monsterHPNow > monsterHPMax)
                {
                    monsterHPNow = monsterHPMax;
                }
                return monsterHPNow;
            }
            set 
            {
                if (value > monsterHPMax)
                {
                    monsterHPNow = monsterHPMax;
                }
                else
                {
                    if (value < 0)
                    {
                        monsterHPNow = 0;
                    }
                    else
                    {
                        monsterHPNow = value;
                    }
                }

            }
        }
        public float monsterAD;//AttackDamage
        public bool isMonsterDie
        {
            get 
            {
                return monsterHPNow <= 0;
            }
        }

        public void GetDamage(float damage)
        {
            if (!isMonsterDie)
            {
                float tempDMG = damage;
                if (damage >= monsterHPNow)
                {
                    tempDMG = monsterHPNow;
                }
                MonsterHP -= damage;

                Managers.instance.UI.battleUICall.EnemyHPBarUpdate( -tempDMG);
            }

        }
        public void MonsterAttack()
        {

        }

        public SpriteRenderer SetMonsterSprite(ref GameObject TargetOBJ)
        {
            SpriteRenderer tempSpriteRenderer;
            tempSpriteRenderer = TargetOBJ.GetComponent<SpriteRenderer>();

            if (tempSpriteRenderer == null) tempSpriteRenderer = TargetOBJ.AddComponent<SpriteRenderer>();
            return tempSpriteRenderer;
        }
    }
    [System.Serializable]
    public class MonsterPrefab
    {
        public MonsterStats stat;
        public GameObject prefab;
    }
    [System.Serializable]
    public class MonsterMoveSlot
    {
        public Vector3 slotPosition;
        public Transform MonsterTR;
    }
}