using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewGameButton : MonoBehaviour
{
    [SerializeField] private DataDefines.ResourceDefine[] labelNames;
    [SerializeField] private Button newGameBTN;

    // Start is called before the first frame update
    void Start()
    {
        newGameBTN.onClick.AddListener(OnClickBTN);
    }
    public void OnClickBTN()
    {
        //TODO : 로딩에 넣어야됨
        Managers.instance.Resource.RegistAllResource(labelNames, (ResourceName, resourceCount, TotalCount) =>
        {
            Debug.Log(ResourceName+"를 로딩중입니다" + resourceCount + "/" + TotalCount);
        }
        , (IluDone,DataDone) =>
        {
            //TODO : 로딩 끝날때 실행될 함수, Iludone == 일러스트 로딩,DataDOne == 모든 데이터 등록 완료시

            if (IluDone)
            {
                if (DataDone)
                {

                    Debug.Log("모든 로딩끝");
                }
                else
                {
                    Managers.instance.UI.LoadingIlust.sprite = null;
                    Debug.Log("일러스트 로딩끝");
                }
            }
        }

        );
    }
}
