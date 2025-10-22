using UnityEngine;

public class IdleState : ShootingTurtleBaseState
{
    public IdleState(ShootingTurtle turtle) : base(turtle) { }
    public override void Enter()
    {
        Debug.Log("Entering Idle State");
    }

    public override void Execute()
    {
        Debug.Log("Executing Idle State");
    }

    public override void Exit()
    {
        Debug.Log("Exiting Idle State");
    }
}
