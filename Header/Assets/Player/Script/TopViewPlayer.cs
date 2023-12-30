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
            moveDirection.x = Input.GetAxisRaw("Horizontal") != 0&& Input.GetAxisRaw("Vertical") == 0 ? Input.GetAxisRaw("Horizontal") : 0;
            lastMoveDir.x = moveDirection.x;
            moveDirection.y = Input.GetAxisRaw("Vertical") != 0 && Input.GetAxisRaw("Horizontal") == 0 ? Input.GetAxisRaw("Vertical") : 0;
            lastMoveDir.y = moveDirection.y;

            transform.position += moveDirection * moveSpeed * Time.deltaTime;
        }
    }
}
