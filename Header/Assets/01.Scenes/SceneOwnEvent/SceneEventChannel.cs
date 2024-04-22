using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.GraphicsBuffer;
[CreateAssetMenu(fileName = "new SceneEventChannel",menuName ="Scriptables/SceneEvents",order = 50)]
public class SceneEventChannel : ScriptableObject
{
    [SerializeField]public List<TutorialEventChannel> Events;
    public int eventChecker
    {
        set
        {
            if (Events.Count> value)
            {
                Events[value + 1].UIPointing();
            }
        }
    }
}

[System.Serializable]
public class TutorialEventChannel
{
    [SerializeField]private string girlText;
    [SerializeField] private string uiTargetName;
    [SerializeField] private int eventArrayInTrigger;
    [SerializeField] private bool isTargetUI;
    private Transform targetParent;
    private Transform targetTR;
    public void UIPointing()
    {

        if (isTargetUI)
        {
            targetTR = GameObject.Find(uiTargetName).transform;
            //targetTR = Managers.instance.UI.LoadingUIProps.SceneMainCanvas.transform.Find(uiTargetName);
            targetParent = targetTR.parent;
            Managers.instance.UI.BattleUICall.SceneBTNParet.gameObject.SetActive(true);
            targetTR.SetParent(Managers.instance.UI.BattleUICall.SceneBTNParet);
        }
        else
        {
            targetTR = GameObject.Find(uiTargetName).transform;
        }

        if (girlText != string.Empty)
        {
            Managers.instance.UI.BattleUICall.GirlBulbExplane = girlText;
            Managers.instance.UI.BattleUICall.GirlParentRT.SetParent(Managers.instance.UI.BattleUICall.SceneBTNParet);
        }

    }

    public void EventDone(Action<int> cb)
    {

        if (girlText != string.Empty)
        {
            Managers.instance.UI.BattleUICall.GirlParentRT.SetParent(Managers.instance.UI.BattleUICall.BattleSceneUI);
        }
        targetTR.SetParent(targetParent);
        cb.Invoke(eventArrayInTrigger);
    }

}