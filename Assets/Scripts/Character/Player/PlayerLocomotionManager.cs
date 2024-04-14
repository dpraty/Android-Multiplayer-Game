using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocomotionManager : CharacterLocomotionManager
{
    // Player Movement Class

    PlayerManager player;

    // we keep a copy of inputs from input manager here to easily update animator params
    public float verticalMovement;
    public float horizontalMovement;
    public float moveAmount;

    private Vector3 moveDirection;
    private Vector3 rotationDirection;

    [SerializeField] float walkingSpeed = 1.2f;
    [SerializeField] float runningSpeed = 4.4f;
    [SerializeField] float rotationSpeed = 1000f;

    protected override void Awake()
    {
        base.Awake();

        player = GetComponent<PlayerManager>();
    }

    // Animator values for both the player and client are set here
    protected override void Update()
    {
        base.Update();

        // player logic
        if (player.IsOwner)
        {
            player.characterNetworkManager.animatorVerticalValue.Value = verticalMovement;
            player.characterNetworkManager.animatorHorizontalValue.Value = horizontalMovement;
            player.characterNetworkManager.animatorMoveAmountValue.Value = moveAmount;
        }

        // client logic
        else
        {
            verticalMovement = player.characterNetworkManager.animatorVerticalValue.Value;
            horizontalMovement = player.characterNetworkManager.animatorHorizontalValue.Value;
            moveAmount = player.characterNetworkManager.animatorMoveAmountValue.Value;

            // the update animator params for the client happens here??
            player.playerAnimatorManager.UpdateAnimatorMovementParameters(0, moveAmount);
        }
    }

    public void HandleAllMovement()
    {
        HandleGroundedMovement();
    }

    // read movement inputs from player input manager
    private void GetMovementInputs()
    {
        verticalMovement = PlayerInputManager.instance.verticalInput;
        horizontalMovement = PlayerInputManager.instance.horizontalInput;
        moveAmount = PlayerInputManager.instance.moveAmount;
    }

    // handle movement and roation
    public void HandleGroundedMovement()
    {
        // read move input and calucalte move direction
        GetMovementInputs();
        
        moveDirection = verticalMovement*Vector3.forward + horizontalMovement*Vector3.right;
        moveDirection.Normalize();
        moveDirection.y = 0;

        // clip speed by moveAmount
        if (PlayerInputManager.instance.moveAmount > 0.5f)
        {
            player.characterController.Move(runningSpeed * Time.deltaTime * moveDirection);
        }
        else if (PlayerInputManager.instance.moveAmount <= 0.5f && PlayerInputManager.instance.moveAmount >= 0.01f)
        {
            player.characterController.Move(Time.deltaTime * walkingSpeed * moveDirection);
        }

        // unlocked rotation logic
        rotationDirection = moveDirection;

        if (rotationDirection == Vector3.zero)
        {
            rotationDirection = transform.forward;
        }

        transform.LookAt(transform.position + rotationSpeed * Time.deltaTime * rotationDirection, Vector3.up);
    }
}
