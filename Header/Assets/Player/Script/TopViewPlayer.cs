using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopViewPlayer : MonoBehaviour
{
    public static TopViewPlayer Instance;
    Vector3 lastMoveDir = Vector3.zero;
    Vector3 moveDirection = Vector3.zero;
    public bool isMoveAble = true;
    public float moveSpeed = 3;
    public string playerAnimState;
    private Animator anim;

    HashSet<Vector2Int> interactionTiles;
    [SerializeField] Vector3 playerColliderCenter;
    [SerializeField]Vector2Int playerConvertedLastPos;
    [SerializeField]Vector2Int playerConvertedNowPos;
    
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        anim = transform.GetChild(0).GetComponent<Animator>();
        playerColliderCenter = GetComponent<Collider2D>().bounds.center;
        Managers.instance.UI.DialogCall.DialogSetting();
        interactionTiles = Managers.instance.Grid.isInteractionAreThere;
    }

    // Update is called once per frame
    void Update()
    {
        if (isMoveAble)
        {

            if (Input.GetAxisRaw("Horizontal") != 0&& Input.GetAxisRaw("Vertical") != 0)
            {
                moveDirection.x = Input.GetAxisRaw("Horizontal") != 0? Input.GetAxisRaw("Horizontal") : 0;
                moveDirection.y = Input.GetAxisRaw("Vertical") != 0? Input.GetAxisRaw("Vertical") : 0;
                moveDirection = moveDirection - lastMoveDir;
            }
            else
            {
                moveDirection.x = Input.GetAxisRaw("Horizontal") != 0 && Input.GetAxisRaw("Vertical") == 0 ? Input.GetAxisRaw("Horizontal") : 0;
                moveDirection.y = Input.GetAxisRaw("Vertical") != 0 && Input.GetAxisRaw("Horizontal") == 0 ? Input.GetAxisRaw("Vertical") : 0;
                lastMoveDir = moveDirection;
            }
            transform.position += moveDirection * moveSpeed * Time.deltaTime;
            SetPlayerState();
            if (!anim.GetCurrentAnimatorStateInfo(0).IsName(playerAnimState))
            {
                anim.Play(playerAnimState, 0);
            }
            if (VectorToIntPos(transform.position + playerColliderCenter) != playerConvertedLastPos)
            {
                if (InteractionInitOutIt(transform.position + playerColliderCenter, ref playerConvertedLastPos))
                {

                }
                else
                {

                }
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Managers.instance.UI.CloseUIStack();
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Managers.instance.Grid.GridCheck(transform.position + playerColliderCenter);
            //플레이어 다리위치로 위치 보정
        }
    }
    public void SetPlayerState()
    {
        if (moveDirection.x != 0)
        {
            playerAnimState = moveDirection.x > 0  ? "Walk_Right" : "Walk_Left";
        }
        else if(moveDirection.y != 0)
        {
            playerAnimState = moveDirection.y > 0  ? "Walk_Front" : "Walk_Back";
        }
        else
        {
            playerAnimState = "Walk_Idle";
        }

    }
    Vector2Int VectorToIntPos(Vector3 tempVec)
    {
        Vector2Int temVecInt = new Vector2Int(Mathf.RoundToInt(tempVec.x), Mathf.RoundToInt(tempVec.y));
        return temVecInt;
    }
    public bool InteractionInitOutIt(Vector3 plrPos, ref Vector2Int lastPos)
    {
        Vector2Int tempBigPos = VectorToIntPos(plrPos);
        if (interactionTiles.Contains(tempBigPos))
        {
            if (interactionTiles.Contains(lastPos))
            {
                Managers.instance.Grid.interactionGrid[lastPos].OutIt();
            }
            Managers.instance.Grid.interactionGrid[tempBigPos].Init();
            lastPos = tempBigPos;
            return true;
        }
        else
        {
            if (interactionTiles.Contains(lastPos))
            {
                Managers.instance.Grid.interactionGrid[lastPos].OutIt();
                lastPos = tempBigPos;
            }
            return false;
        }

    }
}
