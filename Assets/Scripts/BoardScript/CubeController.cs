using UnityEngine;
using System;
using System.Collections;
public class CubeController : MonoBehaviour
{
    public ColorType currentType;
    private MeshRenderer cubeRenderer;
    private MaterialPropertyBlock cubeProperty;
    public int gridX;
    public int gridY;
    private void Awake()
    {
        cubeProperty = new MaterialPropertyBlock();
        cubeRenderer = GetComponentInChildren<MeshRenderer>();
    }

    public void Init(ColorType type, Sprite texture)
    {
        currentType = type;
        //var block = new MaterialPropertyBlock();
        cubeProperty.SetTexture("_MainTex", texture.texture);
        cubeRenderer.SetPropertyBlock(cubeProperty);
    }
    public void OnHitByTurtle()
    {
        Debug.Log($"Cube of type {currentType} was hit!");
        StartCoroutine(HandleDestroyCube());
    }
    private IEnumerator HandleDestroyCube()
    {
        yield return new WaitForSeconds(0.05f); // Giảm delay cho nhanh hơn

        if (BoardManager.d_Instance != null)
            BoardManager.d_Instance.OnCubeDestroyed(this);

        yield return new WaitForEndOfFrame(); // Cho DropCubes xử lý xong
        ObjectPools.d_Instance.ReturnToPool("Cube", gameObject);
    }
}
