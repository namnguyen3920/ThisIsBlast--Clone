using UnityEngine;

public class MovingState : ShootingTurtleBaseState
{
    private Vector3 targetPos;
    private Transform targetSpot;
    private float moveSpeed = 5f;
    public MovingState(ShootingTurtle turtle) : base(turtle) { }
    public override void Enter()
    {
        targetSpot = ShootingTurtleController.d_Instance.GetAvailableSpot();
        if (targetSpot == null)
        {
            turtle.ChangeState(ShootingTurtle.TurtleStateType.IDLE);
            return;
        }

        ShootingTurtleController.d_Instance.OccupySpot(targetSpot, turtle);
        targetPos = targetSpot.position;
    }

    public override void Execute()
    {
        turtle.transform.position = Vector3.Lerp(
           turtle.transform.position,
           targetPos,
           moveSpeed * Time.deltaTime);

        if (Vector3.Distance(turtle.transform.position, targetPos) < 0.05f)
        {
            turtle.ChangeState(ShootingTurtle.TurtleStateType.SHOOTING);
        }

    }

    public override void Exit()
    {
    }
}