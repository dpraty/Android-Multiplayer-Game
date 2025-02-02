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

    [Header("Attack Types Damage Multiplier")]
    public float light_Attack_01_Modifier = 1.1f;

    [Header("Stamina Costs")]
    public int baseStaminaCost = 20;
    public float lightAttackStaminaCostMultiplier = 0.9f;

    [Header("Actions")]
    public WeaponItemAction lightAttack;
}
