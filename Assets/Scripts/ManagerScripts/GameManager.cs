using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameManager : Singleton_Mono_Method<GameManager>
{
    public LevelSO[] levels;
    public BoardManager boardManager;
    public TurtleShootingGridManager turtleShootingGridManager;
    public ShootingTurtleController shootingTurtleController;
    public Transform finishConfettiHolder;
    public GameObject finishConfetti;

    public bool IsGameOver => isGameOver;
    public int currentLevelIndex;
    private bool isLevelFinished = false;
    public bool isGameOver = false;
    private bool isVFXPlayed = false;

    [HideInInspector] public ShootingTurtle selectedTurtle;

    private void Start()
    {
        if (!PlayerPrefs.HasKey("FirstLaunch"))
        {
            PlayerPrefs.SetInt("FirstLaunch", 1);
            PlayerPrefs.SetInt("CurrentLevel", 0);
        }
        currentLevelIndex = PlayerPrefs.GetInt("CurrentLevel", 1);
        LoadLevel(currentLevelIndex);
    }
    private void Update()
    {
        if (!isLevelFinished && BoardManager.d_Instance.IsLevelFinished)
        {
            isLevelFinished = true;
            StartCoroutine(CheckFinishLevel());
        }
    }
    public void LoadLevel(int index)
    {
        LevelSO currentLevel = levels[index];
        AudioManager.d_Instance.ChangeBGMusic(1f, 1f);
        boardManager.MapGenerator(currentLevel.levelLayout);
        turtleShootingGridManager.LoadLevelFromLayout(currentLevel.shootingTurtleLayout);
        shootingTurtleController.InitializeLevel(currentLevel.availableShootingTowers);

        isLevelFinished = false;
        isVFXPlayed = false;
    }
    public void RestartLevel()
    {
        isGameOver = false;

        UIManager.d_Instance.ShowGameOverPanel(false);

        CleanUpLevel();
        
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }
    public void TryAgain()
    {
        StartCoroutine(TryAgainRoutine());
    }
    private IEnumerator TryAgainRoutine()
    {
        CleanUpLevel();

        LoadLevel(currentLevelIndex);

        isGameOver = false;

        yield return null;
        
        UIManager.d_Instance.ShowGameOverPanel(false);
    }
    public void LoadNextLevel()
    {
        currentLevelIndex++;
        if (currentLevelIndex > levels.Length)
        {
            currentLevelIndex = 0;
        }
        CleanUpLevel();
        StartCoroutine(LoadNextLevelDelay());
    }
    private IEnumerator LoadNextLevelDelay()
    {
        yield return null;
        LoadLevel(currentLevelIndex);
        UIManager.d_Instance.ShowFinishLevelPanel(false);
    }
    public void ChecKGameOver()
    {
        ShootingTurtleController.d_Instance.CheckLoseCondition();
    }
    public IEnumerator HandleGameOver()
    {
        Debug.Log("Game Over Triggered");
        AudioManager.d_Instance.ChangeBGMusic();
        yield return new WaitForSeconds(1f);
        UIManager.d_Instance.ShowGameOverPanel(true);
        isGameOver = true;
    }
    public IEnumerator CheckFinishLevel()
    {
        AudioManager.d_Instance.PlayApplauseSound(finishConfettiHolder.position);
        GameObject confettiVFX = Instantiate(finishConfetti, finishConfettiHolder);   
        confettiVFX.SetActive(true);
        yield return new WaitForSeconds(1f);
        UIManager.d_Instance.ShowFinishLevelPanel(true);
        confettiVFX.SetActive(false);
    }
    private void CleanUpLevel()
    {
        boardManager.ClearBoard();
        turtleShootingGridManager.ClearAllTurtles();
        shootingTurtleController.ClearAllTowers();
    }
}
