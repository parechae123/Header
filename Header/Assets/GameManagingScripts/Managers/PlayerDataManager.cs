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
}
