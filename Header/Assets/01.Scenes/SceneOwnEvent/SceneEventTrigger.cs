using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneEventTrigger : MonoBehaviour
{
    [SerializeField]private SceneEventChannel targetEvent;
    private int nowEventArray;

    void Start()
    {
        StartCoroutine(TempCor());
    }
    IEnumerator TempCor()
    {
        yield return new WaitForSeconds(3);
        targetEvent.Events[nowEventArray].UIPointing();
        nowEventArray = 0;
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            targetEvent.Events[nowEventArray].EventDone((cbNum)=>
            {
                targetEvent.eventChecker = cbNum;
                if (targetEvent.Events.Count == nowEventArray)
                {
                }
            });
        }

    }
}
