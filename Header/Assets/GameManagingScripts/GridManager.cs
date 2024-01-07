using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

public class GridManager
{
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
}
