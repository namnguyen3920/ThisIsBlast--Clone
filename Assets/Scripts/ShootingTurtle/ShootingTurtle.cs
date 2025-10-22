using TMPro;
using UnityEngine;

public class ShootingTurtle : MonoBehaviour
{
    public enum TurtleStateType { IDLE, MOVING, SHOOTING, END }

    public TurtleStateType currentStateType;
    public TurtleData assignedData { get; private set; }
    public int ammoCount;
    [SerializeField] private TextMeshProUGUI ammoText;
    public GameObject projectilePrefab;
    public Transform firePoint;

    private IShootingTurtleState currentState;
    private Renderer modelRender;
    public int gridX;
    public int gridY;
    private void Awake()
    {
        modelRender = GetComponentInChildren<Renderer>();
    }
    private void Update()
    {
        currentState?.Execute();
    }
    public void Initialize(TurtleData data, int turtleAmmo)
    {
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
        Debug.Log($"You clicked on {gameObject.name}");
        if (!ShootingTurtleController.d_Instance.HasAvailableSpot()) return;
        if (currentStateType == TurtleStateType.IDLE)
        {
            if (TurtleShootingGridManager.d_Instance.IsTopRowTurtle(this))
            {
                TurtleShootingGridManager.d_Instance.OnTurtleSelected(this);
                ChangeState(TurtleStateType.MOVING);
            }
            
            
        }
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
