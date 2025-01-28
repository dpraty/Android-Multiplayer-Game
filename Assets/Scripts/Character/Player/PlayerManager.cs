using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class PlayerManager : CharacterManager
{
    //Base Class for the player

    [HideInInspector] public PlayerLocomotionManager playerLocomotionManager;
    [HideInInspector] public PlayerAnimatorManager playerAnimatorManager;
    [HideInInspector] public PlayerNetworkManager playerNetworkManager;
    [HideInInspector] public PlayerStatsManager playerStatsManager;

    protected override void Awake()
    {
        base.Awake();

        playerNetworkManager = GetComponent<PlayerNetworkManager>();
        playerStatsManager = GetComponent<PlayerStatsManager>();
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

        playerStatsManager.RegenerateStamina();

    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        // singleton logic for player transform is run after on network spawn
        if (IsOwner)
        {
            PlayerInputManager.instance.player = this;

            playerNetworkManager.currentStamina.OnValueChanged += PlayerUIManager.instance.playerUIHudManager.SetNewStaminaValue;
            playerNetworkManager.currentStamina.OnValueChanged += playerStatsManager.ResetStaminaRegenTimer;
            playerNetworkManager.currentHealth.OnValueChanged += PlayerUIManager.instance.playerUIHudManager.SetNewHealthValue;

            playerNetworkManager.maxHealth.Value = playerStatsManager.CalculateHealth(playerNetworkManager.vitality.Value);
            playerNetworkManager.currentHealth.Value = playerStatsManager.CalculateHealth(playerNetworkManager.vitality.Value);
            playerNetworkManager.maxStamina.Value = playerStatsManager.CalculateStamina(playerNetworkManager.endurance.Value);
            playerNetworkManager.currentStamina.Value = playerStatsManager.CalculateStamina(playerNetworkManager.endurance.Value);

            PlayerUIManager.instance.playerUIHudManager.SetMaxStaminaValue(playerNetworkManager.maxStamina.Value);
            PlayerUIManager.instance.playerUIHudManager.SetMaxHealthValue(playerNetworkManager.maxHealth.Value);

        }
    }
}
