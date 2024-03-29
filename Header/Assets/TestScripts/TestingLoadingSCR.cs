using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingLoadingSCR : MonoBehaviour
{
    [SerializeField] private DataDefines.ResourceDefine[] labelNames;
    // Start is called before the first frame update
    void Awake()
    {
        Managers.instance.ResetManagingArrays();
        Managers.instance.Resource.RegistAllResource(labelNames, (ResourceName, resourceCount, TotalCount) =>
        {
            Debug.Log(ResourceName + "를 로딩중입니다" + resourceCount + "/" + TotalCount);

        }
            , (IluDone, DataDone) =>
            {
                //MAINTANCE : 로딩 끝날때 실행될 함수, Iludone == 일러스트 로딩,DataDOne == 모든 데이터 등록 완료시

                if (IluDone)
                {
                    if (DataDone)
                    {
                        Debug.Log("모든 로딩끝");
                    }
                    else
                    {
                        //FLOW : 로딩 일러스트 파일면은 LoadingIlust_숫자로 명명 , IlustMinMax를 파일 갯수에 맞춰서 수정해줘야함
                        Debug.Log("일러스트 로딩끝");
                    }
                }
            }
        );
    }
}
