using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventoryManager : CharacterInventoryManager
{
    public WeaponItem currentRightHandWeapon;
    public WeaponItem currentLeftHandWeapon;

    [Header("Inventory Slots")]
    public WeaponItem[] weaponsInRightHandSlots = new WeaponItem[3];
    public WeaponItem[] weaponsInLeftHandSlots = new WeaponItem[3];
}
