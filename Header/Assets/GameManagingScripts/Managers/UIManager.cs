using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class UIManager
{
    private LoadingUI loadingUIProps;
    public LoadingUI LoadingUIProps
    {
        get 
        { 
            if (loadingUIProps == null)
            {
                loadingUIProps = new LoadingUI();
            }
            return loadingUIProps;
        }
    }
    private TopViewSceneUI topViewSceneUIs;
    public TopViewSceneUI TopViewSceneUIs
    {
        get 
        { 
            if(topViewSceneUIs == null)topViewSceneUIs = new TopViewSceneUI();
            return topViewSceneUIs;
        }
    }

}

public class LoadingUI
{
    private RectTransform sceneMainCanvas;
    public RectTransform SceneMainCanvas
    {
        get
        {
            if (sceneMainCanvas == null)
            {
                GameObject targetTempOBJ = GameObject.Find("Canvas");
                RectTransform tempTR = targetTempOBJ == null ? new GameObject("Canvas").AddComponent<Canvas>().transform as RectTransform : targetTempOBJ.transform as RectTransform;
                sceneMainCanvas = tempTR;
            }
            return sceneMainCanvas;
        }
    }
    private Image loadingIlust = null;
    public Image LoadingIlust
    {
        get
        {
            if (loadingIlust == null)
            {
                GameObject alreadyInOBJ = GameObject.Find("LoadingIlust");
                if (alreadyInOBJ != null)
                {
                    loadingIlust = alreadyInOBJ.GetComponent<Image>();
                }
                else
                {
                    loadingIlust = new GameObject("LoadingIlust").AddComponent<Image>();
                    RectTransform tempIluTR = loadingIlust.transform as RectTransform;
                    
                    tempIluTR.SetParent(SceneMainCanvas);
                    tempIluTR.anchorMin = Vector2.zero;
                    tempIluTR.anchorMax = Vector2.one;
                    tempIluTR.sizeDelta = Vector2.zero;
                    tempIluTR.anchoredPosition = Vector2.zero;
                    tempIluTR.SetAsFirstSibling();
                }

            }
            return loadingIlust;
        }
    }
    private Slider loadingSlider = null;
    public Slider LoadingSlider 
    {
        get
        {
            if (loadingSlider == null)
            {
                GameObject alreadyInOBJ = GameObject.Find("LoadingSlider");
                if (alreadyInOBJ != null)
                {
                    loadingSlider = alreadyInOBJ.GetComponent<Slider>();
                }
                else
                {
                    //FLOW : �ε��� �̹��� ����,���� ������ �ʿ��ϸ� ���⼭ ����
                    Slider tempLoadingSlider = new GameObject("LoadingSlider").AddComponent<Slider>();
                    //Vector2 CanvasSize = new Vector2(SceneMainCanvas.rect.width, SceneMainCanvas.rect.height);
                    tempLoadingSlider.wholeNumbers = false;
                    tempLoadingSlider.maxValue = 100;
                    RectTransform tempSliderTR = tempLoadingSlider.transform as RectTransform;

                    tempLoadingSlider.AddComponent<Image>().color = Color.grey;
                    Image tempIMG = new GameObject("Handle").AddComponent<Image>();
                    tempIMG.color = Color.green;
                    tempLoadingSlider.fillRect = tempIMG.rectTransform;

                    // Slider�� �θ�-�ڽ� ���� ����  
                    tempLoadingSlider.fillRect.SetParent(tempSliderTR);
                    tempLoadingSlider.fillRect.offsetMax = new Vector2(tempLoadingSlider.fillRect.offsetMax.x, 0);
                    tempLoadingSlider.fillRect.offsetMin = new Vector2(tempLoadingSlider.fillRect.offsetMin.x, 0);
                    tempLoadingSlider.fillRect.anchorMin = new Vector2(0, 0); // �θ��� ���� �ϴ��� ��������
                    tempLoadingSlider.fillRect.anchorMax = new Vector2(1, 1); // �θ��� ���� ����� ��������
                    tempLoadingSlider.fillRect.pivot = new Vector2(0, 0.5f); // �θ��� ���� �߰��� ��������
                    tempLoadingSlider.fillRect.sizeDelta = Vector2.zero;
                    tempLoadingSlider.fillRect.anchoredPosition = Vector2.zero;

                    tempSliderTR.SetParent(SceneMainCanvas);
                    tempSliderTR.SetAsLastSibling();
                    tempSliderTR.anchorMin = new Vector2(0.33f, 0.20f);
                    tempSliderTR.anchorMax = new Vector2(0.66f, 0.25f);
                    tempSliderTR.sizeDelta = Vector2.zero;
                    tempSliderTR.anchoredPosition = Vector2.zero;
                    tempLoadingSlider.interactable = false;
                    tempLoadingSlider.enabled = false;
                    tempLoadingSlider.enabled = true;
                    loadingSlider = tempLoadingSlider;
                    Debug.Log("�ε��� ���� ��");
                }
            }
            return loadingSlider;
        }
    }
}
public class TopViewSceneUI
{
    private RectTransform interactionKeyPanel;
    private RectTransform InteractionKeyPanel 
    {  
        get 
        {
            if (interactionKeyPanel == null)
            {
                Image UIBackGround = new GameObject { name = "interactionKeyPanel" }.AddComponent<Image>();
                Canvas tempCanvas= Managers.instance.UI.LoadingUIProps.SceneMainCanvas.GetComponent<Canvas>();
                interactionKeyPanel = UIBackGround.rectTransform;
                interactionKeyPanel.SetParent(tempCanvas.transform as RectTransform);

                //UIBackGround.sprite = ������ ���� �̸�;
                //TODO : Ű ���ͷ��� �ȳ��ǳ� ������ UIBackGround ������ sprite �������־����
                interactionKeyPanel.anchorMin = new Vector2(0.30f, 0.40f);
                interactionKeyPanel.anchorMax = new Vector2(0.70f, 0.80f);
                interactionKeyPanel.sizeDelta = Vector2.zero;
                interactionKeyPanel.anchoredPosition = Vector2.zero;
            }
            return interactionKeyPanel;
        } 
    }
    private Text interactionKeyTextTitle;
    private Text InteractionKeyTextTitle 
    {  
        get 
        {
            if (interactionKeyTextTitle == null)
            {
                interactionKeyTextTitle = new GameObject { name = "interactionKeyTextTitle" }.AddComponent<Text>();
                //TODO : ��Ʈ����,������ �����ʿ� �߾����� �ؾ���
                interactionKeyTextTitle.rectTransform.SetParent(InteractionKeyPanel);
                interactionKeyTextTitle.rectTransform.anchorMin = new Vector2(0, 0);
                interactionKeyTextTitle.rectTransform.anchorMax = new Vector2(1, 1);
                interactionKeyTextTitle.rectTransform.sizeDelta = Vector2.zero;
            }
            return interactionKeyTextTitle;
        } 
    }
    public void KeyInteractionOnOFF(bool OnOFF)
    {
        if (OnOFF)
        {
            InteractionKeyTextTitle.text = "�����̽��� Ű�� ���� ���ͷ��� �ϼ���";
            InteractionKeyTextTitle.fontSize = 25;


        }
        else
        {
            
        }
        InteractionKeyPanel.gameObject.SetActive(OnOFF);
    }

}