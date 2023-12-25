using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager
{
    private RectTransform sceneMainCanvas;
    private RectTransform SceneMainCanvas 
    { 
        get 
        {
            if (sceneMainCanvas == null)
            {
                RectTransform tempTR = GameObject.Find("Canvas").transform as RectTransform;

                sceneMainCanvas = tempTR == null ? new GameObject("Canvas").AddComponent<Canvas>().transform as RectTransform: tempTR;
            }
            return sceneMainCanvas; 
        } 
    }
    private Image loadingIlust =null;
    public Image LoadingIlust 
    { 
        get 
        {
            if (loadingIlust == null)
            {
                loadingIlust = new GameObject("LoadingIlust").AddComponent<Image>();
                RectTransform tempIluTR = loadingIlust.transform as RectTransform;
                tempIluTR.SetParent(SceneMainCanvas);
                tempIluTR.anchorMin = Vector2.zero;
                tempIluTR.anchorMax = Vector2.one;
                tempIluTR.sizeDelta = Vector2.zero;
                tempIluTR.anchoredPosition = Vector2.zero;
            }
            return loadingIlust; 
        } 
    }
}
