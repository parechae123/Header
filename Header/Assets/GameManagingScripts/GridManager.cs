using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager
{
    public Dictionary<Vector2Int,Interactions> interactionGrid = new Dictionary<Vector2Int,Interactions>();
    public HashSet<Vector2Int> isInteractionAreThere = new HashSet<Vector2Int>();

    public void AddInteraction(Vector2Int tempBigPos,InteractionDefines.InteractionTypes tempTypes)
    {
        isInteractionAreThere.Add(tempBigPos);
        switch (tempTypes)
        {
            case InteractionDefines.InteractionTypes.None:
                Debug.LogError("인터렉션 추가자에서 타입등록을 해주세요");
                break;
            case InteractionDefines.InteractionTypes.DialogInteraction:
                interactionGrid.Add(tempBigPos, new DialogInteraction());
                break;
            case InteractionDefines.InteractionTypes.ToBattleScene:
                interactionGrid.Add(tempBigPos, new ToBattleSceneInteraction());
                break;
        }
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
