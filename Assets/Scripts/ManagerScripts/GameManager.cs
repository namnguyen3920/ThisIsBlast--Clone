using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class GameManager : MonoBehaviour
{
    public LevelSO currentLevel;
    public BoardManager boardManager;
    public TurtleShootingGridManager turtleShootingGridManager;
    public ShootingTurtleController shootingTurtleController;

    [HideInInspector] public ShootingTurtle selectedTurtle;
    private void Start()
    {
        LoadLevel();
    }
    public void LoadLevel()
    {
        boardManager.MapGenerator(currentLevel.levelLayout);        
        turtleShootingGridManager.LoadLevelFromLayout(currentLevel.shootingTurtleLayout);
        shootingTurtleController.InitializeLevel(currentLevel.availableShootingTowers);
    }
}
