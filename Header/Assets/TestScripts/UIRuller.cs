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
            "¾ÞÄ¿ min : " +rt.anchorMin +"\n"+
            "¾ÞÄ¿ max : " + rt.anchorMax + "\n"+
            "µ¨Å¸ »çÀÌÁî : " + rt.sizeDelta + "\n"+
            "¾ÞÄ¿ position : " + rt.anchoredPosition);
    }

}
