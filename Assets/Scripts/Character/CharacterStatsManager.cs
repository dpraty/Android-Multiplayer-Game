using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class CharacterStatsManager : MonoBehaviour
{
    CharacterManager character;

    [Header("Stamina Regen")]
    [SerializeField]
    float staminaRegen = 2;
    private float staminaRegenTimer = 0;
    private float staminaTick = 0;
    [SerializeField] float staminaRegenDelay = 1;

    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
    }

    public int CalculateHealth(int vitality)
    {
        int health = 0;
        health = vitality * 10;
        return health;
    }

    public int CalculateStamina(int endurance)
    {
        int stamina = 0;
        stamina = endurance * 10;
        return stamina;
    }

    public virtual void RegenerateStamina()
    {
        if (!character.IsOwner)
            return;

        if (character.isPerformingAction)
            return;

        staminaRegenTimer += Time.deltaTime;

        if (staminaRegenTimer >= staminaRegenDelay)
        {
            if (character.characterNetworkManager.currentStamina.Value < character.characterNetworkManager.maxStamina.Value)
            {
                staminaTick += Time.deltaTime;

                if (staminaTick >= 0.1)
                {
                    staminaTick = 0;
                    character.characterNetworkManager.currentStamina.Value += staminaRegen;
                }
            }
        }
    }

    public virtual void ResetStaminaRegenTimer(float previousStamina, float currentStamina)
    {
        if (currentStamina < previousStamina)
        {
            staminaRegenTimer = 0;
        }
        
    }
}
