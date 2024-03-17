using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
}
