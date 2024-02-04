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
        while (LoadingPercent.Count>0) 
        { 
            yield return null;
        }
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneNumber, LoadSceneMode.Single);
        //HEOYOON : �ӽ� �Ѿ� �׽����ڵ�
        Managers.instance.PlayerDataManager.AddBall
            (new HeaderPadDefines.BallStat { ballBouncienss = 0.5f, ballFriction = 0.7f, ballName = "Bullet_Basic", weight = 1, ballKoreanName = "�⺻��" ,ballStartForce = 25});
         Managers.instance.PlayerDataManager.AddBall
                    (new HeaderPadDefines.BallStat { ballBouncienss = 0.5f, ballFriction = 0.7f, ballName = "Bullet_Basic", weight = 1, ballKoreanName = "�⺻�� �׽�Ʈ1" ,ballStartForce = 25});
         Managers.instance.PlayerDataManager.AddBall
                    (new HeaderPadDefines.BallStat { ballBouncienss = 0.5f, ballFriction = 0.7f, ballName = "Bullet_Basic123", weight = 1, ballKoreanName = "�⺻�� �׽�Ʈ2" ,ballStartForce = 25});
        for (int i = 3; i < 17; i++)
        {
            Managers.instance.PlayerDataManager.AddBall
           (new HeaderPadDefines.BallStat { ballBouncienss = 0.5f, ballFriction = 0.7f, ballName = "Bullet_Basic"+i, weight = 1, ballKoreanName = "�⺻�� �׽�Ʈ" + i, ballStartForce = 25 });
        }
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
