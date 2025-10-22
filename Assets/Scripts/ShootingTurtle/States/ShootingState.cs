using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class ShootingState : ShootingTurtleBaseState
{
    public ShootingState(ShootingTurtle turtle) : base(turtle) { }
    
    private bool isShooting;
    public List<CubeController> targetCubes;
    private Coroutine shootingRoutine;
    public override void Enter()
    {
        Debug.Log("Entering Shooting State");
        isShooting = true;
        shootingRoutine = turtle.StartCoroutine(ShootingCoroutine());
    }

    public override void Execute()
    {
        Debug.Log("Executing Shooting State");
        
    }

    public override void Exit()
    {
        Debug.Log("Exiting Shooting State");
        
    }
    private IEnumerator ShootingCoroutine()
    {
        targetCubes = BoardManager.d_Instance.GetTopRowCubes();
        float fireDelay = 1f / turtle.assignedData.firingRate;
        while (isShooting && turtle.ammoCount > 0)
        {
            //int col = turtle.gridX;
            foreach (var targetCube in targetCubes)
            {
                if (targetCube != null && targetCube.currentType == turtle.assignedData.turtleShootingType)
                {
                    Debug.DrawLine(turtle.firePoint.position, targetCube.transform.position, Color.green, 20f);
                    turtle.StartCoroutine(ShootProjectile(targetCube.transform.position));
                    targetCube.OnHitByTurtle();
                }
                else
                {
                    yield return new WaitForSeconds(0.1f);
                    continue;
                }
                yield return new WaitForSeconds(fireDelay);
            }
        }
        turtle.ChangeState(ShootingTurtle.TurtleStateType.END);

    }
    private IEnumerator ShootProjectile(Vector3 targetPos)
    {
        GameObject bullet = ObjectPools.d_Instance.SpawnFromPool("Projectile", turtle.firePoint.position, Quaternion.identity);
        
        float elapsed = 0f;
        float duration = 0.1f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            bullet.transform.position = Vector3.Lerp(turtle.firePoint.position, targetPos, elapsed / duration);
            yield return null;
        }

        bullet.transform.position = targetPos;
        turtle.ConsumeAmmo();
        ObjectPools.d_Instance.ReturnToPool("Projectile", bullet);
    }
}
