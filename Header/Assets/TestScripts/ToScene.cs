using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ToScene : MonoBehaviour
{
    [SerializeField] int TargetScene;
    [SerializeField] Button SceneChangeButton;
    
    // Start is called before the first frame update
    void Start()
    {
        SceneChangeButton.onClick.AddListener(OnBTNClick);
    }
    public void OnBTNClick()
    {
        Managers.instance.ResetManagingArrays();
        SceneManager.LoadScene(TargetScene);
    }
}
