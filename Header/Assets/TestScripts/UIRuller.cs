using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIRuller : MonoBehaviour
{
    RectTransform rt;
    // Start is called before the first frame update
    void Start()
    {
        rt = GetComponent<RectTransform>();
        Debug.LogError(gameObject.name+ "\n"+
            "��Ŀ min : " +rt.anchorMin +"\n"+
            "��Ŀ max : " + rt.anchorMax + "\n"+
            "��Ÿ ������ : " + rt.sizeDelta + "\n"+
            "��Ŀ position : " + rt.anchoredPosition);
    }

}
