using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NewGameButton : MonoBehaviour
{
    public int sceneNumber = 0;
    public Transform loadToHideThings;
    [SerializeField] private DataDefines.ResourceDefine[] labelNames;
    [SerializeField] private UnityEngine.UI.Button newGameBTN;
    Queue<Vector2> LoadingPercent = new Queue<Vector2>();
    (byte, byte) IlustMinMax;
    // Start is called before the first frame update
    void Start()
    {
        newGameBTN.onClick.AddListener(OnClickBTN);
    }
    private void Update()
    {
        if (LoadingPercent.Count > 0)
        {
            Debug.Log("슬라이더 업데이트중");
            Vector2 tempValue = LoadingPercent.Dequeue();
            Managers.instance.UI.LoadingUIProps.LoadingSlider.maxValue = tempValue.x;
            Managers.instance.UI.LoadingUIProps.LoadingSlider.value = tempValue.y;

/*            Managers.instance.UI.LoadingUIProps.LoadingSlider.enabled = false;
            Managers.instance.UI.LoadingUIProps.LoadingSlider.enabled = true;*/
        }
    }
    public void OnClickBTN()
    {
        Managers.instance.UI.ResetUI();
        Managers.instance.Pool.Clear();
        Managers.instance.Grid.ResetGrids();
        loadToHideThings.SetAsFirstSibling();
        Managers.instance.Resource.RegistAllResource(labelNames, (ResourceName, resourceCount, TotalCount) =>
        {
            //Debug.Log(ResourceName + "를 로딩중입니다" + resourceCount + "/" + TotalCount);
            Debug.Log("토탈" + TotalCount);
            Debug.Log("현재" + resourceCount);
            Debug.Log(ResourceName);
            if (ResourceName.Contains("LoadingIlust"))
            {
                IlustMinMax.Item2++;
            }
            else
            {
                LoadingPercent.Enqueue(new Vector2(TotalCount, resourceCount));
            }

        }
            , (IluDone, DataDone) =>
            {
                //MAINTANCE : 로딩 끝날때 실행될 함수, Iludone == 일러스트 로딩,DataDOne == 모든 데이터 등록 완료시

                if (IluDone)
                {
                    if (DataDone)
                    {
                        Debug.Log("모든 로딩끝");
                        StartCoroutine(LoadSceneAsyncCoroutine());
                    }
                    else
                    {
                        //FLOW : 로딩 일러스트 파일면은 LoadingIlust_숫자로 명명 , IlustMinMax를 파일 갯수에 맞춰서 수정해줘야함
                        Managers.instance.UI.LoadingUIProps.LoadingIlust.sprite = Managers.instance.Resource.Load<Sprite>("LoadingIlust_" + Random.Range(IlustMinMax.Item1, IlustMinMax.Item2));
                        Debug.Log("일러스트 로딩끝");
                    }
                }
            }
        );
    }

    IEnumerator LoadSceneAsyncCoroutine()
    {
        // 비동기적으로 씬을 로드합니다.
        while (LoadingPercent.Count>0) 
        { 
            yield return null;
        }
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneNumber, LoadSceneMode.Single);
        //HEOYOON : 임시 총알 테스팅코드
        Managers.instance.PlayerDataManager.AddBall
            (new HeaderPadDefines.BallStat { ballBouncienss = 0.5f, ballFriction = 0.7f, ballName = "Bullet_Basic", weight = 1, ballKoreanName = "기본공" ,ballStartForce = 25});
         Managers.instance.PlayerDataManager.AddBall
                    (new HeaderPadDefines.BallStat { ballBouncienss = 0.5f, ballFriction = 0.7f, ballName = "Bullet_Basic", weight = 1, ballKoreanName = "기본공 테스트1" ,ballStartForce = 25});
         Managers.instance.PlayerDataManager.AddBall
                    (new HeaderPadDefines.BallStat { ballBouncienss = 0.5f, ballFriction = 0.7f, ballName = "Bullet_Basic123", weight = 1, ballKoreanName = "기본공 테스트2" ,ballStartForce = 25});
        for (int i = 3; i < 17; i++)
        {
            Managers.instance.PlayerDataManager.AddBall
           (new HeaderPadDefines.BallStat { ballBouncienss = 0.5f, ballFriction = 0.7f, ballName = "Bullet_Basic"+i, weight = 1, ballKoreanName = "기본공 테스트" + i, ballStartForce = 25 });
        }
        // 씬 로딩이 완료될 때까지 대기합니다.
        while (!asyncLoad.isDone)
        {
            // 로딩 퍼센트를 얻거나 로딩 상태를 확인할 수 있습니다.
            Debug.Log("비동기 신 로딩중");
            // 로딩 중 UI 업데이트 등을 수행할 수 있습니다.

            yield return null;
        }

        // 로딩이 완료되면 실행될 코드를 여기에 추가합니다.
    }
}
