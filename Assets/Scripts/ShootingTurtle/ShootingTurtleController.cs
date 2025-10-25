using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Printing;
using UnityEngine;

public class ShootingTurtleController : Singleton_Mono_Method<ShootingTurtleController>
{
    [SerializeField] const string FIRING_SPOT_NAME = "Spot";
    public List<Transform> shootingSpots = new List<Transform>();

    [Header("Configuration")]
    [SerializeField] private float towerSpacing = 4.5f;
    [SerializeField] private Transform shootingTowerHolder;
    [SerializeField] private GameObject shootingTowerPrefab;
    [SerializeField] private float marginTowers = 1f;

    [Header("Runtime Data")]
    private Dictionary<Transform, ShootingTurtle> occupiedSpots = new Dictionary<Transform, ShootingTurtle>();

    [Header("Turtle Exit Spots")]
    public Transform ranOffSpotLeft;
    public Transform ranOffSpotRight;
    public Transform shootingPlacesCenter;

    public void InitializeLevel(int numberOfTowers)
    {
        foreach (Transform child in shootingTowerHolder)
        {
            Destroy(child.gameObject);
        }
        shootingSpots.Clear();
        occupiedSpots.Clear();

        float halfScreenWidth = Camera.main.orthographicSize * Camera.main.aspect;
        float maxDisplayWidth = halfScreenWidth * 2f;
        float usableWidth = maxDisplayWidth - marginTowers * 2f;

        float towerWidth = 1f;
        float autoSpacing = Mathf.Min(towerSpacing,
            (usableWidth - towerWidth) / (numberOfTowers - 1));

        float totalWidth = (numberOfTowers - 1) * autoSpacing;
        float startX = -totalWidth * 0.5f;

        for (int i = 0; i < numberOfTowers; i++)
        {
            float xPos = startX + i * autoSpacing;
            Vector3 towerPosition = new Vector3(xPos, 0, 0);

            GameObject towerObj = Instantiate(shootingTowerPrefab, shootingTowerHolder);
            towerObj.transform.localPosition = towerPosition;

            Transform spot = towerObj.transform.Find(FIRING_SPOT_NAME);

            if (spot != null)
            {
                shootingSpots.Add(spot);
                occupiedSpots[spot] = null;
            }
        }
        //DistributeTargets();
    }
    public void DistributeTargets()
    {
        List<CubeController> topRow = BoardManager.d_Instance.topRowCubes;
        if (topRow == null || topRow.Count == 0)
            return;

        foreach (var spot in occupiedSpots)
        {
            ShootingTurtle turtle = spot.Value;
            if (turtle == null || !turtle.gameObject.activeInHierarchy)
                continue;

            turtle.targetList.RemoveAll(t => t == null || !t.gameObject.activeInHierarchy);

            if (turtle.targetList.Count == 0)
            {
                AssignColorTargets(turtle, topRow);
            }
        }
    }
    public void AssignColorTargets(ShootingTurtle turtle, List<CubeController> topRow)
    {
        if (turtle == null || topRow == null || topRow.Count == 0)
            return;

        int targetCount = Mathf.CeilToInt(turtle.ammoCount * UnityEngine.Random.Range(0.5f, 0.6f));
        int addedCubes = 0;

        turtle.targetList.Clear();

        foreach (var cube in topRow)
        {
            if (addedCubes >= targetCount)
                break;

            if (cube == null || !cube.gameObject.activeInHierarchy)
                continue;

            if (cube.isTargeted)
                continue;

            if (cube.currentType != turtle.assignedData.turtleShootingType)
                continue;

            cube.isTargeted = true;
            turtle.targetList.Add(cube);
            addedCubes++;
        }
    }
    public void ClearAllTowers()
    {
        foreach (Transform child in shootingTowerHolder)
        {
            Destroy(child.gameObject);
        }
        shootingSpots.Clear();
        occupiedSpots.Clear();
    }
    public Transform GetAvailableSpot()
    {
        foreach (var spot in shootingSpots)
        {
            if (occupiedSpots[spot] == null)
            {
                return spot;
            }
        }
        return null;
    }
    public void CheckLoseCondition()
    {
        if (GameManager.d_Instance.IsGameOver)
            return;
        if (BoardManager.d_Instance == null || BoardManager.d_Instance.topRowCubes == null)
            return;
        
        bool allSpotsFull = true;
        foreach (var spot in occupiedSpots)
        {
            if (spot.Value == null)
            {
                allSpotsFull = false;
                break;
            }
        }

        if (!allSpotsFull)
            return;

        bool validTarget = false;
        foreach (var spot in occupiedSpots)
        {
            var turtle = spot.Value;
            if (turtle == null)
                continue;

            if (turtle.HasValidTarget())
            {
                validTarget = true;
                break;
            }
        }

        if (!validTarget)
        {
            StartCoroutine(GameManager.d_Instance.HandleGameOver());
        }
    }
    public void OccupySpot(Transform spot, ShootingTurtle turtle)
    {
        if (occupiedSpots.ContainsKey(spot))
            occupiedSpots[spot] = turtle;
    }

    public void ReleaseSpot(Transform spot)
    {
        if (occupiedSpots.ContainsKey(spot))
            occupiedSpots[spot] = null;
    }

    public bool HasAvailableSpot()
    {
        return GetAvailableSpot() != null;
    }
}