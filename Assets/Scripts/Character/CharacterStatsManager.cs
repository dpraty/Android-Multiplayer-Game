using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class CharacterStatsManager : MonoBehaviour
{
    CharacterManager character;

    [Header("Stamina Regen")]
    [SerializeField] float staminaRegen = 2;
    private float staminaRegenTimer = 0;
    private float staminaTick = 0;
    [SerializeField] float staminaRegenDelay = 1;

    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
    }

    // Calculate Health based on vitality
    public int CalculateHealth(int vitality)
    {
        int health = 0;
        health = vitality * 10;
        return health;
    }

    // Calculate Stamina based on Endurance
    public int CalculateStamina(int endurance)
    {
        int stamina = 0;
        stamina = endurance * 10;
        return stamina;
    }

    // Function to Regenerate Stamina
    public virtual void RegenerateStamina()
    {
        if (!character.IsOwner)
            return;

        if (character.isPerformingAction)
            return;

        staminaRegenTimer += Time.deltaTime;

        // Stamina can regenerate only after a delay
        if (staminaRegenTimer >= staminaRegenDelay)
        {
            if (character.characterNetworkManager.currentStamina.Value < character.characterNetworkManager.maxStamina.Value)
            {
                staminaTick += Time.deltaTime;

                // Add a tick delay to regeneration
                if (staminaTick >= 0.1)
                {
                    staminaTick = 0;
                    character.characterNetworkManager.currentStamina.Value += staminaRegen;
                }
            }
        }
    }

    // Reset Stamina Regen Timer whenever we consume stamina
    public virtual void ResetStaminaRegenTimer(float previousStamina, float currentStamina)
    {
        if (currentStamina < previousStamina)
        {
            staminaRegenTimer = 0;
        }
        
    }
}
