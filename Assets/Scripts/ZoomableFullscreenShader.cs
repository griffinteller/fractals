using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class ZoomableFullscreenShader : MonoBehaviour 
{
    public Shader shader;
    public float zoomSensitivity = 2;
    public float panSensitivity = 100;
    public Vector2 c;
    public int iterations = 10;
    public float divergence = 2;
    public float colorDecay = 10;
    public Slider realSlider;
    public Slider imSlider;

    private Material _material;
    private Camera _thisCamera;

    private float3x3 _zoomMatrix = new float3x3(1, 0, 0, 0, 1, 0, 0, 0, 1); // identity
    private float ZoomScale => _zoomMatrix.c0.x;
    private float2 ZoomTrans => new float2(_zoomMatrix.c2.x, _zoomMatrix.c2.y);

    // Creates a private material used to the effect
    public void Awake ()
    {
        _thisCamera = GetComponent<Camera>();
        _material = new Material(shader);
    }

    public void Update()
    {
        c.x = realSlider.value;
        c.y = imSlider.value;
        
        float zoomAmount = Mathf.Pow(zoomSensitivity, -Input.mouseScrollDelta.y * Time.deltaTime);
        Vector3 rawMousePos = Input.mousePosition;
        Vector2 mousePos = new Vector2(
            rawMousePos.x / Screen.width * 2 - 1, 
            (rawMousePos.y / Screen.height * 2 - 1) / _thisCamera.aspect);
        Zoom(mousePos, zoomAmount);
        
        if (Input.GetMouseButton(1))
            Pan(-MouseTracker.MouseDelta * panSensitivity);
    }

    private void Zoom(float2 center, float scale)
    {
        float3x3 scaleM = ScaleMatrix(center, scale);
        _zoomMatrix = math.mul(_zoomMatrix, scaleM);
    }

    private void Pan(float2 pan)
    {
        _zoomMatrix = math.mul(_zoomMatrix, PanMatrix(pan));
    }

    private static float3x3 ScaleMatrix(float2 center, float scale)
    {
        return new float3x3(scale, 0, center.x - center.x * scale, 0, scale, center.y - center.y * scale, 0, 0, 1);
    }

    private static float3x3 PanMatrix(float2 pan)
    {
        return new float3x3(1, 0, pan.x, 0, 1, pan.y, 0, 0, 1);
    }

    // Postprocess the image
    public void OnRenderImage (RenderTexture source, RenderTexture destination)
    {
        _material.SetFloat("_Aspect", _thisCamera.aspect);
        _material.SetFloat("_ZoomScale", ZoomScale);
        _material.SetVector("_ZoomTrans", (Vector2) ZoomTrans);
        _material.SetFloat("_DivergeLimit", divergence);
        _material.SetFloat("_IterLimit", iterations);
        _material.SetVector("_CParameter", c);
        _material.SetFloat("_ColorDecay", colorDecay);
        Graphics.Blit (source, destination, _material);
    }
}