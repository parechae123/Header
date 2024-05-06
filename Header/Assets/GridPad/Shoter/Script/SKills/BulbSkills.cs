using HeaderPadDefines;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;

public abstract class BulbSkills
{
    protected BallScript OriginBall;
    public bool isBulbFired;
    public virtual void InitializeSetting()
    {
        OriginBall = ShoterController.Instance.targetBall;
    }
    public abstract void StartEventSkills();
    public abstract void UpdateSkills();
    public abstract void BreakEventSkills();
    public abstract void Reset();
}
public class TorchBulbSKill : BulbSkills
{
    public override void InitializeSetting()
    {
        base.InitializeSetting();
    }
    public override void StartEventSkills()
    {
        
    }
    public override void UpdateSkills()
    {
        
    }
    public override void BreakEventSkills()
    {
        GameObject tempTorch = GameObject.Instantiate<GameObject>(Managers.instance.Resource.Load<GameObject>("InstalledTorch"), null);
        tempTorch.transform.position = OriginBall.transform.position;
        tempTorch.transform.rotation = OriginBall.transform.rotation;
        tempTorch.GetComponent<TorchScript>().SetStart();
    }
    public override void Reset()
    {
        
    }
}
public class DecoBulbSkill : BulbSkills
{
    private SpriteShapeController decoShapeCtrler;
    private SpriteShapeRenderer decoShape;
    private bool isUpdateDone = false;
    private bool isStartedForceNegative;
    private Vector2[] lineArray = new Vector2[3];
    //쏘는값이 음수일떄 트루를 반환
    public override void InitializeSetting()
    {
        base.InitializeSetting();
        isBulbFired = false;
        isUpdateDone = false;
        
        //초기화
    }
    public override void StartEventSkills()
    {
        decoShapeCtrler = GameObject.Instantiate(Managers.instance.Resource.Load<GameObject>("DecoBulbLIne"), null).GetComponent<SpriteShapeController>();
        decoShape = decoShapeCtrler.gameObject.GetComponent<SpriteShapeRenderer>();
        lineArray = new Vector2[3];
        //컴포넌트 할당
        decoShapeCtrler.transform.position = Vector3.zero;
        isStartedForceNegative = OriginBall.BallRB.velocity.y>0? false: true;
        lineArray[0] = (Vector2)OriginBall.transform.position+(ShoterController.Instance.normalizedRelValue*2.2f);
        isUpdateDone =false;
        isBulbFired = true;
    }
    //발사 후 항시
    public override void UpdateSkills()
    {
        if (!isUpdateDone&&isBulbFired)
        {
            if (isStartedForceNegative)
            {
                if (OriginBall.BallRB.velocity.y <= 0)
                {
                    lineArray[1] = OriginBall.transform.position;
                    decoShapeCtrler.RefreshSpriteShape();
                    isUpdateDone = true;
                }
            }
            else
            {
                if (OriginBall.BallRB.velocity.y >= 0)
                {
                    lineArray[1] = OriginBall.transform.position;
                    decoShapeCtrler.RefreshSpriteShape();
                    isUpdateDone = true;
                }
            }
        }

    }
    //전기 파괴시
    public override void BreakEventSkills()
    {
        lineArray[2] = OriginBall.transform.position;
        decoShapeCtrler.edgeCollider.enabled = true;
        decoShapeCtrler.enableTangents = false;
        if (lineArray[1] == Vector2.zero)
        {
            lineArray[1] = (lineArray[2] + lineArray[0])/2;
        }
        for (int i = 0; i < lineArray.Length; i++)
        {

            if (Vector2.Distance(lineArray[i],ShoterController.Instance.transform.position)<= 1)
            {
                GameObject.Destroy(decoShape.gameObject);
                return;
            }
            if (i == 0)
            {
                decoShapeCtrler.spline.SetPosition(i, lineArray[i]);
                decoShapeCtrler.spline.SetHeight(i, 0.2f);
                decoShapeCtrler.edgeCollider.points[i] = lineArray[i];
            }
            else
            {
                if (Vector2.Distance(lineArray[i], lineArray[i-1])>= 0.04f)
                {
                    decoShapeCtrler.spline.SetPosition(i, lineArray[i]);
                    decoShapeCtrler.spline.SetHeight(i, 0.2f);
                    decoShapeCtrler.edgeCollider.points[i] = lineArray[i];
                }
                else
                {
                    lineArray[i] = lineArray[i] + (Vector2.one * 0.04f);
                    decoShapeCtrler.spline.SetPosition(i, lineArray[i]);
                    decoShapeCtrler.spline.SetHeight(i, 0.2f);
                    decoShapeCtrler.edgeCollider.points[i] = lineArray[i];
                }
            }
            Debug.Log(lineArray[i]);
        }

        decoShape.enabled = true;
        decoShapeCtrler.RefreshSpriteShape();
        decoShape = null;
        decoShapeCtrler = null;
        isBulbFired = false;
    }
    //발사하지 않고 전구를 넘기거나 할때
    public override void Reset()
    {
        Debug.Log("스킬 리셋됨");

        if (decoShapeCtrler!= null)
        {
            GameObject.Destroy(decoShapeCtrler.gameObject);
        }
        isBulbFired = false;
        decoShape = null;
        
        
    }
}
public class ShineBulbSkill : BulbSkills
{
    public Transform CloneBall;
    public override void InitializeSetting()
    {
        base.InitializeSetting();
    }
    public override void StartEventSkills()
    {
        CloneBall = new GameObject("CloneBall").transform;
        CloneBall.AddComponent<CloneBalls>().OnBallCreate(OriginBall.BallCol.radius, OriginBall.IMG.sprite);
    }
    public override void UpdateSkills()
    {
        CloneBall.transform.position = new Vector3(OriginBall.transform.position.x*-1, OriginBall.transform.position.y, OriginBall.transform.position.z);
        CloneBall.transform.rotation = OriginBall.transform.rotation;
    }
    public override void BreakEventSkills()
    {
        GameObject.Destroy(CloneBall.gameObject);
    }
    public override void Reset()
    {

    }
}
public class AimBulbSkill : BulbSkills
{
    public Vector3 targetPosition;
    public Vector3 targetPositionRel;
    private float arriveTime;
    private float firedTime;
    public override void InitializeSetting()
    {
        base.InitializeSetting();

    }
    public override void StartEventSkills()
    {
        targetPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
        firedTime = 0;
        arriveTime = 0;
        arriveTime = BallArriveTime();
        isBulbFired = true;
        OriginBall.BallCol.enabled = false;
    }
    public override void UpdateSkills()
    {
        if (isBulbFired)
        {
            firedTime += Time.deltaTime;
            Debug.Log("발사 시간 " + firedTime + "," + "활성화 시간 " + arriveTime);
            if (firedTime >= arriveTime)
            {
                if (!OriginBall.BallCol.enabled)
                {
                    Debug.LogError("발사 시간 " + firedTime + "," + "활성화 시간 " + arriveTime);
                    OriginBall.BallCol.enabled = true;
                    OriginBall.BallRB.gravityScale = ShoterController.Instance.NowBallStat.SkillValueOne;
                }
            }
        }
    }
    public override void BreakEventSkills()
    {
        if (ShoterController.Instance.NowBallStat != null)
        {
            OriginBall.BallRB.gravityScale = ShoterController.Instance.NowBallStat.weight;
        }

        isBulbFired = false;
    }
    public override void Reset()
    {
        isBulbFired = false;
    }
    private float BallArriveTime()
    {
        float time = 0;
        Vector3 originPos = ShoterController.Instance.transform.position;
        float targetDistance = Vector2.Distance(originPos, targetPosition);
        float relDistance = OriginBall.BallRB.velocity.magnitude;
        time = targetDistance / relDistance;

        return time;
    }
}

public class HPBulbSkill : BulbSkills
{
    public override void InitializeSetting()
    {
        base.InitializeSetting();
    }
    public override void StartEventSkills()
    {

    }
    public override void UpdateSkills()
    {

    }
    public override void BreakEventSkills()
    {
        Managers.instance.PlayerDataManager.ChangePlayerHP((-ShoterController.Instance.targetDamage)/ShoterController.Instance.NowBallStat.SkillValueOne);
    }
    public override void Reset()
    {

    }
}

/*
public class DefaultBulbSkillForm : BulbSkills
{
    public override void InitializeSetting()
    {
        base.InitializeSetting();
    }
    public override void StartEventSkills()
    {

    }
    public override void UpdateSkills()
    {

    }
    public override void BreakEventSkills()
    {

    }
    public override void Reset()
    {

    }
}
*/