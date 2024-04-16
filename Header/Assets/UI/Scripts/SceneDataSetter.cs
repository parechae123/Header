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

        public override void OnInspectorGUI()           //����Ƽ�� �ν����� �Լ��� ������
        {
            base.OnInspectorGUI();
            SceneDataSetter gameSystem = (SceneDataSetter)target;//����Ƽ �ν����� �Լ� ������ ���� �Ѵ�.(Base)

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
                Managers.instance.UI.ShopUICall.IsShopActivate = Managers.instance.UI.ShopUICall.IsShopActivate == true ? false : true;
            });
            if (challengeSceneNames != null)
            {
                for (int i = 0; i < challengeSceneNames.Length; i++)
                {
                    Array.Resize(ref sceneButtons,i+1);
                    int tempArray = i;
                    //closure �̽��� ���� ������ ���� �������
                    sceneButtons[i] = scenePickPannel.GetChild(i).GetComponent<Button>();
                    sceneButtons[i].onClick.RemoveAllListeners();
                    sceneButtons[i].onClick.AddListener(() => 
                    {
                        
                        ToChallengeScene(tempArray);
                    });
                }
            }
            else
            {
                GetArrayScenesInFolder();
            }
        }

#if UNITY_EDITOR
        public string folderPath; // �� ���ϵ��� ��ġ�� ����
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
        public void ToChallengeScene(int targetArray)
        {

            SceneManager.LoadScene(challengeSceneNames[targetArray]);
        }

#endif
    }
}

