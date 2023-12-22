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
        // TODO : ���ҽ� Ÿ�� �߰��Ҷ� ���⼭ Enum �߰��۾� �ʿ�
        GameObject,Sprites,DataSheets
    }
}