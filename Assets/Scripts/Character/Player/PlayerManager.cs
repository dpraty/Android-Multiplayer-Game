using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class PlayerManager : CharacterManager
{
    //Base Class for the player

    [Header("DEBUG MENU")]
    [SerializeField] bool respawnCharacter = false;
    [SerializeField] bool switchMeleeWeapon = false;

    [HideInInspector] public PlayerLocomotionManager playerLocomotionManager;
    [HideInInspector] public PlayerAnimatorManager playerAnimatorManager;
    [HideInInspector] public PlayerNetworkManager playerNetworkManager;
    [HideInInspector] public PlayerStatsManager playerStatsManager;
    [HideInInspector] public PlayerInventoryManager playerInventoryManager;
    [HideInInspector] public PlayerEquipmentManager playerEquipmentManager;

    protected override void Awake()
    {
        base.Awake();

        playerNetworkManager = GetComponent<PlayerNetworkManager>();
        playerStatsManager = GetComponent<PlayerStatsManager>();
        playerLocomotionManager = GetComponent<PlayerLocomotionManager>();
        playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
        playerInventoryManager = GetComponent<PlayerInventoryManager>();
        playerEquipmentManager = GetComponent<PlayerEquipmentManager>();
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

        DebugMenu();

    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        // singleton logic for player transform is run after on network spawn
        if (IsOwner)
        {
            PlayerInputManager.instance.player = this;

            // Set up UI Hud Manager to update health and stamina values
            playerNetworkManager.currentStamina.OnValueChanged += PlayerUIManager.instance.playerUIHudManager.SetNewStaminaValue;
            playerNetworkManager.currentHealth.OnValueChanged += PlayerUIManager.instance.playerUIHudManager.SetNewHealthValue;

            // Set up the Stamina Regen Timer 
            playerNetworkManager.currentStamina.OnValueChanged += playerStatsManager.ResetStaminaRegenTimer;

            // Set up Max Health and Stamina on network spawn
            playerNetworkManager.maxHealth.Value = playerStatsManager.CalculateHealth(playerNetworkManager.vitality.Value);
            playerNetworkManager.currentHealth.Value = playerStatsManager.CalculateHealth(playerNetworkManager.vitality.Value);
            playerNetworkManager.maxStamina.Value = playerStatsManager.CalculateStamina(playerNetworkManager.endurance.Value);
            playerNetworkManager.currentStamina.Value = playerStatsManager.CalculateStamina(playerNetworkManager.endurance.Value);

            // Set up UI Hud Manager to show Max Health and Stamina
            PlayerUIManager.instance.playerUIHudManager.SetMaxStaminaValue(playerNetworkManager.maxStamina.Value);
            PlayerUIManager.instance.playerUIHudManager.SetMaxHealthValue(playerNetworkManager.maxHealth.Value);

        }

        playerNetworkManager.currentHealth.OnValueChanged += playerNetworkManager.CheckHP;

        playerNetworkManager.currentMeleeWeaponID.OnValueChanged += playerNetworkManager.OnCurrentMeleeWeaponIDChange;
        playerNetworkManager.currentRangedWeaponID.OnValueChanged += playerNetworkManager.OnCurrentRangedWeaponIDChange;
    }

    public override IEnumerator ProcessDeathEvent()
    {
        if (IsOwner)
        {
            PlayerUIManager.instance.playerUIPopUpManager.SendDeathPopUp();
        }

        return base.ProcessDeathEvent();
    }

    public override void ReviveCharacter()
    {
        base.ReviveCharacter();

        if (IsOwner)
        {
            playerNetworkManager.currentHealth.Value = playerNetworkManager.maxHealth.Value;
            playerNetworkManager.currentStamina.Value = playerNetworkManager.maxStamina.Value;

            playerAnimatorManager.PlayTargetActionAnimation("Empty", false);
        }
    }

    private void DebugMenu()
    {
        if (respawnCharacter)
        {
            respawnCharacter = false;
            ReviveCharacter();
        }

        if (switchMeleeWeapon)
        {
            switchMeleeWeapon = false;
            playerEquipmentManager.SwitchMeleeWeapon();
        }
    }
}
