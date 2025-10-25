using UnityEngine;
using System;
using System.Collections;
public class CubeController : Singleton_Mono_Method<CubeController>
{
    public ColorType currentType;
    private MeshRenderer cubeRenderer;
    private MaterialPropertyBlock cubeProperty;
    public int gridX;
    public int gridY;
    public bool isBeingDestroyed = false;
    public bool isTargeted = false;
    public static event Action<CubeController> OnCubeHit;

    private void Awake()
    {
        cubeProperty = new MaterialPropertyBlock();
        cubeRenderer = GetComponentInChildren<MeshRenderer>();
    }
    public void Init(ColorType type, Sprite texture)
    {
        currentType = type;
        cubeProperty.SetTexture("_MainTex", texture.texture);
        cubeRenderer.SetPropertyBlock(cubeProperty);
    }
    public void OnHitByTurtle()
    {
        if (isBeingDestroyed) return;
        isTargeted = false;
        OnCubeHit?.Invoke(this);        
    }

}
