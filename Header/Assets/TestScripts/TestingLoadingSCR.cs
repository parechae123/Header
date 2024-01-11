using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingLoadingSCR : MonoBehaviour
{
    [SerializeField] private DataDefines.ResourceDefine[] labelNames;
    // Start is called before the first frame update
    void Awake()
    {
        Managers.instance.UI.ResetUI();
        Managers.instance.Pool.Clear();
        Managers.instance.Grid.ResetGrids();
        Managers.instance.Grid.BattleGridData.Clear();
        Managers.instance.Resource.RegistAllResource(labelNames, (ResourceName, resourceCount, TotalCount) =>
        {
            Debug.Log(ResourceName + "�� �ε����Դϴ�" + resourceCount + "/" + TotalCount);

        }
            , (IluDone, DataDone) =>
            {
                //MAINTANCE : �ε� ������ ����� �Լ�, Iludone == �Ϸ���Ʈ �ε�,DataDOne == ��� ������ ��� �Ϸ��

                if (IluDone)
                {
                    if (DataDone)
                    {
                        Debug.Log("��� �ε���");
                    }
                    else
                    {
                        //FLOW : �ε� �Ϸ���Ʈ ���ϸ��� LoadingIlust_���ڷ� ��� , IlustMinMax�� ���� ������ ���缭 �����������
                        Debug.Log("�Ϸ���Ʈ �ε���");
                    }
                }
            }
        );
    }
}
