using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using HeaderPadDefines;
using System;

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
        DontDestroyOnLoad(newGameBTN.gameObject);
        Managers.instance.UI.ResetUI();
        Managers.instance.Pool.Clear();
        Managers.instance.Grid.ResetGrids();
        loadToHideThings.SetAsFirstSibling();
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
        //HEOYOON : �ӽ� �Ѿ� �׽����ڵ�
        

        // �� �ε��� �Ϸ�� ������ ����մϴ�.
        while (!asyncLoad.isDone)
        {
            // �ε� �ۼ�Ʈ�� ��ų� �ε� ���¸� Ȯ���� �� �ֽ��ϴ�.
            Debug.Log("�񵿱� �� �ε���");
            // �ε� �� UI ������Ʈ ���� ������ �� �ֽ��ϴ�.

            yield return null;
        }
        AfterWeaponLoadDone(() =>
        {
            Destroy(this.gameObject);
            Debug.Log("�ӵ�");
        });
        // �ε��� �Ϸ�Ǹ� ����� �ڵ带 ���⿡ �߰��մϴ�.
    }
    private void AfterWeaponLoadDone(Action done)
    {
        JObject tempJson = JObject.Parse(Managers.instance.Resource.Load<TextAsset>("Weapon_Table").text);
        JToken tempJToken = tempJson["Weapon_Table"];
        ExtraBallStat[] tempBallTable = tempJToken.ToObject<ExtraBallStat[]>();
        for (int i = 0; i < tempBallTable.Length; i++)
        {
            Managers.instance.Resource._weaponDictionary.Add(tempBallTable[i].ballName, tempBallTable[i]);
        }
        done.Invoke();
    }
}
