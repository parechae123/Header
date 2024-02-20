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
        
        Managers.instance.PlayerDataManager.PlayerHP = (100,100);
        Managers.instance.PlayerDataManager.PlayerHP = (Managers.instance.PlayerDataManager.PlayerHP.Item1+10, Managers.instance.PlayerDataManager.PlayerHP.Item2 + 100);
    }
}
