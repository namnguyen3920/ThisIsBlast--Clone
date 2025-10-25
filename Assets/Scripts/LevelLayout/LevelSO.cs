using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct InitTurtlePlacement
{
    public Vector2Int coords;
    public TurtleData data;
}

[CreateAssetMenu(fileName = "Level 01", menuName = "Level Data")]
public class LevelSO : ScriptableObject
{
    public TextAsset levelLayout;

    public TextAsset shootingTurtleLayout;
    [Header("Shooting Tower Settings")]
    public int availableShootingTowers = 3;

}
