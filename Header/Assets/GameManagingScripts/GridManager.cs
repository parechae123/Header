using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

public class GridManager
{

    #region ���̾�α� �׸���
    public Dictionary<Vector2Int,Interactions> interactionGrid = new Dictionary<Vector2Int,Interactions>();
    public HashSet<Vector2Int> isInteractionAreThere = new HashSet<Vector2Int>();

    public void AddInteraction(InteractionDefines.InteractionInstallerProps tempInteractionData)
    {
        isInteractionAreThere.Add(tempInteractionData.interactionPosition);
        switch (tempInteractionData.interactionTypes)
        {
            case InteractionDefines.InteractionTypes.None:
                Debug.LogError("���ͷ��� �߰��ڿ��� Ÿ�Ե���� ���ּ���");
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
    #region ������ �׸���
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
                    Debug.LogError("�ش���� ���� �׸��带 ã�� ���߽��ϴ�. �׸��� ���翩�ο� �׸����̸��� Ȯ�����ּ���.");
                    // TODO : �ؽ�Ʈ�� �ҷ��ͼ� �������ִ°� ���� �� �� ������?..
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
