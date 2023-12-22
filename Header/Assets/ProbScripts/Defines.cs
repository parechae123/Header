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
        // TODO : 리소스 타입 추가할때 여기서 Enum 추가작업 필요
        GameObject,Sprites,DataSheets
    }
}