using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchSquareButton : MonoBehaviour
{
    public RectTransform buttonBoundingBox;
    public Vector3 cornerPoint_1;
    public Vector3 cornerPoint_2;

    public bool buttonPressed;
    private float buttonPressStart;
    private float buttonPhase;

    private void Awake()
    {
        buttonBoundingBox = GetComponent<RectTransform>();
    }

    private void Start()
    {
        CalculateButtonValues();
    }

    private void Update()
    {
        // Once pressed the button cannot be pressed again till phase >= 1
        if (buttonPressed)
        {
            buttonPhase = Mathf.Lerp(0.8f, 1, (Time.time - buttonPressStart) / 0.1f);

            if (buttonPhase >= 1) buttonPressed = false;
        }
    }

    private void CalculateButtonValues()
    {
        Vector3[] v = new Vector3[4]; 
        buttonBoundingBox.GetWorldCorners(v);

        cornerPoint_1 = v[0];
        cornerPoint_2 = v[2];
    }

    public void PressButton()
    {
        buttonPressed = true;
        buttonPressStart = Time.time;
        buttonPhase = 0.8f;
    }
}
