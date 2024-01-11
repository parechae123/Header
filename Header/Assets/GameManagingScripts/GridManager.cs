using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

public class GridManager
{

    #region 다이얼로그 그리드
    public Dictionary<Vector2Int,Interactions> interactionGrid = new Dictionary<Vector2Int,Interactions>();
    public HashSet<Vector2Int> isInteractionAreThere = new HashSet<Vector2Int>();

    public void AddInteraction(InteractionDefines.InteractionInstallerProps tempInteractionData)
    {
        isInteractionAreThere.Add(tempInteractionData.interactionPosition);
        switch (tempInteractionData.interactionTypes)
        {
            case InteractionDefines.InteractionTypes.None:
                Debug.LogError("인터렉션 추가자에서 타입등록을 해주세요");
                break;
            case InteractionDefines.InteractionTypes.DialogInteraction:
                interactionGrid.Add(tempInteractionData.interactionPosition, new DialogInteraction { detail = tempInteractionData.detail,interactionKeyNumber = tempInteractionData.keyNumber});
                break;
            case InteractionDefines.InteractionTypes.ToBattleScene:
                interactionGrid.Add(tempInteractionData.interactionPosition, new ToBattleSceneInteraction { detail = tempInteractionData.detail, interactionKeyNumber = tempInteractionData.keyNumber });
                break;
        }
    }    
    public void RemoveInteraction(Vector2Int RemovePosition)
    {
        isInteractionAreThere.Remove(RemovePosition);
        interactionGrid.Remove(RemovePosition);
    }
    public void GridCheck(Vector3 plrPos)
    {
        Vector2Int tempBigPos = VectorToIntPos(plrPos);
        Debug.Log(tempBigPos);
        if (isInteractionAreThere.Contains(tempBigPos))
        {
            interactionGrid[tempBigPos].Interaction();

        }
    }
    

    public void ResetGrids()
    {
        interactionGrid.Clear();
        isInteractionAreThere.Clear();
    }
    Vector2Int VectorToIntPos(Vector3 tempVec)
    {
        Vector2Int temVecInt = new Vector2Int(Mathf.RoundToInt(tempVec.x), Mathf.RoundToInt(tempVec.y));
        return temVecInt;
    }
    #endregion
    #region 전투씬 그리드
    private Grid battleGrid = null;
    public Grid BattleGrid 
    { 
        get 
        { 
            if (battleGrid == null) 
            {
                if (GameObject.Find("BattleGrid").TryGetComponent<Grid>(out Grid result))
                {
                    BattleGridData.Clear();
                    battleGrid = result;
                }
                else
                {
                    Debug.LogError("해당씬의 전투 그리드를 찾지 못했습니다. 그리드 존재여부와 그리드이름을 확인해주세요.");
                    // TODO : 텍스트로 불러와서 설정해주는거 가능 할 것 같은데?..
                }
            }
            return battleGrid; 
        } 
    }
    private Dictionary<Vector2, HeaderPadDefines.BlockObjects> battleGridData;
    public Dictionary<Vector2, HeaderPadDefines.BlockObjects> BattleGridData 
    { 
        get 
        {
            if (battleGridData == null)
            {
                battleGridData = new Dictionary<Vector2, HeaderPadDefines.BlockObjects> ();
            }
            return battleGridData; 
        } 
    }
    public void AddBattleGridData(Vector2 pos,HeaderPadDefines.BlockObjects data)
    {
        BattleGridData.Add(pos, data);
        Debug.Log(BattleGridData[pos].blockCondition);
    }
    public void OnReset()
    {
        foreach (var item in BattleGridData)
        {
            item.Value.OnResetBlocks();
        }
    }
    public void OnColide(Vector2 ColidePos)
    {
        BattleGridData[ColidePos].OnColideBlock();
    }
    #endregion
}
