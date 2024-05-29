using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HeaderPadDefines;
using System;
using DG.Tweening;

[System.Serializable]
public class PlayerDataManager
{
    public List<ExtraBallStat> playerOwnBalls = new List<ExtraBallStat>();
    [SerializeField]private int playerMoney;
    public int PlayerMoney
    {
        get 
        {
            return playerMoney;
        }
        set 
        {
            Managers.instance.UI.shopUICall.MoneyUpdate(value);
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
            if (playerHPMax >= value.Item2 || playerHPMax ==0)
            {
                playerHPMax = value.Item1;
                playerHPNow = value.Item2;
            }
            else
            {
                playerHPNow = value.Item1;
            }

            if (Managers.instance.UI.battleUICall.IsInBattleScene)
            {
                Managers.instance.UI.battleUICall.SettingPlayerBattleUI();
                Managers.instance.UI.battleUICall.HPBarSetting(true, playerHPMax, playerHPNow);
            }
            
        }
    }
    public bool isChallengeMode = false;
    public int girlAttackChance = 30;
    //1´ç 1% ÃÑ 100%È®·ü
    public float girlAD = 20;
    public bool isParabolaTurnOn = true;
    public void ChangePlayerHP(float Damage)
    {
        SetPlayerHP = (Managers.instance.PlayerDataManager.SetPlayerHP.Item1, Managers.instance.PlayerDataManager.SetPlayerHP.Item2 - Damage);
        if (playerHPNow <= 0)
        {
            ShoterController.Instance.isReadyFire = false;
            Managers.instance.UI.battleUICall.GameOverBTN.enabled = true;
            Managers.instance.UI.battleUICall.ToDialogSceneBTN.gameObject.SetActive(false);
        }
        if (Damage <= 0)
        {
            Managers.instance.UI.battleUICall.PlayerHPBarFrontIMG.DOComplete();
            Managers.instance.UI.battleUICall.PlayerHPBarFrontIMG.DOColor(Color.green, 0.1f).OnComplete(() =>
            {
                Managers.instance.UI.battleUICall.PlayerHPBarFrontIMG.DOColor(Color.white, 0.1f).OnComplete(() =>
                {
                    Managers.instance.UI.battleUICall.PlayerHPBarFrontIMG.DOColor(Color.green, 0.1f).OnComplete(() =>
                    {
                        Managers.instance.UI.battleUICall.PlayerHPBarFrontIMG.DOColor(Color.white, 0.1f);
                    });
                });
            });
        }

        if (Managers.instance.UI.battleUICall.IsInBattleScene)
        {
            Managers.instance.UI.battleUICall.SettingPlayerBattleUI();

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
            Managers.instance.UI.shopUICall.UpdateInvenBulb(playerOwnBalls);
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
            Managers.instance.UI.shopUICall.UpdateInvenBulb(playerOwnBalls);
        }
        return false;
    }
    public void CheckWeaponNextBeforeButton()
    {
        Managers.instance.UI.battleUICall.WeaponButtonCheck(playerOwnBalls.Count > 1 ? false: true);
    }
    public void PlayerBeforeBallPick()
    {
        ShoterController.Instance.SetBallOnBehind();
        


    }
    public void PlayerNextBallPick()
    {

        ShoterController.Instance.SetBallOnNext();
    }
    /// <summary>
    /// ResetPlayerDatas
    /// </summary>
    public void ResetPlayer()
    {
        playerOwnBalls.Clear();
        playerMoney = 0;
        SetPlayerHP = (100, 100);
    }
}