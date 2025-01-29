using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchJoystick : MonoBehaviour
{
    public RectTransform joystickBoundingBox;
    public RectTransform knob;
    
    public Vector2 joystickCenterScreenPosition;
    public float joystickScreenRadius;

    private void Awake()
    {
        joystickBoundingBox = GetComponent<RectTransform>();
        knob = transform.GetChild(0).GetComponent<RectTransform>();
    }

    private void Start()
    {
        CalculateJoystickValues();
    }

    // Calculate world coordinates for the joystick
    private void CalculateJoystickValues()
    {
        
        Vector3[] v = new Vector3[4];
        joystickBoundingBox.GetWorldCorners(v);

        joystickCenterScreenPosition.y = (v[0].y + v[1].y) / 2;
        joystickCenterScreenPosition.x = (v[1].x + v[2].x) / 2;

        joystickScreenRadius = (v[2].x - v[1].x)/2;
    }
}
