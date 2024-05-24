using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CreateAssetMenu(fileName = "new SceneEventChannel",menuName ="Scriptables/SceneEvents",order = 50)]
public class SceneEventChannel : ScriptableObject
{
    [SerializeField]public List<TutorialEventChannel> Events;
#if UNITY_EDITOR
    public void SetEventArray()
    {
        for (int i = 0; i < Events.Count; i++)
        {
            Events[i].eventArrayInTrigger = i;
        }
    }
#endif
}

[System.Serializable]
public class TutorialEventChannel
{
    [SerializeField]private string girlText;
    [SerializeField] private string uiTargetName;
    [SerializeField] public int eventArrayInTrigger;
    [SerializeField] private bool isTargetUI;
    private Transform targetParent;
    private Transform targetTR;
    public void UIPointing()
    {

        if (isTargetUI)
        {
            if (uiTargetName != string.Empty)
            {
                targetTR = GameObject.Find(uiTargetName).transform;
                //targetTR = Managers.instance.UI.LoadingUIProps.SceneMainCanvas.transform.Find(uiTargetName);
                targetParent = targetTR.parent;
                targetTR.SetParent(Managers.instance.UI.battleUICall.AmbientPanel);
            }
            Managers.instance.UI.battleUICall.AmbientPanel.gameObject.SetActive(true);

        }
        else
        {
            targetTR = GameObject.Find(uiTargetName).transform;
        }

        if (girlText != string.Empty)
        {
            Managers.instance.UI.battleUICall.GirlBulbExplane = girlText;
            Managers.instance.UI.battleUICall.GirlParentRT.SetParent(Managers.instance.UI.battleUICall.AmbientPanel);
        }

    }

    public void EventDone(Action<int> cb = null)
    {
        if (isTargetUI)
        {
            if (uiTargetName != string.Empty)
            {
                targetTR.SetParent(targetParent);
            }
        }

        if (girlText != string.Empty)
        {
            Managers.instance.UI.battleUICall.GirlParentRT.SetParent(Managers.instance.UI.battleUICall.BattleSceneUI);
        }

        Managers.instance.UI.battleUICall.AmbientPanel.gameObject.SetActive(false);
        Managers.instance.UI.battleUICall.AmbientPanel.SetAsLastSibling();

        if (cb != null)
        {
            cb.Invoke(eventArrayInTrigger);
        }
    }



}
#if UNITY_EDITOR
[CustomEditor(typeof(SceneEventChannel))]
public class EventButons : Editor
{
    public override void OnInspectorGUI()           //유니티의 인스펙터 함수를 재정의
    {
        base.OnInspectorGUI();
        SceneEventChannel eventScriptableObject = (SceneEventChannel)target;//유니티 인스펙터 함수 동작을 같이 한다.(Base)

        if (GUILayout.Button("GetArray"))
        {
            eventScriptableObject.SetEventArray();
        }
    }
}
#endif