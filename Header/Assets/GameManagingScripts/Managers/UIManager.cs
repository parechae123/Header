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
    public LoadingUI LoadingUIProps = new LoadingUI();
    public TopViewSceneUI TopViewSceneUIs = new TopViewSceneUI();
    public DialogSystem DialogCall = new DialogSystem();
    public BattleUI BattleUICall = new BattleUI();

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
                handle.rectTransform.SetAsLastSibling();

                Image BackGround = new GameObject("BackGround").AddComponent<Image>();
                BackGround.color = new Color(0.9137256f, 0.3333333f, 0.3019608f, 1);
                BackGround.rectTransform.SetParent(tempHpTR);
                BackGround.rectTransform.anchorMax = new Vector2(1f, 0.75f);
                BackGround.rectTransform.anchorMin = new Vector2(0f, 0.25f);
                BackGround.rectTransform.sizeDelta = Vector2.zero;
                BackGround.rectTransform.pivot = Vector2.one / 2f;
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
                tempPlayerHPbar.fillRect = tempIMG.rectTransform;

                // Slider의 부모-자식 관계 설정  



                tempHpTR.SetParent(PlayerStatusUI.rectTransform);
                tempHpTR.SetAsLastSibling();
                tempHpTR.anchorMin = new Vector2(0.1f, 0.88f);
                tempHpTR.anchorMax = new Vector2(0.9f, 0.93f);
                tempHpTR.sizeDelta = Vector2.zero;
                tempHpTR.anchoredPosition = Vector2.zero;
                tempPlayerHPbar.interactable = false;
                tempPlayerHPbar.enabled = false;
                tempPlayerHPbar.enabled = true;
                playerHpBar = tempPlayerHPbar;
                Debug.Log("체력바 세팅 끝");
            }
            return playerHpBar;
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
                weaponImage.rectTransform.SetParent(PlayerStatusUI.rectTransform);
                weaponImage.sprite = Managers.instance.Resource.Load<Sprite>("Bullet_Basic");
                weaponImage.rectTransform.anchorMin = new Vector2(0.330525041f, 0.483485878f);
                weaponImage.rectTransform.anchorMax = new Vector2(0.669326663f, 0.582539618f);
                weaponImage.rectTransform.sizeDelta = Vector2.zero;
                weaponImage.rectTransform.anchoredPosition = Vector2.zero;
            }
            return weaponImage;
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
                handle.color = Color.white;
                handle.rectTransform.SetParent(tempHandleArea);
                handle.rectTransform.anchorMax = Vector2.up;
                handle.rectTransform.anchorMin = Vector2.zero;
                handle.rectTransform.pivot = Vector2.one / 2f;
                handle.rectTransform.sizeDelta = Vector2.right * 50;
                tempEnemyHPbar.handleRect = handle.rectTransform;
                handle.rectTransform.SetAsLastSibling();

                Image BackGround = new GameObject("BackGround").AddComponent<Image>();
                BackGround.color = new Color(0.9137256f, 0.3333333f, 0.3019608f, 1);
                BackGround.rectTransform.SetParent(tempHpTR);
                BackGround.rectTransform.anchorMax = new Vector2(1f, 0.75f);
                BackGround.rectTransform.anchorMin = new Vector2(0f, 0.25f);
                BackGround.rectTransform.sizeDelta = Vector2.zero;
                BackGround.rectTransform.pivot = Vector2.one / 2f;
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
                tempEnemyHPbar.fillRect = tempIMG.rectTransform;

                // Slider의 부모-자식 관계 설정  



                tempHpTR.SetParent(EnemyStatusUI.rectTransform);
                tempHpTR.SetAsLastSibling();
                tempHpTR.anchorMin = new Vector2(0.1f, 0.88f);
                tempHpTR.anchorMax = new Vector2(0.9f, 0.93f);
                tempHpTR.sizeDelta = Vector2.zero;
                tempHpTR.anchoredPosition = Vector2.zero;
                tempEnemyHPbar.interactable = false;
                tempEnemyHPbar.enabled = false;
                tempEnemyHPbar.enabled = true;
                enemyHpBar = tempEnemyHPbar;
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
    #region 관련 함수
    public void SettingPlayerBattleUI()
    {
        //TODO : 매니저의 인스턴스 선언 후 해당구문 수정필요,아래의 Enabled는 테스트코드로 해당 작업 시 삭제필요
        //PlayerHPBar.value = ;
        //PlayerHPBar.maxValue = ;
        //PlayerPortrait.sprite = ;
        //WeaponImage =
        PlayerHPBar.enabled = true;
        WeaponImage.enabled = true;
        PlayerPortrait.enabled = true;
        PlayerWeaponName.text = "무기이름";
        WeaponBeforeBTN.onClick.AddListener(Managers.instance.PlayerDataManager.PlayerBeforeBallPick);
        WeaponNextBTN.onClick.AddListener(Managers.instance.PlayerDataManager.PlayerNextBallPick);
        UpdateScore(0,0);
        EnemyUISetting();
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
    public void UpdateScore(int PlayerScore,int EnemyScore)
    {
        ScoreBoardText.text = PlayerScore + " : " + EnemyScore;
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