using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using DG.Tweening;

public class NewGameButton : MonoBehaviour
{
    public int sceneNumber = 0;
    public Transform loadToHideThings;
    [SerializeField] private Image splashArt;
    [SerializeField] private DataDefines.ResourceDefine[] labelNames;

    [SerializeField] private UnityEngine.UI.Button newGameBTN;
    [SerializeField] private UnityEngine.UI.Button optionBTN;
    [SerializeField] private UnityEngine.UI.Button exitGameBTN;

    Queue<Vector2> LoadingPercent = new Queue<Vector2>();
    [SerializeField] bool isChallengeModeBTN = false;
    (byte, byte) IlustMinMax;
    // Start is called before the first frame update
    private void Start()
    {
        if (!isChallengeModeBTN)
        {

            if (!Managers.instance.Resource.isResourceLoadDone)
            {
                Managers.instance.Resource.LoadAllAsync<AudioClip>("PreLoadAudios", (TempString, Num) =>
                {
                    Managers.instance.SoundManager.BGM.enabled = true;
                    Managers.instance.SoundManager.SFX.enabled = true;

                });
                Managers.instance.Resource.LoadAllAsync<Sprite>("PreLoadUI", (TempString, Num) =>
                {
                    Debug.Log("프리로드 UI 로드 완료");

                });
                Managers.instance.Resource.LoadAllAsync<Font>("Font", (TempString, Num) =>
                {
                    Debug.Log("프리로드 UI 로드 완료");

                });
                splashArt.DOFade(0, 3f).OnComplete(() =>
                {
                    Managers.instance.UI.RegistEventTrigger(newGameBTN.transform as RectTransform);
                    newGameBTN.onClick.AddListener(OnClickBTN);
                    if (optionBTN != null)
                    {
                        Managers.instance.UI.RegistEventTrigger(optionBTN.transform as RectTransform);
                        optionBTN.onClick.AddListener(() =>
                        {
                            Managers.instance.UI.CloseUIStack();
                        });
                    }
                    else
                    {
                        Debug.LogError("옵션 버튼 지정안됨");
                    }
                    if (exitGameBTN != null)
                    {
                        Managers.instance.UI.RegistEventTrigger(exitGameBTN.transform as RectTransform);
                        exitGameBTN.onClick.AddListener(Managers.instance.GameExitBTN);
                    }
                    else
                    {
                        Debug.LogError("게임종료 버튼 지정안됨");
                    }
                    splashArt.gameObject.SetActive(false);
                });
            }
            else
            {
                newGameBTN.onClick.RemoveAllListeners();
                newGameBTN.onClick.AddListener(OnClickBTN);

                optionBTN.onClick.RemoveAllListeners();
                optionBTN.onClick.AddListener(() =>
                {
                    Managers.instance.UI.CloseUIStack();
                });

                exitGameBTN.onClick.RemoveAllListeners();
                exitGameBTN.onClick.AddListener(Managers.instance.GameExitBTN);


                splashArt.gameObject.SetActive(false);
            }
        }
        else
        {
            Managers.instance.UI.RegistEventTrigger(newGameBTN.transform as RectTransform);
            newGameBTN.onClick.AddListener(OnClickBTN);
        }

    }
    private void Update()
    {
        if (LoadingPercent.Count > 0)
        {

            Vector2 tempValue = LoadingPercent.Dequeue();
            Managers.instance.UI.LoadingUIProps.LoadingSlider.maxValue = tempValue.x;
            Managers.instance.UI.LoadingUIProps.LoadingSlider.value = tempValue.y;

            /*            Managers.instance.UI.LoadingUIProps.LoadingSlider.enabled = false;
                        Managers.instance.UI.LoadingUIProps.LoadingSlider.enabled = true;*/
        }
    }
    public void OnClickBTN()
    {
        newGameBTN.transform.SetParent(null);
        Managers.instance.ResetManagingArrays();
        loadToHideThings.SetAsFirstSibling();
        if (Managers.instance.Resource.isResourceLoadDone)
        {
            Managers.instance.ResetManagingArrays();
            Managers.instance.PlayerDataManager.ResetPlayer();
            StartCoroutine(LoadSceneAsyncCoroutine());
        }
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
                Debug.Log(LoadingPercent.Count);
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
                        if (!isChallengeModeBTN)
                        {
                            for (int i = 0; i < 10; i++)
                            {
                                Managers.instance.PlayerDataManager.AddBall(Managers.instance.Resource._weaponDictionary["stone_bulb"]);
                                Managers.instance.PlayerDataManager.AddBall(Managers.instance.Resource._weaponDictionary["nine_bulb"]);
                            }
                        }


                        StartCoroutine(LoadSceneAsyncCoroutine());
                    }
                    else
                    {
                        //FLOW : 로딩 일러스트 파일면은 LoadingIlust_숫자로 명명 , IlustMinMax를 파일 갯수에 맞춰서 수정해줘야함
                        Managers.instance.UI.LoadingUIProps.LoadingIlust.sprite = Managers.instance.Resource.Load<Sprite>("LoadingIlust_" + UnityEngine.Random.Range(IlustMinMax.Item1, IlustMinMax.Item2));
                        Debug.Log("일러스트 로딩끝");

                    }
                }
            }
        );
    }

    IEnumerator LoadSceneAsyncCoroutine()
    {
        Managers.instance.PlayerDataManager.isChallengeMode = isChallengeModeBTN;
        // 비동기적으로 씬을 로드합니다.
        while (LoadingPercent.Count > 0)
        {
            yield return null;
        }
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneNumber, LoadSceneMode.Single);


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
