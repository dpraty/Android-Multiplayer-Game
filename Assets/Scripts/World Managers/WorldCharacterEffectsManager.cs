using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldCharacterEffectsManager : MonoBehaviour
{
    // World Character Effects Manager - singleton

    public static WorldCharacterEffectsManager instance;

    // we can add different damage types here
    [Header("Damage")]
    public TakeDamageEffect takeDamageEffect;

    // we can add timed effects and static effects here
    [SerializeField] List<InstantCharacterEffect> instantEffects;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // generate effect ids for all effect types
    private void GenerateEffectsIDs()
    {
        for (int i = 0; i < instantEffects.Count; i++)
        {
            instantEffects[i].instantEffectID = i;
        }
    }
}
