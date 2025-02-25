using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocomotionManager : CharacterLocomotionManager
{
    // Player Movement Class

    PlayerManager player;

    // we keep a copy of inputs from input manager here to easily update animator params
    [HideInInspector] public float verticalMovement;
    [HideInInspector] public float horizontalMovement;
    [HideInInspector] public float moveAmount;

    [Header("Movement Settings")]
    private Vector3 moveDirection;
    private Vector3 rotationDirection;
    [SerializeField] float moveSpeed = 4.4f;
    [SerializeField] float rotationSpeed = 1000f;

    [Header("Dodge")]
    private Vector3 rollDirection;
    [SerializeField] float dodgeStaminaCost = 20;

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
            if (player.playerNetworkManager.isLockedOn.Value)
            {
                Vector3 targetDirection = player.playerCombatManager.currentTarget.transform.position - player.transform.position;

                Vector2 direction = new Vector2(targetDirection.x, targetDirection.z);
                direction.Normalize();
                Vector2 movement = new Vector2(horizontalMovement, verticalMovement);
                movement.Normalize();

                float verticalValue = Vector3.Dot(movement, direction);
                float crossZ = movement.x * direction.y - movement.y * targetDirection.x;
                float horizontalMag = Mathf.Sqrt(Mathf.Abs(movement.sqrMagnitude - Mathf.Pow(verticalValue, 2)));
                float horizontalValue = Mathf.Sign(crossZ) * horizontalMag;

                player.playerAnimatorManager.UpdateAnimatorMovementParameters(horizontalValue, verticalValue);
            }
            else
            {
                player.playerAnimatorManager.UpdateAnimatorMovementParameters(0, moveAmount);
            }
        }
    }

    public void HandleAllMovement()
    {
        HandleGroundedMovement();
        HandleRotation();
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
        moveDirection.y = 0;
        moveDirection.Normalize();
        
        if (!player.canMove)
            return;

            player.characterController.Move(moveSpeed * Time.deltaTime * moveDirection);    
    }

    private void HandleRotation()
    {
        if (!player.canRotate)
            return;

        if (player.playerNetworkManager.isLockedOn.Value)
        {
            rotationDirection = player.playerCombatManager.currentTarget.transform.position - player.transform.position;
            rotationDirection.Normalize();
            rotationDirection.y = 0;

            player.transform.LookAt(player.transform.position + rotationSpeed * Time.deltaTime * rotationDirection, Vector3.up);

        }
        // unlocked rotation logic - this will be different when we lock on to an enemy
        else
        {
            rotationDirection = moveDirection;

            if (rotationDirection == Vector3.zero)
            {
                rotationDirection = player.transform.forward;
            }

            player.transform.LookAt(player.transform.position + rotationSpeed * Time.deltaTime * rotationDirection, Vector3.up);
        }
    }

    public void AttemptToPerformDodge()
    {
        if (player.isPerformingAction)
            return;

        if (player.playerNetworkManager.currentStamina.Value <= 0)
            return;

        // if the player is moving, perform a roll in the direction of movement
        if (moveAmount > 0)
        {
            rollDirection = verticalMovement * Vector3.forward + horizontalMovement * Vector3.right;
            rollDirection.Normalize();
            rollDirection.y = 0;

            Quaternion playerRotation = Quaternion.LookRotation(rollDirection);
            player.transform.rotation = playerRotation;

            player.playerAnimatorManager.PlayTargetActionAnimation("Roll_Forward", true, true);
        }
        // else we back step
        else
        {
            player.playerAnimatorManager.PlayTargetActionAnimation("Backstep", true, true);
        }

        player.playerNetworkManager.currentStamina.Value -= dodgeStaminaCost;
    }
}
