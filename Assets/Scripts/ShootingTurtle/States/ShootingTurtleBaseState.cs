using UnityEngine;

public abstract class ShootingTurtleBaseState : IShootingTurtleState
{
    protected ShootingTurtle turtle;

    public ShootingTurtleBaseState(ShootingTurtle turtle)
    {
        this.turtle = turtle;
    }

    public abstract void Enter();
    public abstract void Execute();
    public abstract void Exit();
}
