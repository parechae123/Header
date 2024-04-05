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
    [SerializeField]private Image splashArt;
    [SerializeField] private DataDefines.ResourceDefine[] labelNames;

    [SerializeField] private UnityEngine.UI.Button newGameBTN;
    Queue<Vector2> LoadingPercent = new Queue<Vector2>();
    (byte, byte) IlustMinMax;
    // Start is called before the first frame update
    private void Start()
    {

        Managers.instance.Resource.LoadAllAsync<AudioClip>("TItleAudios", (TempString, Num) =>
        {
            Managers.instance.SoundManager.BGM.enabled = true;
            Managers.instance.SoundManager.SFX.enabled = true;

        });
        Managers.instance.Resource.LoadAllAsync<Sprite>("PreLoadUI", (TempString, Num) =>
        {
            Debug.Log("�����ε� UI �ε� �Ϸ�");

        });
        Managers.instance.Resource.LoadAllAsync<Font>("Font", (TempString, Num) =>
        {
            Debug.Log("�����ε� UI �ε� �Ϸ�");

        });
        splashArt.DOFade(0, 3f).OnComplete(() =>
        {
            newGameBTN.onClick.AddListener(OnClickBTN);
            splashArt.gameObject.SetActive(false);
        });
        
    }
    private void Update()
    {
        if (LoadingPercent.Count > 0)
        {
            Debug.Log("�����̴� ������Ʈ��");
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
            //Debug.Log(ResourceName + "�� �ε����Դϴ�" + resourceCount + "/" + TotalCount);
            Debug.Log("��Ż" + TotalCount);
            Debug.Log("����" + resourceCount);
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
                //MAINTANCE : �ε� ������ ����� �Լ�, Iludone == �Ϸ���Ʈ �ε�,DataDOne == ��� ������ ��� �Ϸ��

                if (IluDone)
                {
                    if (DataDone)
                    {
                        Debug.Log("��� �ε���");
                        StartCoroutine(LoadSceneAsyncCoroutine());
                    }
                    else
                    {
                        //FLOW : �ε� �Ϸ���Ʈ ���ϸ��� LoadingIlust_���ڷ� ��� , IlustMinMax�� ���� ������ ���缭 �����������
                        Managers.instance.UI.LoadingUIProps.LoadingIlust.sprite = Managers.instance.Resource.Load<Sprite>("LoadingIlust_" + UnityEngine.Random.Range(IlustMinMax.Item1, IlustMinMax.Item2));
                        Debug.Log("�Ϸ���Ʈ �ε���");
                    }
                }
            }
        );
    }

    IEnumerator LoadSceneAsyncCoroutine()
    {
        // �񵿱������� ���� �ε��մϴ�.
        while (LoadingPercent.Count>0) 
        { 
            yield return null;
        }
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneNumber, LoadSceneMode.Single);
        

        // �� �ε��� �Ϸ�� ������ ����մϴ�.
        while (!asyncLoad.isDone)
        {
            // �ε� �ۼ�Ʈ�� ��ų� �ε� ���¸� Ȯ���� �� �ֽ��ϴ�.
            Debug.Log("�񵿱� �� �ε���");
            // �ε� �� UI ������Ʈ ���� ������ �� �ֽ��ϴ�.

            yield return null;
        }
        // �ε��� �Ϸ�Ǹ� ����� �ڵ带 ���⿡ �߰��մϴ�.
    }
}
