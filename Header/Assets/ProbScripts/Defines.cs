using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;

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
