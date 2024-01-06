using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
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
    private DialogSystem dialogCall;
    public DialogSystem DialogCall 
    { 
        get 
        {
            if (dialogCall == null)dialogCall = new DialogSystem();
            return dialogCall;
        } 
    }

    private Stack<Transform> UIStack = new Stack<Transform>();
    public List<Transform> MoveAbleCheckerList = new List<Transform>();
    public void RegistUIStack(Transform target)
    {
        UIStack.Push(target);
    }
    public void CloseUIStack()
    {
        if (UIStack.Count > 0)
        {
            UIStack.Pop().gameObject.SetActive(false);
            TopViewPlayer.Instance.isMoveAble = MoveAbleChecker();
        }
    }
    public void CheckerRegist(Transform tr)
    {
        MoveAbleCheckerList.Add(tr);
    }
    public void ResetUI()
    {
        UIStack.Clear();
        MoveAbleCheckerList.Clear();
    }

    private bool MoveAbleChecker()
    {
        for (int i = 0; i < MoveAbleCheckerList.Count; i++)
        {
            if (MoveAbleCheckerList[i].gameObject.activeSelf)
            {
                return false;
            }
        }
        return true;
    }
    public void TargetUIOnOff(Transform target,bool isTurnOn)
    {
        // TODO : Ư�� UI �ݱ��ư ������ �������־�� �� �Լ� ���� isTurnOn�� false ������ true
        target.gameObject.SetActive(isTurnOn );
        if (isTurnOn)
        {
            RegistUIStack(target);
        }
        else
        {
            UIStack.TryPop(out target);
        }
        TopViewPlayer.Instance.isMoveAble = MoveAbleChecker();
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
                    tempLoadingSlider.fillRect.anchorMin = Vector2.zero; // �θ��� ���� �ϴ��� ��������
                    tempLoadingSlider.fillRect.anchorMax = Vector2.one; // �θ��� ���� ����� ��������
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
                interactionKeyPanel.anchorMin = new Vector2(0.4f, 0.25f);
                interactionKeyPanel.anchorMax = new Vector2(0.6f, 0.35f);
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

                interactionKeyTextTitle.rectTransform.SetParent(InteractionKeyPanel);
                interactionKeyTextTitle.rectTransform.anchorMin = Vector2.zero;
                interactionKeyTextTitle.rectTransform.anchorMax = Vector2.one;
                interactionKeyTextTitle.rectTransform.sizeDelta = Vector2.zero;
                interactionKeyTextTitle.rectTransform.anchoredPosition = Vector2.zero;
                interactionKeyTextTitle.fontSize = 25;
                interactionKeyTextTitle.color = Color.black;
                interactionKeyTextTitle.font = Managers.instance.Resource.Load<Font>("InGameFont");
                interactionKeyTextTitle.alignment = TextAnchor.MiddleCenter;
            }
            return interactionKeyTextTitle;
        } 
    }
    public void KeyInteractionOnOFF(bool OnOFF)
    {
        if (OnOFF)
        {
            InteractionKeyPanel.anchoredPosition = Camera.main.WorldToViewportPoint(TopViewPlayer.Instance.transform.position);
            InteractionKeyTextTitle.text = "�����̽��� Ű�� ���� ���ͷ��� �ϼ���";
            InteractionKeyTextTitle.fontSize = 25;


        }
        InteractionKeyPanel.gameObject.SetActive(OnOFF);
    }

}

public class DialogSystem
{
    private RectTransform fullDialogPanel;
    public RectTransform FullDialogPanel
    {
        get
        {
            if (fullDialogPanel == null)
            {
                Image UIBackGround = new GameObject { name = "dialogPanel" }.AddComponent<Image>();
                Canvas tempCanvas = Managers.instance.UI.LoadingUIProps.SceneMainCanvas.GetComponent<Canvas>();
                fullDialogPanel = UIBackGround.rectTransform;
                fullDialogPanel.SetParent(tempCanvas.transform as RectTransform);
                UIBackGround.sprite = Managers.instance.Resource.Load<Sprite>("dialogue_panel");
                //UIBackGround.sprite = ������ ���� �̸�;
                //TODO : Ű ���ͷ��� �ȳ��ǳ� ������ UIBackGround ������ sprite �������־����
                fullDialogPanel.anchorMin = new Vector2(0.05f, 0.05f);
                fullDialogPanel.anchorMax = new Vector2(0.95f, 0.3f);
                fullDialogPanel.sizeDelta = Vector2.zero;
                fullDialogPanel.anchoredPosition = Vector2.zero;
            }
            return fullDialogPanel;
        }
    }
    private Image dialogCharactorIMG;
    private Image DialogCharactorIMG
    {
        get
        {
            if (dialogCharactorIMG == null)
            {
                dialogCharactorIMG = new GameObject { name = "dialogueCharactorIlust" }.AddComponent<Image>();
                dialogCharactorIMG.rectTransform.SetParent(FullDialogPanel);
                dialogCharactorIMG.sprite = Managers.instance.Resource.Load<Sprite>("dialogue_protraitpanel");
                //UIBackGround.sprite = ������ ���� �̸�;
                //TODO : Ű ���ͷ��� �ȳ��ǳ� ������ UIBackGround ������ sprite �������־����
                dialogCharactorIMG.rectTransform.anchorMin = new Vector2(0, 1);
                dialogCharactorIMG.rectTransform.anchorMax = new Vector2(0.12f, 1.9f);
                dialogCharactorIMG.rectTransform.sizeDelta = Vector2.zero;
                dialogCharactorIMG.rectTransform.anchoredPosition = Vector2.zero;
            }
            return dialogCharactorIMG;
        }
    }
    private RectTransform dialogPanel;
    private RectTransform DialogPanel
    {
        get
        {
            if (dialogPanel == null)
            {
                Image UIBackGround = new GameObject { name = "dialogPanel" }.AddComponent<Image>();
                dialogPanel = UIBackGround.rectTransform;
                dialogPanel.SetParent(FullDialogPanel);
                UIBackGround.color = Color.clear;
                //UIBackGround.sprite = ������ ���� �̸�;
                //TODO : Ű ���ͷ��� �ȳ��ǳ� ������ UIBackGround ������ sprite �������־����
                dialogPanel.anchorMin = Vector2.zero;
                dialogPanel.anchorMax = new Vector2(1f, 1f-(1f / 4f));
                dialogPanel.sizeDelta = Vector2.zero;
                dialogPanel.anchoredPosition = Vector2.zero;
            }
            return dialogPanel;
        }
    }
    private RectTransform namePanel;
    private RectTransform NamePanel
    {
        get
        {
            if (namePanel == null)
            {
                Image UIBackGround = new GameObject { name = "dialogNamePanel" }.AddComponent<Image>();
                namePanel = UIBackGround.rectTransform;
                namePanel.SetParent(FullDialogPanel);
                UIBackGround.sprite = Managers.instance.Resource.Load<Sprite>("dialogue_namebar");
                //UIBackGround.sprite = ������ ���� �̸�;
                //TODO : Ű ���ͷ��� �ȳ��ǳ� ������ UIBackGround ������ sprite �������־����
                UIBackGround.color = Color.gray;
                namePanel.anchorMin = new Vector2(0f, 3f/4f);
                namePanel.anchorMax = new Vector2(1f/6f, 1);
                namePanel.sizeDelta = Vector2.zero;
                namePanel.anchoredPosition = Vector2.zero;
            }
            return namePanel;
        }
    }

    private Text nameText;
    private Text NameText
    {
        get
        {
            if (nameText == null)
            {
                nameText = new GameObject { name = "nameText" }.AddComponent<Text>();
                //TODO : ��Ʈ����,������ �����ʿ� �߾����� �ؾ���
                nameText.rectTransform.SetParent(NamePanel);
                nameText.rectTransform.anchorMin = Vector2.zero;
                nameText.rectTransform.anchorMax = Vector2.one;
                nameText.rectTransform.sizeDelta = Vector2.zero;
                nameText.rectTransform.anchoredPosition = Vector2.zero;
                nameText.fontSize = 25;
                nameText.color = Color.black;
                nameText.font = Managers.instance.Resource.Load<Font>("InGameFont");
                nameText.alignment = TextAnchor.MiddleCenter;
            }
            return nameText;
        }
    }
    private Text dialogText;
    private Text DialogText
    {
        get
        {
            if (dialogText == null)
            {
                dialogText = new GameObject { name = "dialogText" }.AddComponent<Text>();
                //TODO : ��Ʈ����,������ �����ʿ� �߾����� �ؾ���
                dialogText.rectTransform.SetParent(DialogPanel);
                dialogText.rectTransform.anchorMin = Vector2.zero;
                dialogText.rectTransform.anchorMax = Vector2.one;
                dialogText.rectTransform.sizeDelta = Vector2.zero;
                dialogText.rectTransform.anchoredPosition = Vector2.zero;
                dialogText.fontSize = 25;
                dialogText.color = Color.black;
                dialogText.font = Managers.instance.Resource.Load<Font>("InGameFont");
                dialogText.alignment = TextAnchor.MiddleCenter; ;
                dialogText.horizontalOverflow = HorizontalWrapMode.Overflow;
            }
            return dialogText;
        }
    }
    public void DialogTextChanger(string talkerName,string dialog, string talkerIMGName)
    {
        NameText.text = talkerName;
        //TODO : ȭ�� �Ϸ���Ʈ ��� �̹��� UI�� �����Ͱ� �Ľ��ϴ� �Լ� �����ʿ�
        DialogText.text = dialog;
    }
    public void DialogSetting()
    {
        DialogCharactorIMG.IsActive();
        DialogText.text = "�׽�Ʈ12";
        NameText.text = "�׽�Ʈ12"; 
        Managers.instance.UI.CheckerRegist(fullDialogPanel);
        fullDialogPanel.gameObject.SetActive(false);
    }

}