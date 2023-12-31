using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager
{
    public Dictionary<Vector2Int,Interactions> interactionGrid = new Dictionary<Vector2Int,Interactions>();

    public void AddInteraction(Vector2Int tempBigPos,InteractionDefines.InteractionTypes tempTypes)
    {
        switch (tempTypes)
        {
            case InteractionDefines.InteractionTypes.None:
                Debug.LogError("인터렉션 추가자에서 타입등록을 해주세요");
                break;
            case InteractionDefines.InteractionTypes.DialogInteraction:
                interactionGrid.Add(tempBigPos, new DialogInteraction());
                break;
            case InteractionDefines.InteractionTypes.ToBattleScene:
                break;
        }
    }
    public void GridCheck(Vector2Int plrPos)
    {
        if (interactionGrid.ContainsKey(plrPos))
        {
            interactionGrid[plrPos].Interaction();
        }
    }

    public void ResetGrids()
    {
        interactionGrid.Clear();
    }
}
