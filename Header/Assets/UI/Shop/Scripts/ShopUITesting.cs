using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopUITesting : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Managers.instance.UI.ShopUICall.ShopUISetting();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Managers.instance.PlayerDataManager.PlayerMoney += 10;
            
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            Managers.instance.UI.ShopUICall.IsShopActivate = Managers.instance.UI.ShopUICall.IsShopActivate == true ? false : true;
        }
    }
}
