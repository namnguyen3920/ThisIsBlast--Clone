using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct InitTurtlePlacement
{
    public Vector2Int coords;
    public TurtleData data;
    //public int ammo;
}

[CreateAssetMenu(fileName = "Level 01", menuName = "Level Data")]
public class LevelSO : ScriptableObject
{
    public TextAsset levelLayout;

    public TextAsset shootingTurtleLayout;
    [Header("Initial Turtle Placements")]
    public int rows = 2;
    public int columns = 2;
    [Header("Shooting Tower Settings")]
    public int availableShootingTowers = 3;

}
