using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockCtroler : MonoBehaviour
{
    void Start()
    {

        gameObject.AddComponent<PolygonCollider2D>();
        Managers.instance.Grid.AddBattleGridData(transform.localPosition, SetBlockData());
        gameObject.layer = 3;
        Destroy(this);
    }
    private HeaderPadDefines.BlockObjects SetBlockData()
    {
        SpriteRenderer tempIMG = GetComponent<SpriteRenderer>();
        HeaderPadDefines.BlockObjects tempBlockData = null;
        switch (tempIMG.sprite.name)
        {
            // FLOW : 그리드 이미지명 다음과 같이
            case "HeaderBlock_BoombBlock":
                tempBlockData = new HeaderPadDefines.BlockObjects
                {
                    originBlockValue = 40,
                    blockValue = 40,
                    blockCondition = HeaderPadDefines.BlockStatus.BoombBlock,
                    SettedBlockCondition  = HeaderPadDefines.BlockStatus.BoombBlock,
                    BlockHP = 2,
                };
                break;            
            case "HeaderBlock_FilledCoin":
                tempBlockData = new HeaderPadDefines.BlockObjects
                {
                    originBlockValue = 1,
                    blockValue = 1,
                    blockCondition = HeaderPadDefines.BlockStatus.FilledCoin,
                    SettedBlockCondition = HeaderPadDefines.BlockStatus.FilledCoin
                };
                break;
            case "HeaderBlock_FIlled":
                tempBlockData = new HeaderPadDefines.BlockObjects
                {
                    originBlockValue = 3,
                    blockValue = 3,
                    blockCondition = HeaderPadDefines.BlockStatus.FIlled,
                    SettedBlockCondition = HeaderPadDefines.BlockStatus.FIlled,
                };
                break;
            case "HeaderBlock_Emptied":
                tempBlockData = new HeaderPadDefines.BlockObjects
                {
                    originBlockValue = 1,
                    blockValue = 1,
                    blockCondition = HeaderPadDefines.BlockStatus.Emptied,
                    SettedBlockCondition = HeaderPadDefines.BlockStatus.Emptied,
                };
                break;
            case "HeaderBlock_Destroyed":
                tempBlockData = new HeaderPadDefines.BlockObjects
                {
                    blockCondition = HeaderPadDefines.BlockStatus.Destroyed,
                    SettedBlockCondition = HeaderPadDefines.BlockStatus.Destroyed,
                };
                break;
            case "HeaderBlock_Reroll":
                tempBlockData = new HeaderPadDefines.BlockObjects
                {
                    blockCondition = HeaderPadDefines.BlockStatus.reroll,
                    SettedBlockCondition = HeaderPadDefines.BlockStatus.reroll,
                };
                break;
        }
        tempBlockData.targetIMG = tempIMG;
        return tempBlockData;
    }
}
