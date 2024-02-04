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
        playerOwnBalls.Add(balls);
        if (isCalledItOnShop)
        {
            Managers.instance.UI.ShopUICall.CreateBulbIcons(playerOwnBalls);
        }
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
        Managers.instance.UI.ShopUICall.CreateBulbIcons(playerOwnBalls);
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