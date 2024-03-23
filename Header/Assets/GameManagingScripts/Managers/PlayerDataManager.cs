using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HeaderPadDefines;
using System;

[System.Serializable]
public class PlayerDataManager
{
    public List<ExtraBallStat> playerOwnBalls = new List<ExtraBallStat>();
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
    private float playerHPMax;
    private float playerHPNow;
    public (float,float) SetPlayerHP
    {
        get 
        {
            if (playerHPMax == 0)
            {
                playerHPMax = 100;
                playerHPNow = 100;
            }
            return (playerHPMax,playerHPNow);
        }
        set
        {
            playerHPMax = value.Item1;
            playerHPNow = value.Item2;
            if (Managers.instance.UI.BattleUICall.IsInBattleScene)
            {
                Managers.instance.UI.BattleUICall.SettingPlayerBattleUI();
                Managers.instance.UI.BattleUICall.HPBarSetting(true, value.Item1, value.Item2);
            }
            
        }
    }

    public int girlAttackChance = 30;
    //1´ç 1% ÃÑ 100%È®·ü
    public float girlAD = 20;
    public void PlayerGetDamage(float Damage)
    {
        SetPlayerHP = (Managers.instance.PlayerDataManager.SetPlayerHP.Item1, Managers.instance.PlayerDataManager.SetPlayerHP.Item2 - Damage);
        if (playerHPNow <= 0)
        {
            ShoterController.Instance.isReadyFire = false;
            Managers.instance.UI.BattleUICall.GameOverBTN.enabled = true;
            Managers.instance.UI.BattleUICall.ToDialogSceneBTN.gameObject.SetActive(false);

        }

        if (Managers.instance.UI.BattleUICall.IsInBattleScene)
        {
            Managers.instance.UI.BattleUICall.SettingPlayerBattleUI();

        }
    }
    

    public void AddBall(ExtraBallStat balls,bool isCalledItOnShop = false)
    {
        if (isCalledItOnShop) 
        {
            if (PlayerMoney < balls.ballPrice)
            {
                return;
            }
            else
            {
                PlayerMoney -= balls.ballPrice;
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
                        balls.amount = 1;
                        playerOwnBalls.Add(balls);
                        break;
                    }
                }
                else
                {
                    balls.amount = 1;
                    playerOwnBalls.Add(balls);
                    break;
                }

            }
        }
        else
        {
            balls.amount = 1;
            playerOwnBalls.Add(balls);
        }

        if (isCalledItOnShop)
        {
            Managers.instance.UI.ShopUICall.UpdateInvenBulb(playerOwnBalls);
        }
    }
    public bool RemoveBall(ExtraBallStat ball, bool isCalledItOnShop = false)
    { 
        foreach (ExtraBallStat item in playerOwnBalls)
        {
            if (item.ballName == ball.ballName && item.amount<=1&& !isCalledItOnShop)
            {
                item.amount--;
                playerOwnBalls.Remove(item);
                if (ShoterController.Instance.NowBallStat != null&&playerOwnBalls.Count <= 1) 
                {
                    CheckWeaponNextBeforeButton();
                }
                return true;
            }
            else if(item.ballName == ball.ballName && item.amount > 1&& !isCalledItOnShop) 
            {
                item.amount--;

            }
        }
        if (isCalledItOnShop)
        {
            Managers.instance.UI.ShopUICall.UpdateInvenBulb(playerOwnBalls);
        }
        return false;
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
    public void ResetPlayer()
    {
        playerOwnBalls.Clear();
        playerMoney = 0;
        SetPlayerHP = (100, 100);
    }
}