using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HeaderPadDefines;

public class PlayerDataManager
{
    public List<BallStat> playerOwnBalls = new List<BallStat>();

    public void AddBall(BallStat balls)
    {
        playerOwnBalls.Add(balls);
    }
    public void RemoveBall(BallStat ball)
    { 
        foreach (BallStat item in playerOwnBalls)
        {
            if (item == ball)
            {
                playerOwnBalls.Remove(item);
            }
        }
    }
    public void CheckWeaponNextBeforeButton()
    {
        Managers.instance.UI.BattleUICall.WeaponButtonCheck(playerOwnBalls.Count > 1 ? true: false);
    }
    public void PlayerBeforeBallPick()
    {
        Debug.Log("이전 공");
    }
    public void PlayerNextBallPick()
    {
        Debug.Log("다음 공");
    }
}
