using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShootingTurtle : MonoBehaviour
{
    public ShootingTurtleController controller;
    public List<CubeController> targetList = new();
    public enum TurtleStateType { IDLE, MOVING, SHOOTING, END }

    [Header("Turtle Properties")]
    public TurtleStateType currentStateType;
    public TurtleData assignedData { get; private set; }
    public int ammoCount;
    [SerializeField] private TextMeshProUGUI ammoText;

    [Header("Coordinates")]
    public int gridX;
    public int gridY;

    [Header("Shooting Properties")]
    public Transform firePoint;

    private IShootingTurtleState currentState;
    private Renderer modelRender;
    private void Awake()
    {
        modelRender = GetComponentInChildren<Renderer>();
    }
    private void Update()
    {
        currentState?.Execute();
    }
    public void Initialize(ShootingTurtleController ctrl, TurtleData data, int turtleAmmo)
    {
        controller = ctrl;
        assignedData = data;
        ammoCount = turtleAmmo;
        ammoText.text = ammoCount.ToString();
        modelRender.material.color = data.turtleModelColor;

        ChangeState(TurtleStateType.IDLE);
    }
    public void ChangeState(TurtleStateType newStateType)
    {
        currentState?.Exit();
        currentStateType = newStateType;

        switch (newStateType)
        {
            case TurtleStateType.IDLE:
                currentState = new IdleState(this);
                break;
            case TurtleStateType.MOVING:
                currentState = new MovingState(this);
                break;
            case TurtleStateType.SHOOTING:
                currentState = new ShootingState(this);
                break;
            case TurtleStateType.END:
                currentState = new EndState(this);
                break;
        }

        currentState?.Enter();
    }
    private void OnMouseDown()
    {
        if (!ShootingTurtleController.d_Instance.HasAvailableSpot()) return;
        UIManager.d_Instance.ShowTutorialPanel(false);
        if (currentStateType == TurtleStateType.IDLE)
        {
            if (TurtleShootingGridManager.d_Instance.IsTopRowTurtle(this))
            {
                TurtleShootingGridManager.d_Instance.OnTurtleSelected(this);
                AudioManager.d_Instance.PlayPickTurtleSound(this.transform.position);
                ChangeState(TurtleStateType.MOVING);
            }
        }
    }
    public bool HasValidTarget()
    {
        foreach (var cube in BoardManager.d_Instance.topRowCubes)
        {
            if (cube != null && cube.gameObject.activeInHierarchy && cube.currentType == this.assignedData.turtleShootingType)
            {
                return true;
            }
        }
        return false;
    }
    public void ConsumeAmmo()
    {
        ammoCount--;
        ammoText.text = ammoCount.ToString();
    }
    public void SetGridCoordinates(int x, int y)
    {
        this.gridX = x;
        this.gridY = y;
    }
}
