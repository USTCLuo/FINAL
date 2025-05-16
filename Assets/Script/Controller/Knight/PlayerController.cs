using System.Collections;
using System.Collections.Generic;
//using System.Numerics;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public float maxMoveSpeed = 5;
    public float moveSpeed = 0;

    public float jumpSpeed = 10;
    public float gravity = 20;
    public bool isGrounded = true;//默认在地面上
    private float verticalSpeed = 0;
    private CharacterController characterController;

    private PlayerInput playerInput;

    private Vector3 move;

    public Transform renderCamera;
    public float angleSpeed = 400;//旋转角速度
    public float accelerationSpeed = 5;//人物加速度
    private Animator animator;
    private void Awake()
    {
        characterController = transform.GetComponent<CharacterController>();
        playerInput = transform.GetComponent<PlayerInput>();
        animator = transform.GetComponent<Animator>();

    }

    private void Update()
    {
        CaculateVerticalSpeed();
        CaculateMoveSpeed();
        CaculateRotation();
    }
    private void OnAnimatorMove()
    {
        CaculateMove();
    }
    public void CaculateMove(){
        // float h = Input.GetAxis("Horizontal");
        // float v = Input.GetAxis("Vertical");

        // Vector3 move = new Vector3(h, 0, v);
        if(isGrounded){
            move = animator.deltaPosition;
        }else{
            move = moveSpeed * transform.forward * Time.deltaTime;
        }
        move = animator.deltaPosition;
        //move.Set(playerInput.Move.x, 0, playerInput.Move.y);
        //move *= Time.deltaTime*moveSpeed;

        //move = renderCamera.TransformDirection(move);
        //transform.rotation = Quaternion.LookRotation(move);
        
  
        move += Vector3.up * verticalSpeed * Time.deltaTime;
        
        characterController.Move(move);
        isGrounded = characterController.isGrounded;
        animator.SetBool("isGround",isGrounded);
    }
    public void CaculateVerticalSpeed(){
        if(isGrounded){
            verticalSpeed = -gravity * 0.3f;
            if(playerInput.Jump){
                verticalSpeed = jumpSpeed;
                isGrounded = false;
            }
        }else{
            if(!Input.GetKeyDown(KeyCode.Space) && verticalSpeed > 0){
                verticalSpeed -= gravity * Time.deltaTime;
            }
            verticalSpeed -= gravity * Time.deltaTime;
            if(characterController.isGrounded){
                isGrounded = true;
            }
        }
        animator.SetFloat("verticalSpeed",verticalSpeed);

    }
    private void CaculateMoveSpeed(){
        moveSpeed = Mathf.MoveTowards(moveSpeed,maxMoveSpeed*playerInput.Move.normalized.magnitude, accelerationSpeed * Time.deltaTime);
        animator.SetFloat("forwardSpeed",moveSpeed);
    }
    private void CaculateRotation(){
        if(playerInput.Move.x != 0 || playerInput.Move.y != 0){
            Vector3 targetDirection = renderCamera.TransformDirection(new Vector3(playerInput.Move.x,0, playerInput.Move.y));
            targetDirection.y = 0;
            
            transform.rotation = Quaternion.RotateTowards( transform.rotation, Quaternion.LookRotation(targetDirection), angleSpeed * Time.deltaTime);
        }
    }
}
