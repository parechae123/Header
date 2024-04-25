using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogEventDialogZeroOnly : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(StartAtZero());
    }

    IEnumerator StartAtZero()
    {
        yield return new WaitForEndOfFrame();
        Managers.instance.Grid.GridCheck(new Vector2(0f,-1f));
    }
}
