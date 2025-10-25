
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;

[System.Serializable]
public struct ShootingTurtleMapping
{
    public char turtleCharacter;
    public TurtleData turtleData;
}

public class TurtleShootingGridManager : Singleton_Mono_Method<TurtleShootingGridManager>
{
    [Header("Grid Settings")]
    [SerializeField] private float turtleSpacing = 2.5f;

    [Header("Prefabs & Holders")]
    [SerializeField] private GameObject turtleShootingPrefab;
    [SerializeField] private Transform turtleHolder;

    [Header("Mappings")]
    public List<ShootingTurtleMapping> turtleMappings;

    public ShootingTurtleController controller;
    private class GridSlot
    {
        public Vector3 worldPosition;
        public ShootingTurtle currentTurtle = null;
        public bool IsOccupied => currentTurtle != null;
    }
    private GridSlot[,] grid;
    private int gridRows;
    private int gridColumns;
    public List<ShootingTurtle> selectableTurtles { get; private set; } = new List<ShootingTurtle>();
    public void LoadLevelFromLayout(TextAsset layoutFile)
    {
        foreach (Transform child in turtleHolder)
            Destroy(child.gameObject);

        string[] rows = layoutFile.text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        gridRows = rows.Length;
        gridColumns = rows[0].Split('-').Length;

        grid = new GridSlot[gridColumns, gridRows];
        float totalWidth = (gridColumns - 1) * turtleSpacing;
        float totalHeight = (gridRows - 1) * turtleSpacing;
        Vector3 offset = new Vector3(-totalWidth * 0.5f, 0, totalHeight * 0.5f);

        for (int y = 0; y < gridRows; y++)
        {
            string[] turtleCells = rows[y].Split('-');
            for (int x = 0; x < turtleCells.Length; x++)
            {
                if (string.IsNullOrWhiteSpace(turtleCells[x])) continue;

                string[] turtleParts = turtleCells[x].Trim().Split(',');
                if (turtleParts.Length != 2) continue;

                char turtleChar = turtleParts[0].Trim()[0];
                int turtleAmmo = int.Parse(turtleParts[1].Trim());
                float xPos = x * turtleSpacing;
                float zPos = -y * turtleSpacing;
                Vector3 spawnPos = turtleHolder.position + offset + new Vector3(xPos, 0, zPos);
                GameObject turtleObj = Instantiate(turtleShootingPrefab, spawnPos, Quaternion.identity, turtleHolder);
                ShootingTurtle turtle = turtleObj.GetComponent<ShootingTurtle>();
                
                turtle.Initialize(controller,GetTurtleDataFromChar(turtleChar), turtleAmmo);

                grid[x, y] = new GridSlot { worldPosition = spawnPos, currentTurtle = turtle };
                turtle.SetGridCoordinates(x, y);
            }
        }

    }
    public void OnTurtleSelected(ShootingTurtle turtle)
    {
        int col = turtle.gridX;
        int row = turtle.gridY;

        grid[col, row].currentTurtle = null;

        for (int y = row + 1; y < gridRows; y++)
        {
            var lowerSlot = grid[col, y];
            if (lowerSlot.IsOccupied)
            {
                ShootingTurtle t = lowerSlot.currentTurtle;
                grid[col, y - 1].currentTurtle = t;
                grid[col, y].currentTurtle = null;

                t.SetGridCoordinates(col, y - 1);
                StartCoroutine(MoveTurtleToSlot(t, grid[col, y - 1].worldPosition));
            }
        }
    }

    private IEnumerator MoveTurtleToSlot(ShootingTurtle turtle, Vector3 targetPos)
    {
        Vector3 startPos = turtle.transform.position;
        float elapsed = 0f, duration = 0.2f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            turtle.transform.position = Vector3.Lerp(startPos, targetPos, elapsed / duration);
            yield return null;
        }

        turtle.transform.position = targetPos;
    }
    public void ClearAllTurtles()
    {
        foreach (Transform child in turtleHolder)
        {
            Destroy(child.gameObject);
        }
        grid = null;
        gridRows = gridColumns = 0;
    }
    private TurtleData GetTurtleDataFromChar(char character)
    {
        foreach (var mapping in turtleMappings)
        {
            if (mapping.turtleCharacter == character)
            {
                return mapping.turtleData;
            }
        }
        return null;
    }
    public bool IsTopRowTurtle(ShootingTurtle turtle)
    {
        if (turtle.gridY == 0)
        {
            return true;
        }
        return false;
    }
}