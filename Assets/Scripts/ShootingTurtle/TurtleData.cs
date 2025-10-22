using UnityEngine;

[CreateAssetMenu(fileName = "New Turtle Data", menuName = "Turtle/Turtle Data")]
public class TurtleData : ScriptableObject
{
    public ColorType turtleShootingType;
    public float firingRate = 10f;
    public Color turtleModelColor;
}
