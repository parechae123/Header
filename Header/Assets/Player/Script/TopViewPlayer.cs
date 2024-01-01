using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopViewPlayer : MonoBehaviour
{
    Vector3 lastMoveDir = Vector3.zero;
    Vector3 moveDirection = Vector3.zero;
    private bool isMoveAble = true;
    public float moveSpeed = 3;
    public string playerAnimState;
    private Animator anim;
    [SerializeField] Vector3 playerColliderCenter;
    [SerializeField]Vector2Int playerConvertedLastPos;
    [SerializeField]Vector2Int playerConvertedNowPos;

    void Start()
    {
        anim = transform.GetChild(0).GetComponent<Animator>();
        playerColliderCenter = GetComponent<Collider2D>().bounds.center;
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

            if (Input.GetKeyDown(KeyCode.Space))
            {
                Managers.instance.Grid.GridCheck(transform.position+playerColliderCenter);
                //플레이어 다리위치로 위치 보정
            }
            if (VectorToIntPos(transform.position + playerColliderCenter) != playerConvertedLastPos)
            {
                if (Managers.instance.Grid.InteractionInitOutIt(transform.position + playerColliderCenter, ref playerConvertedLastPos))
                {

                }
                else
                {

                }
            }
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

}
