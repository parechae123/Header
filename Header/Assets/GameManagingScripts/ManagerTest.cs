using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerTest : MonoBehaviour
{
    [SerializeField] private DataDefines.ResourceDefine[] labelNames;
    // Start is called before the first frame update
    void Start()
    {
        //TODO : 로딩에 넣어야됨
        Managers.instance.Resource.RegistAllResource(labelNames,(aa,bb,cc)=> 
        {
            Debug.Log(aa + bb + "ㅇㅇ" + cc);
        }
        , (done) =>
        {
            //TODO : 로딩 끝날때 실행될 함수
            Debug.Log("로딩끝");
        }
        
        );
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
