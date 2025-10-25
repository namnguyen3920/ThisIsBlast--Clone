using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingState : ShootingTurtleBaseState
{
    public ShootingState(ShootingTurtle turtle) : base(turtle) { }
    
    private bool isShooting;
    public List<CubeController> targetCubes;
    private Coroutine shootingRoutine;
    public override void Enter()
    {
        isShooting = true;

        shootingRoutine = turtle.StartCoroutine(ShootingCoroutine());
    }

    public override void Execute()
    {
        GameManager.d_Instance.ChecKGameOver();
        if (turtle.ammoCount <= 0)
        {
            Debug.Log("Executing Shooting State");
            turtle.StopCoroutine(shootingRoutine);
            turtle.ChangeState(ShootingTurtle.TurtleStateType.END);
            return;
        }
    }

    public override void Exit()
    {
        Debug.Log("Exiting Shooting State");
        isShooting = false;
        turtle.StopCoroutine(shootingRoutine);
    }
    private IEnumerator ShootingCoroutine()
    {
        float fireDelay = 1f / turtle.assignedData.firingRate;

        while (isShooting && turtle.ammoCount > 0)
        {
            
            if (turtle.targetList.Count == 0)
            {
                turtle.controller.DistributeTargets();
                
                yield return new WaitForSeconds(0.1f);
                continue;
            }

            foreach (var targetCube in new List<CubeController>(turtle.targetList))
            {
                if (targetCube == null || !targetCube.gameObject.activeInHierarchy)
                {
                    turtle.targetList.Remove(targetCube);
                    continue;
                }

                Debug.DrawLine(turtle.firePoint.position, targetCube.transform.position, Color.green, 20f);
                turtle.StartCoroutine(ShootProjectile(targetCube));

                Vector3 direction = targetCube.transform.position - turtle.firePoint.position;
                Quaternion lookRotate = Quaternion.LookRotation(direction);
                turtle.transform.rotation = Quaternion.Lerp(turtle.transform.rotation, lookRotate, Time.deltaTime * 5f);

                targetCube.OnHitByTurtle();
                yield return new WaitForSeconds(fireDelay);
            }

            yield return null;
        }

        turtle.ChangeState(ShootingTurtle.TurtleStateType.END);

    }
    private IEnumerator ShootProjectile(CubeController target)
    {
        if (target == null || target.isBeingDestroyed)
        {
            yield break;
        }

        GameObject bullet = ObjectPools.d_Instance.SpawnFromPool("Projectile", turtle.firePoint.position, Quaternion.identity);
        AudioManager.d_Instance.PlayShootSound(turtle.firePoint.position);
        GameObject shootVFX = ObjectPools.d_Instance.SpawnFromPool("ShootVFX", turtle.firePoint.position, Quaternion.identity);
        ObjectPools.d_Instance.ReturnToPool("ShootVFX", shootVFX, 0.3f);

        float elapsed = 0f;
        float duration = 0.1f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;            
            bullet.transform.position = Vector3.Lerp(turtle.firePoint.position, target.transform.position, elapsed / duration);
            yield return null;
        }

        turtle.ConsumeAmmo();
        yield return new WaitForEndOfFrame();
        ObjectPools.d_Instance.ReturnToPool("Projectile", bullet);
    }
}
