using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventoryManager : CharacterInventoryManager
{
    public WeaponItem currentMeleeWeapon;
    public WeaponItem currentRangedWeapon;

    [Header("Inventory Slots")]
    public MeleeWeaponItem[] meleeWeaponsInSlots = new MeleeWeaponItem[3];
    public int meleeWeaponIndex = 0;
    
    public RangedWeaponItem[] rangedWeaponsInSlots = new RangedWeaponItem[3];
    public int rangedWeaponIndex = 0;
}
