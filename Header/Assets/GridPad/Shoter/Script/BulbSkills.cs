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
public class torchBulbSKill : BulbSkills
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
        GameObject tempTorch = new GameObject("");
        tempTorch.AddComponent<TorchScript>();
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
    //��°��� �����ϋ� Ʈ�縦 ��ȯ
    public override void InitializeSetting()
    {
        base.InitializeSetting();



        isBulbFired = false;
        isUpdateDone = false;
        
        //�ʱ�ȭ
    }
    public override void StartEventSkills()
    {
        decoShapeCtrler = GameObject.Instantiate(Managers.instance.Resource.Load<GameObject>("DecoBulbLIne"), null).GetComponent<SpriteShapeController>();
        decoShape = decoShapeCtrler.gameObject.GetComponent<SpriteShapeRenderer>();
        lineArray = new Vector2[3];
        //������Ʈ �Ҵ�
        decoShapeCtrler.transform.position = Vector3.zero;
        isStartedForceNegative = OriginBall.BallRB.velocity.y>0? false: true;
        lineArray[0] = (Vector2)OriginBall.transform.position+(ShoterController.Instance.normalizedRelValue*2.2f);
        isUpdateDone =false;
        isBulbFired = true;
    }
    //�߻� �� �׽�
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
    //���� �ı���
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
    //�߻����� �ʰ� ������ �ѱ�ų� �Ҷ�
    public override void Reset()
    {
        Debug.Log("��ų ���µ�");

        if (decoShapeCtrler!= null)
        {
            GameObject.Destroy(decoShapeCtrler.gameObject);
        }
        isBulbFired = false;
        decoShape = null;
        
        
    }
}
