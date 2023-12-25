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
        //TODO : �ε��� �־�ߵ�
        Managers.instance.Resource.RegistAllResource(labelNames, (ResourceName, resourceCount, TotalCount) =>
        {
            Debug.Log(ResourceName+"�� �ε����Դϴ�" + resourceCount + "/" + TotalCount);
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
                    Managers.instance.UI.LoadingIlust.sprite = null;
                    Debug.Log("�Ϸ���Ʈ �ε���");
                }
            }
        }

        );
    }
}
