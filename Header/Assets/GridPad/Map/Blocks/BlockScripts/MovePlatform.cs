using UnityEngine;
using HeaderPadDefines;

public class MovePlatform : MonoBehaviour
{
    [SerializeField] BlockObjects blockInfo = new BlockObjects();
    [SerializeField] MovePlatformType platformType;
    [SerializeField] Vector2 targetPosition = new Vector2();
    [SerializeField] float speed = 9.8f;
    
    // Start is called before the first frame update
    void Start()
    {
        gameObject.layer = 3;
        switch (platformType)
        {
            case MovePlatformType.ballFastPass:
                Managers.instance.Grid.AddBattleGridData(transform.position, blockInfo, RegistBallFastPass);
                break;
            case MovePlatformType.parabola:
                Managers.instance.Grid.AddBattleGridData(transform.position, blockInfo, RegistParabolaBall);
                break;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawCube(targetPosition, Vector2.one);
    }
    public void RegistBallFastPass()
    {
        ShoterController.Instance.TargetBall.BallToTarget(targetPosition,speed);
        Debug.Log("플랫폼 테스트");
    }
    public void RegistParabolaBall()
    {
        ShoterController.Instance.TargetBall.BallParaBola(targetPosition);
        Debug.Log("플랫폼 테스트");
    }
}
public enum MovePlatformType
{
    ballFastPass,parabola,
    //해당 위치를 빠르게 지나가는 타입,곡선,
}
