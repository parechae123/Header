using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using DataDefines;
using InteractionDefines;
using DG.Tweening;
using UnityEngine.Video;
using HeaderPadDefines;
using System.Threading.Tasks;
using MonsterDefines;
using UnityEngine.EventSystems;
using System.Net.Http;


public class UIManager
{
    public LoadingUI LoadingUIProps = new LoadingUI();
    public TopViewSceneUI TopViewSceneUIs = new TopViewSceneUI();
    public DialogSystem DialogCall = new DialogSystem();
    public BattleUI BattleUICall = new BattleUI();
    public ShopUI ShopUICall = new ShopUI();
    public OptionUI OptionUICall = new OptionUI();

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
            if (TopViewPlayer.Instance != null)
            {
                TopViewPlayer.Instance.isMoveAble = MoveAbleChecker();
            }
        }
        else
        {
            TargetUIOnOff(OptionUICall.OptionPannel.rectTransform, true);
        }
    }
    public void CheckerRegist(Transform tr)
    {
        MoveAbleCheckerList.Add(tr);
    }
    public void ResetUI()
    {
        UIStack.Clear();
        ShopUICall.shopWeaponItems = null;
        MoveAbleCheckerList.Clear();
        //        BattleUICall.ResetMonsterQueueIMG();
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
    public void TargetUIOnOff(Transform target, bool isTurnOn, bool isPush = true)
    {
        // TODO : 특정 UI 닫기버튼 누를때 연결해주어야 할 함수 끌때 isTurnOn을 false 열때는 true
        target.gameObject.SetActive(isTurnOn);
        if (isPush)
        {
            if (isTurnOn)
            {
                RegistUIStack(target);
            }
            else
            {
                UIStack.TryPop(out target);
            }
            if (TopViewPlayer.Instance != null)
            {
                TopViewPlayer.Instance.isMoveAble = MoveAbleChecker();
            }
        }
    }

    public void RegistEventTrigger(RectTransform target)
    {
        EventTrigger tempET = target.GetOrAddComponent<EventTrigger>();
        tempET.TryGetComponent<Button>(out Button TargetBTN);
        Button tempBTN = TargetBTN;

        var pointerClick = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerClick
        };
        var pointerHover = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerEnter
        };
        if (tempBTN != null)
        {
            pointerClick.callback.AddListener((data) =>
            {
                if (tempBTN.interactable)
                {
                    Managers.instance.UI.OnMouseHover();
                }

            });
            pointerHover.callback.AddListener((data) =>
            {
                if (tempBTN.interactable)
                {
                    Managers.instance.UI.OnMouseClick();
                }
            });
        }
        else
        {
            pointerClick.callback.AddListener((data) =>
            {
                Managers.instance.UI.OnMouseHover();

            });
            pointerHover.callback.AddListener((data) =>
            {
                Managers.instance.UI.OnMouseHover();
            });
        }


        tempET.triggers.Add(pointerClick);
        tempET.triggers.Add(pointerHover);
    }

    public void OnMouseHover()
    {
        Managers.instance.SoundManager.SFXPlayOneshot(Managers.instance.Resource.Load<AudioClip>("ButtonMouseOver"), false, true);
    }
    public void OnMouseClick()
    {
        Managers.instance.SoundManager.SFXPlayOneshot(Managers.instance.Resource.Load<AudioClip>("ButtonMouseClick"), false, true);
    }
    public void SetUISize(ref RectTransform TargetRect, Vector2 min, Vector2 max)
    {
        TargetRect.anchorMin = min;
        TargetRect.anchorMax = max;
        TargetRect.sizeDelta = Vector2.zero;
        TargetRect.anchoredPosition = Vector2.zero;
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
                tempTR.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
                
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
                Image UIBackGround = new GameObject { name = "fullDialogPanel" }.AddComponent<Image>();
                fullDialogPanel = UIBackGround.rectTransform;
                fullDialogPanel.SetParent(DialogueBackGround.rectTransform);
                UIBackGround.sprite = Managers.instance.Resource.Load<Sprite>("dialogue_panel");
                //UIBackGround.sprite = 변경할 에셋 이름;
                //TODO : 키 인터렉션 안내판넬 받으면 UIBackGround 변수의 sprite 변경해주어야함
                Vector2 spriteSize = UIBackGround.sprite.rect.size*2;
                float percent = DialogueBackGround.rectTransform.rect.size.x / DialogueBackGround.rectTransform.rect.size.y;
                for (int i = 0; i < ((int)UIBackGround.sprite.rect.size.x).ToString().Length; i++)
                {
                    spriteSize = spriteSize * 0.1f;
                }
                spriteSize.y = spriteSize.y*percent;
                Vector2 center = new Vector2(0.5f,spriteSize.y);
                Managers.instance.UI.SetUISize(ref fullDialogPanel,center-spriteSize, center + spriteSize);
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
                dialogPanel.anchorMin = new Vector2(NamePanel.anchorMax.x, 0);
                dialogPanel.anchorMax = new Vector2(1f- NamePanel.anchorMax.x, 1f);
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
                Vector2 tempVec = UIBackGround.sprite.bounds.size;
                //사이즈 불일치로 보정값 추가, size이기에 원래는 2f여야함
                tempVec = tempVec / 4f;
                for (int i = 0; i < ((int)UIBackGround.sprite.bounds.size.x).ToString().Length; i++)
                {
                    tempVec = tempVec * 0.1f;
                }
                float parentSIzePercent = FullDialogPanel.rect.size.x/FullDialogPanel.rect.size.y;
                tempVec.y = tempVec.y * parentSIzePercent;
                Vector2 centerPos = Vector2.zero;
                centerPos.x = tempVec.x + 0.01f;
                centerPos.y = 0.14827601f;
                Managers.instance.UI.SetUISize(ref namePanel,centerPos-tempVec, centerPos + tempVec);
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
                nameText.rectTransform.SetParent(NamePanel);
                nameText.rectTransform.anchorMin = Vector2.zero;
                nameText.rectTransform.anchorMax = Vector2.one;
                nameText.rectTransform.sizeDelta = Vector2.zero;
                nameText.rectTransform.anchoredPosition = Vector2.zero;
                nameText.fontSize = 50;
                nameText.resizeTextForBestFit = true;
                nameText.resizeTextMaxSize = 50;

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
                dialogText.rectTransform.SetParent(DialogPanel);
                dialogText.rectTransform.anchorMin = Vector2.zero;
                dialogText.rectTransform.anchorMax = Vector2.one;
                dialogText.rectTransform.sizeDelta = Vector2.zero;
                dialogText.rectTransform.anchoredPosition = Vector2.zero;
                dialogText.fontSize = 40;
                dialogText.resizeTextForBestFit = true;
                DialogText.resizeTextMaxSize=50;
                DialogText.resizeTextMinSize=30;
                dialogText.color = Color.white;
                dialogText.font = Managers.instance.Resource.Load<Font>("InGameFont");
                dialogText.alignment = TextAnchor.MiddleCenter; ;
                dialogText.horizontalOverflow = HorizontalWrapMode.Wrap;
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
    private RawImage untillVideoBackGround;
    public RawImage UntillVideoBackground
    {
        get
        {
            if (untillVideoBackGround == null)
            {
                untillVideoBackGround = new GameObject { name = "DialoueUntilVideoPanel" }.AddComponent<RawImage>();
                untillVideoBackGround.rectTransform.SetParent(Managers.instance.UI.LoadingUIProps.SceneMainCanvas);
                untillVideoBackGround.texture = Managers.instance.Resource.Load<Texture>("BackGroundVideoTexture");
                RectTransform TempRect = untillVideoBackGround.rectTransform;
                Managers.instance.UI.SetUISize(ref TempRect,Vector2.zero,Vector2.one);
                TempRect.SetAsLastSibling();
            }
            return untillVideoBackGround;
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
                dialogueBackGround.rectTransform.SetParent(UntillVideoBackground.rectTransform);
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
            else if (EventNumber != tempDialogArray)
            {
                continue;
            }
        }
        DialogTextChanger();
        Debug.Log(tempData.Count);
    }
    public void DialogNextOnly()
    {
        DataDefines.DialogDatas dialogData = dataQueue.Dequeue();
        Debug.Log(dataQueue.Count);
        if (dialogData.Name.Contains("None"))
        {
            NameText.text = string.Empty;
            DialogCharactorIMG.GameObject().SetActive(false);
            NamePanel.gameObject.SetActive(false);
            if (dialogData.dialogue == string.Empty|| dialogData.dialogue == null)
            {
                FullDialogPanel.gameObject.SetActive(false);
            }
            else
            {
                FullDialogPanel.gameObject.SetActive(true);
            }
        }
        else
        {
            FullDialogPanel.gameObject.SetActive(true);
            DialogCharactorIMG.GameObject().SetActive(true);
            NamePanel.gameObject.SetActive(true);
            NameText.text = dialogData.Name;

            DialogText.text = dialogData.dialogue;
            DialogCharactorIMG.sprite = Managers.instance.Resource.Load<Sprite>(dialogData.Portrait);
            RectTransform tempCharImgRect = DialogCharactorIMG.rectTransform;
            Vector2 charactorIMGSizeCharactorIMG = DialogCharactorIMG.sprite.rect.size / 2f;
            float percent = FullDialogPanel.rect.size.y / FullDialogPanel.rect.size.x;
            int forTimes = ((int)DialogCharactorIMG.sprite.rect.size.y).ToString().Length;
            
            for (int i = 0; i < forTimes; i++)
            {
                charactorIMGSizeCharactorIMG = charactorIMGSizeCharactorIMG * 0.1f;
            }
            charactorIMGSizeCharactorIMG.x = charactorIMGSizeCharactorIMG.x * percent;
            Vector2 charactorIMGCenterPos = new Vector2((NamePanel.anchorMin.x + NamePanel.anchorMax.x) / 2f, NamePanel.anchorMax.y + charactorIMGSizeCharactorIMG.y);
            Managers.instance.UI.SetUISize(ref tempCharImgRect, charactorIMGCenterPos - charactorIMGSizeCharactorIMG, charactorIMGCenterPos + charactorIMGSizeCharactorIMG);
        }
        //TODO : 사운드 출력 함수 구문 추가필요

        VideoClip tempVideo = Managers.instance.Resource.Load<VideoClip>(dialogData.Background);
        if (dialogData.Background !="None")
        {
            DialogueBackGround.color = Color.white;
        }
        else
        {
            DialogueBackGround.color = Color.black;
            DialogueBackGround.texture = null;
        }
        if (dialogData.sound == "None"|| dialogData.sound == string.Empty)
        {
            Managers.instance.SoundManager.SFXLoopCheck = false;
        }
        else
        {
            //TODO : 데이터테이블 추후 한글 제거필요
            if (dialogData.sound.Contains("(반복재생)"))
            {
                string tempSoundName = dialogData.sound.Replace("(반복재생)", string.Empty);
                Managers.instance.SoundManager.SFXLoop(Managers.instance.Resource.Load<AudioClip>(tempSoundName));

            }
            else
            {
                Managers.instance.SoundManager.SFXPlayOneshot(Managers.instance.Resource.Load<AudioClip>(dialogData.sound));

            }
        }

        AudioClip tempAudioClip = Managers.instance.Resource.Load<AudioClip>("dialog_voice_" + dialogData.EventName);
        if(tempAudioClip != null) Managers.instance.SoundManager.PlayVoice(tempAudioClip);

        if (tempVideo != null)
        {

            BackGroundVideo.enabled = true;
            DialogueBackGround.texture = Managers.instance.Resource.Load<Texture>("BackGroundVideoTexture");
            BackGroundVideo.clip = tempVideo;
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
            Managers.instance.UI.TargetUIOnOff(UntillVideoBackground.rectTransform, false, false);
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
            Managers.instance.UI.TargetUIOnOff(UntillVideoBackground.rectTransform, false, false);
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
            Managers.instance.UI.TargetUIOnOff(UntillVideoBackground.rectTransform, false, false);
        }
    }
    #endregion
    public void DialogSetting()
    {
        DialogCharactorIMG.IsActive();
        DialogText.text = "테스트12";
        NameText.text = "테스트12";
        Managers.instance.UI.CheckerRegist(UntillVideoBackground.rectTransform);
        UntillVideoBackground.gameObject.SetActive(false);
    }

}
public class BattleUI
{
    private RectTransform battleSceneUI;
    public RectTransform BattleSceneUI
    {
        get
        {
            if (battleSceneUI == null)
            {
                battleSceneUI = new GameObject("BattleSceneUIFolder").AddComponent<RectTransform>();
                battleSceneUI.SetParent(Managers.instance.UI.LoadingUIProps.SceneMainCanvas);
                battleSceneUI.anchorMin = Vector2.zero;
                battleSceneUI.anchorMax = Vector2.one;
                battleSceneUI.anchoredPosition = Vector2.zero;
                battleSceneUI.sizeDelta = Vector2.zero;
            }
            return battleSceneUI;
        }
    }
    public bool IsInBattleScene
    {
        get
        {
            if (battleSceneUI == null)
            {
                BattleSceneUI.gameObject.SetActive(false);
            }
            return BattleSceneUI.gameObject.activeSelf;
        }
        set
        {

            BattleSceneUI.gameObject.SetActive(value);
        }
    }

    #region 플레이어 관련 변수
    private Image playerStatusUI;
    private Image PlayerStatusUI
    {
        get
        {
            if (playerStatusUI == null)
            {
                playerStatusUI = new GameObject("PlayerStatusPanel").AddComponent<Image>();
                playerStatusUI.rectTransform.SetParent(BattleSceneUI);
                RectTransform tempRectTransform = playerStatusUI.rectTransform;
                Managers.instance.UI.SetUISize(ref tempRectTransform, Vector2.zero, new Vector2(0.169283211f, 0.709656119f));
                playerStatusUI.sprite = Managers.instance.Resource.Load<Sprite>("battle_panel");
                playerStatusUI.raycastTarget = false;
            }
            return playerStatusUI;
        }
    }
    private Image playerPortrait;
    public Image PlayerPortrait
    {
        get
        {
            if (playerPortrait == null)
            {
                playerPortrait = new GameObject("PlayerPortrait").AddComponent<Image>();
                playerPortrait.rectTransform.SetParent(PlayerStatusUI.rectTransform);
                playerPortrait.sprite = Managers.instance.Resource.Load<Sprite>("battle_portrait");
                RectTransform tempRect = playerPortrait.rectTransform;
                float parentSizePercent = PlayerStatusUI.rectTransform.rect.width / PlayerStatusUI.rectTransform.rect.height;
                Vector2 centerPos = new Vector2(0.5f, 0.5009767f);
                int widthNumberSize = ((int)playerPortrait.sprite.rect.width).ToString().Length;
                float getCurrentMag = 1;
                for (int i = 0; i < widthNumberSize; i++)
                {
                    getCurrentMag = getCurrentMag * 0.1f;
                }
                Vector2 halfSize = (new Vector2(playerPortrait.sprite.rect.width, playerPortrait.sprite.rect.height * parentSizePercent) * getCurrentMag) * 1.5f;
                Managers.instance.UI.SetUISize(ref tempRect, centerPos - halfSize, centerPos + halfSize);

            }
            return playerPortrait;
        }
    }
    private Image playerHPBarFrontIMG;
    public Image PlayerHPBarFrontIMG
    {
        get 
        {
            if (playerHPBarFrontIMG == null)
            {

                Image tempBackGround = new GameObject("playerHPBarBackGround").AddComponent<Image>();
                tempBackGround.sprite = Managers.instance.Resource.Load<Sprite>("HPbar_1_empty");
                RectTransform tempBackRect = tempBackGround.rectTransform;
                tempBackRect.SetParent(PlayerStatusUI.rectTransform);
                int imageSizeLength = (int)tempBackGround.sprite.rect.height.ToString().Length;
                float ScaledSize = 1;
                float ScalePercent = PlayerStatusUI.rectTransform.rect.width / PlayerStatusUI.rectTransform.rect.height;
                Vector2 CenterPos = new Vector2(0.5f, 0.80f);
                
                for (int i = 0; i < imageSizeLength; i++)
                {
                    ScaledSize = ScaledSize * 0.1f;
                }
                Vector2 ImgSize = (new Vector2(tempBackGround.sprite.rect.width, tempBackGround.sprite.rect.height*ScalePercent)*ScaledSize)*2;
                Managers.instance.UI.SetUISize(ref tempBackRect, CenterPos-ImgSize,CenterPos+ImgSize);
                playerHPBarFrontIMG = new GameObject("playerHPBarFront").AddComponent<Image>();
                playerHPBarFrontIMG.sprite = Managers.instance.Resource.Load<Sprite>("HPbar_1");
                playerHPBarFrontIMG.type = Image.Type.Filled;
                playerHPBarFrontIMG.fillMethod = Image.FillMethod.Vertical;
                playerHPBarFrontIMG.fillAmount = 1;
                RectTransform HPFrontRT = playerHPBarFrontIMG.rectTransform;
                HPFrontRT.SetParent(tempBackRect);
                Managers.instance.UI.SetUISize(ref HPFrontRT, Vector2.zero, Vector2.one);

            }
            return playerHPBarFrontIMG;
        }
    }

/*    private Slider playerHpBar = null;
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
                handle.sprite = Managers.instance.Resource.Load<Sprite>("HP_icon");
                handle.color = Color.clear;
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
                Managers.instance.UI.SetUISize(ref tempHpTR, new Vector2(0.0956228f, 0.7894221f), new Vector2(0.9043772f, 0.95f));
                tempPlayerHPbar.interactable = false;
                tempPlayerHPbar.enabled = true;
                tempHandleArea.SetAsLastSibling();
                tempPlayerHPbar.SetDirection(Slider.Direction.RightToLeft, true);
                playerHpBar = tempPlayerHPbar;

                Debug.Log("체력바 세팅 끝");
            }
            return playerHpBar;
        }
    }*/
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
                RectTransform tempRect = weaponImagePanel.rectTransform;
                Vector2 centerPos = new Vector2(0.5f, 0.25330975f);
                float percent = PlayerStatusUI.rectTransform.rect.width / PlayerStatusUI.rectTransform.rect.height;
                Vector2 Size = new Vector2(0.6f, 0.6f * percent)/4;
                Managers.instance.UI.SetUISize(ref tempRect, centerPos- Size, centerPos+ Size);
                WeaponImagePanel.AddComponent<RectMask2D>();
                weaponImagePanel.raycastTarget = false;
            }
            return weaponImagePanel;
        }
    }
    private Image weaponImage;
    public Image WeaponImage
    {
        get
        {
            if (weaponImage == null)
            {
                weaponImage = new GameObject("weaponIMG").AddComponent<Image>();
                weaponImage.rectTransform.SetParent(WeaponImagePanel.rectTransform);
                weaponImage.sprite = Managers.instance.Resource.Load<Sprite>("Bullet_Basic");
                weaponImage.raycastTarget = true;
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
                RectTransform tempRect = weaponNamePannel.rectTransform;
                Vector2 centerPos = new Vector2(0.5f, 0.1496811f);
                int imgSizeLength = ((int)weaponNamePannel.sprite.rect.width).ToString().Length;
                float scaleMag = 1;
                for (int i = 0; i < imgSizeLength; i++)
                {
                    scaleMag = scaleMag * 0.1f;
                }
                float percent = PlayerStatusUI.rectTransform.rect.width / PlayerStatusUI.rectTransform.rect.height;
                Vector2 Size = (new Vector2(weaponNamePannel.sprite.rect.width, weaponNamePannel.sprite.rect.height * percent)* scaleMag)*1.5f;
                Managers.instance.UI.SetUISize(ref tempRect, centerPos - Size, centerPos + Size);
            }
            return weaponNamePannel;
        }
    }

    private Text playerWeaponName;
    public Text PlayerWeaponName
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
                weaponBeforeBTN = new GameObject("weaponBeforeBTN").AddComponent<Button>();
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
    private RectTransform girlParentRT;
    public RectTransform GirlParentRT
    {
        get
        {
            if (girlParentRT == null)
            {
                
                girlParentRT = new GameObject("GirlParent").AddComponent<RectTransform>();
                girlParentRT.SetParent(BattleSceneUI);
                
                Managers.instance.UI.SetUISize(ref girlParentRT, Vector2.zero, Vector2.one);
            }
            return girlParentRT;
        }
    }
    private Image girlPortrait;
    public Image GirlPortrait
    {
        get
        {
            if (girlPortrait == null)
            {
                girlPortrait = new GameObject("BattleSceneGirlPortrait").AddComponent<Image>();
                girlPortrait.rectTransform.SetParent(GirlParentRT);
                girlPortrait.sprite = Managers.instance.Resource.Load<Sprite>("battle_portrait_girl");
                girlPortrait.raycastTarget = false;
                RectTransform tempRect = girlPortrait.rectTransform;
                float percent = GirlParentRT.rect.width / GirlParentRT.rect.height;
                float ScaleMag = 1;
                int SizeLenght = ((int)GirlParentRT.rect.width.ToString().Length);
                for (int i = 0; i < SizeLenght; i++)
                {
                    ScaleMag = 0.1f * ScaleMag;
                }
                Vector2 portraitSize = (new Vector2(girlPortrait.sprite.rect.width, girlPortrait.sprite.rect.height*percent)* ScaleMag);
                Vector2 centerPos = new Vector2(0f, 1f);
                centerPos.x = centerPos.x + portraitSize.x;
                centerPos.y = centerPos.y - portraitSize.y;

                Managers.instance.UI.SetUISize(ref tempRect, centerPos- portraitSize, centerPos+ portraitSize);

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
                girlChatBubble.rectTransform.SetParent(GirlParentRT);
                RectTransform tempRect = girlChatBubble.rectTransform;
                girlChatBubble.sprite = Managers.instance.Resource.Load<Sprite>("chatbubble");
                girlChatBubble.raycastTarget = false;
                float percent = GirlParentRT.rect.width / GirlParentRT.rect.height;
                float magScaler = 1;
                int widthLength = (int)girlChatBubble.sprite.rect.width.ToString().Length;
                for (int i = 0; i < widthLength; i++)
                {
                    magScaler = magScaler * 0.1f;
                }
                Vector2 imageSize = (new Vector2(girlChatBubble.sprite.rect.width, girlChatBubble.sprite.rect.height*percent) * magScaler)/3f;
                Vector2 GetCenterPos = new Vector2(GirlPortrait.rectTransform.anchorMax.x +imageSize.x, 1- imageSize.y);


                Managers.instance.UI.SetUISize(ref tempRect, GetCenterPos - imageSize, GetCenterPos + imageSize);

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
                RectTransform rectTemp = girlText.rectTransform;
                Managers.instance.UI.SetUISize(ref rectTemp, Vector2.zero, Vector2.one);
                girlText.raycastTarget = false;
                girlText.font = Managers.instance.Resource.Load<Font>("InGameFont");
                girlText.alignment = TextAnchor.MiddleCenter;
                girlText.color = Color.black;
                girlText.resizeTextForBestFit = true;
            }
            return girlText;
        }
    }
    private Image girlBomb;
    public Image GirlBomb
    {
        get
        {
            if (girlBomb == null)
            {
                girlBomb = new GameObject("UIBomb").AddComponent<Image>();
                girlBomb.sprite = Managers.instance.Resource.Load<Sprite>("bomb");
                girlBomb.rectTransform.SetParent(battleSceneUI);
                girlBomb.rectTransform.anchorMax = Vector2.one / 2f;
                girlBomb.rectTransform.anchorMin = Vector2.one / 2f;
                girlBomb.rectTransform.sizeDelta = girlBomb.sprite.rect.size;
                girlBomb.rectTransform.anchoredPosition = Vector2.zero;

            }
            return girlBomb;
        }
    }
    public string GirlBulbExplane
    {
        set
        {
            if (value.Contains("라고 하네요"))
            {
                GirlText.DOKill();
                GirlText.DOComplete();
                GirlChatBubble.rectTransform.DOComplete();
                GirlParentRT.gameObject.SetActive(true);

                GirlText.text = string.Empty;
                GirlChatBubble.color = Color.gray;
                GirlText.color = Color.white;

                GirlText.DOText(value, 1).OnComplete(() =>
                {
                    GirlText.DOColor(Color.white, 3).OnComplete(() =>
                    {
                        GirlParentRT.gameObject.SetActive(false);
                    });
                });
            }
            else
            {
                GirlText.DOKill();
                GirlText.DOComplete();
                GirlChatBubble.rectTransform.DOComplete();
                GirlParentRT.gameObject.SetActive(true);
                
                GirlText.text = string.Empty;
                GirlText.text = value;
                GirlChatBubble.color = Color.gray;
                GirlText.color = Color.white;
                GirlText.DOColor(Color.white, 3).OnComplete(() =>
                {
                    GirlParentRT.gameObject.SetActive(false);
                });
            }
        }
    }
    private RectTransform ambientPanel;
    public RectTransform AmbientPanel
    {
        get
        {
            if (ambientPanel == null)
            {
                Image parentBackGround = new GameObject("GameOverParentBackground").AddComponent<Image>();
                parentBackGround.color = new Color32(0, 0, 0, 200);
                RectTransform parentBackGroundRect = parentBackGround.rectTransform;
                parentBackGroundRect.SetParent(BattleSceneUI);
                Managers.instance.UI.SetUISize(ref parentBackGroundRect, Vector2.zero, Vector2.one);
                parentBackGround.rectTransform.SetAsLastSibling();
                ambientPanel = parentBackGroundRect;
            }
            ambientPanel.SetAsLastSibling();
            return ambientPanel;
        }
    }
    private RectTransform sceneBTNParet;
    public RectTransform SceneBTNParet
    {
        get
        {

            if (sceneBTNParet == null)
            {

                sceneBTNParet = new GameObject("GameOverParent").AddComponent<RectTransform>();
                sceneBTNParet.gameObject.AddComponent<Image>().sprite = Managers.instance.Resource.Load<Sprite>("shop_bag_panel");

                sceneBTNParet.SetParent(AmbientPanel);
                Managers.instance.UI.SetUISize(ref sceneBTNParet, new Vector2(0.3f, 0.4f), new Vector2(0.7f, 0.6f));
            }
            return sceneBTNParet;
        }
    }
    private Text gameOverText;
    public string GameOverText
    {
        set
        {
            if (gameOverText == null)
            {
                gameOverText = new GameObject("WarningText").AddComponent<Text>();
                RectTransform warningTextRect = gameOverText.rectTransform;
                gameOverText.rectTransform.SetParent(SceneBTNParet);
                SceneBTNParet.parent.gameObject.SetActive(true);
                Managers.instance.UI.SetUISize(ref warningTextRect, new Vector2(0, 1), new Vector2(1, 2));
                gameOverText.raycastTarget = false;
                gameOverText.color = Color.white;
                gameOverText.fontSize = 102;
                gameOverText.alignment = TextAnchor.MiddleCenter;
                gameOverText.font = Managers.instance.Resource.Load<Font>("InGameFont");
            }
            gameOverText.text = value;
        }
    }
    private Button gameOverBTN;
    public Button GameOverBTN
    {
        get
        {
            if (gameOverBTN == null)
            {

                gameOverBTN = new GameObject("GameOverBTN").AddComponent<Button>();
                RectTransform BTNRect = gameOverBTN.AddComponent<RectTransform>();
                BTNRect.SetParent(SceneBTNParet);
                SceneBTNParet.parent.gameObject.SetActive(true);
                Image btnImage = BTNRect.gameObject.AddComponent<Image>();
                btnImage.sprite = Managers.instance.Resource.Load<Sprite>("shop_portrait_panel");
                gameOverBTN.targetGraphic = btnImage;
                BTNRect.anchorMax = Vector2.one;
                BTNRect.anchorMin = new Vector2(0.5f, 0);
                BTNRect.anchoredPosition = Vector2.zero;
                BTNRect.sizeDelta = Vector2.zero;
                Text tempText = new GameObject("GameOverText").AddComponent<Text>();
                tempText.rectTransform.SetParent(BTNRect);
                tempText.rectTransform.anchorMax = Vector2.one;
                tempText.rectTransform.anchorMin = Vector2.zero;
                tempText.rectTransform.anchoredPosition = Vector2.zero;
                tempText.rectTransform.sizeDelta = Vector2.zero;
                tempText.alignment = TextAnchor.MiddleCenter;
                tempText.text = "To MainTitle";
                tempText.font = Managers.instance.Resource.Load<Font>("InGameFont");
                tempText.fontSize = (int)BTNRect.rect.size.y / 9;
                tempText.color = Color.white;
                gameOverBTN.onClick.AddListener(() => { Managers.instance.OnBTNChangeScene("MainTitle"); });
                Managers.instance.UI.RegistEventTrigger(BTNRect);
                GameOverText = "You DIed";
                gameOverText.color = Color.red;
            }
            return gameOverBTN;
        }
    }

    private Button toDialogSceneBTN;
    public Button ToDialogSceneBTN
    {
        get
        {
            if (toDialogSceneBTN == null)
            {

                toDialogSceneBTN = new GameObject("toDialogSceneBTN").AddComponent<Button>();
                RectTransform BTNRect = toDialogSceneBTN.AddComponent<RectTransform>();
                BTNRect.SetParent(SceneBTNParet);
                Image btnImage = BTNRect.gameObject.AddComponent<Image>();
                btnImage.sprite = Managers.instance.Resource.Load<Sprite>("shop_portrait_panel");
                toDialogSceneBTN.targetGraphic = btnImage;
                BTNRect.anchorMax = new Vector2(0.5f, 1);
                BTNRect.anchorMin = Vector2.zero;
                BTNRect.anchoredPosition = Vector2.zero;
                BTNRect.sizeDelta = Vector2.zero;
                Text tempText = new GameObject("ToDialogScene").AddComponent<Text>();
                tempText.rectTransform.SetParent(BTNRect);
                tempText.rectTransform.anchorMax = Vector2.one;
                tempText.rectTransform.anchorMin = Vector2.zero;
                tempText.rectTransform.anchoredPosition = Vector2.zero;
                tempText.rectTransform.sizeDelta = Vector2.zero;
                tempText.alignment = TextAnchor.MiddleCenter;
                tempText.text = "Continue";
                
                tempText.font = Managers.instance.Resource.Load<Font>("InGameFont");
                tempText.fontSize = (int)BTNRect.rect.size.y / 9;
                tempText.color = Color.white;
                toDialogSceneBTN.onClick.RemoveAllListeners();
                toDialogSceneBTN.onClick.AddListener(() => 
                {
                    Managers.instance.OnBTNChangeScene(Managers.instance.PlayerDataManager.isChallengeMode ? "ChallengeModeLobby" : ShoterController.Instance.nextSceneName);

                     
                }
                );
                Managers.instance.UI.RegistEventTrigger(BTNRect);
                GameOverText = "Clear!";
            }
            return toDialogSceneBTN;
        }
    }
    Image monsterTarget;
    public Image MonsterTarget
    {
        get
        {
            if (monsterTarget == null)
            {

                monsterTarget = new GameObject("MonsterTargetUI").AddComponent<Image>();
                monsterTarget.rectTransform.SetParent(BattleSceneUI);
                monsterTarget.sprite = Managers.instance.Resource.Load<Sprite>("TargetImg");
                monsterTarget.color = Color.red;
                monsterTarget.raycastTarget = false;
            }
            return monsterTarget;
        }
    }
    private Text[] bulbHPText;
    private Tween[] bulbHPTweens;
    public Text BulbHPText
    {
        get
        {
            if (bulbHPText != null)
            {
                if (bulbHPText.Length > 0)
                {
                    for (int i = 0; i < bulbHPText.Length; i++)
                    {
                        if (!bulbHPTweens[i].IsPlaying())
                        {
                            bulbHPText[i].DOKill();
                            bulbHPText[i].rectTransform.position = Camera.main.WorldToScreenPoint(ShoterController.Instance.TargetBall.transform.position);
                            bulbHPTweens[i] = bulbHPText[i].rectTransform.DOAnchorPosY(bulbHPText[i].rectTransform.anchoredPosition.y + 40, 1.5f).OnComplete(() => { bulbHPText[i].gameObject.SetActive(false); bulbHPText[i].DOKill(); });
                            return bulbHPText[i];
                        }
                        else if (bulbHPTweens[i].IsPlaying() && i == bulbHPText.Length - 1)
                        {
                            Array.Resize(ref bulbHPText, bulbHPText.Length + 1);
                            Array.Resize(ref bulbHPTweens, bulbHPTweens.Length + 1);
                            bulbHPText[bulbHPText.Length - 1] = new GameObject("BulbHPText").AddComponent<Text>();
                            bulbHPText[bulbHPText.Length - 1].raycastTarget = false;
                            RectTransform tempTextRect = bulbHPText[bulbHPText.Length - 1].rectTransform;
                            tempTextRect.SetParent(BattleSceneUI);
                            bulbHPText[bulbHPText.Length - 1].alignment = TextAnchor.MiddleCenter;
                            bulbHPText[bulbHPTweens.Length - 1].font = Managers.instance.Resource.Load<Font>("GridiculousMax");
                            bulbHPText[bulbHPTweens.Length - 1].color = Color.red;
                            bulbHPText[bulbHPTweens.Length - 1].fontSize = 30;
                            tempTextRect.position = Camera.main.WorldToScreenPoint(ShoterController.Instance.TargetBall.transform.position);
                            bulbHPTweens[bulbHPTweens.Length - 1] = tempTextRect.DOAnchorPosY(tempTextRect.anchoredPosition.y + 40, 1.5f).OnComplete(() => { tempTextRect.gameObject.SetActive(false); tempTextRect.DOKill(); });
                            return bulbHPText[bulbHPTweens.Length - 1];
                        }
                    }
                }
            }
            bulbHPText = new Text[1];
            bulbHPTweens = new Tween[1];
            bulbHPText[bulbHPText.Length - 1] = new GameObject("BulbHPText").AddComponent<Text>();
            RectTransform tempTextRectr = bulbHPText[bulbHPText.Length - 1].rectTransform;
            tempTextRectr.SetParent(BattleSceneUI);
            bulbHPText[bulbHPText.Length - 1].alignment = TextAnchor.MiddleCenter;
            bulbHPText[bulbHPTweens.Length - 1].font = Managers.instance.Resource.Load<Font>("GridiculousMax");
            bulbHPText[bulbHPTweens.Length - 1].color = Color.red;
            bulbHPText[bulbHPTweens.Length - 1].fontSize = 30;
            bulbHPText[bulbHPText.Length - 1].raycastTarget = false;
            tempTextRectr.position = Camera.main.WorldToScreenPoint(ShoterController.Instance.TargetBall.transform.position);
            bulbHPTweens[bulbHPTweens.Length - 1] = tempTextRectr.DOAnchorPosY(tempTextRectr.anchoredPosition.y + 40, 1.5f).OnComplete(() => { tempTextRectr.gameObject.SetActive(false); tempTextRectr.DOKill(); });
            return bulbHPText[0];


        }
    }
    private Text comboText;
    public int comboStack;
    public int ComboText
    {
        get
        {
            if (comboStack == 0)
            {
                comboStack = 1;
            }
            return comboStack;
        }
        set
        {
            if (comboText == null)
            {
                RectTransform comboParent = new GameObject("ComBoTextParent").AddComponent<RectTransform>();
                comboParent.SetParent(BattleSceneUI);
                Managers.instance.UI.SetUISize(ref comboParent, new Vector2(0.4f, 0.3f), new Vector2(0.6f, 0.7f));
                comboParent.SetAsLastSibling();
                comboText = new GameObject("ComboTest").AddComponent<Text>();
                RectTransform tempTextTR = comboText.rectTransform;
                tempTextTR.SetParent(comboParent);
                Managers.instance.UI.SetUISize(ref tempTextTR, Vector2.zero, Vector2.one);
                comboText.fontSize = 60;
                comboText.color = new Color(0.1933962f, 0.8095468f, 1, 1);
                comboText.font = Managers.instance.Resource.Load<Font>("GridiculousMax");
                comboText.rectTransform.position = Camera.main.WorldToScreenPoint(ShoterController.Instance.transform.position + (Vector3.down * 1.5f));
                comboText.raycastTarget = false;
                comboText.alignment = TextAnchor.MiddleCenter;
            }
            comboText.text = "Combo!\nX" + value;
            comboStack = value;
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
                enemyStatusUI.rectTransform.SetParent(BattleSceneUI);
                RectTransform enemyStatusUIRect = enemyStatusUI.rectTransform;
                Managers.instance.UI.SetUISize(ref enemyStatusUIRect, new Vector2(0.8307168f, 0f), new Vector2(1, 0.7096561f));
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
                tempHpTR.gameObject.SetActive(false);

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
                handle.sprite = Managers.instance.Resource.Load<Sprite>("HP_icon");
                handle.color = Color.clear;


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
                Managers.instance.UI.SetUISize(ref tempHpTR, new Vector2(0.1042682f, 0.3454925f), new Vector2(0.9130226f, 0.5160704f));
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
    private Text enemyHPBarText;
    public string EnemyHPBarText
    {
        set
        {
            if (enemyHPBarText == null)
            {
                Image tempHeartIcon = new GameObject("HeartIconOnTheHPBar").AddComponent<Image>();

                RectTransform tempIconTR = tempHeartIcon.rectTransform;
                tempIconTR.SetParent(EnemyHPBar.fillRect.parent);
                RectTransform tempParentRectTR = EnemyHPBar.fillRect.parent as RectTransform;
                tempHeartIcon.sprite = Managers.instance.Resource.Load<Sprite>("HP_icon");
                float tempParentSizePercent = tempParentRectTR.rect.size.x / tempParentRectTR.rect.size.y;

                int heartIconNumberLenght = ((int)tempHeartIcon.sprite.rect.size.x).ToString().Length;
                float scaler = 1;
                for (int i = 0; i < heartIconNumberLenght; i++)
                {
                    scaler = scaler * 0.1f;
                }
                Vector2 tempEndPos = new Vector2(tempHeartIcon.sprite.rect.width, tempHeartIcon.sprite.rect.height * tempParentSizePercent) / 2;
                Managers.instance.UI.SetUISize(ref tempIconTR, Vector2.up, (tempEndPos * scaler) + Vector2.up);



                enemyHPBarText = new GameObject("enemyHPBarText").AddComponent<Text>();
                RectTransform tempRect = enemyHPBarText.rectTransform;
                tempRect.SetParent(EnemyHPBar.fillRect.parent);
                Managers.instance.UI.SetUISize(ref tempRect, new Vector2(tempIconTR.anchorMax.x, 1f), Vector2.right + (Vector2.up * tempIconTR.anchorMax.y));
                enemyHPBarText.font = Managers.instance.Resource.Load<Font>("InGameFont");
                enemyHPBarText.color = new Color(0.7169812f, 0.7169812f, 0.7169812f, 1);
                enemyHPBarText.resizeTextForBestFit = true;
                enemyHPBarText.alignment = TextAnchor.MiddleLeft;

            }
            enemyHPBarText.text = value;
        }
    }
    private Text playerHPBarText;
    public string PlayerHPBarText
    {
        set
        {
            if (playerHPBarText == null)
            {
                Image tempHeartIcon = new GameObject("HeartIconOnTheHPBar").AddComponent<Image>();
                RectTransform tempIconTR = tempHeartIcon.rectTransform;
                tempIconTR.SetParent(PlayerHPBarFrontIMG.rectTransform);
                RectTransform tempParentRectTR = PlayerHPBarFrontIMG.rectTransform;
                tempHeartIcon.sprite = Managers.instance.Resource.Load<Sprite>("HP_icon");
                float tempParentSizePercent = tempParentRectTR.rect.size.x / tempParentRectTR.rect.size.y;

                int heartIconNumberLenght = ((int)tempHeartIcon.sprite.rect.size.x).ToString().Length;
                float scaler = 1;
                for (int i = 0; i < heartIconNumberLenght; i++)
                {
                    scaler = scaler * 0.1f;
                }
                Vector2 tempEndPos = new Vector2(tempHeartIcon.sprite.rect.width, tempHeartIcon.sprite.rect.height * tempParentSizePercent) / 2;
                Managers.instance.UI.SetUISize(ref tempIconTR, Vector2.up, (tempEndPos * scaler) + Vector2.up);



                playerHPBarText = new GameObject("PlayerHPText").AddComponent<Text>();
                RectTransform tempRect = playerHPBarText.rectTransform;
                tempRect.SetParent(PlayerHPBarFrontIMG.rectTransform);
                Managers.instance.UI.SetUISize(ref tempRect, new Vector2(tempIconTR.anchorMax.x, 1f), Vector2.right + (Vector2.up * tempIconTR.anchorMax.y));
                playerHPBarText.font = Managers.instance.Resource.Load<Font>("InGameFont");
                playerHPBarText.color = new Color(0.7169812f, 0.7169812f, 0.7169812f, 1);
                playerHPBarText.resizeTextForBestFit = true;
                playerHPBarText.alignment = TextAnchor.MiddleLeft;
                tempHeartIcon.enabled = false;
                //UI 구조 수정으로 아이콘 임시삭제
            }
            playerHPBarText.text = value;
        }
    }


    private Text warningText;
    public string WarningText
    {
        set
        {
            if (warningText == null)
            {
                warningText = new GameObject("WarningText").AddComponent<Text>();
                RectTransform warningTextRect = warningText.rectTransform;
                warningTextRect.SetParent(BattleSceneUI);
                Managers.instance.UI.SetUISize(ref warningTextRect, Vector2.zero, Vector2.one);
                warningText.raycastTarget = false;
                warningText.color = Color.red;
                warningText.fontSize = 102;
                warningText.font = Managers.instance.Resource.Load<Font>("InGameFont");
            }
            warningText.text = value;
        }
    }
    public bool isInFeverMode
    {
        get
        {
            return enemyHpBar.value < enemyHpBar.maxValue / 2;
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
    private Image nextMonsterPannel;
    private Image NextMonsterPannel
    {
        get
        {
            if (nextMonsterPannel == null)
            {
                nextMonsterPannel = new GameObject("MonsterQueuePreveiwPanel").AddComponent<Image>();
                nextMonsterPannel.sprite = Managers.instance.Resource.Load<Sprite>("nextmonster_panel");
                RectTransform tempRect = nextMonsterPannel.rectTransform;
                tempRect.SetParent(EnemyStatusUI.rectTransform);
                nextMonsterPannel.type = Image.Type.Sliced;

                float parentPercent = EnemyStatusUI.rectTransform.rect.size.x / EnemyStatusUI.rectTransform.rect.size.y;
                //위 숫자에 y를 곱해주면 원본비율
                Vector2 centerPos = new Vector2(0.5f, 0.7493f);
                Vector2 PanelSize = new Vector2(nextMonsterPannel.sprite.rect.width, nextMonsterPannel.sprite.rect.height) * 1.5f;
                string getOriginSizeNumberSize = ((int)PanelSize.x).ToString();
                float imageScale = 1;
                for (int i = 0; i < getOriginSizeNumberSize.Length; i++)
                {
                    imageScale = imageScale * 0.1f;
                }
                Vector2 imageSize = new Vector2(PanelSize.x, PanelSize.y * parentPercent) * imageScale;
                Managers.instance.UI.SetUISize(ref tempRect, centerPos - imageSize, centerPos + imageSize);
                Debug.Log("ㅁㄴㅇ");
                NextMonsterIcon.enabled = true;
            }
            return nextMonsterPannel;
        }
    }
    private Image nextMonsterIcon;
    private Image NextMonsterIcon
    {
        get
        {
            if (nextMonsterIcon == null)
            {
                nextMonsterIcon = new GameObject("NextMonsterIcon").AddComponent<Image>();
                nextMonsterIcon.sprite = Managers.instance.Resource.Load<Sprite>("Next_Monster_icon");
                RectTransform tempRect = nextMonsterIcon.rectTransform;
                tempRect.SetParent(NextMonsterPannel.rectTransform);
                Vector2 minPos = new Vector2(0f, 1f);
                float parentPercent = NextMonsterPannel.rectTransform.rect.size.x / NextMonsterPannel.rectTransform.rect.size.y;
                int sizeLength = ((int)nextMonsterIcon.sprite.rect.width).ToString().Length;
                float tempScaledNum = 1;
                for (int i = 0; i < sizeLength; i++)
                {
                    tempScaledNum = tempScaledNum * 0.1f;
                }
                Vector2 maxPos = (new Vector2(nextMonsterIcon.sprite.rect.width, nextMonsterIcon.sprite.rect.height * parentPercent) * tempScaledNum) * 0.5f;
                Managers.instance.UI.SetUISize(ref tempRect, minPos, maxPos + Vector2.up);
                Text tempText = new GameObject("nextMonsterText").AddComponent<Text>();
                tempText.alignment = TextAnchor.MiddleLeft;
                tempText.text = " Next";
                tempText.font = Managers.instance.Resource.Load<Font>("InGameFont");
                tempText.resizeTextForBestFit = true;
                tempText.color = new Color(0.7169812f, 0.7169812f, 0.7169812f, 1);
                RectTransform tempTextRect = tempText.rectTransform;
                tempTextRect.SetParent(NextMonsterPannel.rectTransform);
                Managers.instance.UI.SetUISize(ref tempTextRect, new Vector2(maxPos.x, 1), (Vector2.up * maxPos.y) + Vector2.one);
            }
            return nextMonsterIcon;
        }
    }
    /*    private Image[] queueMonsterIMG;*/
    //array.resize로 재할당
    private Image beforeMonsterIMG;
    private Image nextMonsterIMG;
    private Image NextMonsterIMG
    {
        get
        {
            if (nextMonsterIMG == null)
            {
                nextMonsterIMG = new GameObject("BeforeMonsterimg").AddComponent<Image>();
                RectTransform TempRect = nextMonsterIMG.rectTransform;
                TempRect.SetParent(NextMonsterPannel.rectTransform);
                TempRect.anchoredPosition = Vector2.right * 400f;
                TempRect.anchorMax = Vector2.one / 2f;
                TempRect.anchorMin = Vector2.one / 2f;
                TempRect.sizeDelta = NextMonsterPannel.rectTransform.rect.size * (2f / 3f);
            }
            return nextMonsterIMG;
        }
    }
    private Image BeforeMonsterIMG
    {
        get
        {
            if (beforeMonsterIMG == null)
            {
                beforeMonsterIMG = new GameObject("NextMonsterimg").AddComponent<Image>();
                RectTransform TempRect = beforeMonsterIMG.rectTransform;
                TempRect.SetParent(NextMonsterPannel.rectTransform);
                TempRect.anchoredPosition = Vector2.zero;
                TempRect.anchorMax = Vector2.one / 2f;
                TempRect.anchorMin = Vector2.one / 2f;
                TempRect.sizeDelta = NextMonsterPannel.rectTransform.rect.size * (2f / 3f);

            }
            return beforeMonsterIMG;
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
                scoreBoardPannel.rectTransform.SetParent(BattleSceneUI);
                scoreBoardPannel.rectTransform.anchorMin = new Vector2(0.398604751f, 0.872544408f);
                scoreBoardPannel.rectTransform.anchorMax = new Vector2(0.604237199f, 0.987735808f);
                scoreBoardPannel.rectTransform.anchoredPosition = Vector2.zero;
                scoreBoardPannel.rectTransform.sizeDelta = Vector2.zero;

            }
            return scoreBoardPannel;
        }
    }
    private Text scoreBoardText;
    private Text ScoreBoardText
    {
        get
        {
            if (scoreBoardText == null)
            {
                scoreBoardText = new GameObject("enemyWeaponName").AddComponent<Text>();
                scoreBoardText.rectTransform.SetParent(ScoreBoardPannel.rectTransform);
                RectTransform scoreTextRect = scoreBoardText.rectTransform;
                Managers.instance.UI.SetUISize(ref scoreTextRect, Vector2.zero, Vector2.one);
                scoreBoardText.alignment = TextAnchor.MiddleCenter;
                scoreBoardText.color = Color.black;
                scoreBoardText.fontSize = 84;
                scoreBoardText.font = Managers.instance.Resource.Load<Font>("InGameFont");
                scoreBoardPannel.gameObject.SetActive(false);
            }
            return scoreBoardText;
        }
    }
    private RectTransform ballForceSliderParent;
    public RectTransform BallForceSliderParent
    {
        get
        {
            if (ballForceSliderParent == null)
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

    private Image ballForceSliderBackGround = null;
    private Image BallForceSliderBackGround
    {
        get
        {
            if (ballForceSliderBackGround == null)
            {
                ballForceSliderBackGround = new GameObject("BallForceSlider").AddComponent<Image>();
                ballForceSliderBackGround.rectTransform.SetParent(BallForceSliderParent);
                ballForceSliderBackGround.rectTransform.anchorMax = Vector2.one / 2;
                ballForceSliderBackGround.rectTransform.anchorMin = Vector2.one / 2;
                Sprite tempSprite = Managers.instance.Resource.Load<Sprite>("charge_empty");
                ballForceSliderBackGround.sprite = tempSprite;
                ballForceSliderBackGround.rectTransform.sizeDelta = tempSprite.rect.size;
                ballForceSliderBackGround.rectTransform.anchoredPosition = Vector2.zero;
                ballForceSliderBackGround.raycastTarget = false;
            }
            return ballForceSliderBackGround;
        }
    }

    private Image ballForceSlider = null;
    private Image BallForceSlider
    {
        get
        {
            if (ballForceSlider == null)
            {
                ballForceSlider = new GameObject("BallForceSlider").AddComponent<Image>();
                ballForceSlider.rectTransform.SetParent(BallForceSliderBackGround.rectTransform);
                ballForceSlider.rectTransform.anchorMax = Vector2.one / 2;
                ballForceSlider.rectTransform.anchorMin = Vector2.one / 2;
                ballForceSlider.rectTransform.anchoredPosition = Vector2.zero;
                Sprite tempSprite = Managers.instance.Resource.Load<Sprite>("charge_full");
                ballForceSlider.sprite = tempSprite;
                ballForceSlider.rectTransform.sizeDelta = tempSprite.rect.size;
                ballForceSlider.fillClockwise = true;
                ballForceSlider.type = Image.Type.Filled;
                ballForceSlider.raycastTarget = false;
            }
            return ballForceSlider;
        }
    }
    public float ForceValue
    {
        set
        {
            BallForceSlider.fillAmount = value;
        }
    }
    Slider[] monsterPriavteHPBar;

    #region 관련 함수

    public void GirlTextAttack(string TXT, Color bubbleColor, Color chattingColor)
    {
        GirlText.DOComplete();
        GirlChatBubble.rectTransform.DOComplete();
        GirlParentRT.gameObject.SetActive(true);
        GirlText.fontSize = 36;
        GirlChatBubble.rectTransform.DOComplete();
        GirlChatBubble.rectTransform.DOPunchScale(Vector3.one, 0.4f, 1, 0.5f).OnComplete(() =>
        {

            GirlChatBubble.rectTransform.DOAnchorPos(GirlChatBubble.rectTransform.anchoredPosition, 5).OnComplete(() =>
            {
                GirlChatBubble.color = Color.white;
                GirlText.color = Color.black;
                GirlText.text = string.Empty;
                GirlChatBubble.rectTransform.DOKill();
                GirlParentRT.gameObject.SetActive(false);
            });
        });
        GirlChatBubble.color = bubbleColor;
        GirlText.color = chattingColor;

        GirlText.text = TXT;
    }

    public void SettingPlayerBattleUI()
    {
        //TODO : 매니저의 인스턴스 선언 후 해당구문 수정필요,아래의 Enabled는 테스트코드로 해당 작업 시 삭제필요
        //PlayerHPBar.value = ;
        //PlayerHPBar.maxValue = ;
        //PlayerPortrait.sprite = ;
        //WeaponImage =
        GirlParentRT.gameObject.SetActive(false);
        PlayerHPBarFrontIMG.enabled = true;
        WeaponImage.enabled = true;
        PlayerPortrait.enabled = true;
        if (ShoterController.Instance.NowBallStat != null)
        {
            PlayerWeaponName.text = ShoterController.Instance.NowBallStat.ballKoreanName;
        }
        else
        {
            PlayerWeaponName.text = "무기가 없어!!";
        }


        UpdateScore(0, 0);
        //EnemyUISetting();
        NextMonsterPannel.enabled = true;
    }
    public void RegistListener()
    {
        Managers.instance.UI.RegistEventTrigger(WeaponBeforeBTN.transform as RectTransform);
        Managers.instance.UI.RegistEventTrigger(WeaponNextBTN.transform as RectTransform);
        WeaponBeforeBTN.onClick.RemoveAllListeners();
        WeaponBeforeBTN.onClick.AddListener(Managers.instance.PlayerDataManager.PlayerBeforeBallPick);
        WeaponNextBTN.onClick.RemoveAllListeners();
        WeaponNextBTN.onClick.AddListener(Managers.instance.PlayerDataManager.PlayerNextBallPick);
    }
    public void WeaponAnim(bool isMoveRight, string ballName, string ballKRName, string PrevBallName)
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
        //        ChangeWeaponUI(true, ballName, ballKRName);
    }
    public void EnemyHPBarUpdate( float valueToAdd)
    {

        EnemyHPBar.value = enemyHpBar.value + valueToAdd;
        EnemyHPBarText = "HP " + EnemyHPBar.value + "/" + EnemyHPBar.maxValue;

        if (EnemyHPBar.value <= EnemyHPBar.maxValue / 2f && warningText == null)
        {
            WarningText = "적의 공격력이 2배 증가합니다!";
            warningText.gameObject.SetActive(false);
            MonsterManager.MonsterManagerInstance.SetFeaverMode();
        }
        if (EnemyHPBar.value <= 0)
        {
            if (Managers.instance.PlayerDataManager.SetPlayerHP.Item2 >= 0)
            {
                Managers.instance.UI.BattleUICall.ToDialogSceneBTN.enabled = true;
            }
            ShoterController.Instance.isReadyFire = false;
        }
    }
    public void HPBarActivate(float maxHP, float nowHP)
    {
        float MaxPerNow = nowHP / maxHP;
        PlayerHPBarFrontIMG.fillAmount = MaxPerNow;
    }
    public void HPBarSetting(bool isPlayer, float maxHP, float nowHP)
    {
        if (isPlayer)
        {
            float MaxPerNow = nowHP / maxHP;
            PlayerHPBarFrontIMG.fillAmount = MaxPerNow;
            PlayerHPBarText = (MaxPerNow * 100) + "%";
        }
        else
        {
            EnemyHPBar.maxValue = maxHP;
            EnemyHPBar.value = nowHP;
            EnemyHPBarText = "HP" + nowHP + "/" + maxHP;
        }
    }
    public void ChangePortrait(bool isPlayer, string spriteName)
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
    public void ChangeWeaponUI(string spriteName, string weaponKoreanName)
    {
        WeaponImage.sprite = Managers.instance.Resource.Load<Sprite>(spriteName);
        PlayerWeaponName.text = weaponKoreanName;
        /*        if (isPlayer)
                {
                    WeaponImage.sprite = Managers.instance.Resource.Load<Sprite>(spriteName);
                    PlayerWeaponName.text = weaponKoreanName;
                }
                else
                {
                    EnemyWeaponImage.sprite = Managers.instance.Resource.Load<Sprite>(spriteName);
                    EnemyWeaponName.text = weaponKoreanName;
                }*/
    }

    public void WeaponButtonCheck(bool isZero)
    {
        WeaponNextBTN.interactable = !isZero;
        weaponBeforeBTN.interactable = !isZero;
    }

    public void UpdateScore(int PlayerScore, int EnemyScore)
    {
        ScoreBoardText.text = PlayerScore + " : " + EnemyScore;
    }
    public void SetBallSliderPos(Vector3 ShooterPosition, bool isSetActive)
    {
        if (isSetActive)
        {
            Vector3 tempVec = Camera.main.WorldToScreenPoint(ShooterPosition + Vector3.up);
            BallForceSliderParent.position = tempVec;
        }
        BallForceSliderParent.gameObject.SetActive(isSetActive);

    }
    public void UpdateBallForce(float value)
    {
        if (!BallForceSliderParent.gameObject.activeSelf)
        {
            BallForceSliderParent.gameObject.SetActive(true);
        }
        ForceValue = value;
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
    public void SetTargetUI(Transform targetTR, Vector2 SpriteSize)
    {
        if (targetTR != null)
        {
            Vector2 spriteSizeInCanvasMax = Camera.main.WorldToScreenPoint(targetTR.position + (Vector3)SpriteSize);
            Vector2 spriteSizeInCanvasMin = Camera.main.WorldToScreenPoint(targetTR.position - (Vector3)SpriteSize);
            MonsterTarget.rectTransform.anchorMin = new Vector2(spriteSizeInCanvasMin.x / Screen.width, spriteSizeInCanvasMin.y / Screen.height);
            MonsterTarget.rectTransform.anchorMax = new Vector2(spriteSizeInCanvasMax.x / Screen.width, spriteSizeInCanvasMax.y / Screen.height);
            MonsterTarget.rectTransform.anchoredPosition = Vector2.zero;
            MonsterTarget.rectTransform.sizeDelta = Vector2.zero;
            MonsterTarget.gameObject.SetActive(targetTR.gameObject.activeSelf);
        }
        else
        {
            MonsterTarget.gameObject.SetActive(false);
        }

    }
    public void SetComboNumber(bool turnOn)
    {
        if (turnOn)
        {
            ComboText = comboStack != 0 ? ComboText * 2 : ComboText;
            comboText.gameObject.SetActive(true);
            ShoterController.Instance.regionalDamage = ShoterController.Instance.regionalDamage * 2;
            ShoterController.Instance.targetDamage = ShoterController.Instance.targetDamage * 2;
        }
        else
        {
            ComboText = 0;
            comboText.gameObject.SetActive(false);
        }

    }
    public void SetBulbDamagedText(int max, int now)
    {
        Text tempText = BulbHPText;
        tempText.gameObject.SetActive(true);
        tempText.text = (now + "/" + max);
    }
    public void ResetBulbDamageText()
    {
        bulbHPText = null;
        bulbHPTweens = null;
    }
    public void SetUIMonsterImageArray(/*Queue<Sprite> spriteQueue*/Sprite NextMonsterSprite, SpriteRenderer monsterSR)
    {
        Sprite tempIMG = BeforeMonsterIMG.sprite;
        if (tempIMG == null)
        {
            if (MonsterManager.MonsterManagerInstance.Monsters[0].Item2 != null)
            {
                tempIMG = MonsterManager.MonsterManagerInstance.Monsters[0].Item2.sprite;
                BeforeMonsterIMG.sprite = tempIMG;
            }
        }
        else if (NextMonsterSprite == null)
        {

            NextMonsterIMG.color = Color.clear;
        }
        else
        {
            NextMonsterIMG.color = Color.white;
            BeforeMonsterIMG.color = Color.white;
        }
        BeforeMonsterIMG.DOComplete();
        NextMonsterIMG.DOComplete();
        BeforeMonsterIMG.rectTransform.localScale = Vector3.one;
        BeforeMonsterIMG.rectTransform.anchoredPosition = Vector2.zero;
        NextMonsterIMG.rectTransform.anchoredPosition = Vector2.right * 300;


        Vector3 targetPos = MonsterManager.MonsterManagerInstance.moveSlots[MonsterManager.MonsterManagerInstance.moveSlots.Length - 1].slotPosition;
        Vector2 temppos = Camera.main.WorldToScreenPoint(targetPos);
        Vector2 PosMin = Camera.main.WorldToScreenPoint(targetPos + monsterSR.bounds.min);
        Vector2 PosMax = Camera.main.WorldToScreenPoint(targetPos + monsterSR.bounds.max);
        float ScaleValue = Vector2.Distance(PosMin, PosMax) / Vector2.Distance(BeforeMonsterIMG.rectTransform.rect.min, BeforeMonsterIMG.rectTransform.rect.max);
        NextMonsterIMG.sprite = NextMonsterSprite;
        BeforeMonsterIMG.rectTransform.DOScale(ScaleValue, 1f);
        if (isInFeverMode)
        {
            NextMonsterIMG.color = Color.red;
            BeforeMonsterIMG.color = Color.red;
        }
        BeforeMonsterIMG.rectTransform.DOJump(temppos, 30, 2, 1.8f, true).OnComplete(() =>
        {
            if (beforeMonsterIMG == null)
            {
                return;
            }
            BeforeMonsterIMG.rectTransform.localScale = Vector3.one;
            BeforeMonsterIMG.rectTransform.anchoredPosition = Vector2.zero;
            NextMonsterIMG.sprite = tempIMG;
            BeforeMonsterIMG.sprite = NextMonsterSprite;
            monsterSR.enabled = true;
            if (NextMonsterSprite == null)
            {
                NextMonsterIMG.color = Color.clear;
                BeforeMonsterIMG.color = Color.clear;
            }
        });
        NextMonsterIMG.rectTransform.DOJump(NextMonsterPannel.rectTransform.position, 40f, 4, 1.8f, true).OnComplete(() =>
        {
            if (beforeMonsterIMG == null)
            {
                return;
            }
            if (NextMonsterSprite == null)
            {
                NextMonsterIMG.color = Color.clear;
                BeforeMonsterIMG.color = Color.clear;
            }
            NextMonsterIMG.rectTransform.anchoredPosition = Vector2.right * 300;
        });

        //기존 nextMonster세팅,Queue형식
        /*        RectTransform tempRec;
                float XPerY = MonsterQueuePannel.rectTransform.rect.size.x / MonsterQueuePannel.rectTransform.rect.size.y;
                float xBlank = 0.025f;
                float yBlank = xBlank * XPerY;
                Vector2 ImageSize = new Vector2(0.2f, 0.2f * XPerY);
                //정사각형 기준
                int savedQueueCount = spriteQueue.Count;
                //부모기준으로 된 절반크기
                if (queueMonsterIMG== null)
                {
                    queueMonsterIMG = new Image[0];
                }
                if (queueMonsterIMG.Length< spriteQueue.Count)
                {
                    Array.Resize(ref queueMonsterIMG, savedQueueCount);
                }
                for (int i = 0; i < savedQueueCount; i++)
                {
                    if (i % 4 == 0)
                    {
                        xBlank = 0.025f;
                    }
                    else
                    {
                        xBlank = 0.05f;
                    }
                    float tempYBlank = (yBlank*2) * (i / 4);
                    Vector2 imageMax = new Vector2((((i%4)+1)*(ImageSize.x+xBlank)) , (1 - yBlank) - tempYBlank-((i/4)*ImageSize.y));
                    queueMonsterIMG[i] = new GameObject(i + "THMonster").AddComponent<Image>();
                    tempRec = queueMonsterIMG[i].rectTransform;
                    tempRec.SetParent(MonsterQueuePannel.rectTransform);

                    Managers.instance.UI.SetUISize(ref tempRec, (imageMax - ImageSize) - new Vector2(i % 4 != 0 ? 0.025f : 0, 0), imageMax - new Vector2(i % 4 != 0 ? 0.025f : 0, 0));

                    queueMonsterIMG[i].sprite = spriteQueue.Dequeue();
                }*/
    }
    public void InstallMonsterHPBar(int maxSlotCount)
    {
        monsterPriavteHPBar = new Slider[maxSlotCount];
        for (int i = 0; i < maxSlotCount; i++)
        {
            monsterPriavteHPBar[i] = new GameObject("Slot" + i + "Bar").AddComponent<Slider>();
            monsterPriavteHPBar[i].wholeNumbers = false;
            monsterPriavteHPBar[i].maxValue = 100;
            //TODO : 슬라이드바 UI 추가시 여기에 작업
            RectTransform tempHpTR = monsterPriavteHPBar[i].transform as RectTransform;


            RectTransform tempHandleArea = new GameObject("HandleArea").AddComponent<RectTransform>();
            tempHandleArea.SetParent(tempHpTR);
            tempHandleArea.anchorMin = Vector2.zero; // 부모의 왼쪽 하단을 기준으로
            tempHandleArea.anchorMax = Vector2.one; // 부모의 왼쪽 상단을 기준으로
            tempHandleArea.pivot = new Vector2(0.5f, 0.5f); // 부모의 왼쪽 중간을 기준으로
            tempHandleArea.sizeDelta = Vector2.zero;
            tempHandleArea.anchoredPosition = Vector2.zero;
            Image handle = new GameObject("handle").AddComponent<Image>();
            handle.raycastTarget = false;
            handle.rectTransform.SetParent(tempHandleArea);
            handle.rectTransform.anchorMax = Vector2.up;
            handle.rectTransform.anchorMin = Vector2.zero;
            handle.rectTransform.pivot = Vector2.one / 2f;
            handle.rectTransform.sizeDelta = Vector2.right * 50;
            monsterPriavteHPBar[i].handleRect = handle.rectTransform;
            handle.sprite = Managers.instance.Resource.Load<Sprite>("HP_icon");
            handle.color = Color.clear;


            Image BackGround = new GameObject("BackGround").AddComponent<Image>();
            BackGround.raycastTarget = false;
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
            tempIMG.raycastTarget = false;
            tempIMG.rectTransform.SetParent(tempFillArea);
            tempIMG.rectTransform.sizeDelta = Vector2.zero;
            tempIMG.color = Color.white;
            tempIMG.sprite = Managers.instance.Resource.Load<Sprite>("HPbar");
            monsterPriavteHPBar[i].fillRect = tempIMG.rectTransform;

            // Slider의 부모-자식 관계 설정  



            tempHpTR.SetParent(BattleSceneUI);
            tempHpTR.SetAsLastSibling();
            Managers.instance.UI.SetUISize(ref tempHpTR, new Vector2(0.45f, 0.48f), new Vector2(0.55f, 0.52f));
            monsterPriavteHPBar[i].interactable = false;
            monsterPriavteHPBar[i].enabled = false;
            monsterPriavteHPBar[i].SetDirection(Slider.Direction.RightToLeft, true);

            tempHandleArea.SetAsLastSibling();
            monsterPriavteHPBar[i].gameObject.SetActive(false);

        }
    }
    public void SetMonsterHPBar(Vector3 SlotPos, int SlotIndex, float maxHP = -100, float nowHP = -100)
    {
        if (maxHP == -100)
        {
            monsterPriavteHPBar[SlotIndex].gameObject.SetActive(false);
            return;
        }
        else
        {
            if (SlotIndex <= -1)
            {
                return;
            }
            if (monsterPriavteHPBar.Length <= SlotIndex)
            {
                return;
            }
            monsterPriavteHPBar[SlotIndex].gameObject.SetActive(true);
            monsterPriavteHPBar[SlotIndex].transform.position = Camera.main.WorldToScreenPoint(SlotPos + Vector3.up);
            monsterPriavteHPBar[SlotIndex].maxValue = maxHP;
            monsterPriavteHPBar[SlotIndex].value = nowHP;
            if (nowHP <= 0)
            {
                monsterPriavteHPBar[SlotIndex].gameObject.SetActive(false);
            }

        }
    }
    /*    public void SetMonsterDeadInQueue(int index)
        {
            queueMonsterIMG[index].color = Color.gray;
        }
        public void ResetMonsterQueueIMG()
        {
            queueMonsterIMG = null;
        }*/
    #endregion
}
public class ShopUI
{
    public bool IsShopActivate
    {
        get { return ShopPanel.gameObject.activeSelf; }
        set
        {
            Managers.instance.UI.TargetUIOnOff(ShopPanel.rectTransform, value);
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
                shopPanel.sprite = Managers.instance.Resource.Load<Sprite>("shop_background_out");
                shopPanel.rectTransform.SetAsLastSibling();
                shopPanel.gameObject.SetActive(false);
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
                shopInnerPanel.sprite = Managers.instance.Resource.Load<Sprite>("shop_background_in");
                //TODO : ShopPanel이 업로드시 해당 키값 작성 요망
            }
            return shopInnerPanel;
        }
    }
    private RectTransform shopPlayerStatusWindowPanel;
    public RectTransform ShopPlayerStatusWindowPanel
    {
        get
        {
            if (shopPlayerStatusWindowPanel == null)
            {
                shopPlayerStatusWindowPanel = new GameObject("ShopPlayerPanel").AddComponent<RectTransform>();
                shopPlayerStatusWindowPanel.SetParent(ShopInnerShopPanel.rectTransform);
                shopPlayerStatusWindowPanel.anchoredPosition = Vector3.zero;
                shopPlayerStatusWindowPanel.anchorMax = new Vector2(0.322f, 1f);
                shopPlayerStatusWindowPanel.anchorMin = Vector2.zero;
                shopPlayerStatusWindowPanel.sizeDelta = Vector2.zero;
                //TODO : ShopPanel이 업로드시 해당 키값 작성 요망
            }
            return shopPlayerStatusWindowPanel;
        }
    }

    private Image merchantPortrait_Pannel;
    public Image MerchantPortrait_Pannel
    {
        get
        {
            if (merchantPortrait_Pannel == null)
            {
                merchantPortrait_Pannel = new GameObject("MerchantPortrait_Pannel").AddComponent<Image>();
                RectTransform tempRect = merchantPortrait_Pannel.rectTransform;
                tempRect.SetParent(ShopPlayerStatusWindowPanel);
                Managers.instance.UI.SetUISize(ref tempRect, new Vector2(0.019f, 0.649f), new Vector2(0.505f, 0.981f));

                merchantPortrait_Pannel.sprite = Managers.instance.Resource.Load<Sprite>("shop_portrait_panel");
                //TODO : ShopPanel이 업로드시 해당 키값 작성 요망
            }
            return merchantPortrait_Pannel;
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
                RectTransform tempRect = merchantPortrait.rectTransform;
                tempRect.SetParent(MerchantPortrait_Pannel.rectTransform);
                Vector2 portraitOutline = new Vector2(0.0683183521f, 0.0676659271f);
                Managers.instance.UI.SetUISize(ref tempRect, Vector2.zero+ portraitOutline, Vector2.one-portraitOutline);
                
                merchantPortrait.sprite = Managers.instance.Resource.Load<Sprite>("shop_portrait");
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
                merchantDialogPanel.rectTransform.SetParent(ShopPlayerStatusWindowPanel);
                merchantDialogPanel.rectTransform.anchoredPosition = Vector3.zero;
                merchantDialogPanel.rectTransform.anchorMax = new Vector2(0.971f, 0.971f);
                merchantDialogPanel.rectTransform.anchorMin = new Vector2(0.511f, 0.675f);
                merchantDialogPanel.rectTransform.sizeDelta = Vector2.zero;
                merchantDialogPanel.sprite = Managers.instance.Resource.Load<Sprite>("shop_chat_panel");
                //TODO : ShopPanel이 업로드시 해당 키값 작성 요망
            }
            return merchantDialogPanel;
        }
    }
    private RectTransform playerBagPanel;
    public RectTransform PlayerBagPanel
    {
        get
        {
            if (playerBagPanel == null)
            {
                playerBagPanel = new GameObject("PlayerBagPanel").AddComponent<RectTransform>();
                playerBagPanel.SetParent(ShopPlayerStatusWindowPanel);
                Managers.instance.UI.SetUISize(ref playerBagPanel, new Vector2(0.019f, 0.0252f), new Vector2(0.971f, 0.624f));
                playerBagPanel.anchoredPosition = Vector3.zero;
                playerBagPanel.anchorMax = new Vector2(0.971f, 0.624f);
                playerBagPanel.anchorMin = new Vector2(0.019f, 0.0252f);
                playerBagPanel.sizeDelta = Vector2.zero;
                //TODO : ShopPanel이 업로드시 해당 키값 작성 요망
            }
            return playerBagPanel;
        }
    }
    private Image playerMoneyPannel;
    public Image PlayerMoneyPannel
    {
        get
        {
            if (playerMoneyPannel == null)
            {
                playerMoneyPannel = new GameObject("PlayerMoneyPannel").AddComponent<Image>();
                RectTransform tempRect = playerMoneyPannel.rectTransform;
                playerMoneyPannel.rectTransform.SetParent(PlayerBagPanel);
                playerMoneyPannel.sprite = Managers.instance.Resource.Load<Sprite>("coin_pannel");
                float yPercentValue = playerMoneyPannel.sprite.bounds.size.y/ playerMoneyPannel.sprite.bounds.size.x;
                Managers.instance.UI.SetUISize(ref tempRect, new Vector2(0, 1 - yPercentValue), Vector2.one);
                //TODO : ShopPanel이 업로드시 해당 키값 작성 요망
            }
            return playerMoneyPannel;
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
                RectTransform tempRect = playerInventoryPanel.rectTransform;
                tempRect.SetParent(PlayerBagPanel);


                playerInventoryPanel.sprite = Managers.instance.Resource.Load<Sprite>("shop_bag_panel");
                float parentPercent = playerInventoryPanel.sprite.bounds.size.y;

                //x가 1일때 y의 비율\
                float walletPanelSize = PlayerMoneyPannel.rectTransform.anchorMax.y - PlayerMoneyPannel.rectTransform.anchorMin.y;
                walletPanelSize = walletPanelSize + (walletPanelSize / 2f);
                Managers.instance.UI.SetUISize(ref tempRect, Vector2.zero, Vector2.one-(Vector2.up* walletPanelSize));
                //TODO : ShopPanel이 업로드시 해당 키값 작성 요망
            }
            return playerInventoryPanel;
        }
    }
    Button nextBTN;
    Button NextBTN
    {
        get
        {
            if (nextBTN == null)
            {
                Button tempNextBTN = new GameObject("WeaponNextBTN").AddComponent<Button>();
                RectTransform NextBTRT = tempNextBTN.transform.AddComponent<RectTransform>();
                NextBTRT.SetParent(PlayerInventoryPanel.rectTransform);
                Image tempNextBTNIMG = NextBTRT.AddComponent<Image>();
                tempNextBTN.targetGraphic = tempNextBTNIMG;
                float percent = PlayerInventoryPanel.rectTransform.rect.size.x / PlayerInventoryPanel.rectTransform.rect.size.y;
                Vector2 BeforeBTNStartPos = new Vector2(0f, 0.4f);
                Vector2 tempSize = new Vector2(0.1f, 0.1f * percent);
                tempNextBTNIMG.sprite = Managers.instance.Resource.Load<Sprite>("select_arrow_panel_R");
                BeforeBTNStartPos = new Vector2(1f, BeforeBTNStartPos.y + tempSize.y);
                NextBTRT.anchorMin = BeforeBTNStartPos - tempSize;
                NextBTRT.anchorMax = BeforeBTNStartPos;
                NextBTRT.anchoredPosition = Vector2.zero;
                NextBTRT.sizeDelta = Vector2.zero;
                tempNextBTN.onClick.AddListener(InvenNextBTN);
                Managers.instance.UI.RegistEventTrigger(NextBTRT);
                nextBTN = tempNextBTN;
            }
            return nextBTN;
        }
    }
    Button beforeBTN;
    Button BeforeBTN
    {
        get
        {
            if (beforeBTN == null)
            {
                Button tempBeforeBTN = new GameObject("WeaponBeforeBTN").AddComponent<Button>();
                RectTransform BeforeBTRT = tempBeforeBTN.transform.AddComponent<RectTransform>();
                Image tempBeforeBTNIMG = BeforeBTRT.AddComponent<Image>();
                tempBeforeBTN.targetGraphic = tempBeforeBTNIMG;
                tempBeforeBTNIMG.sprite = Managers.instance.Resource.Load<Sprite>("select_arrow_panel_L");


                BeforeBTRT.SetParent(PlayerInventoryPanel.rectTransform);
                float percent = PlayerInventoryPanel.rectTransform.rect.size.x / PlayerInventoryPanel.rectTransform.rect.size.y;
                Vector2 BeforeBTNStartPos = new Vector2(0f, 0.4f);
                Vector2 tempSize = new Vector2(0.1f, 0.1f * percent);
                BeforeBTRT.anchorMin = BeforeBTNStartPos;
                BeforeBTRT.anchorMax = BeforeBTNStartPos + tempSize;
                BeforeBTRT.anchoredPosition = Vector2.zero;
                BeforeBTRT.sizeDelta = Vector2.zero;
                tempBeforeBTN.onClick.AddListener(InvenBeforeBTN);
                Managers.instance.UI.RegistEventTrigger(BeforeBTRT);
                beforeBTN = tempBeforeBTN;
            }
            return beforeBTN;
        }
    }



/*    private Image playerMoneyIMG;
    public Image PlayerMoneyIMG
    {
        get
        {
            if (playerMoneyIMG == null)
            {
                playerMoneyIMG = new GameObject("PlayerMoneyIMG").AddComponent<Image>();
                playerMoneyIMG.rectTransform.SetParent(PlayerMoneyPanel.rectTransform);

                playerMoneyIMG.rectTransform.anchoredPosition = Vector3.zero;
                Vector2 startPos = new Vector2(0.055f, 0.225f);
                Vector2 tempSize = new Vector2(0.15f, 0.15f * (PlayerMoneyPanel.rectTransform.rect.size.x / PlayerMoneyPanel.rectTransform.rect.size.y));
                playerMoneyIMG.rectTransform.anchorMax = startPos + tempSize;
                playerMoneyIMG.rectTransform.anchorMin = startPos;
                playerMoneyIMG.rectTransform.sizeDelta = Vector2.zero;
                PlayerMoneyIMG.sprite = Managers.instance.Resource.Load<Sprite>("coin");
                //TODO : ShopPanel이 업로드시 해당 키값 작성 요망
            }
            return playerMoneyIMG;
        }
    }*/
    private Text goldAmountText;
    public Text GoldAmountText
    {
        get
        {
            if (goldAmountText == null)
            {
                goldAmountText = new GameObject { name = "GoldAmountText" }.AddComponent<Text>();
                goldAmountText.rectTransform.SetParent(PlayerMoneyPannel.rectTransform);
                goldAmountText.rectTransform.anchorMin = new Vector2(0.400000006f, 0.224999994f);
                goldAmountText.rectTransform.anchorMax = Vector2.one;
                goldAmountText.rectTransform.sizeDelta = Vector2.zero;
                goldAmountText.rectTransform.anchoredPosition = Vector2.zero;
                goldAmountText.fontSize = 70;
                goldAmountText.resizeTextForBestFit = true;
                goldAmountText.color = Color.white;
                goldAmountText.font = Managers.instance.Resource.Load<Font>("InGameFont");
                goldAmountText.alignment = TextAnchor.LowerLeft;
                goldAmountText.horizontalOverflow = HorizontalWrapMode.Wrap;

            }
            return goldAmountText;
        }
    }
    private (Image, Text)[] inventoryIcons = new (Image, Text)[6];
    public (Image, Text)[] InventoryIcons
    {
        get
        {
            if (inventoryIcons.Length == 0)
            {
                inventoryIcons = new (Image, Text)[6];
            }
            for (int i = 0; i < inventoryIcons.Length; i++)
            {
                if (inventoryIcons[i].Item1 == null || inventoryIcons[i].Item2 == null)
                {
                    if (inventoryIcons[i].Item1 == null)
                    {
                        float xAixs = (float)i % 2f == 0f ? 0.27f : 0.62f;
                        //i가 홀수일때 x는 오른쪽에 위치하도록,짝수 혹은 0일때 왼쪽에 위치하도록
                        float yAixs = xAixs == 0.27f ? ((float)i / 2f) / 3f : (((float)i - 1f) / 2f) / 3f;
                        Vector2 maxValue = new Vector2(xAixs, 0.93f - yAixs);
                        float percent = PlayerInventoryPanel.rectTransform.rect.size.x / PlayerInventoryPanel.rectTransform.rect.size.y;
                        Vector2 IconSize = new Vector2(0.16f, 0.16f * percent);
                        //i를 2로 나눴을때 1이 나오면 y가 중앙에 위치하도록 설정,그렇지 않으면 x에서 구한 홀수 짝수를 이용해 값을 구함
                        inventoryIcons[i].Item1 = new GameObject("WeaponIconImage" + i).AddComponent<Image>();
                        inventoryIcons[i].Item1.rectTransform.SetParent(PlayerInventoryPanel.rectTransform);
                        inventoryIcons[i].Item1.rectTransform.anchorMin = maxValue - (Vector2.up * IconSize.y);
                        inventoryIcons[i].Item1.rectTransform.anchorMax = maxValue + (Vector2.right * IconSize.x);
                        inventoryIcons[i].Item1.rectTransform.anchoredPosition = Vector2.zero;
                        inventoryIcons[i].Item1.rectTransform.sizeDelta = Vector2.zero;
                    }
                    else
                    {
                        float xAixs = (float)i % 2f == 0f ? 0f : 0.5f;
                        //i가 홀수일때 x는 오른쪽에 위치하도록,짝수 혹은 0일때 왼쪽에 위치하도록
                        float yAixs = xAixs == 0 ? ((float)i / 2f) / 3f : (((float)i - 1f) / 2f) / 3f;
                        Vector2 maxValue = new Vector2(xAixs, 1 - yAixs);
                        //i를 2로 나눴을때 1이 나오면 y가 중앙에 위치하도록 설정,그렇지 않으면 x에서 구한 홀수 짝수를 이용해 값을 구함
                        inventoryIcons[i].Item1.rectTransform.SetParent(PlayerInventoryPanel.rectTransform);
                        inventoryIcons[i].Item1.rectTransform.anchorMin = maxValue - (Vector2.one * 0.11f);
                        inventoryIcons[i].Item1.rectTransform.anchorMax = maxValue;
                        inventoryIcons[i].Item1.rectTransform.anchoredPosition = Vector2.zero;
                        inventoryIcons[i].Item1.rectTransform.sizeDelta = Vector2.zero;
                    }
                    if (inventoryIcons[i].Item2 == null)
                    {
                        inventoryIcons[i].Item2 = new GameObject("WeaponAmountText" + i).AddComponent<Text>();
                        inventoryIcons[i].Item2.font = Managers.instance.Resource.Load<Font>("InGameFont");
                        inventoryIcons[i].Item2.fontSize = 40;
                        inventoryIcons[i].Item2.alignment = TextAnchor.LowerLeft;
                        inventoryIcons[i].Item2.color = Color.gray;
                        inventoryIcons[i].Item2.rectTransform.SetParent(inventoryIcons[i].Item1.rectTransform);
                        inventoryIcons[i].Item2.rectTransform.anchorMax = Vector2.one + Vector2.right;
                        inventoryIcons[i].Item2.rectTransform.anchorMin = Vector2.right;
                        inventoryIcons[i].Item2.rectTransform.anchoredPosition = Vector2.zero;
                        inventoryIcons[i].Item2.rectTransform.sizeDelta = Vector2.zero;
                    }
                    else
                    {
                        inventoryIcons[i].Item2.rectTransform.SetParent(inventoryIcons[i].Item1.rectTransform);
                        inventoryIcons[i].Item2.rectTransform.anchorMax = Vector2.one + Vector2.right;
                        inventoryIcons[i].Item2.rectTransform.anchorMin = Vector2.right;
                        inventoryIcons[i].Item2.rectTransform.anchoredPosition = Vector2.zero;
                        inventoryIcons[i].Item2.rectTransform.sizeDelta = Vector2.zero;

                    }
                }
            }

            return inventoryIcons;
        }
    }
    public List<(Sprite, string)> Inventory = new List<(Sprite, string)>();
    int invenPage;
    int NowPage
    {
        set
        {
            if (value <= 0)
            {
                BeforeBTN.interactable = false;
            }
            else
            {
                BeforeBTN.interactable = true;
            }
            if (value < 0)
            {
                invenPage = 0;
            }
            else
            {
                SetInvenIMG(value);
                invenPage = value;
            }
        }
    }
    private Image shoppingPanel;
    public Image ShoppingPanel
    {
        get
        {
            if (shoppingPanel == null)
            {
                shoppingPanel = new GameObject("shoppingPanel").AddComponent<Image>();
                shoppingPanel.rectTransform.SetParent(ShopInnerShopPanel.rectTransform);
                shoppingPanel.rectTransform.anchorMax = Vector2.one;
                shoppingPanel.rectTransform.anchorMin = new Vector2(0.33f, 0f);
                shoppingPanel.rectTransform.sizeDelta = Vector2.zero;
                shoppingPanel.rectTransform.anchoredPosition = Vector2.zero;
                shoppingPanel.sprite = Managers.instance.Resource.Load<Sprite>("shop_buy_panel");
            }
            return shoppingPanel;
        }
    }
    public Button[] shopWeaponItems;
    public void CreateWeaponBuyButtons(ExtraBallStat stat, int ballArray)
    {
        if (shopWeaponItems == null)
        {
            shopWeaponItems = new Button[0];
        }
        if (ballArray >= shopWeaponItems.Length)
        {
            Array.Resize(ref shopWeaponItems, shopWeaponItems.Length + 1);
            int arrayNum = shopWeaponItems.Length - 1;
            shopWeaponItems[arrayNum] = new GameObject("ShopWeaponBTN" + (shopWeaponItems.Length)).AddComponent<Button>();
            RectTransform tempParent = new GameObject("ShopWeaponBTNParentOBJ" + shopWeaponItems.Length).AddComponent<RectTransform>();
            tempParent.SetParent(ShoppingPanel.rectTransform);




            RectTransform tempRect = shopWeaponItems[arrayNum].AddComponent<RectTransform>();
            Image tempImage = shopWeaponItems[arrayNum].AddComponent<Image>();
            shopWeaponItems[arrayNum].targetGraphic = tempImage;
            tempRect.SetParent(tempParent);


            float shoppingPanelSizePercent = ShoppingPanel.rectTransform.rect.size.x / ShoppingPanel.rectTransform.rect.size.y;
            //얘를 버튼  y에 곱해주면 정사각형이 됨,X는 배열과 정비례하나 Y는 반비례함
            int tempClumm = arrayNum / 4;
            int tempRow = arrayNum % 4;
            /*        if (tempClumm > 0)
                    {
                        int tempLength = (shopWeaponItems.Length / 5) + shopWeaponItems.Length;
                        tempRow = tempLength % 5;
                        tempClumm = tempLength / 5;
                        tempRow = tempRow== 0 ? 1: tempRow ;
                    }*/

            float tempMaxX = tempRow * 0.25f;
            float tempMaxY = 1 - ((tempClumm * shoppingPanelSizePercent) * 0.25f);
            //row = 행 Clum = 열
            tempParent.anchorMax = new Vector2(tempMaxX + 0.25f, tempMaxY);
            tempParent.anchorMin = new Vector2(tempMaxX, tempMaxY + (-0.25f * shoppingPanelSizePercent));
            tempParent.sizeDelta = Vector2.zero;
            tempParent.anchoredPosition = Vector2.zero;

            Text priceText = new GameObject("ShopWeaponPriceText" + shopWeaponItems.Length).AddComponent<Text>();
            priceText.font = Managers.instance.Resource.Load<Font>("InGameFont");
            priceText.alignment = TextAnchor.UpperCenter;
            priceText.text = stat.ballPrice + "$";
            priceText.color = Color.gray;
            priceText.rectTransform.SetParent(tempParent);
            priceText.rectTransform.anchorMax = new Vector2(1f, 0.4f);
            priceText.rectTransform.anchorMin = Vector2.zero;
            priceText.rectTransform.sizeDelta = Vector2.zero;
            priceText.rectTransform.anchoredPosition = Vector2.zero;
            priceText.fontSize = (int)(70);

            tempRect.anchorMax = new Vector2(0.8f, 1);
            tempRect.anchorMin = new Vector2(0.2f, 0.4f);
            tempRect.sizeDelta = Vector2.zero;
            tempRect.anchoredPosition = Vector2.zero;
            tempImage.sprite = Managers.instance.Resource.Load<Sprite>(stat.ballName);
        }
        else
        {
            shopWeaponItems[ballArray].GetComponent<Image>().sprite = Managers.instance.Resource.Load<Sprite>(stat.ballName);
            shopWeaponItems[ballArray].transform.parent.Find("ShopWeaponPriceText" + ballArray + 1);
            for (int i = ballArray + 1; i < shopWeaponItems.Length; i++)
            {
                shopWeaponItems[i].transform.parent.transform.gameObject.SetActive(false);
            }
            shopWeaponItems[ballArray].transform.parent.transform.gameObject.SetActive(true);
        }
        Managers.instance.UI.RegistEventTrigger(shopWeaponItems[ballArray].transform as RectTransform);
        shopWeaponItems[ballArray].onClick.RemoveAllListeners();
        shopWeaponItems[ballArray].onClick.AddListener(() =>
        {
            Managers.instance.PlayerDataManager.AddBall(stat, true);
        });


    }
    #endregion
    public void InvenBeforeBTN()
    {
        NowPage = invenPage - 1;
    }
    public void InvenNextBTN()
    {
        NowPage = invenPage + 1;
    }
    public void UpdateInvenBulb(List<ExtraBallStat> ballList)
    {
        (Sprite, string) tempSet;
        for (int i = 0; i < ballList.Count; i++)
        {
            if (i >= Inventory.Count || Inventory.Count == 0)
            {
                NowPage = i / 6;
                tempSet.Item1 = Managers.instance.Resource.Load<Sprite>(ballList[i].ballName);
                tempSet.Item2 = ballList[i].amount.ToString();
                Inventory.Add(tempSet);
            }
            else if (Inventory[i].Item1.name != ballList[i].ballName || ballList[i].amount.ToString() != Inventory[i].Item2.ToString())
            {
                NowPage = i / 6;
                if (Inventory[i].Item1.name != ballList[i].ballName && ballList[i].amount.ToString() != Inventory[i].Item2.ToString())
                {
                    tempSet.Item1 = Managers.instance.Resource.Load<Sprite>(ballList[i].ballName);
                    tempSet.Item2 = ballList[i].amount.ToString();
                    Inventory[i] = tempSet;
                }
                else if (Inventory[i].Item1.name != ballList[i].ballName)
                {
                    tempSet.Item1 = Managers.instance.Resource.Load<Sprite>(ballList[i].ballName);
                    tempSet.Item2 = Inventory[i].Item2;
                    Inventory[i] = tempSet;
                }
                else if (ballList[i].amount.ToString() != Inventory[i].Item2.ToString())
                {
                    tempSet.Item1 = Inventory[i].Item1;
                    tempSet.Item2 = ballList[i].amount.ToString();
                    Inventory[i] = tempSet;
                }
            }

            if (invenPage < 0)
            {
                NowPage = 0;
            }
        }
        SetInvenIMG();
    }
    private void SetInvenIMG(int targetPage = -1)
    {
        if (targetPage == -1)
        {
            targetPage = invenPage;
        }
        int caculatePage = 1 + targetPage;
        int arrayUI = 0;
        for (int i = targetPage * 6; i <= (caculatePage * 5) + targetPage; i++)
        {
            if (Inventory.Count > i)
            {
                NextBTN.interactable = true;
                InventoryIcons[arrayUI].Item1.sprite = Inventory[i].Item1;
                InventoryIcons[arrayUI].Item2.text = Inventory[i].Item2;
                InventoryIcons[arrayUI].Item1.gameObject.SetActive(true);
                InventoryIcons[arrayUI].Item2.gameObject.SetActive(true);
            }
            else
            {
                NextBTN.interactable = false;
                InventoryIcons[arrayUI].Item1.sprite = null;
                InventoryIcons[arrayUI].Item2.text = string.Empty;
                InventoryIcons[arrayUI].Item1.gameObject.SetActive(false);
                InventoryIcons[arrayUI].Item2.gameObject.SetActive(false);

            }
            arrayUI++;
        }
    }
    public void MoneyUpdate(int money)
    {
        IsShopActivate = Managers.instance.UI.ShopUICall.IsShopActivate == true ? true : false;
        GoldAmountText.text = money.ToString();
    }
    public void ShopUISetting()
    {
        MerchantPortrait.enabled = true;
        MerchantDialogPanel.enabled = true;
        PlayerInventoryPanel.enabled = true;
        MoneyUpdate(Managers.instance.PlayerDataManager.PlayerMoney);
        ShopPanel.gameObject.SetActive(false);
        UpdateInvenBulb(Managers.instance.PlayerDataManager.playerOwnBalls);
        NextBTN.enabled = true;
        BeforeBTN.enabled = true;
        ShoppingPanel.enabled = true;
    }

}
public class OptionUI
{
    private Image optionPannel;
    public Image OptionPannel 
    { 
        get 
        {
            if (optionPannel == null)
            {
                optionPannel = new GameObject("OptionPannel").AddComponent<Image>();
                RectTransform tempRect = optionPannel.rectTransform;
                optionPannel.rectTransform.SetParent(Managers.instance.UI.LoadingUIProps.SceneMainCanvas);
                Sprite tempSprite = Managers.instance.Resource.Load<Sprite>("Option_background");
                optionPannel.sprite = tempSprite;
                float GetPercent = 1;
                //TODO : 키값 입력필요
                for (int i = 0; i < ((int)tempSprite.rect.height).ToString().Length; i++)
                {
                    GetPercent = GetPercent * 0.1f;
                }
                Vector2 size = (new Vector2(tempSprite.rect.width* (Managers.instance.UI.LoadingUIProps.SceneMainCanvas.rect.height/Managers.instance.UI.LoadingUIProps.SceneMainCanvas.rect.width), tempSprite.rect.height) * GetPercent)/2f;
                Vector2 pos = new Vector2(0.5f,0.5f);
                Managers.instance.UI.SetUISize(ref tempRect, pos-size , pos + size);
                ParabolaCheckBox.gameObject.SetActive(true);
                SFXSlider.gameObject.SetActive(true);
                BGMSlider.gameObject.SetActive(true);
                ExitGameBTN.gameObject.SetActive(true);
                ExitOptionBTN.gameObject.SetActive(true);
            }
            return optionPannel; 
        }
    }


    private Toggle parabolaCheckBox;
    private Toggle ParabolaCheckBox
    {
        get
        {
            if (parabolaCheckBox == null)
            {

                parabolaCheckBox = new GameObject("parabolaCheckbox").AddComponent<Toggle>();
                RectTransform CheckBoxParent = parabolaCheckBox.transform as RectTransform;
                CheckBoxParent.SetParent(OptionPannel.rectTransform);
                Vector2 centerPosition = new Vector2(0.5f,0.3f);
                Vector2 parentHalfSize = new Vector2(0.2666f, 0.05f)/2f;
                Managers.instance.UI.SetUISize(ref CheckBoxParent,centerPosition-parentHalfSize, centerPosition + parentHalfSize);
                Image CheckBoxTarget = new GameObject("CheckBoxBackGround").AddComponent<Image>();
                CheckBoxTarget.rectTransform.SetParent(CheckBoxParent);


                RectTransform CheckBoxRect = CheckBoxTarget.rectTransform;
                Managers.instance.UI.SetUISize(ref CheckBoxRect,new Vector2(1f- (CheckBoxParent.rect.height / CheckBoxParent.rect.width ), 0),Vector2.one);
                CheckBoxTarget.sprite = Managers.instance.Resource.Load<Sprite>("Option_checkbox");

                Image Checker = new GameObject("parabolaChecker").AddComponent<Image>();
                RectTransform checkerRect = Checker.rectTransform;
                checkerRect.SetParent(CheckBoxRect);
                Managers.instance.UI.SetUISize(ref checkerRect,Vector2.zero,Vector2.one);
                Text tempText = new GameObject("PrabolaText").AddComponent<Text>();
                tempText.text = "궤적 표시";
                tempText.rectTransform.SetParent(CheckBoxParent);
                tempText.font = Managers.instance.Resource.Load<Font>("InGameFont");
                tempText.color = Color.black;
                tempText.resizeTextForBestFit = true;
                tempText.alignment = TextAnchor.MiddleCenter;
                RectTransform tempTextRect = tempText.rectTransform;
                Managers.instance.UI.SetUISize(ref tempTextRect, Vector2.zero, CheckBoxRect.anchorMin+Vector2.up);
                
                Outline TextOutLine = tempText.AddComponent<Outline>();
                TextOutLine.effectColor = Color.white;
                TextOutLine.effectDistance = Vector2.down + Vector2.right;
                TextOutLine.useGraphicAlpha = true;


                parabolaCheckBox.targetGraphic = CheckBoxTarget;
                parabolaCheckBox.graphic = Checker;
                parabolaCheckBox.isOn = Managers.instance.PlayerDataManager.isParabolaTurnOn;
                Managers.instance.UI.RegistEventTrigger(CheckBoxParent);
                parabolaCheckBox.onValueChanged.AddListener((checkStatus) =>
                {
                    ChangedCheckBoxValue();
                });
            }
            return parabolaCheckBox;
        }
    }
    public bool ChangedCheckBoxValue()
    {
        bool value = ParabolaCheckBox.isOn;
        Managers.instance.PlayerDataManager.isParabolaTurnOn = value;
        if (ShoterController.Instance != null)
        {
            ShoterController.Instance.isParabolaOn = value;
            ShoterController.Instance.ParabolaOnOFF(value);

        }
        ParabolaCheckBox.isOn = value;
        return value;

    }

    private Slider sfxSlider = null;
    private Slider SFXSlider
    {
        get
        {
            if (sfxSlider == null)
            {
                Slider tempSFX = new GameObject("SFXSlider").AddComponent<Slider>();
                tempSFX.wholeNumbers = false;
                tempSFX.maxValue = 1;
                //TODO : enemy 체력 구현하면 여기에 넣어줘야함

                RectTransform tempSFXRect = tempSFX.transform as RectTransform;
                tempSFXRect.gameObject.SetActive(false);

                RectTransform tempHandleArea = new GameObject("HandleArea").AddComponent<RectTransform>();
                tempHandleArea.SetParent(tempSFXRect);

                tempHandleArea.pivot = new Vector2(0.5f, 0.5f); // 부모의 왼쪽 중간을 기준으로
                tempHandleArea.sizeDelta = Vector2.zero;
                tempHandleArea.anchoredPosition = Vector2.zero;


                Image handle = new GameObject("handle").AddComponent<Image>();
                handle.rectTransform.SetParent(tempHandleArea);
                handle.rectTransform.anchorMax = Vector2.up;
                handle.rectTransform.anchorMin = Vector2.zero;
                handle.rectTransform.pivot = Vector2.one / 2f;
                tempSFX.handleRect = handle.rectTransform;
                handle.sprite = Managers.instance.Resource.Load<Sprite>("Option_point");

                Vector2 handleSize = new Vector2(handle.sprite.rect.size.x, handle.sprite.rect.size.y);


                handle.rectTransform.anchoredPosition = Vector2.zero;
                handle.rectTransform.sizeDelta = handleSize;
                handle.color = Color.white;


                Image BackGround = new GameObject("BackGround").AddComponent<Image>();
                BackGround.rectTransform.SetParent(tempSFXRect);
                BackGround.rectTransform.anchorMax = Vector2.one;
                BackGround.rectTransform.anchorMin = Vector2.zero;
                BackGround.rectTransform.sizeDelta = Vector2.zero;
                BackGround.rectTransform.pivot = Vector2.one / 2f;
                BackGround.color = Color.white;
                BackGround.sprite = Managers.instance.Resource.Load<Sprite>("Option_soundbar_empty");
                BackGround.rectTransform.SetAsFirstSibling();


                RectTransform tempFillArea = new GameObject("FillArea").AddComponent<RectTransform>();
                tempFillArea.SetParent(tempSFXRect);
                tempFillArea.anchorMin = Vector2.zero; // 부모의 왼쪽 하단을 기준으로
                tempFillArea.anchorMax = Vector2.one; // 부모의 왼쪽 상단을 기준으로
                tempFillArea.pivot = new Vector2(0.5f, 0.5f); // 부모의 왼쪽 중간을 기준으로
                tempFillArea.sizeDelta = Vector2.zero;
                tempFillArea.anchoredPosition = Vector2.zero;
                Image tempIMG = new GameObject("FillRect").AddComponent<Image>();
                tempIMG.rectTransform.SetParent(tempFillArea);
                tempIMG.rectTransform.sizeDelta = Vector2.zero;
                tempIMG.color = Color.white;
                tempIMG.sprite = Managers.instance.Resource.Load<Sprite>("Option_soundbar_full");
                tempSFX.fillRect = tempIMG.rectTransform;

                // Slider의 부모-자식 관계 설정  


                //슬라이더바 위치 설정
                Vector2 CenterPos = new Vector2(0.5f, 0.486111f);
                Vector2 BackgroundSize = BackGround.sprite.bounds.size * (4f/5f);
                float percent = 1;
                for (int i = 0; i < ((int)BackgroundSize.x).ToString().Length; i++)
                {
                    percent = percent * 0.1f;
                }
                BackgroundSize.x = BackgroundSize.x * (OptionPannel.rectTransform.rect.size.x / OptionPannel.rectTransform.rect.size.y);
                BackgroundSize = BackgroundSize * percent;

                tempSFXRect.SetParent(OptionPannel.rectTransform);
                tempSFXRect.SetAsLastSibling();
                Managers.instance.UI.SetUISize(ref tempSFXRect, CenterPos - BackgroundSize, CenterPos + BackgroundSize);
                tempSFX.interactable = true;
                tempSFX.enabled = true;
                tempSFX.SetDirection(Slider.Direction.LeftToRight, true);

                //핸들 부모 크기 설정
                Vector2 handleParentSize = handleSize;
                float widPerHei = tempSFXRect.rect.size.y / tempSFXRect.rect.size.x;
                percent = 1;
                for (int i = 0; i < ((int)handle.sprite.rect.size.x).ToString().Length; i++)
                {
                    percent = percent * 0.1f;
                }
                handleParentSize = handleParentSize * percent;
                handleParentSize = new Vector2(handleParentSize.x , handleParentSize.y * widPerHei)/2;


                tempHandleArea.anchorMin = new Vector2(0f, (-1f / 3f)- (handleParentSize.y*2)); // 부모의 왼쪽 하단을 기준으로
                tempHandleArea.anchorMax = new Vector2(1f, (-1f / 3f)- handleParentSize.y); // 부모의 왼쪽 상단을 기준으로
                tempHandleArea.anchoredPosition = Vector2.down * BackGround.sprite.rect.size.y / 3f;
                sfxSlider = tempSFX;
                sfxSlider.value = Managers.instance.SoundManager.SFX.volume;
                tempHandleArea.SetAsLastSibling();

                sfxSlider.onValueChanged.RemoveAllListeners();
                sfxSlider.onValueChanged.AddListener((value) =>
                {
                    Managers.instance.SoundManager.SetSoundValue(false, value);
                });

                Text tempText = new GameObject("SFXText").AddComponent<Text>();
                tempText.text = "효과음";
                tempText.rectTransform.SetParent(tempSFXRect);
                tempText.font = Managers.instance.Resource.Load<Font>("InGameFont");
                tempText.color = Color.black;
                tempText.resizeTextForBestFit = true;
                tempText.alignment = TextAnchor.MiddleCenter;
                RectTransform tempTextRect = tempText.rectTransform;
                Managers.instance.UI.SetUISize(ref tempTextRect, Vector2.up, Vector2.one + (Vector2.up * 4f));

                Outline TextOutLine = tempText.AddComponent<Outline>();
                TextOutLine.effectColor = Color.white;
                TextOutLine.effectDistance = Vector2.down + Vector2.right;
                TextOutLine.useGraphicAlpha = true;


            }
            return sfxSlider;
        }
    }
    private Slider bgmSlider = null;
    private Slider BGMSlider
    {
        get
        {
            if (bgmSlider == null)
            {
                Slider tempBGMSlider = new GameObject("BGMSlider").AddComponent<Slider>();
                tempBGMSlider.wholeNumbers = false;
                tempBGMSlider.maxValue = 1;
                //TODO : enemy 체력 구현하면 여기에 넣어줘야함

                RectTransform tempBGMSliderRect = tempBGMSlider.transform as RectTransform;
                tempBGMSliderRect.gameObject.SetActive(false);

                RectTransform tempHandleArea = new GameObject("HandleArea").AddComponent<RectTransform>();
                tempHandleArea.SetParent(tempBGMSliderRect);

                tempHandleArea.pivot = new Vector2(0.5f, 0.5f); // 부모의 왼쪽 중간을 기준으로
                tempHandleArea.sizeDelta = Vector2.zero;
                tempHandleArea.anchoredPosition = Vector2.zero;


                Image handle = new GameObject("handle").AddComponent<Image>();
                handle.rectTransform.SetParent(tempHandleArea);
                handle.rectTransform.anchorMax = Vector2.up;
                handle.rectTransform.anchorMin = Vector2.zero;
                handle.rectTransform.pivot = Vector2.one / 2f;
                tempBGMSlider.handleRect = handle.rectTransform;
                handle.sprite = Managers.instance.Resource.Load<Sprite>("Option_point");

                Vector2 handleSize = new Vector2(handle.sprite.rect.size.x, handle.sprite.rect.size.y);


                handle.rectTransform.anchoredPosition = Vector2.zero;
                handle.rectTransform.sizeDelta = handleSize;
                handle.color = Color.white;


                Image BackGround = new GameObject("BackGround").AddComponent<Image>();
                BackGround.rectTransform.SetParent(tempBGMSliderRect);
                BackGround.rectTransform.anchorMax = Vector2.one;
                BackGround.rectTransform.anchorMin = Vector2.zero;
                BackGround.rectTransform.sizeDelta = Vector2.zero;
                BackGround.rectTransform.pivot = Vector2.one / 2f;
                BackGround.color = Color.white;
                BackGround.sprite = Managers.instance.Resource.Load<Sprite>("Option_soundbar_empty");
                BackGround.rectTransform.SetAsFirstSibling();


                RectTransform tempFillArea = new GameObject("FillArea").AddComponent<RectTransform>();
                tempFillArea.SetParent(tempBGMSliderRect);
                tempFillArea.anchorMin = Vector2.zero; // 부모의 왼쪽 하단을 기준으로
                tempFillArea.anchorMax = Vector2.one; // 부모의 왼쪽 상단을 기준으로
                tempFillArea.pivot = new Vector2(0.5f, 0.5f); // 부모의 왼쪽 중간을 기준으로
                tempFillArea.sizeDelta = Vector2.zero;
                tempFillArea.anchoredPosition = Vector2.zero;
                Image tempIMG = new GameObject("FillRect").AddComponent<Image>();
                tempIMG.rectTransform.SetParent(tempFillArea);
                tempIMG.rectTransform.sizeDelta = Vector2.zero;
                tempIMG.color = Color.white;
                tempIMG.sprite = Managers.instance.Resource.Load<Sprite>("Option_soundbar_full");
                tempBGMSlider.fillRect = tempIMG.rectTransform;

                // Slider의 부모-자식 관계 설정  


                //슬라이더바 위치 설정
                Vector2 CenterPos = new Vector2(0.5f, 1 - 0.309722f);

                Vector2 BackgroundSize = BackGround.sprite.bounds.size*(4f/5f);
                float percent = 1;
                for (int i = 0; i < ((int)BackgroundSize.x).ToString().Length; i++)
                {
                    percent = percent * 0.1f;
                }
                BackgroundSize.x = BackgroundSize.x * (OptionPannel.rectTransform.rect.size.x / OptionPannel.rectTransform.rect.size.y);
                BackgroundSize = BackgroundSize * percent;

                tempBGMSliderRect.SetParent(OptionPannel.rectTransform);
                tempBGMSliderRect.SetAsLastSibling();
                Managers.instance.UI.SetUISize(ref tempBGMSliderRect, CenterPos - BackgroundSize, CenterPos + BackgroundSize);
                tempBGMSlider.interactable = true;
                tempBGMSlider.enabled = true;
                tempBGMSlider.SetDirection(Slider.Direction.LeftToRight, true);

                //핸들 부모 크기 설정
                Vector2 handleParentSize = handleSize;
                float widPerHei = tempBGMSliderRect.rect.size.y / tempBGMSliderRect.rect.size.x;
                percent = 1;
                for (int i = 0; i < ((int)handle.sprite.rect.size.x).ToString().Length; i++)
                {
                    percent = percent * 0.1f;
                }
                handleParentSize = handleParentSize * percent;
                handleParentSize = new Vector2(handleParentSize.x, handleParentSize.y * widPerHei) / 2;


                tempHandleArea.anchorMin = new Vector2(0f, (-1f / 3f) - (handleParentSize.y * 2)); // 부모의 왼쪽 하단을 기준으로
                tempHandleArea.anchorMax = new Vector2(1f, (-1f / 3f) - handleParentSize.y); // 부모의 왼쪽 상단을 기준으로
                tempHandleArea.anchoredPosition = Vector2.down * BackGround.sprite.rect.size.y / 3f;
                bgmSlider = tempBGMSlider;
                bgmSlider.value = Managers.instance.SoundManager.BGM.volume;
                tempHandleArea.SetAsLastSibling();
                bgmSlider.onValueChanged.RemoveAllListeners();
                bgmSlider.onValueChanged.AddListener((value) =>
                {
                    Managers.instance.SoundManager.SetSoundValue(true, value);
                });



                Text tempText = new GameObject("BGMText").AddComponent<Text>();
                tempText.text = "배경음악";
                tempText.rectTransform.SetParent(tempBGMSliderRect);
                tempText.font = Managers.instance.Resource.Load<Font>("InGameFont");
                tempText.color = Color.black;
                tempText.resizeTextForBestFit = true;
                tempText.alignment = TextAnchor.MiddleCenter;
                RectTransform tempTextRect = tempText.rectTransform;
                Managers.instance.UI.SetUISize(ref tempTextRect, Vector2.up, Vector2.one+(Vector2.up*4f));

                Outline TextOutLine = tempText.AddComponent<Outline>();
                TextOutLine.effectColor = Color.white;
                TextOutLine.effectDistance = Vector2.down + Vector2.right;
                TextOutLine.useGraphicAlpha = true;
            }
            return bgmSlider;
        }
    }

    private Button exitGameBTN;
    public Button ExitGameBTN
    {
        get 
        {
            if (exitGameBTN == null)
            {
                exitGameBTN = new GameObject("ExitGameBTN").AddComponent<Button>();
                exitGameBTN.AddComponent<Image>().color = Color.clear;
                RectTransform tempRect = exitGameBTN.transform as RectTransform;
                tempRect.SetParent(OptionPannel.rectTransform);
                Vector2 centerPos = new Vector2(0.5f, 0.139f) ;
                Vector2 BTNSize = new Vector2(0.1f,0.025f);
                Managers.instance.UI.SetUISize(ref tempRect,centerPos-BTNSize, centerPos + BTNSize);
                Text tempText = new GameObject("ExitText").AddComponent<Text>();
                exitGameBTN.targetGraphic = tempText;
                RectTransform textTR = tempText.rectTransform;

                textTR.SetParent(tempRect);
                tempText.text = "게임종료";
                tempText.font = Managers.instance.Resource.Load<Font>("InGameFont");
                tempText.color = Color.red;
                tempText.resizeTextForBestFit = true;
                tempText.alignment = TextAnchor.MiddleCenter;
                tempText.resizeTextMaxSize = 1000;
                Managers.instance.UI.SetUISize(ref textTR, Vector2.zero, Vector2.one);

                Outline TextOutLine = tempText.AddComponent<Outline>();
                TextOutLine.effectColor = Color.white;
                TextOutLine.effectDistance = Vector2.down + Vector2.right;
                TextOutLine.useGraphicAlpha = true;
                exitGameBTN.onClick.RemoveAllListeners();
                exitGameBTN.onClick.AddListener(() =>
                {
                    Managers.instance.GameExitBTN();
                });
                Managers.instance.UI.RegistEventTrigger(tempRect);

            }
            return exitGameBTN;
        }
    }
    private Button exitOptionBTN;
    public Button ExitOptionBTN
    {
        get
        {
            if (exitOptionBTN == null)
            {
                exitOptionBTN = new GameObject("OptionExitBTn").AddComponent<Button>();
                //가장 우측 상단에 위치함으로 vector2.one을 기준으로 사이즈만큼 깎아서 min을 산정
                Image tempImage = exitOptionBTN.AddComponent<Image>();
                tempImage.sprite = Managers.instance.Resource.Load<Sprite>("Option_quit_button");
                RectTransform BTNRT = tempImage.rectTransform;
                BTNRT.SetParent(OptionPannel.rectTransform);
                float heiPerWid = OptionPannel.rectTransform.rect.size.x / OptionPannel.rectTransform.rect.size.y;
                Vector2 tempVec = tempImage.sprite.rect.size;
                tempVec.y = tempVec.y * heiPerWid;

                for (int i = 0; i < ((int)tempImage.sprite.rect.size.x).ToString().Length; i++)
                {
                    tempVec = tempVec * 0.1f;
                }

                Vector2 CenterPos = new Vector2(0.94252491f, 0.952696085f);
                tempVec = tempVec / 6f;

                Managers.instance.UI.SetUISize(ref BTNRT, CenterPos - tempVec, CenterPos);
                exitOptionBTN.targetGraphic = tempImage;
                exitOptionBTN.onClick.RemoveAllListeners();
                exitOptionBTN.onClick.AddListener(() =>
                {
                    Managers.instance.UI.TargetUIOnOff(OptionPannel.rectTransform, false);
                });
                Managers.instance.UI.RegistEventTrigger(BTNRT);
            }
            return exitOptionBTN;
        }
    }


    /*    public bool CheckBoxValue
        {
            get
            {
                if (OptionPannel.gameObject.activeSelf)
                {
                    Managers.instance.UI.TargetUIOnOff(OptionPannel.rectTransform, false);
                }
                return ParabolaCheckBox.isOn;
            }
            set
            {

                Managers.instance.PlayerDataManager.isParabolaTurnOn = value;
                if (ShoterController.Instance != null)
                {
                    ShoterController.Instance.isPrabolaOn = value;
                }
                ParabolaCheckBox.isOn = value;
            }
        }*/


}