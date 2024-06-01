using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using System;
using UnityEngine.UI;

namespace ChallengeSceneData
{
#if UNITY_EDITOR
    [CustomEditor(typeof(SceneDataSetter))]
    public class LoadSceneData : Editor
    {

        public override void OnInspectorGUI()           //유니티의 인스펙터 함수를 재정의
        {
            base.OnInspectorGUI();
            SceneDataSetter gameSystem = (SceneDataSetter)target;//유니티 인스펙터 함수 동작을 같이 한다.(Base)

            if (GUILayout.Button("GetScenes"))
            {
                gameSystem.GetArrayScenesInFolder();
            }
            if (GUILayout.Button("CheckScenes"))
            {
                gameSystem.CheckSceneInfo();
            }
        }

    }

#endif
    public class SceneDataSetter : MonoBehaviour
    {
        [SerializeField]private Button shopUIBTN;
        [SerializeField]private RectTransform scenePickPannel;
        [SerializeField]public Button sceneUIBTN;
        [SerializeField]private Button sceneBeforeBTN;
        [SerializeField]private Button sceneAfterBTN;
        [SerializeField]private int scenePage = 0;
        [SerializeField]private int maxPage = 0;
        private int ScenePage
        {
            get
            {
                return scenePage;
            }
            set
            {
                if (value > 0)
                {
                    sceneBeforeBTN.interactable = true;
                    if (maxPage == value)
                    {
                        sceneAfterBTN.interactable = false;
                    }
                    scenePage = value >= maxPage ? maxPage : value;
                }
                else
                {
                    sceneBeforeBTN.interactable = false;
                    if (maxPage> 0)
                    {
                        sceneAfterBTN.interactable = true;
                    }
                    scenePage = 0;
                }
                
            }
        }

        [SerializeField]public Button[] sceneButtons;
        [SerializeField]public string[] challengeSceneNames;

        private void Awake()
        {
            shopUIBTN.onClick.RemoveAllListeners();
            sceneUIBTN.onClick.AddListener(() =>
            {
                Managers.instance.UI.TargetUIOnOff(scenePickPannel, !scenePickPannel.gameObject.activeSelf);
            });
            shopUIBTN.onClick.AddListener(() =>
            {
                Managers.instance.UI.shopUICall.IsShopActivate = Managers.instance.UI.shopUICall.IsShopActivate == true ? false : true;
            });
            maxPage = challengeSceneNames.Length%40 == 0? (challengeSceneNames.Length / 40) -1: challengeSceneNames.Length / 40;
            if (challengeSceneNames != null)
            {
                for (int i = 0; i < challengeSceneNames.Length; i++)
                {
                    if (i >= 40) break;
                    Array.Resize(ref sceneButtons,i+1);
                    int tempArray = i;
                    //closure 이슈로 인해 변수를 따로 만들어줌
                    Text buttonText;
                    if (scenePickPannel.transform.GetChild(2).childCount <= i)
                    {
                        //페이지 넘기는 버튼으로 인해 배열 +2가 됨
                        sceneButtons[i] = new GameObject("Stage" + i).AddComponent<Button>();
                        sceneButtons[i].transform.parent = scenePickPannel.transform.GetChild(2);
                        Image tempButtonIMG = sceneButtons[i].AddComponent<Image>();
                        sceneButtons[i].targetGraphic = tempButtonIMG;
                        tempButtonIMG.sprite = Managers.instance.Resource.Load<Sprite>("Stage_button");
                        buttonText = new GameObject("ButtonText").AddComponent<Text>();
                        buttonText.transform.SetParent(sceneButtons[i].transform);
                        buttonText.color = Color.black;
                    }
                    else
                    {
                        sceneButtons[i] = scenePickPannel.transform.GetChild(2).GetChild(i).GetComponent<Button>();
                        buttonText = sceneButtons[i].transform.GetChild(0).GetComponent<Text>();
                    }
                    buttonText.font = Managers.instance.Resource.Load<Font>("InGameFont");
                    buttonText.resizeTextForBestFit = true;
                    buttonText.resizeTextMaxSize = 30;
                    buttonText.alignment = TextAnchor.MiddleCenter;

                    buttonText.text = (i+1).ToString();
                    sceneButtons[i].onClick.RemoveAllListeners();
                    sceneButtons[i].onClick.AddListener(() => 
                    {
                        ToChallengeScene(tempArray);
                    });
                }
            }
            else
            {
#if UNITY_EDITOR
                GetArrayScenesInFolder();
#endif

            }
            ScenePage = 0;
            sceneBeforeBTN.onClick.AddListener(() =>
            {
                ChangePage(false);
            });
            sceneAfterBTN.onClick.AddListener(() =>
            {
                ChangePage(true);
            });

        }

#if UNITY_EDITOR
        public string folderPath; // 씬 파일들이 위치한 폴더
        public void GetArrayScenesInFolder()
        {
            if (folderPath == string.Empty) 
            {
                Debug.Log("Empty folderPath String");
                return; 
            }
            var scenePaths = Directory.GetFiles(folderPath, "*.unity", SearchOption.AllDirectories);
            Array.Resize(ref challengeSceneNames, 0);
            foreach (var path in scenePaths)
            {
                Debug.Log("Found scene: " + path);
                Debug.Log(challengeSceneNames.Length + 1);

                Array.Resize(ref challengeSceneNames, challengeSceneNames.Length+1);
                string tempNameSceneName = path.Replace(".unity", "");
                tempNameSceneName = tempNameSceneName.Replace(folderPath+ "\\", "");
                challengeSceneNames[challengeSceneNames.Length -1] = tempNameSceneName;
            }
        }
        public void CheckSceneInfo()
        {
            for (int i = 0; i < challengeSceneNames.Length; i++)
            {
                Debug.Log(challengeSceneNames[i]);
            }
        }

#endif
        public void ChangePage(bool isNextBTN)
        {
            ScenePage = isNextBTN ? ScenePage +1: ScenePage - 1;
            int tempNumber = ScenePage * 40;
            for (int i = 0; i < 40; i++)
            {
                if (challengeSceneNames.Length > tempNumber + i)
                {
                    sceneButtons[i].onClick.RemoveAllListeners();
                    int stageNumber = tempNumber + i;
                    sceneButtons[i].onClick.AddListener(() =>
                    {
                        ToChallengeScene(stageNumber);
                    });
                    sceneButtons[i].gameObject.SetActive(true);
                    sceneButtons[i].transform.GetChild(0).GetComponent<Text>().text = (tempNumber + i+1).ToString();
                }
                else
                {
                    sceneButtons[i].gameObject.SetActive(false);
                }
            }
        }
        public void ToChallengeScene(int targetArray)
        {

            SceneManager.LoadScene(challengeSceneNames[targetArray]);
        }
    }
}

