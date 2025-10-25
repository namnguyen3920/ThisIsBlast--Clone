using System.Collections;
using UnityEngine;
using static ShootingTurtle;

public class EndState : ShootingTurtleBaseState
{
    public EndState(ShootingTurtle turtle) : base(turtle) { }

    public Transform ranOffSpotLeft;
    public Transform ranOffSpotRight;
    private Transform shootingAreaCenter;

    public override void Enter()
    {
        ranOffSpotLeft = turtle.controller.ranOffSpotLeft;
        ranOffSpotRight = turtle.controller.ranOffSpotRight;
        shootingAreaCenter = turtle.controller.shootingPlacesCenter;
        turtle.StartCoroutine(MoveOffAndReturn());
    }
    public override void Execute()
    {
    }
    public override void Exit()
    {
        turtle.StopAllCoroutines();
        turtle.ChangeState(TurtleStateType.IDLE);
    }
    private IEnumerator MoveOffAndReturn()
    {
        bool isLeftSide = turtle.transform.position.x <= shootingAreaCenter.position.x;
        Transform targetSpot = isLeftSide ? ranOffSpotLeft : ranOffSpotRight;

        float moveDuration = 0.75f;
        float elapsed = 0f;
        Vector3 startPos = turtle.transform.position;
        Vector3 targetPos = targetSpot.position;

        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            turtle.transform.position = Vector3.Lerp(startPos, targetPos, elapsed / moveDuration);
            yield return null;
        }

        turtle.transform.position = targetPos;

        ObjectPools.d_Instance.ReturnToPool("ShootingTurtle", turtle.gameObject);

        yield return null;
    }
}
