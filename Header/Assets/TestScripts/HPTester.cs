using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPTester : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(HPTest);
    }
    public void HPTest()
    {
        
        Managers.instance.PlayerDataManager.SetPlayerHP = (100,100);
        Managers.instance.PlayerDataManager.SetPlayerHP = (Managers.instance.PlayerDataManager.SetPlayerHP.Item1+10, Managers.instance.PlayerDataManager.SetPlayerHP.Item2 + 100);
    }
}
