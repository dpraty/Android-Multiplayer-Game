using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class PlayerManager : CharacterManager
{
    //Base Class for the player

    [HideInInspector] public PlayerLocomotionManager playerLocomotionManager;
    [HideInInspector] public PlayerAnimatorManager playerAnimatorManager;

    protected override void Awake()
    {
        base.Awake();

        playerLocomotionManager = GetComponent<PlayerLocomotionManager>();
        playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
    }

    protected override void Start()
    {
        base.Start();

        // set Cinemachine LookAt and Follow targets to player transform at Start
        if (IsOwner)
        {
            PlayerCamera.instance.vCam.LookAt = transform;
            PlayerCamera.instance.vCam.Follow = transform;
        }
    }

    protected override void Update()
    {
        base.Update();

        // We do not execute movement logic for clients, rather we read the transform values from the network
        if (!IsOwner)
            return;

        playerLocomotionManager.HandleAllMovement();

    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        // singleton logic for player transform is run after on network spawn
        if (IsOwner)
        {
            PlayerInputManager.instance.player = this;
        }
    }
}
