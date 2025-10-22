using UnityEngine;
using System.Collections.Generic;
using System; // Cần cho Action

public class ShootingTurtleController : Singleton_Mono_Method<ShootingTurtleController>
{
    [SerializeField] const string FIRING_SPOT_NAME = "Spot";
    public List<Transform> shootingSpots = new List<Transform>();

    [Header("Configuration")]
    [SerializeField] private float towerSpacing = 4.5f;
    [SerializeField] private Transform shootingTowerHolder;
    [SerializeField] private GameObject shootingTowerPrefab;

    [Header("Runtime Data")]
    private Dictionary<Transform, ShootingTurtle> occupiedSpots = new Dictionary<Transform, ShootingTurtle>();

    public void InitializeLevel(int numberOfTowers)
    {
        foreach (Transform child in shootingTowerHolder)
        {
            Destroy(child.gameObject);
        }
        shootingSpots.Clear();
        occupiedSpots.Clear();

        float totalWidth = (numberOfTowers - 1) * towerSpacing;
        float startX = -totalWidth / 2f;

        for (int i = 0; i < numberOfTowers; i++)
        {
            float xPos = startX + i * towerSpacing;
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