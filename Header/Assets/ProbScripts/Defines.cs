using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;
using UnityEngine.UI;

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
        // MAINTANCE : ���ҽ� Ÿ�� �߰��Ҷ� ���⼭ Enum �߰��۾� �ʿ�
        GameObject, Sprites,DataSheets,Fonts,RenderTexture,Video,Texture2D
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
    }

    public enum InteractionTypes
    {
        //MAINTANCE : ���ͷ��� ���� �߰� �� ���⿡ ���� �ʿ�
        None, DialogInteraction,ToBattleScene
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
        public void OnColideBlock()
        {
            switch (blockCondition)
            {
                case BlockStatus.Destroyed:
                    break;
                case BlockStatus.Emptied:
                    blockCondition = BlockStatus.Destroyed;
                    targetIMG.transform.GetComponent<PolygonCollider2D>().enabled = false;
                    targetIMG.sprite = Managers.instance.Resource.Load<Sprite>("HeaderBlock_Destroyed");
                    //TODO : �� �浹�� ���ŵǴ� ���� ���⿡ ������ɵ�,EX : ������
                    break;
                case BlockStatus.FIlled:
                    blockCondition = BlockStatus.Emptied;
                    targetIMG.sprite = Managers.instance.Resource.Load<Sprite>("HeaderBlock_Emptied");
                    break;
                case BlockStatus.FilledCoin:
                    blockCondition = BlockStatus.Emptied;
                    targetIMG.sprite = Managers.instance.Resource.Load<Sprite>("HeaderBlock_Emptied");
                    break;
                case BlockStatus.BoombBlock:
                    if (BlockHP>= 0)
                    {
                        //targetIMG.sprite = Managers.instance.Resource.Load<Sprite>("HeaderBlock_Emptied"+BlockHP);
                        BlockHP--;
                    }
                    else
                    {
                        blockCondition = BlockStatus.Emptied;
                        targetIMG.transform.GetComponent<PolygonCollider2D>().enabled = false;
                        targetIMG.sprite = Managers.instance.Resource.Load<Sprite>("HeaderBlock_Destroyed");
                    }
                    break;
            }
            if (BE != null)
            {
                BE();
            }
        }
        public void OnResetBlocks()
        {
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
                }
                blockCondition = SettedBlockCondition;
                targetIMG.transform.GetComponent<PolygonCollider2D>().enabled = true;
            }
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
        public float weight; //���� mass�� �־������
        public int amount = 1;
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
        Destroyed,Emptied,FIlled,FilledCoin,BoombBlock,MovePlatform
    }
}
