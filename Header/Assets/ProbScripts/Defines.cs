using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

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
        GameObject, Sprites,DataSheets,Fonts
    }
}
namespace InteractionDefines
{
    public class InteractionDetailPosition
    {
        public Vector3 size = new Vector3(1, 1, 1);
        //128,128,128 == �׸����� ���߾�
        public byte x = 128;
        public byte y = 128;
        public byte z = 128;
    }
    [System.Serializable]
    public class InteractionInstallerProps
    {
        public Vector2Int interactionPosition;
        public InteractionDetailPosition detail;
        public InteractionTypes interactionTypes;
    }

    public enum InteractionTypes
    {
        //MAINTANCE : ���ͷ��� ���� �߰� �� ���⿡ ���� �ʿ�
        None, DialogInteraction,ToBattleScene
    }
}
