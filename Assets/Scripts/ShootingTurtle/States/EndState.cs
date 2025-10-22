using UnityEngine;

public class EndState : ShootingTurtleBaseState
{
    public EndState(ShootingTurtle turtle) : base(turtle) { }
    public override void Enter()
    {
        Debug.Log("Entering End State");
    }

    public override void Execute()
    {
        Debug.Log("Executing End State");
    }

    public override void Exit()
    {
        Debug.Log("Exiting End State");
    }
}
