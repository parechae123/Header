using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BallScript : MonoBehaviour
{
    [SerializeField]private Collider2D[] pauseCollector;
    public float ballBouncienss;
    public float ballFriction;
    public float ballRadios;
    public int ballNowHP;
    public float blinkTimer;
    public int breakBlockCount;
    public BulbSkills bulbSkills;
    private Light2D bulbLight;
    public bool isPlayerShooting = false;
    public ParticleSystem breakParticle;
    public ParticleSystem bounceParticle;
    private SpriteRenderer breakAnimSprite;
    public SpriteRenderer BreakAnimSprite
    {
        get
        {
            if (breakAnimSprite == null)
            {
                breakAnimSprite = new GameObject("BreakBulbAnim").AddComponent<SpriteRenderer>();
                breakAnimSprite.sortingLayerID = IMG.sortingLayerID;
                breakAnimSprite.gameObject.SetActive(false);
            }
            return breakAnimSprite;
        }
    }
    private Queue<ParticleSystem> bounceParticleQueue = new Queue<ParticleSystem>();
    public Queue<ParticleSystem> BounceParticleQueue
    {
        get
        {
            if (bounceParticleQueue.Count == 0)
            {
                bounceParticleQueue.Enqueue(GameObject.Instantiate<ParticleSystem>(bounceParticle, bounceParticle.transform.parent));
            }
            return bounceParticleQueue;
        }
    }
    public Light2D BulbLight 
    {
        get 
        { 
            if (bulbLight == null)
            {
                bulbLight = transform.GetChild(0).GetComponent<Light2D>();
            }
            return bulbLight;
        }
    }

    protected Rigidbody2D ballRB;
    public Rigidbody2D BallRB
    {
        get 
        {
            if (ballRB == null)
            {
                if (transform.TryGetComponent<Rigidbody2D>(out Rigidbody2D tempRB))
                {
                    ballRB = tempRB;
                    ballRB.gravityScale = 0;
                }
                else
                {
                    ballRB = transform.AddComponent<Rigidbody2D>();
                    ballRB.gravityScale = 0;
                }
                
            }
            return ballRB;
        }    
    }
    CircleCollider2D ballCol;
    public CircleCollider2D BallCol
    {
        get
        { 
            if (ballCol == null)
            {
                IMG.enabled = true;
                if (transform.TryGetComponent<CircleCollider2D>(out CircleCollider2D temp))
                {
                    ballCol = temp;
                }
                else
                {
                    ballCol = transform.AddComponent<CircleCollider2D>();
                }
                    ballCol.offset = Vector2.up* 0.09f;
                    ballCol.radius = 0.51f;
            }
            return ballCol;
        }
    }
    protected SpriteRenderer img;
    public SpriteRenderer IMG
    {
        get
        {
            if (img == null)
            {
                if (transform.TryGetComponent<SpriteRenderer>(out SpriteRenderer temp))
                {
                    img = temp;
                }
                else
                {
                    img = transform.AddComponent<SpriteRenderer>();
                }
                img.sprite = Managers.instance.Resource.Load<Sprite>("Bullet_Basic");
            }
            return img;
        }
    }

    protected void Update()
    {
        if (BallRB.velocity == Vector2.zero)
        {
            pauseCollector = Physics2D.OverlapCircleAll(transform.position,BallCol.bounds.size.x,8);
            for (int i = 0; i < pauseCollector.Length; i++)
            {
                Managers.instance.Grid.OnColide(pauseCollector[i].transform.position);
            }
        }
        if (ballNowHP == 1)
        {
            blinkTimer += Time.deltaTime;
            BulbLight.intensity = SinFunc(blinkTimer);
        }
        if (bulbSkills != null&& isPlayerShooting)
        {
            bulbSkills.UpdateSkills();
        }
    }

    public void ChangeBallSprite(string ballName)
    {
        IMG.sprite = Managers.instance.Resource.Load<Sprite>(ballName);
    } 
    public void BallPause()
    {
        IMG.enabled = true;
        if (bulbSkills != null)
        {
            bulbSkills.BreakEventSkills();
            isPlayerShooting = false;
        }
        BulbLight.intensity = 0;
        BallRB.simulated = false;
        BallCol.enabled = false;
        ballRadios = BallCol.bounds.size.x;
        transform.localPosition = Vector3.zero;
        BallRB.velocity = Vector2.zero;

    }
    public void Ballsetting(PhysicsMaterial2D phyMat, float Weight)
    {
        ShoterController.Instance.isReadyFire = true; 
        ShoterController.Instance.lineRenderer.enabled = true;
        BallRB.simulated = false;
        BallCol.enabled = false;
        ballRadios = BallCol.bounds.size.x;
        BallRB.gravityScale = Weight;
        transform.localPosition = Vector3.zero;
        BallRB.velocity = Vector2.zero;
        if (BallRB.sharedMaterial != phyMat)
        {
            BallRB.sharedMaterial = phyMat;
        }
    }
    public AudioClip tempAC;
    public AudioSource AS;
    public void BallFire(Vector2 FireToward,float FireForce)
    {
        Managers.instance.SoundManager.SFXPlayOneshot(Managers.instance.Resource.Load<AudioClip>("BulbThrowing1"), true, true);
        if(ShoterController.Instance.collPointPreview != null) ShoterController.Instance.collPointPreview.gameObject.SetActive(false);
        Managers.instance.UI.battleUICall.WeaponButtonCheck(true);
        ShoterController.Instance.isReadyFire = false;
        ShoterController.Instance.lineRenderer.enabled = false;
        BallRB.simulated = true;
        BallCol.enabled = true;
        FireToward = FireToward * FireForce;
        FireToward.y += 0.5f * BallRB.gravityScale * BallRB.mass * Mathf.Pow(Time.fixedDeltaTime, 2);
        BallRB.velocity = FireToward;
        if (bulbSkills != null)
        {
            bulbSkills.StartEventSkills();
            isPlayerShooting = true;

            //타입을 비교하는 비교문
            if (bulbSkills is AimBulbSkill)
            {
                AimBulbSkill tempAimSkill = bulbSkills as AimBulbSkill;
                tempAimSkill.targetPositionRel = FireToward;
            }
        }
    }
    protected void OnCollisionEnter2D(Collision2D collision)
    {
        
        if (collision.gameObject.layer == 3) 
        {
            Managers.instance.Grid.OnColide(collision.transform.position);
        }
        else if (collision.gameObject.layer == 7) 
        {
            ballNowHP = 1;
        }
        if (ballNowHP== 1)
        {

            ballNowHP--;
            EndPlayerTurn();
        }
        else
        {
            ballNowHP--;
            ParticleSystem tempBounceParticle = BounceParticleQueue.Dequeue();
            tempBounceParticle.transform.position = transform.position;
            tempBounceParticle.Play();
            StartCoroutine(BounceParticleChecker(tempBounceParticle));
            Managers.instance.UI.battleUICall.SetBulbDamagedText(ShoterController.Instance.NowBallStat.ballHealth,ballNowHP);
        }
    }
    public void BallToTarget(Vector2 targetPos,float Speed)
    {
        BallRB.velocity = Vector2.zero;
        BallRB.velocity = CarculateVelocity(targetPos,Speed);
    }
    Vector2 CarculateVelocity(Vector2 targetPos, float Speed)
    {
        return (targetPos - (Vector2)transform.position).normalized * (Vector2.Distance(targetPos, transform.position) * (Speed * BallRB.gravityScale));
    }
    public void BallParaBola(Vector2 targetPos)
    {
        BallRB.velocity = Vector2.zero;
        Vector2 tempVec = CarculateDirection(targetPos) * Vector2.Distance(transform.position, targetPos);
        tempVec += ((BallRB.gravityScale*Vector2.up) * 9.8f)/2;
        BallRB.velocity = tempVec;
    }
    protected Vector2 CarculateDirection(Vector2 targetPos)
    {
        return (targetPos - (Vector2)transform.position).normalized;
    }
    public float SinFunc(float timer)
    {
        float tempFloat = Mathf.Sin(timer*15)+1;
        tempFloat = tempFloat* 7.5f;
        return tempFloat;
    }
    public void ResetBall()
    {
        FakeBreakOnOff(true);
        Managers.instance.UI.battleUICall.SetComboNumber(false);
        BulbLight.intensity = 15;
        ShoterController.Instance.SetBall();
        blinkTimer = 0;
    }
    public IEnumerator BounceParticleChecker(ParticleSystem target)
    {
        while (target.IsAlive())
        {
            yield return null;
        }
        bounceParticleQueue.Enqueue(target);
        Debug.LogError("파티클 현재 " + bounceParticleQueue.Count);
    }
    public IEnumerator BreakParticleChecker(ParticleSystem target,Action callBack)
    {
        BallRB.velocity = Vector2.zero;
        BallCol.enabled = false;
        BallRB.simulated = false;
        Sprite[] tempSprites = new Sprite[1];
        string tempNum = "00";
        Managers.instance.SoundManager.SFXPlayOneshot(Managers.instance.Resource.Load<AudioClip>("Bulb_Broken_Sounds1"),true,true);

       for (int i = 0; i < int.MaxValue;i++)
        {
            tempNum = tempNum.Remove(tempNum.Length- (i.ToString().Length));
            tempNum = tempNum + i;

            Sprite singleSprite = Managers.instance.Resource.Load<Sprite>(ShoterController.Instance.NowBallStat.ballName + tempNum);
            if (singleSprite != null)
            {
                Array.Resize(ref tempSprites, tempSprites.Length + 1);
                tempSprites[i] = singleSprite;
            }
            else
            {
                Array.Resize(ref tempSprites, tempSprites.Length - 1);
                break;
            }
        }
        target.transform.position = transform.position;
        target.Play();
        while (!target.IsAlive())
        {
            yield return null;
        }
        BreakAnimSprite.transform.position = transform.position;
        BreakAnimSprite.transform.rotation = transform.rotation;

        IMG.enabled = false;
        //보정값 추가
        BreakAnimSprite.transform.localScale = new Vector3(0.483031034f, 0.483031034f, 0.483031034f);
        BreakAnimSprite.gameObject.SetActive(true);
        for (int i = 0; i < tempSprites.Length; i++)
        {
            BreakAnimSprite.sprite = tempSprites[i];
            yield return new WaitForSeconds(1f / tempSprites.Length >= Time.deltaTime? 1f / tempSprites.Length : Time.deltaTime*tempSprites.Length);
        }
        BreakAnimSprite.gameObject.SetActive(false);
        while (target.IsAlive())
        {
            yield return null;
        }
        BallPause();
        callBack.Invoke();
    }
    public void EndPlayerTurn()
    {
        StartCoroutine(BreakParticleChecker(breakParticle, () =>
        {

            MonsterManager.MonsterManagerInstance.NextTurnFunctions(ShoterController.Instance.regionalDamage, ShoterController.Instance.targetDamage, ShoterController.Instance.TargetMonsterTR, () =>
            {
                if (100 - Managers.instance.PlayerDataManager.girlAttackChance <= UnityEngine.Random.Range(0, 101))
                {
                    MonsterManager.MonsterManagerInstance.DamageToAllMonsters(Managers.instance.PlayerDataManager.girlAD, (total, count) =>
                    {
                        ResetBall();
                        ShoterController.Instance.regionalDamage = 0;
                        ShoterController.Instance.targetDamage = 0;
                    }, true);
                    Managers.instance.UI.battleUICall.GirlTextAttack("영차!!", Color.blue, Color.white);
                    //TODO : 소녀 공격 구현
                }
                else
                {
                    Managers.instance.UI.battleUICall.GirlTextAttack("아쉬운거죠 뭐", Color.red, Color.white);
                    ResetBall();
                    ShoterController.Instance.regionalDamage = 0;
                    ShoterController.Instance.targetDamage = 0;
                }
                if (Managers.instance.PlayerDataManager.RemoveBall(ShoterController.Instance.NowBallStat))
                {
                    ShoterController.Instance.SetBallOnNext();
                }

            }
            );
        }));
    }
    public void FakeBreakOnOff(bool isOn)
    {
        IMG.enabled = isOn;
        BallCol.enabled = isOn;
        BallRB.simulated = isOn;
    }
}

