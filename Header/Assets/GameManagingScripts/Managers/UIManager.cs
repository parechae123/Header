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
using DG.Tweening;
using UnityEngine.Video;
using UnityEditor.Build.Pipeline.Interfaces;
using HeaderPadDefines;

public class UIManager
{
    public LoadingUI LoadingUIProps = new LoadingUI();
    public TopViewSceneUI TopViewSceneUIs = new TopViewSceneUI();
    public DialogSystem DialogCall = new DialogSystem();
    public BattleUI BattleUICall = new BattleUI();
    public ShopUI ShopUICall = new ShopUI();

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
                tempTR.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
                tempTR.gameObject.layer = 5;
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
                TempParent.rectTransform.anchorMin = new Vector2(0.02f, 1);
                TempParent.rectTransform.anchorMax = new Vector2(0.14f, 2.1f);
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
                dialogText.color = Color.white;
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
public class BattleUI
{
    #region 플레이어 관련 변수
    private Image playerStatusUI;
    private Image PlayerStatusUI
    {
        get
        {
            if (playerStatusUI == null)
            {
                playerStatusUI = new GameObject("PlayerStatusPanel").AddComponent<Image>();
                playerStatusUI.rectTransform.SetParent(Managers.instance.UI.LoadingUIProps.SceneMainCanvas);
                playerStatusUI.rectTransform.anchorMax = new Vector2(0.172000006f, 1f);
                playerStatusUI.rectTransform.anchorMin = Vector2.zero;
                playerStatusUI.rectTransform.sizeDelta = Vector2.zero;
                playerStatusUI.rectTransform.anchoredPosition = Vector2.zero;
                playerStatusUI.sprite = Managers.instance.Resource.Load<Sprite>("battle_panel");
            }
            return playerStatusUI;
        }
    }
    private Image playerPortrait;
    private Image PlayerPortrait
    {
        get
        {
            if (playerPortrait == null)
            {
                playerPortrait = new GameObject("PlayerStatusPanel").AddComponent<Image>();
                playerPortrait.rectTransform.SetParent(PlayerStatusUI.rectTransform);
                playerPortrait.rectTransform.anchorMax = new Vector2(0.899999976f, 0.847f);
                playerPortrait.rectTransform.anchorMin = new Vector2(0.100000001f, 0.597f);
                playerPortrait.rectTransform.sizeDelta = Vector2.zero;
                playerPortrait.rectTransform.anchoredPosition = Vector2.zero;
                playerPortrait.sprite = Managers.instance.Resource.Load<Sprite>("battle_portrait");
            }
            return playerStatusUI;
        }
    }

    private Slider playerHpBar = null;
    private Slider PlayerHPBar
    {
        get
        {
            if (playerHpBar == null)
            {
                Slider tempPlayerHPbar = new GameObject("PlayerHPBar").AddComponent<Slider>();
                tempPlayerHPbar.wholeNumbers = false;
                tempPlayerHPbar.maxValue = 100;
                //TODO : player 체력 구현하면 여기에 넣어줘야함
                RectTransform tempHpTR = tempPlayerHPbar.transform as RectTransform;


                RectTransform tempHandleArea = new GameObject("HandleArea").AddComponent<RectTransform>();
                tempHandleArea.SetParent(tempHpTR);
                tempHandleArea.anchorMin = Vector2.zero; // 부모의 왼쪽 하단을 기준으로
                tempHandleArea.anchorMax = Vector2.one; // 부모의 왼쪽 상단을 기준으로
                tempHandleArea.pivot = new Vector2(0.5f, 0.5f); // 부모의 왼쪽 중간을 기준으로
                tempHandleArea.sizeDelta = Vector2.zero;
                tempHandleArea.anchoredPosition = Vector2.zero;
                Image handle = new GameObject("handle").AddComponent<Image>();
                handle.color = Color.white;
                handle.rectTransform.SetParent(tempHandleArea);
                handle.rectTransform.anchorMax = Vector2.up;
                handle.rectTransform.anchorMin = Vector2.zero;
                handle.rectTransform.pivot = Vector2.one / 2f;
                handle.rectTransform.sizeDelta = Vector2.right * 50;
                tempPlayerHPbar.handleRect = handle.rectTransform;
                handle.color = Color.white;
                handle.sprite = Managers.instance.Resource.Load<Sprite>("HP_icon");
                handle.rectTransform.SetAsLastSibling();

                Image BackGround = new GameObject("BackGround").AddComponent<Image>();
                BackGround.sprite = Managers.instance.Resource.Load<Sprite>("HPbar");
                BackGround.rectTransform.SetParent(tempHpTR);
                BackGround.rectTransform.anchorMax = new Vector2(1f, 0.75f);
                BackGround.rectTransform.anchorMin = new Vector2(0f, 0.25f);
                BackGround.rectTransform.sizeDelta = Vector2.zero;
                BackGround.rectTransform.pivot = Vector2.one / 2f;
                BackGround.color = Color.grey;
                BackGround.sprite = Managers.instance.Resource.Load<Sprite>("HPbar");
                BackGround.rectTransform.SetAsFirstSibling();


                RectTransform tempFillArea = new GameObject("FillArea").AddComponent<RectTransform>();
                tempFillArea.SetParent(tempHpTR);
                tempFillArea.anchorMin = new Vector2(0f, 0.25f); // 부모의 왼쪽 하단을 기준으로
                tempFillArea.anchorMax = new Vector2(1f, 0.75f); // 부모의 왼쪽 상단을 기준으로
                tempFillArea.pivot = new Vector2(0.5f, 0.5f); // 부모의 왼쪽 중간을 기준으로
                tempFillArea.sizeDelta = Vector2.zero;
                tempFillArea.anchoredPosition = Vector2.zero;
                Image tempIMG = new GameObject("FillRect").AddComponent<Image>();
                tempIMG.color = Color.white;
                tempIMG.sprite = Managers.instance.Resource.Load<Sprite>("HPbar");



                tempIMG.rectTransform.SetParent(tempFillArea);
                tempIMG.rectTransform.sizeDelta = Vector2.zero;
                tempPlayerHPbar.fillRect = tempIMG.rectTransform;

                // Slider의 부모-자식 관계 설정  



                tempHpTR.SetParent(PlayerStatusUI.rectTransform);
                tempHpTR.SetAsLastSibling();
                tempHpTR.anchorMin = new Vector2(0.1f, 0.88f);
                tempHpTR.anchorMax = new Vector2(0.9f, 0.93f);
                tempHpTR.sizeDelta = Vector2.zero;
                tempHpTR.anchoredPosition = Vector2.zero;
                tempPlayerHPbar.interactable = false;
                tempPlayerHPbar.enabled = true;
                tempHandleArea.SetAsLastSibling();
                tempPlayerHPbar.SetDirection(Slider.Direction.RightToLeft,true);
                playerHpBar = tempPlayerHPbar;

                Debug.Log("체력바 세팅 끝");
            }
            return playerHpBar;
        }
    }
    private Image weaponImagePanel;
    private Image WeaponImagePanel
    {
        get 
        { 
            if (weaponImagePanel == null) 
            {
                weaponImagePanel = new GameObject("weaponIMGPanel").AddComponent<Image>();
                weaponImagePanel.rectTransform.SetParent(PlayerStatusUI.rectTransform);
                weaponImagePanel.sprite = null;
                weaponImagePanel.color = new Color(0, 0, 0, 0);
                weaponImagePanel.rectTransform.anchorMin = new Vector2(0.330525041f, 0.483485878f);
                weaponImagePanel.rectTransform.anchorMax = new Vector2(0.669326663f, 0.582539618f);
                weaponImagePanel.rectTransform.sizeDelta = Vector2.zero;
                weaponImagePanel.rectTransform.anchoredPosition = Vector2.zero;
                WeaponImagePanel.AddComponent<RectMask2D>();
            }
            return weaponImagePanel;
        }
    }
    private Image weaponImage;
    private Image WeaponImage
    {
        get 
        { 
            if (weaponImage == null) 
            {
                weaponImage = new GameObject("weaponIMG").AddComponent<Image>();
                weaponImage.rectTransform.SetParent(WeaponImagePanel.rectTransform);
                weaponImage.sprite = Managers.instance.Resource.Load<Sprite>("Bullet_Basic");
                WeaponImagePrev.enabled = true;
                weaponImage.rectTransform.anchorMin = Vector2.zero;
                weaponImage.rectTransform.anchorMax = Vector2.one;
                weaponImage.rectTransform.sizeDelta = Vector2.zero;
                weaponImage.rectTransform.anchoredPosition = Vector2.zero;
            }
            return weaponImage;
        }
    }
    private Image weaponImagePrev;
    private Image WeaponImagePrev
    {
        get 
        { 
            if (weaponImagePrev == null) 
            {
                weaponImagePrev = new GameObject("weaponIMGPrev").AddComponent<Image>();
                weaponImagePrev.rectTransform.SetParent(WeaponImagePanel.rectTransform);
                weaponImagePrev.sprite = Managers.instance.Resource.Load<Sprite>("Bullet_Basic");
                weaponImagePrev.rectTransform.anchorMin = Vector2.left;
                weaponImagePrev.rectTransform.anchorMax = Vector2.up;
                weaponImagePrev.rectTransform.sizeDelta = Vector2.zero;
                weaponImagePrev.rectTransform.anchoredPosition = Vector2.zero;
            }
            return weaponImagePrev;
        }
    }
    private Image weaponNamePannel;
    private Image WeaponNamePannel 
    { 
        get 
        { 
            if (weaponNamePannel == null)
            {
                weaponNamePannel = new GameObject("WeaponNamePannel").AddComponent<Image>();
                weaponNamePannel.rectTransform.SetParent(PlayerStatusUI.rectTransform);
                weaponNamePannel.sprite = Managers.instance.Resource.Load<Sprite>("select_panel");
                weaponNamePannel.rectTransform.anchorMin = new Vector2(0.23300001f, 0.390000015f);
                weaponNamePannel.rectTransform.anchorMax = new Vector2(0.771000028f, 0.474000037f);
                weaponNamePannel.rectTransform.sizeDelta = Vector2.zero;
                weaponNamePannel.rectTransform.anchoredPosition = Vector2.zero;
            }
            return weaponNamePannel;
        } 
    }

    private Text playerWeaponName;
    private Text PlayerWeaponName
    {
        get
        {
            if (playerWeaponName == null)
            {
                playerWeaponName = new GameObject("playerWeaponName").AddComponent<Text>();
                playerWeaponName.rectTransform.SetParent(WeaponNamePannel.rectTransform);
                playerWeaponName.rectTransform.anchorMin = Vector2.zero;
                playerWeaponName.rectTransform.anchorMax = Vector2.one;
                playerWeaponName.rectTransform.sizeDelta = Vector2.zero;
                playerWeaponName.rectTransform.anchoredPosition = Vector2.zero;
                playerWeaponName.alignment = TextAnchor.MiddleCenter;
                playerWeaponName.color = Color.black;
                playerWeaponName.fontSize = 36;
                playerWeaponName.font = Managers.instance.Resource.Load<Font>("InGameFont");

            }
            return playerWeaponName;
        }
    }

    private Button weaponBeforeBTN;
    private Button WeaponBeforeBTN
    {
        get 
        {
            if (weaponBeforeBTN == null)
            {
                weaponBeforeBTN = new GameObject("weaponNextBTN").AddComponent<Button>();
                Image tempIMG = weaponBeforeBTN.AddComponent<Image>();
                weaponBeforeBTN.targetGraphic = tempIMG;
                tempIMG.rectTransform.SetParent(WeaponNamePannel.rectTransform);
                tempIMG.rectTransform.anchorMin = new Vector2(-0.300000012f, 0.253313094f);
                tempIMG.rectTransform.anchorMax = new Vector2(-0.0799999982f, 0.709656298f);
                tempIMG.rectTransform.anchoredPosition = Vector2.zero;
                tempIMG.rectTransform.sizeDelta = Vector2.zero;
                tempIMG.sprite = Managers.instance.Resource.Load<Sprite>("select_arrow_panel_L");
            }
            return weaponBeforeBTN;
        }
    }
    private Button weaponNextBTN;
    private Button WeaponNextBTN
    {
        get 
        {
            if (weaponNextBTN == null)
            {
                weaponNextBTN = new GameObject("weaponNextBTN").AddComponent<Button>();
                Image tempIMG = weaponNextBTN.AddComponent<Image>();
                weaponNextBTN.targetGraphic = tempIMG;
                tempIMG.rectTransform.SetParent(WeaponNamePannel.rectTransform);
                tempIMG.rectTransform.anchorMin = new Vector2(1.08000004f, 0.253313124f);
                tempIMG.rectTransform.anchorMax = new Vector2(1.29999995f, 0.709656298f);
                tempIMG.rectTransform.anchoredPosition = Vector2.zero;
                tempIMG.rectTransform.sizeDelta = Vector2.zero;
                tempIMG.sprite = Managers.instance.Resource.Load<Sprite>("select_arrow_panel_R");
            }
            return weaponNextBTN;
        }
    }
    private Image girlPortrait;
    private Image GirlPortrait
    {
        get 
        {
            if (girlPortrait == null)
            {
                girlPortrait = new GameObject("BattleSceneGirlPortrait").AddComponent<Image>();
                girlPortrait.rectTransform.SetParent(PlayerStatusUI.rectTransform);
                girlPortrait.rectTransform.anchorMax = new Vector2(0.784737825f, 0.217769772f);
                girlPortrait.rectTransform.anchorMin = new Vector2(0.216160953f, 0.0475479327f);
                girlPortrait.rectTransform.anchoredPosition= Vector2.zero;
                girlPortrait.rectTransform.sizeDelta= Vector2.zero;
                girlPortrait.sprite = Managers.instance.Resource.Load<Sprite>("battle_portrait_girl");
            }
            return girlPortrait; 
        }
    }
    private Image girlChatBubble;
    private Image GirlChatBubble
    {
        get 
        {
            if (girlChatBubble == null)
            {
                girlChatBubble = new GameObject("girlChatBubble").AddComponent<Image>();
                girlChatBubble.rectTransform.SetParent(PlayerStatusUI.rectTransform);
                girlChatBubble.rectTransform.anchorMax = new Vector2(0.86668098f, 0.371693075f);
                girlChatBubble.rectTransform.anchorMin = new Vector2(0.132425845f, 0.226054743f);
                girlChatBubble.rectTransform.anchoredPosition= Vector2.zero;
                girlChatBubble.rectTransform.sizeDelta= Vector2.zero;
                girlChatBubble.sprite = Managers.instance.Resource.Load<Sprite>("chatbubble ");
            }
            return girlChatBubble; 
        }
    }
    private Text girlText;
    private Text GirlText
    {
        get 
        {
            if (girlText == null)
            {
                girlText = new GameObject("girlChatText").AddComponent<Text>();
                girlText.rectTransform.SetParent(GirlChatBubble.rectTransform);
                girlText.rectTransform.anchorMin = Vector2.zero;
                girlText.rectTransform.anchorMax = Vector2.one;
                girlText.rectTransform.sizeDelta = Vector2.zero;
                girlText.rectTransform.anchoredPosition = Vector2.zero;

                girlText.font = Managers.instance.Resource.Load<Font>("InGameFont");
                girlText.alignment = TextAnchor.MiddleCenter;
                girlText.color = Color.black;
                girlText.fontSize = 36;
            }
            return girlText; 
        }
    }
    

    #endregion

    #region 적 statusUI관련 변수

    private Image enemyStatusUI;
    private Image EnemyStatusUI
    {
        get
        {
            if (enemyStatusUI == null)
            {
                enemyStatusUI = new GameObject("EnemyStatusPanel").AddComponent<Image>();
                enemyStatusUI.rectTransform.SetParent(Managers.instance.UI.LoadingUIProps.SceneMainCanvas);
                enemyStatusUI.rectTransform.anchorMax = Vector2.one;
                enemyStatusUI.rectTransform.anchorMin = new Vector2(0.828000009f, 0f);
                enemyStatusUI.rectTransform.sizeDelta = Vector2.zero;
                enemyStatusUI.rectTransform.anchoredPosition = Vector2.zero;
                enemyStatusUI.sprite = Managers.instance.Resource.Load<Sprite>("battle_panel");
            }
            return enemyStatusUI;
        }
    }
    private Image enemyPortrait;
    private Image EnemyPortrait
    {
        get
        {
            if (enemyPortrait == null)
            {
                enemyPortrait = new GameObject("EnemyStatusPanel").AddComponent<Image>();
                enemyPortrait.rectTransform.SetParent(EnemyStatusUI.rectTransform);
                enemyPortrait.rectTransform.anchorMax = new Vector2(0.899999976f, 0.847f);
                enemyPortrait.rectTransform.anchorMin = new Vector2(0.100000001f, 0.597f);
                enemyPortrait.rectTransform.sizeDelta = Vector2.zero;
                enemyPortrait.rectTransform.anchoredPosition = Vector2.zero;
                enemyPortrait.sprite = Managers.instance.Resource.Load<Sprite>("battle_portrait_crasher");
            }
            return enemyStatusUI;
        }
    }

    private Slider enemyHpBar = null;
    private Slider EnemyHPBar
    {
        get
        {
            if (enemyHpBar == null)
            {
                Slider tempEnemyHPbar = new GameObject("EnemyHPBar").AddComponent<Slider>();
                tempEnemyHPbar.wholeNumbers = false;
                tempEnemyHPbar.maxValue = 100;
                //TODO : enemy 체력 구현하면 여기에 넣어줘야함
                RectTransform tempHpTR = tempEnemyHPbar.transform as RectTransform;


                RectTransform tempHandleArea = new GameObject("HandleArea").AddComponent<RectTransform>();
                tempHandleArea.SetParent(tempHpTR);
                tempHandleArea.anchorMin = Vector2.zero; // 부모의 왼쪽 하단을 기준으로
                tempHandleArea.anchorMax = Vector2.one; // 부모의 왼쪽 상단을 기준으로
                tempHandleArea.pivot = new Vector2(0.5f, 0.5f); // 부모의 왼쪽 중간을 기준으로
                tempHandleArea.sizeDelta = Vector2.zero;
                tempHandleArea.anchoredPosition = Vector2.zero;
                Image handle = new GameObject("handle").AddComponent<Image>();
                handle.rectTransform.SetParent(tempHandleArea);
                handle.rectTransform.anchorMax = Vector2.up;
                handle.rectTransform.anchorMin = Vector2.zero;
                handle.rectTransform.pivot = Vector2.one / 2f;
                handle.rectTransform.sizeDelta = Vector2.right * 50;
                tempEnemyHPbar.handleRect = handle.rectTransform;
                handle.color = Color.white;
                handle.sprite = Managers.instance.Resource.Load<Sprite>("HP_icon");


                Image BackGround = new GameObject("BackGround").AddComponent<Image>();
                BackGround.rectTransform.SetParent(tempHpTR);
                BackGround.rectTransform.anchorMax = new Vector2(1f, 0.75f);
                BackGround.rectTransform.anchorMin = new Vector2(0f, 0.25f);
                BackGround.rectTransform.sizeDelta = Vector2.zero;
                BackGround.rectTransform.pivot = Vector2.one / 2f;
                BackGround.color = Color.grey;
                BackGround.sprite = Managers.instance.Resource.Load<Sprite>("HPbar");
                BackGround.rectTransform.SetAsFirstSibling();


                RectTransform tempFillArea = new GameObject("FillArea").AddComponent<RectTransform>();
                tempFillArea.SetParent(tempHpTR);
                tempFillArea.anchorMin = new Vector2(0f, 0.25f); // 부모의 왼쪽 하단을 기준으로
                tempFillArea.anchorMax = new Vector2(1f, 0.75f); // 부모의 왼쪽 상단을 기준으로
                tempFillArea.pivot = new Vector2(0.5f, 0.5f); // 부모의 왼쪽 중간을 기준으로
                tempFillArea.sizeDelta = Vector2.zero;
                tempFillArea.anchoredPosition = Vector2.zero;
                Image tempIMG = new GameObject("FillRect").AddComponent<Image>();
                tempIMG.rectTransform.SetParent(tempFillArea);
                tempIMG.rectTransform.sizeDelta = Vector2.zero;
                tempIMG.color = Color.white;
                tempIMG.sprite = Managers.instance.Resource.Load<Sprite>("HPbar");
                tempEnemyHPbar.fillRect = tempIMG.rectTransform;

                // Slider의 부모-자식 관계 설정  



                tempHpTR.SetParent(EnemyStatusUI.rectTransform);
                tempHpTR.SetAsLastSibling();
                tempHpTR.anchorMin = new Vector2(0.1f, 0.88f);
                tempHpTR.anchorMax = new Vector2(0.9f, 0.93f);
                tempHpTR.sizeDelta = Vector2.zero;
                tempHpTR.anchoredPosition = Vector2.zero;
                tempEnemyHPbar.interactable = false;
                tempEnemyHPbar.enabled = true;
                tempEnemyHPbar.SetDirection(Slider.Direction.RightToLeft, true);
                enemyHpBar = tempEnemyHPbar;
                tempHandleArea.SetAsLastSibling();
                Debug.Log("체력바 세팅 끝");
            }
            return enemyHpBar;
        }
    }
    private Image enemyWeaponImage;
    private Image EnemyWeaponImage
    {
        get
        {
            if (enemyWeaponImage == null)
            {
                enemyWeaponImage = new GameObject("EnemyWeaponImage").AddComponent<Image>();
                enemyWeaponImage.rectTransform.SetParent(EnemyStatusUI.rectTransform);
                enemyWeaponImage.sprite = Managers.instance.Resource.Load<Sprite>("hammer");
                enemyWeaponImage.rectTransform.anchorMin = new Vector2(0.330525041f, 0.483485878f);
                enemyWeaponImage.rectTransform.anchorMax = new Vector2(0.669326663f, 0.582539618f);
                enemyWeaponImage.rectTransform.sizeDelta = Vector2.zero;
                enemyWeaponImage.rectTransform.anchoredPosition = Vector2.zero;
            }
            return enemyWeaponImage;
        }
    }
    private Image enemyWeaponNamePannel;
    private Image EnemyWeaponNamePannel
    {
        get
        {
            if (enemyWeaponNamePannel == null)
            {
                enemyWeaponNamePannel = new GameObject("EnemyWeaponNamePannel").AddComponent<Image>();
                enemyWeaponNamePannel.rectTransform.SetParent(EnemyStatusUI.rectTransform);
                enemyWeaponNamePannel.sprite = Managers.instance.Resource.Load<Sprite>("select_panel");
                enemyWeaponNamePannel.rectTransform.anchorMin = new Vector2(0.23300001f, 0.390000015f);
                enemyWeaponNamePannel.rectTransform.anchorMax = new Vector2(0.771000028f, 0.474000037f);
                enemyWeaponNamePannel.rectTransform.sizeDelta = Vector2.zero;
                enemyWeaponNamePannel.rectTransform.anchoredPosition = Vector2.zero;
            }
            return enemyWeaponNamePannel;
        }
    }
    private Text enemyWeaponName;
    private Text EnemyWeaponName
    {
        get 
        {
            if (enemyWeaponName == null)
            {
                enemyWeaponName = new GameObject("enemyWeaponName").AddComponent<Text>();
                enemyWeaponName.rectTransform.SetParent(EnemyWeaponNamePannel.rectTransform);
                enemyWeaponName.rectTransform.anchorMin = Vector2.zero;
                enemyWeaponName.rectTransform.anchorMax = Vector2.one;
                enemyWeaponName.rectTransform.sizeDelta = Vector2.zero;
                enemyWeaponName.rectTransform.anchoredPosition = Vector2.zero;
                enemyWeaponName.alignment = TextAnchor.MiddleCenter;
                enemyWeaponName.color = Color.black;
                enemyWeaponName.fontSize = 36;
                enemyWeaponName.font = Managers.instance.Resource.Load<Font>("InGameFont");

            }
            return enemyWeaponName;
        }
    }
    #endregion
    private Image scoreBoardPannel;
    private Image ScoreBoardPannel 
    { 
        get 
        { 
            if (scoreBoardPannel == null)
            {
                scoreBoardPannel = new GameObject("ScoreBoardPannel").AddComponent<Image>();
                scoreBoardPannel.sprite = Managers.instance.Resource.Load<Sprite>("score_panel");
                scoreBoardPannel.rectTransform.SetParent(Managers.instance.UI.LoadingUIProps.SceneMainCanvas);
                scoreBoardPannel.rectTransform.anchorMin = new Vector2(0.398604751f, 0.872544408f);
                scoreBoardPannel.rectTransform.anchorMax = new Vector2(0.604237199f, 0.987735808f);
                scoreBoardPannel.rectTransform.anchoredPosition = Vector2.zero;
                scoreBoardPannel.rectTransform.sizeDelta= Vector2.zero;

            }
            return scoreBoardPannel;
        }
    }
    private Text scoreBoardText;
    private Text ScoreBoardText
    {
        get 
        { 
            if(scoreBoardText == null)
            {
                scoreBoardText = new GameObject("enemyWeaponName").AddComponent<Text>();
                scoreBoardText.rectTransform.SetParent(ScoreBoardPannel.rectTransform);
                scoreBoardText.rectTransform.anchorMin = Vector2.zero;
                scoreBoardText.rectTransform.anchorMax = Vector2.one;
                scoreBoardText.rectTransform.sizeDelta = Vector2.zero;
                scoreBoardText.rectTransform.anchoredPosition = Vector2.zero;
                scoreBoardText.alignment = TextAnchor.MiddleCenter;
                scoreBoardText.color = Color.black;
                scoreBoardText.fontSize = 84;
                scoreBoardText.font = Managers.instance.Resource.Load<Font>("InGameFont");
            }
            return scoreBoardText;
        }
    }
    private RectTransform ballForceSliderParent;
    private RectTransform BallForceSliderParent
    {
        get 
        {
            if(ballForceSliderParent == null)
            {
                ballForceSliderParent = new GameObject("BallForceParent").AddComponent<RectTransform>();
                ballForceSliderParent.SetParent(Managers.instance.UI.LoadingUIProps.SceneMainCanvas);
                ballForceSliderParent.anchorMin = new Vector2(0, 0);
                ballForceSliderParent.anchorMax = new Vector2(1, 1);
                ballForceSliderParent.sizeDelta = Vector2.zero;
                ballForceSliderParent.anchoredPosition = Vector2.zero;
            }
            return ballForceSliderParent;
        }
    }

    private Slider ballForceSlider = null;
    private Slider BallForceSlider
    {
        get
        {
            if (ballForceSlider == null)
            {
                Slider tempBallForceBar = new GameObject("EnemyHPBar").AddComponent<Slider>();
                tempBallForceBar.wholeNumbers = false;
                tempBallForceBar.maxValue = 100;
                //TODO : 슬라이드바 UI 추가시 여기에 작업
                RectTransform tempHpTR = tempBallForceBar.transform as RectTransform;


                RectTransform tempHandleArea = new GameObject("HandleArea").AddComponent<RectTransform>();
                tempHandleArea.SetParent(tempHpTR);
                tempHandleArea.anchorMin = Vector2.zero; // 부모의 왼쪽 하단을 기준으로
                tempHandleArea.anchorMax = Vector2.one; // 부모의 왼쪽 상단을 기준으로
                tempHandleArea.pivot = new Vector2(0.5f, 0.5f); // 부모의 왼쪽 중간을 기준으로
                tempHandleArea.sizeDelta = Vector2.zero;
                tempHandleArea.anchoredPosition = Vector2.zero;
                Image handle = new GameObject("handle").AddComponent<Image>();
                handle.rectTransform.SetParent(tempHandleArea);
                handle.rectTransform.anchorMax = Vector2.up;
                handle.rectTransform.anchorMin = Vector2.zero;
                handle.rectTransform.pivot = Vector2.one / 2f;
                handle.rectTransform.sizeDelta = Vector2.right * 50;
                tempBallForceBar.handleRect = handle.rectTransform;
                handle.color = Color.white;
                handle.sprite = Managers.instance.Resource.Load<Sprite>("HP_icon");


                Image BackGround = new GameObject("BackGround").AddComponent<Image>();
                BackGround.rectTransform.SetParent(tempHpTR);
                BackGround.rectTransform.anchorMax = new Vector2(1f, 0.75f);
                BackGround.rectTransform.anchorMin = new Vector2(0f, 0.25f);
                BackGround.rectTransform.sizeDelta = Vector2.zero;
                BackGround.rectTransform.pivot = Vector2.one / 2f;
                BackGround.color = Color.grey;
                BackGround.sprite = Managers.instance.Resource.Load<Sprite>("HPbar");
                BackGround.rectTransform.SetAsFirstSibling();


                RectTransform tempFillArea = new GameObject("FillArea").AddComponent<RectTransform>();
                tempFillArea.SetParent(tempHpTR);
                tempFillArea.anchorMin = new Vector2(0f, 0.25f); // 부모의 왼쪽 하단을 기준으로
                tempFillArea.anchorMax = new Vector2(1f, 0.75f); // 부모의 왼쪽 상단을 기준으로
                tempFillArea.pivot = new Vector2(0.5f, 0.5f); // 부모의 왼쪽 중간을 기준으로
                tempFillArea.sizeDelta = Vector2.zero;
                tempFillArea.anchoredPosition = Vector2.zero;
                Image tempIMG = new GameObject("FillRect").AddComponent<Image>();
                tempIMG.rectTransform.SetParent(tempFillArea);
                tempIMG.rectTransform.sizeDelta = Vector2.zero;
                tempIMG.color = Color.white;
                tempIMG.sprite = Managers.instance.Resource.Load<Sprite>("HPbar");
                tempBallForceBar.fillRect = tempIMG.rectTransform;

                // Slider의 부모-자식 관계 설정  



                tempHpTR.SetParent(BallForceSliderParent);
                tempHpTR.SetAsLastSibling();
                tempHpTR.anchorMin = new Vector2(0.45f, 0.48f);
                tempHpTR.anchorMax = new Vector2(0.55f, 0.52f);
                tempHpTR.sizeDelta = Vector2.zero;
                tempHpTR.anchoredPosition = Vector2.zero;
                tempBallForceBar.interactable = false;
                tempBallForceBar.enabled = true;
                tempBallForceBar.SetDirection(Slider.Direction.RightToLeft, true);
                ballForceSlider = tempBallForceBar;
                tempHandleArea.SetAsLastSibling();
                Debug.Log("포스바 세팅 끝");
            }
            return ballForceSlider;
        }
    }
    #region 관련 함수
    public void SettingPlayerBattleUI()
    {
        //TODO : 매니저의 인스턴스 선언 후 해당구문 수정필요,아래의 Enabled는 테스트코드로 해당 작업 시 삭제필요
        //PlayerHPBar.value = ;
        //PlayerHPBar.maxValue = ;
        //PlayerPortrait.sprite = ;
        //WeaponImage =
        GirlText.enabled = true;
        GirlPortrait.enabled = true;
        PlayerHPBar.enabled = true;
        WeaponImage.enabled = true;
        PlayerPortrait.enabled = true;
        PlayerWeaponName.text = "무기이름";
        WeaponBeforeBTN.onClick.AddListener(Managers.instance.PlayerDataManager.PlayerBeforeBallPick);
        WeaponNextBTN.onClick.AddListener(Managers.instance.PlayerDataManager.PlayerNextBallPick);
        UpdateScore(0,0);
        EnemyUISetting();
    }
    public void WeaponAnim(bool isMoveRight,string ballName,string ballKRName,string PrevBallName)
    {
        WeaponImagePrev.rectTransform.DOComplete();
        WeaponImage.rectTransform.DOComplete();
        WeaponImage.sprite = Managers.instance.Resource.Load<Sprite>(ballName);
        WeaponImagePrev.sprite = Managers.instance.Resource.Load<Sprite>(PrevBallName);
        if (!isMoveRight)
        {
            WeaponImage.rectTransform.anchorMin = Vector2.left;
            WeaponImage.rectTransform.anchorMax = Vector2.up;
            WeaponImage.rectTransform.anchoredPosition = Vector2.zero;
            WeaponImage.rectTransform.sizeDelta = Vector2.zero;

            WeaponImagePrev.rectTransform.anchorMin = Vector2.zero;
            WeaponImagePrev.rectTransform.anchorMax = Vector2.one;
            WeaponImagePrev.rectTransform.anchoredPosition = Vector2.zero;
            WeaponImagePrev.rectTransform.sizeDelta = Vector2.zero;

            WeaponImage.rectTransform.DOAnchorMax(Vector2.one, 0.7f);
            WeaponImage.rectTransform.DOAnchorMin(Vector2.zero, 0.7f);
            WeaponImagePrev.rectTransform.DOAnchorMax(Vector2.one + Vector2.right, 0.7f);
            WeaponImagePrev.rectTransform.DOAnchorMin(Vector2.right, 0.7f);
        }
        else
        {
            WeaponImagePrev.rectTransform.anchorMin = Vector2.zero;
            WeaponImagePrev.rectTransform.anchorMax = Vector2.one;
            WeaponImagePrev.rectTransform.anchoredPosition = Vector2.zero;
            WeaponImagePrev.rectTransform.sizeDelta = Vector2.zero;

            WeaponImage.rectTransform.anchorMin = Vector2.right;
            WeaponImage.rectTransform.anchorMax = Vector2.one + Vector2.right;
            WeaponImage.rectTransform.anchoredPosition = Vector2.zero;
            WeaponImage.rectTransform.sizeDelta = Vector2.zero;

            WeaponImage.rectTransform.DOAnchorMax(Vector2.one, 0.7f);
            WeaponImage.rectTransform.DOAnchorMin(Vector2.zero, 0.7f);
            WeaponImagePrev.rectTransform.DOAnchorMax(Vector2.up, 0.7f);
            WeaponImagePrev.rectTransform.DOAnchorMin(Vector2.left, 0.7f);
        }
        ChangeWeaponUI(true, ballName, ballKRName);
    }
    public void HPBarUpdate(bool isPlayer,float maxHP,float nowHP)
    {
        if (isPlayer)
        {
            PlayerHPBar.maxValue = maxHP;
            PlayerHPBar.value = nowHP;
        }
        else
        {
            EnemyHPBar.maxValue = maxHP;
            EnemyHPBar.value = nowHP;
        }
    }
    public void ChangePortrait(bool isPlayer,string spriteName)
    {
        if (isPlayer)
        {
            PlayerPortrait.sprite = Managers.instance.Resource.Load<Sprite>(spriteName);
        }
        else
        {
            EnemyPortrait.sprite = Managers.instance.Resource.Load<Sprite>(spriteName);
        }
    }
    public void ChangeWeaponUI(bool isPlayer,string spriteName,string weaponKoreanName)
    {
        if (isPlayer)
        {
            WeaponImage.sprite = Managers.instance.Resource.Load<Sprite>(spriteName);
            PlayerWeaponName.text = weaponKoreanName;
        }
        else
        {
            EnemyWeaponImage.sprite = Managers.instance.Resource.Load<Sprite>(spriteName);
            EnemyWeaponName.text = weaponKoreanName;
        }
    }

    public void WeaponButtonCheck(bool isZero)
    {
        WeaponNextBTN.interactable = !isZero;
        weaponBeforeBTN.interactable = !isZero;
    }

    public void UpdateScore(int PlayerScore,int EnemyScore)
    {
        ScoreBoardText.text = PlayerScore + " : " + EnemyScore;
    }
    public void SetBallSliderPos(Vector3 ShooterPosition,bool isSetActive)
    {
        if (isSetActive)
        {
            Vector3 tempVec = Camera.main.WorldToScreenPoint(ShooterPosition-Vector3.up);
            BallForceSliderParent.position = tempVec;
        }
        BallForceSliderParent.gameObject.SetActive(isSetActive);
        
    }
    public void UpdateBallForce(float max ,float value)
    {
        if (BallForceSlider.maxValue != max)
        {
            BallForceSlider.maxValue = max;
        }
        ballForceSlider.value = value;
    }


    private void EnemyUISetting()
    {
        EnemyHPBar.enabled = true;
        EnemyWeaponImage.enabled = true;
        EnemyPortrait.enabled = true;
        EnemyWeaponImage.enabled = true;
        EnemyWeaponNamePannel.enabled = true;
        EnemyWeaponName.text = "무기이름";
    }
    #endregion
}
public class ShopUI
{
    public bool IsShopActivate
    {
        get { return ShopPanel.gameObject.activeSelf; }
        set 
        { 
            ShopPanel.gameObject.SetActive(value);
        }
    }
    #region 변수
    private Image shopPanel;
    public Image ShopPanel
    {
        get 
        { 
            if (shopPanel == null)
            {
                shopPanel = new GameObject("ShopPanel").AddComponent<Image>();
                shopPanel.rectTransform.SetParent(Managers.instance.UI.LoadingUIProps.SceneMainCanvas);
                shopPanel.rectTransform.anchoredPosition = Vector3.zero;
                shopPanel.rectTransform.anchorMax = Vector2.one;
                shopPanel.rectTransform.anchorMin = Vector2.zero;
                shopPanel.rectTransform.sizeDelta = Vector2.zero;
                shopPanel.sprite = Managers.instance.Resource.Load<Sprite>("");
                shopPanel.rectTransform.SetAsLastSibling();
                //TODO : ShopPanel이 업로드시 해당 키값 작성 요망
            }
            return shopPanel;
        }
    }
    private Image shopInnerPanel;
    public Image ShopInnerShopPanel
    {
        get
        {
            if (shopInnerPanel == null)
            {
                shopInnerPanel = new GameObject("ShopInnerPanel").AddComponent<Image>();
                shopInnerPanel.rectTransform.SetParent(ShopPanel.rectTransform);
                shopInnerPanel.rectTransform.anchoredPosition = Vector3.zero;
                shopInnerPanel.rectTransform.anchorMax = new Vector2(0.99f, 0.98f);
                shopInnerPanel.rectTransform.anchorMin = new Vector2(0.01f, 0.02f);
                shopInnerPanel.rectTransform.sizeDelta = Vector2.zero;
                shopInnerPanel.sprite = Managers.instance.Resource.Load<Sprite>("");
                //TODO : ShopPanel이 업로드시 해당 키값 작성 요망
            }
            return shopInnerPanel;
        }
    }
    private Image shopPlayerStatusWindowPanel;
    public Image ShopPlayerStatusWindowPanel
    {
        get
        {
            if (shopPlayerStatusWindowPanel == null)
            {
                shopPlayerStatusWindowPanel = new GameObject("ShopPlayerPanel").AddComponent<Image>();
                shopPlayerStatusWindowPanel.rectTransform.SetParent(ShopInnerShopPanel.rectTransform);
                shopPlayerStatusWindowPanel.rectTransform.anchoredPosition = Vector3.zero;
                shopPlayerStatusWindowPanel.rectTransform.anchorMax = new Vector2(0.322f, 1f);
                shopPlayerStatusWindowPanel.rectTransform.anchorMin = Vector2.zero;
                shopPlayerStatusWindowPanel.rectTransform.sizeDelta = Vector2.zero;
                shopPlayerStatusWindowPanel.sprite = Managers.instance.Resource.Load<Sprite>("");
                //TODO : ShopPanel이 업로드시 해당 키값 작성 요망
            }
            return shopPlayerStatusWindowPanel;
        }
    }

    private Image merchantPortrait;
    public Image MerchantPortrait
    {
        get
        {
            if (merchantPortrait == null)
            {
                merchantPortrait = new GameObject("MerchantPortrait").AddComponent<Image>();
                merchantPortrait.rectTransform.SetParent(ShopPlayerStatusWindowPanel.rectTransform);
                merchantPortrait.rectTransform.anchoredPosition = Vector3.zero;
                merchantPortrait.rectTransform.anchorMax = new Vector2(0.505f, 0.981f);
                merchantPortrait.rectTransform.anchorMin = new Vector2(0.019f, 0.649f); 
                merchantPortrait.rectTransform.sizeDelta = Vector2.zero;
                merchantPortrait.color = Color.red;
                merchantPortrait.sprite = Managers.instance.Resource.Load<Sprite>("");
                //TODO : ShopPanel이 업로드시 해당 키값 작성 요망
            }
            return merchantPortrait;
        }
    }
    private Image merchantDialogPanel;
    public Image MerchantDialogPanel
    {
        get
        {
            if (merchantDialogPanel == null)
            {
                merchantDialogPanel = new GameObject("merchantDialogPanel").AddComponent<Image>();
                merchantDialogPanel.rectTransform.SetParent(ShopPlayerStatusWindowPanel.rectTransform);
                merchantDialogPanel.rectTransform.anchoredPosition = Vector3.zero;
                merchantDialogPanel.rectTransform.anchorMax = new Vector2(0.971f, 0.971f);
                merchantDialogPanel.rectTransform.anchorMin = new Vector2(0.511f, 0.675f);
                merchantDialogPanel.rectTransform.sizeDelta = Vector2.zero;
                merchantDialogPanel.color = Color.red;
                merchantDialogPanel.sprite = Managers.instance.Resource.Load<Sprite>("");
                //TODO : ShopPanel이 업로드시 해당 키값 작성 요망
            }
            return merchantDialogPanel;
        }
    }
    private Image playerBagPanel;
    public Image PlayerBagPanel
    {
        get
        {
            if (playerBagPanel == null)
            {
                playerBagPanel = new GameObject("PlayerBagPanel").AddComponent<Image>();
                playerBagPanel.rectTransform.SetParent(ShopPlayerStatusWindowPanel.rectTransform);
                playerBagPanel.rectTransform.anchoredPosition = Vector3.zero;
                playerBagPanel.rectTransform.anchorMax = new Vector2(0.971f, 0.624f);
                playerBagPanel.rectTransform.anchorMin = new Vector2(0.019f, 0.0252f);
                playerBagPanel.rectTransform.sizeDelta = Vector2.zero;
                playerBagPanel.color = Color.red;
                playerBagPanel.sprite = Managers.instance.Resource.Load<Sprite>("");
                //TODO : ShopPanel이 업로드시 해당 키값 작성 요망
            }
            return playerBagPanel;
        }
    }
    private Image playerMoneyPanel;
    public Image PlayerMoneyPanel
    {
        get
        {
            if (playerMoneyPanel == null)
            {
                playerMoneyPanel = new GameObject("PlayerMoneyPanel").AddComponent<Image>();
                playerMoneyPanel.rectTransform.SetParent(PlayerBagPanel.rectTransform);
                playerMoneyPanel.rectTransform.anchoredPosition = Vector3.zero;
                playerMoneyPanel.rectTransform.anchorMax = Vector2.one;
                playerMoneyPanel.rectTransform.anchorMin = new Vector2(0,0.5f);
                playerMoneyPanel.rectTransform.sizeDelta = Vector2.zero;
                playerMoneyPanel.color = Color.red;
                playerMoneyPanel.sprite = Managers.instance.Resource.Load<Sprite>("");
                //TODO : ShopPanel이 업로드시 해당 키값 작성 요망
            }
            return playerMoneyPanel;
        }
    }
    private Image playerInventoryPanel;
    public Image PlayerInventoryPanel
    {
        get
        {
            if (playerInventoryPanel == null)
            {
                playerInventoryPanel = new GameObject("PlayerInventoryPanel").AddComponent<Image>();
                ScrollRect tempScrollRect = playerInventoryPanel.AddComponent<ScrollRect>();
                playerInventoryPanel.rectTransform.SetParent(PlayerBagPanel.rectTransform);
                playerInventoryPanel.rectTransform.anchoredPosition = Vector3.zero;
                playerInventoryPanel.rectTransform.anchorMax = new Vector2(1, 0.5f);
                playerInventoryPanel.rectTransform.anchorMin = new Vector2(0,0);
                playerInventoryPanel.rectTransform.sizeDelta = Vector2.zero;
                playerInventoryPanel.color = Color.red;
                playerInventoryPanel.sprite = Managers.instance.Resource.Load<Sprite>("");


                // Scroll Rect에 Content 연결
                RectTransform ViewPortTR = new GameObject("ViewPort").AddComponent<RectTransform>();
                ViewPortTR.AddComponent<Image>();
                Mask tempMask  = ViewPortTR.AddComponent<Mask>();

                ViewPortTR.SetParent(tempScrollRect.transform, false);
                ViewPortTR.anchorMax = Vector2.one;
                ViewPortTR.anchorMin = Vector2.zero;
                ViewPortTR.sizeDelta = Vector2.zero;
                tempScrollRect.viewport = ViewPortTR;
                Image BackGroundIMG = new GameObject("BackGround").AddComponent<Image>();
                BackGroundIMG.rectTransform.SetParent(ViewPortTR);
                BackGroundIMG.rectTransform.SetAsFirstSibling();
                BackGroundIMG.rectTransform.anchorMax = Vector2.one;
                BackGroundIMG.rectTransform.anchorMin = Vector2.zero;
                BackGroundIMG.rectTransform.sizeDelta = Vector2.zero;
                BackGroundIMG.rectTransform.anchoredPosition = Vector2.zero;
                BackGroundIMG.sprite = Managers.instance.Resource.Load<Sprite>("");

                Image ContentImage = new GameObject("Content").AddComponent<Image>();
                

                ContentImage.rectTransform.anchorMax = Vector2.one;
                ContentImage.rectTransform.anchorMin = Vector2.zero;
                ContentImage.rectTransform.sizeDelta = Vector2.zero;
                ContentImage.transform.SetParent(ViewPortTR, false);
                tempScrollRect.content = ContentImage.rectTransform;
                GridLayoutGroup tempGroup = ContentImage.AddComponent<GridLayoutGroup>();
                tempGroup.startAxis = GridLayoutGroup.Axis.Horizontal;
                tempGroup.startCorner = GridLayoutGroup.Corner.UpperLeft;
                tempGroup.childAlignment = TextAnchor.MiddleLeft;
                tempGroup.constraint = GridLayoutGroup.Constraint.FixedRowCount;
                tempGroup.constraintCount = 2;
                float resolutionPercentX = Screen.currentResolution.width/1920;
                float resolutionPercentY = Screen.currentResolution.height/1080;

                tempGroup.cellSize = new Vector2(resolutionPercentX*90, resolutionPercentY*90);
                tempGroup.spacing = new Vector2(tempGroup.cellSize.x*2, ContentImage.rectTransform.rect.size.y/20);
                invenGrid = tempGroup;

                // Scrollbar를 추가하고 연결 (옵션)
                Scrollbar tempScrollbar = new GameObject("Scrollbar_Horizontal").AddComponent<Scrollbar>();
                RectTransform ScrollbarTransform = tempScrollbar.transform as RectTransform;
                tempScrollbar.AddComponent<Image>();

                tempScrollbar.transform.SetParent(tempScrollRect.transform, false);
                ScrollbarTransform.anchorMax = new Vector2(1,0.1f);
                ScrollbarTransform.anchorMin = Vector2.zero;
                ScrollbarTransform.sizeDelta = Vector2.zero;
                tempScrollRect.horizontal = true;
                tempScrollRect.horizontalScrollbar = tempScrollbar;


                Image ScrollHandle = new GameObject("ScrollBarHandle").AddComponent<Image>();
                ScrollHandle.color = Color.red;
                tempScrollRect.horizontalScrollbar.handleRect = ScrollHandle.rectTransform;
                ScrollHandle.rectTransform.SetParent(ScrollbarTransform);
                ScrollHandle.rectTransform.anchorMax = Vector2.one;
                ScrollHandle.rectTransform.anchorMin = Vector2.zero;
                ScrollHandle.rectTransform.sizeDelta = Vector2.zero;
                ScrollHandle.rectTransform.anchoredPosition = Vector2.zero;
                ContentImage.enabled = false;
                tempScrollRect.vertical = false;
                tempScrollRect.enabled = false;
                tempScrollRect.enabled = true;


                // UI 텍스트를 Content에 추가 (예시)
                playerInventoryPanel = ContentImage;
                //TODO : ShopPanel이 업로드시 해당 키값 작성 요망
            }
            return playerInventoryPanel;
        }
    }
    private GridLayoutGroup invenGrid;


    private Image playerMoneyIMG;
    public Image PlayerMoneyIMG
    {
        get
        {
            if (playerMoneyIMG == null)
            {
                playerMoneyIMG = new GameObject("PlayerMoneyIMG").AddComponent<Image>();
                playerMoneyIMG.rectTransform.SetParent(PlayerMoneyPanel.rectTransform);
                playerMoneyIMG.rectTransform.anchoredPosition = Vector3.zero;
                playerMoneyIMG.rectTransform.anchorMax = new Vector2(0.298f, 0.65f);
                playerMoneyIMG.rectTransform.anchorMin = new Vector2(0.055f, 0.225f);
                playerMoneyIMG.rectTransform.sizeDelta = Vector2.zero;
                playerMoneyIMG.color = Color.yellow;
                playerMoneyIMG.sprite = Managers.instance.Resource.Load<Sprite>("");
                //TODO : ShopPanel이 업로드시 해당 키값 작성 요망
            }
            return playerMoneyIMG;
        }
    }
    private Text goldAmountText;
    public Text GoldAmountText
    {
        get
        {
            if (goldAmountText == null)
            {
                goldAmountText = new GameObject { name = "GoldAmountText" }.AddComponent<Text>();
                goldAmountText.rectTransform.SetParent(PlayerMoneyPanel.rectTransform);
                goldAmountText.rectTransform.anchorMin = new Vector2(0.400000006f, 0.224999994f);
                goldAmountText.rectTransform.anchorMax = Vector2.one;
                goldAmountText.rectTransform.sizeDelta = Vector2.zero;
                goldAmountText.rectTransform.anchoredPosition = Vector2.zero;
                goldAmountText.fontSize = 70;
                goldAmountText.color = Color.black;
                goldAmountText.font = Managers.instance.Resource.Load<Font>("InGameFont");
                goldAmountText.alignment = TextAnchor.LowerLeft;
                goldAmountText.horizontalOverflow = HorizontalWrapMode.Wrap;

            }
            return goldAmountText;
        }
    }
    private List<(Image, Text)> Inventory = new List<(Image, Text)>();
    #endregion
    public void CreateBulbIcons(List<BallStat> ballList)
    {

        for (int i = 0; i < ballList.Count; i++)
        {
            if (Inventory.Count<=i)
            {
                Inventory.Add((null,null));
            }
            if (Inventory[i].Item1 == null|| Inventory[i].Item2 == null)
            {
                Inventory[i] = (new GameObject(ballList[i].ballName+"ShopIcon").AddComponent<Image>(),new GameObject().AddComponent<Text>());
                Inventory[i].Item1.rectTransform.SetParent(PlayerInventoryPanel.rectTransform);
                Inventory[i].Item2.rectTransform.SetParent(Inventory[i].Item1.rectTransform);

                Inventory[i].Item2.rectTransform.anchorMax = new Vector2(3,1);
                Inventory[i].Item2.rectTransform.anchorMin = Vector2.right;
                Inventory[i].Item2.rectTransform.anchoredPosition = Vector2.zero;
                Inventory[i].Item2.rectTransform.sizeDelta = Vector2.zero;
                Inventory[i].Item1.sprite = Managers.instance.Resource.Load<Sprite>(ballList[i].ballName);
                Inventory[i].Item2.font = Managers.instance.Resource.Load<Font>("InGameFont");
                Inventory[i].Item2.fontSize = 30;
                Inventory[i].Item2.color = Color.black;
                Inventory[i].Item2.text = ballList[i].ballKoreanName+"이거 갯수로 바꿔야함";
                if (PlayerInventoryPanel.rectTransform.rect.size.x < (invenGrid.cellSize.x+invenGrid.spacing.x)*((Inventory.Count+1)/2))
                {
                    PlayerInventoryPanel.rectTransform.sizeDelta += (invenGrid.cellSize.x + invenGrid.spacing.x) * Vector2.right;
                }
            }
            else if(Inventory[i].Item1.sprite.name != ballList[i].ballName)
            {
                Inventory[i].Item1.sprite = Managers.instance.Resource.Load<Sprite>(ballList[i].ballName);
                Inventory[i].Item2.text = ballList[i].ballKoreanName + "이거 갯수로 바꿔야함";
                Debug.Log("공이름과 이미지 이름 불일치");
            }
            if (Inventory[i].Item2.text != ballList[i].ballKoreanName + "이거 갯수로 바꿔야함")
            {
                Inventory[i].Item2.text = ballList[i].ballKoreanName + "이거 갯수로 바꿔야함";
            }
        }
        shopPanel.gameObject.SetActive(IsShopActivate);
    }
    public void MoneyUpdate(int money)
    {
        GoldAmountText.text = money.ToString();
        shopPanel.gameObject.SetActive(IsShopActivate);
    }
    public void ShopUISetting()
    {
        MerchantPortrait.enabled = true;
        MerchantDialogPanel.enabled = true;
        PlayerInventoryPanel.enabled = true;
        PlayerMoneyIMG.enabled = true;
        MoneyUpdate(Managers.instance.PlayerDataManager.PlayerMoney);
        ShopPanel.gameObject.SetActive(false);
        CreateBulbIcons(Managers.instance.PlayerDataManager.playerOwnBalls);
    }
}