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
        string[] rows = levelLayout.text
        .Replace("\r", "")
        .Trim()
        .Split('\n');
        int rowCount = rows.Length;
        int colCount = rows[0].Split('-').Length;
        Debug.Log($"Row count: {rowCount}, ColCount: {colCount}");
        cubeGrid = new CubeController[colCount, rowCount];
        for (int y = 0; y < rows.Length; y++)
        {
            string[] columns = rows[rows.Length-1-y].Split('-');
            for(int x = 0; x < columns.Length; x++)
            {
                Vector3 spawnPos = new Vector3(
                    startPosition.x + (x * cubeSize), 0,
                    startPosition.y + (y * cubeSize)
                );

                char cubeTypeChar = columns[x][0];
                if (cubeTypeChar == '_')
                {
                    cubeGrid[x, y] = null;
                    continue;
                }
                var cubeToSpawn = GetCubeMapChar(cubeTypeChar);
                //GameObject newCube = ObjectPools.d_Instance.SpawnFromPool("Cube", spawnPos, Quaternion.identity);
                GameObject newCube = Instantiate(baseCubePrefab, spawnPos, Quaternion.identity, boardHolder);
                CubeController cubeCtrl = newCube.GetComponent<CubeController>();
                cubeCtrl.GetComponent<CubeController>().Init(cubeToSpawn.Value.cubeColorType, cubeToSpawn.Value.cubeColorSprite);

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
        float lowestZ = float.MaxValue;
        foreach (var cubeObj in cubeList)
        {
            
            if (cubeObj.transform.position.z < lowestZ)
            {
                lowestZ = cubeObj.transform.position.z;
            }
        }
        foreach (var cubeObj in cubeList)
        {
            if (Mathf.Approximately(cubeObj.transform.position.z, lowestZ))
            {
                CubeController cubeCtrl = cubeObj.GetComponent<CubeController>();
                if (cubeCtrl != null && cubeObj.gameObject.activeInHierarchy)
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
    public void HandleCubeHit(CubeController cube)
    {
        if (cube.isBeingDestroyed) return;
        cube.isBeingDestroyed = true;
        StartCoroutine(HandleDestroyCube(cube));
    }
    private IEnumerator HandleDestroyCube(CubeController cube)
    {
        Vector3 originalScale = cube.transform.localScale;
        Vector3 expandedScale = originalScale * 1.2f;
        float scaleDuration = 0.1f;
        float elapsed = 0f;

        while (elapsed < scaleDuration)
        {
            elapsed += Time.deltaTime;
            cube.transform.localScale = Vector3.Lerp(originalScale, expandedScale, elapsed / scaleDuration);
            yield return null;
        }

        elapsed = 0f;
        while (elapsed < scaleDuration)
        {
            elapsed += Time.deltaTime;
            cube.transform.localScale = Vector3.Lerp(expandedScale, Vector3.zero, elapsed / scaleDuration);
            yield return null;
        }

        GameObject hitVFX = ObjectPools.d_Instance.SpawnFromPool("ShootVFX", cube.transform.position, Quaternion.identity);
        ObjectPools.d_Instance.ReturnToPool("ShootVFX", hitVFX, 0.5f);

        BoardManager.d_Instance.OnCubeDestroyed(cube);
        cube.transform.localScale = originalScale;
        cube.isBeingDestroyed = false;
        cube.isTargeted = false;
        ObjectPools.d_Instance.ReturnToPool("Cube", cube.gameObject);
    }
    private void OnCubeDestroyed(CubeController cube)
    {
        int x = cube.gridX;
        int y = cube.gridY;
        cubeGrid[x, y] = null;
        cubeList.Remove(cube.gameObject);
        topRowCubes.Remove(cube);
        topRowCubes = GetTopRowCubes();
        //ShootingTurtleController.d_Instance.DistributeTargets();
        DropCubes(x, y);
    }
    private void DropCubes(int destroyedCol, int destroyedRow)
    {
        for (int row = destroyedRow + 1; row < cubeGrid.GetLength(1); row++)
        {
            CubeController cubeAbove = cubeGrid[destroyedCol, row];
            if (cubeAbove != null)
            {
                int targetRow = row - 1;

                cubeGrid[destroyedCol, targetRow] = cubeAbove;
                cubeGrid[destroyedCol, row] = null;

                cubeAbove.gridY = targetRow;

                Vector3 targetPos = new Vector3(
                    startPosition.x + (destroyedCol * cubeSize),
                    0,
                    startPosition.y + (targetRow * cubeSize)
                );
                StartCoroutine(MoveCubeDown(cubeAbove, targetPos));
            }
        }
    }
    public bool IsLevelFinished
    {
        get
        {
            return cubeList.Count == 0;
        }
    }
    public void ClearBoard()
    {
        for (int i = boardHolder.childCount - 1; i >= 0; i--)
        {
            Transform child = boardHolder.GetChild(i);

            if (child.name == "Background")
                continue;

            ObjectPools.d_Instance.ReturnToPool("Cube", child.gameObject);
        }

        cubeList.Clear();
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
    private void OnEnable()
    {
        CubeController.OnCubeHit += HandleCubeHit;
    }
    private void OnDisable()
    {
        CubeController.OnCubeHit -= HandleCubeHit;
    }
}
