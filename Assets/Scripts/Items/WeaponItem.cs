using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponItem : Item
{

    [Header("Weapon Model")]
    public GameObject weaponModel;

    [Header("Weapon Base Damage")]
    public int physicalDamage = 0;

    [Header("Weapon Poise Damage")]
    public float poiseDamage = 10;

    [Header("Stamina Costs")]
    public int baseStaminaCost = 20;
}
