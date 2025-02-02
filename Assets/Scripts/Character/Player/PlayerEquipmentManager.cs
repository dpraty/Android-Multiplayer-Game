using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEquipmentManager : CharacterEquipmentManager
{
    PlayerManager player;

    public WeaponModelInstantiationSlot meleeWeaponSlot;
    public WeaponModelInstantiationSlot rangedWeaponSlot;

    [SerializeField] MeleeWeaponManager meleeWeaponManager;
    [SerializeField] RangedWeaponManager rangedWeaponManager;

    public GameObject meleeWeaponModel;
    public GameObject rangedWeaponModel;

    protected override void Awake()
    {
        base.Awake();

        player = GetComponent<PlayerManager>();

        InitializeWeaponSlots();
    }

    protected override void Start()
    {
        base.Start();

        LoadWeaponsOnBothHands();
    }

    private void InitializeWeaponSlots()
    {
        WeaponModelInstantiationSlot[] weaponSlots = GetComponentsInChildren<WeaponModelInstantiationSlot>();

        foreach (var weaponSlot in weaponSlots)
        {
            if (weaponSlot.weaponSlot == WeaponModelSlot.MeleeWeapon)
            {
                meleeWeaponSlot = weaponSlot;
            }
            else if (weaponSlot.weaponSlot == WeaponModelSlot.RangedWeapon)
            {
                rangedWeaponSlot = weaponSlot;
            }
        }
    }

    public void LoadWeaponsOnBothHands()
    {
        LoadMeleeWeapon();
        LoadRangedWeapon();
    }

    // Right Weapon

    public void LoadMeleeWeapon()
    {
        if (player.playerInventoryManager.currentMeleeWeapon != null)
        {
            meleeWeaponSlot.UnloadWeapon();

            meleeWeaponModel = Instantiate(player.playerInventoryManager.currentMeleeWeapon.weaponModel);
            meleeWeaponSlot.LoadWeapon(meleeWeaponModel);
            meleeWeaponManager = meleeWeaponModel.GetComponent<MeleeWeaponManager>();
            meleeWeaponManager.SetWeaponDamage(player, player.playerInventoryManager.currentMeleeWeapon);

            if (player.IsOwner)
                PlayerUIManager.instance.playerUIHudManager.SwitchWeaponLogo(player.playerInventoryManager.currentMeleeWeapon.itemIcon);
        }
    }

    public void SwitchMeleeWeapon()
    {
        if (!player.IsOwner)
            return;

        player.playerAnimatorManager.PlayTargetActionAnimation("Equip_Melee_Weapon", false, true, true, true);

        WeaponItem selectedWeapon = null;

        player.playerInventoryManager.meleeWeaponIndex += 1;

        if (player.playerInventoryManager.meleeWeaponIndex <0 || player.playerInventoryManager.meleeWeaponIndex > 2)
        {
            player.playerInventoryManager.meleeWeaponIndex = 0;
        }

        selectedWeapon = player.playerInventoryManager.meleeWeaponsInSlots[player.playerInventoryManager.meleeWeaponIndex];
        player.playerNetworkManager.currentMeleeWeaponID.Value = selectedWeapon.itemID;

    }

    // Left Weapon

    public void LoadRangedWeapon()
    {
        if (player.playerInventoryManager.currentRangedWeapon != null)
        {
            rangedWeaponSlot.UnloadWeapon();

            rangedWeaponModel = Instantiate(player.playerInventoryManager.currentRangedWeapon.weaponModel);
            rangedWeaponSlot.LoadWeapon(rangedWeaponModel);
            rangedWeaponManager = rangedWeaponModel.GetComponent<RangedWeaponManager>();
            //rangedWeaponManager.SetWeaponDamage(player, player.playerInventoryManager.currentRangedWeapon);
        }
        
    }

    public void SwitchRangedWeapon()
    {
        if (!player.IsOwner)
            return;

        player.playerAnimatorManager.PlayTargetActionAnimation("Equip_Ranged_Weapon", false, true, true, true);

        WeaponItem selectedWeapon = null;

        player.playerInventoryManager.rangedWeaponIndex += 1;

        if (player.playerInventoryManager.rangedWeaponIndex < 0 || player.playerInventoryManager.rangedWeaponIndex > 2)
        {
            player.playerInventoryManager.rangedWeaponIndex = 0;
        }

        selectedWeapon = player.playerInventoryManager.rangedWeaponsInSlots[player.playerInventoryManager.rangedWeaponIndex];
        player.playerNetworkManager.currentRangedWeaponID.Value = selectedWeapon.itemID;
    }

    // Damage Colliders


    public void OpenDamageCollider()
    {
        meleeWeaponManager.meleeDamageCollider.EnableDamageCollider();
    }

    public void CloseDamageCollider()
    {
        meleeWeaponManager.meleeDamageCollider.DisableDamageCollider();
    }
}
