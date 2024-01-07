using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using DataDefines;
using InteractionDefines;
using UnityEditor;
using System.Diagnostics.Tracing;
using UnityEngine.Video;

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
            if (topViewSceneUIs == null) topViewSceneUIs = new TopViewSceneUI();
            return topViewSceneUIs;
        }
    }
    private DialogSystem dialogCall;
    public DialogSystem DialogCall
    {
        get
        {
            if (dialogCall == null) dialogCall = new DialogSystem();
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
    public void TargetUIOnOff(Transform target, bool isTurnOn)
    {
        // TODO : 특정 UI 닫기버튼 누를때 연결해주어야 할 함수 끌때 isTurnOn을 false 열때는 true
        target.gameObject.SetActive(isTurnOn);
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
                    //FLOW : 로딩바 이미지 변경,색상 변경이 필요하면 여기서 변경
                    Slider tempLoadingSlider = new GameObject("LoadingSlider").AddComponent<Slider>();
                    //Vector2 CanvasSize = new Vector2(SceneMainCanvas.rect.width, SceneMainCanvas.rect.height);
                    tempLoadingSlider.wholeNumbers = false;
                    tempLoadingSlider.maxValue = 100;
                    RectTransform tempSliderTR = tempLoadingSlider.transform as RectTransform;

                    tempLoadingSlider.AddComponent<Image>().color = Color.grey;
                    Image tempIMG = new GameObject("Handle").AddComponent<Image>();
                    tempIMG.color = Color.green;
                    tempLoadingSlider.fillRect = tempIMG.rectTransform;

                    // Slider의 부모-자식 관계 설정  
                    tempLoadingSlider.fillRect.SetParent(tempSliderTR);
                    tempLoadingSlider.fillRect.offsetMax = new Vector2(tempLoadingSlider.fillRect.offsetMax.x, 0);
                    tempLoadingSlider.fillRect.offsetMin = new Vector2(tempLoadingSlider.fillRect.offsetMin.x, 0);
                    tempLoadingSlider.fillRect.anchorMin = Vector2.zero; // 부모의 왼쪽 하단을 기준으로
                    tempLoadingSlider.fillRect.anchorMax = Vector2.one; // 부모의 왼쪽 상단을 기준으로
                    tempLoadingSlider.fillRect.pivot = new Vector2(0, 0.5f); // 부모의 왼쪽 중간을 기준으로
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
                    Debug.Log("로딩바 세팅 끝");
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
                Canvas tempCanvas = Managers.instance.UI.LoadingUIProps.SceneMainCanvas.GetComponent<Canvas>();
                interactionKeyPanel = UIBackGround.rectTransform;
                interactionKeyPanel.SetParent(tempCanvas.transform as RectTransform);

                //UIBackGround.sprite = 변경할 에셋 이름;
                //TODO : 키 인터렉션 안내판넬 받으면 UIBackGround 변수의 sprite 변경해주어야함
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
            InteractionKeyTextTitle.text = "스페이스바 키를 눌러 인터렉션 하세요";
            InteractionKeyTextTitle.fontSize = 25;


        }
        InteractionKeyPanel.gameObject.SetActive(OnOFF);
    }

}

public class DialogSystem
{
    #region 다이얼로그 변수
    private RectTransform fullDialogPanel;
    public RectTransform FullDialogPanel
    {
        get
        {
            if (fullDialogPanel == null)
            {
                Image UIBackGround = new GameObject { name = "dialogPanel" }.AddComponent<Image>();
                fullDialogPanel = UIBackGround.rectTransform;
                fullDialogPanel.SetParent(DialogueBackGround.rectTransform);
                UIBackGround.sprite = Managers.instance.Resource.Load<Sprite>("dialogue_panel");
                //UIBackGround.sprite = 변경할 에셋 이름;
                //TODO : 키 인터렉션 안내판넬 받으면 UIBackGround 변수의 sprite 변경해주어야함
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
                Image TempParent = new GameObject { name = "dialogueCharactorIlustPanel" }.AddComponent<Image>();
                TempParent.sprite = Managers.instance.Resource.Load<Sprite>("dialogue_protraitpanel");
                TempParent.rectTransform.SetParent(FullDialogPanel);
                //UIBackGround.sprite = 변경할 에셋 이름;
                //TODO : 키 인터렉션 안내판넬 받으면 UIBackGround 변수의 sprite 변경해주어야함
                TempParent.rectTransform.anchorMin = new Vector2(0, 1);
                TempParent.rectTransform.anchorMax = new Vector2(0.12f, 1.9f);
                TempParent.rectTransform.sizeDelta = Vector2.zero;
                TempParent.rectTransform.anchoredPosition = Vector2.zero;
                dialogCharactorIMG = new GameObject { name = "dialogueCharactorIlust" }.AddComponent<Image>();

                dialogCharactorIMG.rectTransform.SetParent(TempParent.rectTransform);
                dialogCharactorIMG.rectTransform.anchorMin = Vector2.zero;
                dialogCharactorIMG.rectTransform.anchorMax = Vector2.one;
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
                //UIBackGround.sprite = 변경할 에셋 이름;
                //TODO : 키 인터렉션 안내판넬 받으면 UIBackGround 변수의 sprite 변경해주어야함
                dialogPanel.anchorMin = Vector2.zero;
                dialogPanel.anchorMax = new Vector2(1f, 1f - (1f / 4f));
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
                //UIBackGround.sprite = 변경할 에셋 이름;
                //TODO : 키 인터렉션 안내판넬 받으면 UIBackGround 변수의 sprite 변경해주어야함
                UIBackGround.color = Color.gray;
                namePanel.anchorMin = new Vector2(0f, 3f / 4f);
                namePanel.anchorMax = new Vector2(1f / 6f, 1);
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
                //TODO : 폰트누락,사이즈 조절필요 중앙정렬 해야함
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
                //TODO : 폰트누락,사이즈 조절필요 중앙정렬 해야함
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

    private Queue<DataDefines.DialogDatas> dataQueue = new Queue<DialogDatas>();

    private VideoPlayer backGroundVideo;
    public VideoPlayer BackGroundVideo
    {
        get
        {
            if (backGroundVideo == null)
            {
                backGroundVideo = DialogueBackGround.AddComponent<VideoPlayer>();
                backGroundVideo.isLooping = true;
                backGroundVideo.targetTexture = Managers.instance.Resource.Load<RenderTexture>("BackGroundVideoTexture");
            }
            return backGroundVideo;
        }
    }
    private RawImage dialogueBackGround;
    public RawImage DialogueBackGround
    {
        get
        {
            if (dialogueBackGround == null)
            {
                dialogueBackGround = new GameObject { name = "DialogueBackGroundPanel" }.AddComponent<RawImage>();
                dialogueBackGround.texture = BackGroundVideo.targetTexture;
                dialogueBackGround.rectTransform.SetParent(Managers.instance.UI.LoadingUIProps.SceneMainCanvas);
                dialogueBackGround.rectTransform.anchorMin = Vector2.zero;
                dialogueBackGround.rectTransform.anchorMax = Vector2.one;
                dialogueBackGround.rectTransform.sizeDelta = Vector2.zero;
                dialogueBackGround.rectTransform.anchoredPosition = Vector2.zero;
                dialogueBackGround.rectTransform.SetAsLastSibling();
            }
            return dialogueBackGround;
        }
    }

    #endregion
    public void SetDialogueData(int EventNumber)
    {
        dataQueue.Clear();
        TextAsset tempTextAsset = Managers.instance.Resource.Load<TextAsset>("DialogueData");
        List<DialogDatas> tempData = JsonConvert.DeserializeObject<List<DialogDatas>>(tempTextAsset.text);
        foreach (DialogDatas data in tempData)
        {
            int tempDialogArray = data.EventName - (data.EventName % 10000);
            tempDialogArray = tempDialogArray / 10000;
            if (tempDialogArray == EventNumber)
            {
                dataQueue.Enqueue(data);
            }
            else if (EventNumber < tempDialogArray)
            {
                break;
            }
        }
        DialogTextChanger();
        Debug.Log(tempData.Count);
    }
    public void DialogNextOnly()
    {
        DataDefines.DialogDatas dialogData = dataQueue.Dequeue();
        if (dialogData.Name.Contains("None"))
        {
            NameText.text = string.Empty;
        }
        else
        {
            NameText.text = dialogData.Name;
        }
        //TODO : 사운드 출력 함수 구문 추가필요
        DialogText.text = dialogData.dialogue;
        DialogCharactorIMG.sprite = Managers.instance.Resource.Load<Sprite>(dialogData.Portrait);
        VideoClip tempVideo = Managers.instance.Resource.Load<VideoClip>(dialogData.Background);
        if (tempVideo != null)
        {
            BackGroundVideo.enabled = true;
            BackGroundVideo.clip = tempVideo;
            DialogueBackGround.texture = Managers.instance.Resource.Load<Texture>("BackGroundVideoTexture");
            BackGroundVideo.Play();
        }
        else
        {
            BackGroundVideo.enabled = false;
            DialogueBackGround.texture = Managers.instance.Resource.Load<Texture2D>(dialogData.Background);
        }

        Debug.Log("백그라운드" + dialogData.Background);

    }
    #region DialogChangers
    public void DialogTextChanger()
    {
        if (dataQueue.Count > 0)
        {
            DialogNextOnly();
        }
        else
        {
            Managers.instance.UI.TargetUIOnOff(DialogueBackGround.rectTransform, false);
        }
    }
    public void DialogTextChanger(Vector2Int RemoveInteractionPosition)
    {
        if (dataQueue.Count > 0)
        {
            DialogNextOnly();
        }
        else
        {
            Managers.instance.Grid.RemoveInteraction(RemoveInteractionPosition);
            Managers.instance.UI.TargetUIOnOff(DialogueBackGround.rectTransform, false);
        }
    }
    public void DialogTextChanger(Vector2Int RemoveInteractionPosition, InteractionInstallerProps AddInteraction)
    {
        if (dataQueue.Count > 0)
        {
            DialogNextOnly();
        }
        else
        {
            Managers.instance.Grid.RemoveInteraction(RemoveInteractionPosition);
            Managers.instance.Grid.AddInteraction(AddInteraction);
            //TODO : 인터렉션 삭제하는 코드 넣어줘야함
            Managers.instance.UI.TargetUIOnOff(DialogueBackGround.rectTransform, false);
        }
    }
    #endregion
    public void DialogSetting()
    {
        DialogCharactorIMG.IsActive();
        DialogText.text = "테스트12";
        NameText.text = "테스트12";
        Managers.instance.UI.CheckerRegist(DialogueBackGround.rectTransform);
        DialogueBackGround.gameObject.SetActive(false);
    }

}