using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchButton : MonoBehaviour
{
    public RectTransform buttonBoundingBox;

    public Vector2 buttonScreenPosition;
    public float buttonRadius;

    public bool buttonPressed;
    private float buttonPhase;
    private float buttonPressStart;
    
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
            buttonBoundingBox.sizeDelta =  buttonPhase * buttonRadius * 2 * Vector2.one;

            if (buttonPhase >= 1) buttonPressed = false;
        }
    }

    // Calculate world coordinates for the button
    private void CalculateButtonValues()
    {
        Vector3[] v = new Vector3[4];
        buttonBoundingBox.GetWorldCorners(v);

        buttonScreenPosition.y = (v[0].y + v[1].y) / 2;
        buttonScreenPosition.x = (v[1].x + v[2].x) / 2;

        buttonRadius = (v[2].x - v[1].x) / 2;

    }

    // Function called when the button is pressed
    public void PressButton()
    {
        buttonPressed=true;
        buttonPhase = 0.8f;
        buttonPressStart = Time.time;

    }
}
