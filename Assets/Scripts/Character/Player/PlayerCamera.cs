using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerCamera : MonoBehaviour
{
    public static PlayerCamera instance;

    public CinemachineVirtualCamera vCam;

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

        vCam = GetComponent<CinemachineVirtualCamera>();
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

}
