using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewGameButton : MonoBehaviour
{
    [SerializeField] private DataDefines.ResourceDefine[] labelNames;
    [SerializeField] private Button newGameBTN;
    (byte, byte) IlustMinMax;
    // Start is called before the first frame update
    void Start()
    {
        newGameBTN.onClick.AddListener(OnClickBTN);
    }
    public void OnClickBTN()
    {
        //TODO : �ε��� �־�ߵ�
        Managers.instance.Resource.RegistAllResource(labelNames, (ResourceName, resourceCount, TotalCount) =>
        {
            Debug.Log(ResourceName+"�� �ε����Դϴ�" + resourceCount + "/" + TotalCount);
            if (ResourceName.Contains("LoadingIlust"))
            {
                IlustMinMax.Item2++;
            }
        }
        , (IluDone,DataDone) =>
        {
            //TODO : �ε� ������ ����� �Լ�, Iludone == �Ϸ���Ʈ �ε�,DataDOne == ��� ������ ��� �Ϸ��

            if (IluDone)
            {
                if (DataDone)
                {

                    Debug.Log("��� �ε���");
                }
                else
                {
                    //FLOW : �ε� �Ϸ���Ʈ ���ϸ��� LoadingIlust_���ڷ� ��� , IlustMinMax�� ���� ������ ���缭 �����������
                    Managers.instance.UI.loadingUIProps.LoadingIlust.sprite = Managers.instance.Resource.Load<Sprite>("LoadingIlust_" + Random.Range(IlustMinMax.Item1, IlustMinMax.Item2));
                    Debug.Log("�Ϸ���Ʈ �ε���");
                }
            }
        }

        );
    }
}
