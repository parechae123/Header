using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopViewPlayer : MonoBehaviour
{
    Vector3 lastMoveDir = Vector3.zero;
    Vector3 moveDirection = Vector3.zero;
    private bool isMoveAble = true;
    public float moveSpeed = 3;
    private Animator anim;
    void Start()
    {
        anim = transform.GetChild(0).GetComponent<Animator>();
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
        }
    }
}
