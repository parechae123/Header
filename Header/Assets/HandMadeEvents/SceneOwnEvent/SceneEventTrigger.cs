using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneEventTrigger : MonoBehaviour
{
    [SerializeField]private SceneEventChannel targetEvent;
    [SerializeField] private Text targetText;
    public int nextEventNumber = 0;
    public int eventChecker
    {
        get
        {
            return nextEventNumber;
        }
        set
        {
            if (targetEvent.Events.Count > value + 1)
            {
                targetEvent.Events[value + 1].UIPointing();
                nextEventNumber = value + 1;
            }
            else if(targetEvent.Events.Count == value + 1)
            {
                targetEvent.Events[value].EventDone();
                this.enabled = false;
            }
        }
    }
    void Start()
    {
        StartCoroutine(TempTimer());
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (targetEvent.Events.Count > eventChecker)
            {
                targetEvent.Events[eventChecker].EventDone((cbNum) =>
                {
                    eventChecker = cbNum;


                });
            }
            else if (targetEvent.Events.Count == eventChecker)
            {
                targetEvent.Events[eventChecker].EventDone();
                this.enabled = false;
            }

        }

    }
    IEnumerator TempTimer()
    {
        yield return new WaitForEndOfFrame();
        targetEvent.Events[eventChecker].UIPointing();
    }
}
