using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerTest : MonoBehaviour
{
    [SerializeField] private DataDefines.ResourceDefine[] labelNames;
    // Start is called before the first frame update
    void Start()
    {
        //TODO : �ε��� �־�ߵ�
        Managers.instance.Resource.RegistAllResource(labelNames,(aa,bb,cc)=> 
        {
            Debug.Log(aa + bb + "����" + cc);
        }
        , (done) =>
        {
            //TODO : �ε� ������ ����� �Լ�
            Debug.Log("�ε���");
        }
        
        );
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
