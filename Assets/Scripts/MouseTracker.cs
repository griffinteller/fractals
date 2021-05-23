using System;
using UnityEngine;

public class MouseTracker : MonoBehaviour
{
    public static Vector2 MouseDelta; // normalized

    private Vector2 _mouseLastFrame;

    public void Start()
    {
        float aspect = (float) Screen.width / Screen.height;
        Vector3 rawMousePos = Input.mousePosition;
        Vector2 mousePos = new Vector2(
            rawMousePos.x / Screen.width * 2 - 1, 
            (rawMousePos.y / Screen.height * 2 - 1) / aspect);
        
        _mouseLastFrame = mousePos;
    }

    public void Update()
    {
        float aspect = (float) Screen.width / Screen.height;
        Vector3 rawMousePos = Input.mousePosition;
        Vector2 mousePos = new Vector2(
            rawMousePos.x / Screen.width * 2 - 1, 
            (rawMousePos.y / Screen.height * 2 - 1) / aspect);

        MouseDelta = (mousePos - _mouseLastFrame) * Time.deltaTime;
        _mouseLastFrame = mousePos;
    }
}