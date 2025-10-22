using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct CubeMap
{
    public char cubeCharacter;
    public Sprite cubeColorSprite;
    public ColorType cubeColorType;
}

public class BoardManager : Singleton_Mono_Method<BoardManager>
{
    public List<CubeController> topRowCubes = new List<CubeController>();
    public List<CubeMap> cubeColorsMap;
    public List<GameObject> cubeList;
    public Transform boardHolder;

    [Header("Board Settings")]
    [SerializeField] private GameObject baseCubePrefab;
    [SerializeField] private float cubeSize = 1.1f;
    [SerializeField] private Vector2 startPosition = new Vector2(0f, 0f);
    private CubeController[,] cubeGrid;
    public void MapGenerator(TextAsset levelLayout)
    {
        string[] rows = levelLayout.text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        int rowCount = rows.Length;
        int colCount = rows[0].Split('-').Length;
        cubeGrid = new CubeController[colCount, rowCount];
        float offsetX = (colCount - 1) * cubeSize * 0.5f;
        float offsetY = (rowCount - 1) * cubeSize * 0.5f;
        for (int y = 0; y < rows.Length; y++)
        {
            string[] columns = rows[y].Split('-');
            for(int x = 0; x < columns.Length; x++)
            {
                Vector3 spawnPos = new Vector3(
                    startPosition.x + (x * cubeSize), 0,
                    startPosition.y - (y * cubeSize)
                );

                char cubeTypeChar = columns[x][0];

                var cubeToSpawn = GetCubeMapChar(cubeTypeChar);
                
                GameObject newCube = Instantiate(baseCubePrefab, spawnPos, Quaternion.identity, boardHolder);
                CubeController cubeCtrl = newCube.GetComponent<CubeController>();
                cubeCtrl.Init(cubeToSpawn.Value.cubeColorType, cubeToSpawn.Value.cubeColorSprite);

                cubeCtrl.gridX = x;
                cubeCtrl.gridY = y;

                cubeGrid[x, y] = cubeCtrl;

                cubeList.Add(newCube);
            }
        }
        topRowCubes = GetTopRowCubes();
    }

    public List<CubeController> GetTopRowCubes()
    {
        List<CubeController> topRowCubes = new List<CubeController>();
        float highestZ = float.MaxValue;
        foreach (var cubeObj in cubeList)
        {
            if (cubeObj.transform.position.z < highestZ)
            {
                highestZ = cubeObj.transform.position.z;
            }
        }
        foreach (var cubeObj in cubeList)
        {
            if (Mathf.Approximately(cubeObj.transform.position.z, highestZ))
            {
                CubeController cubeCtrl = cubeObj.GetComponent<CubeController>();
                if (cubeCtrl != null)
                {
                    topRowCubes.Add(cubeCtrl);
                }
            }
        }
        return topRowCubes;
    }
    private CubeMap? GetCubeMapChar(char cubeChar)
    {
        foreach(var mapping in cubeColorsMap)
        {
            if(mapping.cubeCharacter == cubeChar)
            {
                return mapping;
            }
        }
        return null;
    }
    public void OnCubeDestroyed(CubeController cube)
    {
        int x = cube.gridX;
        int y = cube.gridY;
        cubeGrid[x, y] = null;
        DropCubes(x, y);
    }
    private void DropCubes(int col, int destroyedRow)
    {
        for (int row = destroyedRow - 1; row >= 0; row--)
        {
            CubeController cubeAbove = cubeGrid[col, row];
            if (cubeAbove != null)
            {
                // Move cube xuống 1 hàng
                cubeGrid[col, row + 1] = cubeAbove;
                cubeGrid[col, row] = null;

                cubeAbove.gridY = row + 1;

                Vector3 targetPos = new Vector3(
                    startPosition.x + (col * cubeSize),
                    0,
                    startPosition.y - ((row + 1) * cubeSize)
                );

                StartCoroutine(MoveCubeDown(cubeAbove, targetPos));
            }
        }
    }
    private IEnumerator MoveCubeDown(CubeController cube, Vector3 targetPos)
    {
        Vector3 startPos = cube.transform.position;
        float t = 0f;
        float duration = 0.2f;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            cube.transform.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        cube.transform.position = targetPos;
    }

}
