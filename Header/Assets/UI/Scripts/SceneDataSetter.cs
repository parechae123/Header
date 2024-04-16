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
        [SerializeField]private Button shopUI;
        [SerializeField]private RectTransform challengeScenePickPannel;
        [SerializeField]public Button[] challengeSceneButtons;
        [SerializeField]public string[] challengeScenes;

        private void Awake()
        {
/*            shopUI.onClick.RemoveAllListeners();
            shopUI.onClick.AddListener(() =>
            {
                Managers.instance.UI.ShopUICall.IsShopActivate = Managers.instance.UI.ShopUICall.IsShopActivate == true ? false : true;
            });*/
            if (challengeScenes != null)
            {
                for (int i = 0; i < challengeScenes.Length; i++)
                {
                    Array.Resize(ref challengeSceneButtons,i+1);
                    int tempArray = i;
                    //closure �̽��� ���� ������ ���� �������
                    challengeSceneButtons[i] = challengeScenePickPannel.GetChild(i).GetComponent<Button>();
                    challengeSceneButtons[i].onClick.RemoveAllListeners();
                    challengeSceneButtons[i].onClick.AddListener(() => 
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
            Array.Resize(ref challengeScenes, 0);
            foreach (var path in scenePaths)
            {
                Debug.Log("Found scene: " + path);
                Debug.Log(challengeScenes.Length + 1);

                Array.Resize(ref challengeScenes, challengeScenes.Length+1);
                string tempNameSceneName = path.Replace(".unity", "");
                tempNameSceneName = tempNameSceneName.Replace(folderPath+ "\\", "");
                challengeScenes[challengeScenes.Length -1] = tempNameSceneName;
            }
        }
        public void CheckSceneInfo()
        {
            for (int i = 0; i < challengeScenes.Length; i++)
            {
                Debug.Log(challengeScenes[i]);
            }
        }
        public void ToChallengeScene(int targetArray)
        {

            SceneManager.LoadScene(challengeScenes[targetArray]);
        }

#endif
    }
}

