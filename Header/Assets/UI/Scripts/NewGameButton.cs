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
        if (LoadingPercent.Count != 0)
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
        //TODO : �ε��� �־�ߵ�
        loadToHideThings.SetAsFirstSibling();
        Managers.instance.Resource.RegistAllResource(labelNames, (ResourceName, resourceCount, TotalCount) =>
        {
            Debug.Log(ResourceName + "�� �ε����Դϴ�" + resourceCount + "/" + TotalCount);
            LoadingPercent.Enqueue(new Vector2(TotalCount, resourceCount));
            if (ResourceName.Contains("LoadingIlust"))
            {
                IlustMinMax.Item2++;
            }

        }
            , (IluDone, DataDone) =>
            {
                //TODO : �ε� ������ ����� �Լ�, Iludone == �Ϸ���Ʈ �ε�,DataDOne == ��� ������ ��� �Ϸ��

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
                        Managers.instance.UI.LoadingUIProps.LoadingIlust.sprite = Managers.instance.Resource.Load<Sprite>("LoadingIlust_" + Random.Range(IlustMinMax.Item1, IlustMinMax.Item2));
                        Debug.Log("�Ϸ���Ʈ �ε���");
                    }
                }
            }
        );
    }

    IEnumerator LoadSceneAsyncCoroutine()
    {
        // �񵿱������� ���� �ε��մϴ�.
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneNumber, LoadSceneMode.Single);

        // �� �ε��� �Ϸ�� ������ ����մϴ�.
        while (!asyncLoad.isDone)
        {
            // �ε� �ۼ�Ʈ�� ��ų� �ε� ���¸� Ȯ���� �� �ֽ��ϴ�.
            LoadingPercent.Enqueue(new Vector2(0.9f, asyncLoad.progress));
            Debug.Log("�񵿱� �� �ε���");
            // �ε� �� UI ������Ʈ ���� ������ �� �ֽ��ϴ�.

            yield return null;
        }

        // �ε��� �Ϸ�Ǹ� ����� �ڵ带 ���⿡ �߰��մϴ�.
    }


}
