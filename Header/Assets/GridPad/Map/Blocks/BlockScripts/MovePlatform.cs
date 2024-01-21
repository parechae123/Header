using UnityEngine;
using HeaderPadDefines;

public class MovePlatform : MonoBehaviour
{
    [SerializeField] BlockObjects blockInfo = new BlockObjects();
    [SerializeField] Vector2 targetPosition = new Vector2();
    // Start is called before the first frame update
    void Start()
    {
        gameObject.layer = 3;
        Managers.instance.Grid.AddBattleGridData(transform.position, blockInfo, asdf);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawCube(targetPosition, Vector2.one);
    }
    public void asdf()
    {
        ShoterController.Instance.TargetBall.BallToTarget(targetPosition);
        Debug.Log("ÇÃ·§Æû Å×½ºÆ®");
    }
}
public enum MovePlatformType
{

}
