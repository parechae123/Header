using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HeaderPadDefines;

[System.Serializable]
public class PlayerDataManager
{
    public List<BallStat> playerOwnBalls = new List<BallStat>();
    private int playerMoney;
    public int PlayerMoney
    {
        get 
        {
            return playerMoney;
        }
        set 
        {
            Managers.instance.UI.ShopUICall.MoneyUpdate(value);
            playerMoney = value;
        }
    }
    

    public void AddBall(BallStat balls,bool isCalledItOnShop = false)
    {
        if (isCalledItOnShop) 
        {
            if (PlayerMoney < balls.price)
            {
                return;
            }
            else
            {
                PlayerMoney -= balls.price;
            }
        }
        if (playerOwnBalls.Count > 0)
        {
            for (int i = 0; playerOwnBalls.Count >= i; i++)
            {
                if (playerOwnBalls.Count> i)
                {
                    if (playerOwnBalls[i].ballName == balls.ballName)
                    {
                        playerOwnBalls[i].amount += 1;
                        break;
                    }
                    else if (playerOwnBalls[i].ballName != balls.ballName && i == playerOwnBalls.Count)
                    {
                        playerOwnBalls.Add(balls);
                        break;
                    }
                }
                else
                {
                    playerOwnBalls.Add(balls);
                    break;
                }

            }
        }
        else
        {
            playerOwnBalls.Add(balls);
        }

        if (isCalledItOnShop)
        {
            Managers.instance.UI.ShopUICall.UpdateInvenBulb(playerOwnBalls);
        }
    }
    public void RemoveBall(BallStat ball, bool isCalledItOnShop = false)
    { 
        foreach (BallStat item in playerOwnBalls)
        {
            if (item == ball)
            {
                playerOwnBalls.Remove(item);
            }
        }
        if (isCalledItOnShop)
        {
            Managers.instance.UI.ShopUICall.UpdateInvenBulb(playerOwnBalls);
        }
    }
    public void CheckWeaponNextBeforeButton()
    {
        Managers.instance.UI.BattleUICall.WeaponButtonCheck(playerOwnBalls.Count > 1 ? false: true);
    }
    public void PlayerBeforeBallPick()
    {
        ShoterController.Instance.SetBallOnBehind();
        
    }
    public void PlayerNextBallPick()
    {
        ShoterController.Instance.SetBallOnNext();
    }
}