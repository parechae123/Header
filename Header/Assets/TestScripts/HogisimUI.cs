using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HogisimUI : MonoBehaviour
{
    // Start is called before the first frame update
    public SpriteRenderer sp;
    public Transform targetPos;
    public Sprite targetImage;
    public Image uiImage;
    public RectTransform uiTR;
    void Start()
    {
        
    }

    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            SetSize();
        }
    }
    private void SetSize()
    {
        uiTR.position = Camera.main.WorldToScreenPoint(targetPos.position);
        Vector2 vecMin = Camera.main.WorldToScreenPoint(targetPos.position - sp.bounds.extents);
        //- (Vector2)sp.bounds.extents
        Vector2 vecMax = Camera.main.WorldToScreenPoint(targetPos.position+sp.bounds.extents);
        //+ (Vector2)sp.bounds.extents;
        uiTR.anchorMin = new Vector2(vecMin.x/Screen.width, vecMin.y / Screen.height);
        uiTR.anchorMax = new Vector2(vecMax.x / Screen.width, vecMax.y / Screen.height);
        uiTR.anchoredPosition = Vector2.zero;
        uiTR.sizeDelta = Vector2.zero;

    }
}
