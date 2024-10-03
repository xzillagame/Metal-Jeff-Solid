using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyInvistigateState : EnemyState
{
    [SerializeField] protected float moveSpeed = 1.5f;
    [SerializeField] protected float waitTimeBetweenPoints = 1f;

    //[SerializeField] protected List<Transform> patrolGameObjectLocations = new List<Transform>();
    protected Queue<Vector3> patrolCornerLocation = new Queue<Vector3>();

    protected Vector3 currentTargetPosition = Vector3.zero;

    protected int currentPatrolPointIteration = 0;
    protected float distnaceOffsetFromPoint = 0.9f;


    public override void EnterState(EnemyController enemy)
    {
        this.enemy = enemy;

        patrolCornerLocation.Clear();

        CalculatePathToNextPatrolPoint();

        currentTargetPosition = patrolCornerLocation.Peek();

        StartCoroutine(GoToHearingPointLocation());
    }

    public override void ExitState(EnemyController enemy)
    {
        StopCoroutine(GoToHearingPointLocation());
    }


    private IEnumerator GoToHearingPointLocation()
    {
        while(true)
        {
            Vector3 newForward = (enemy.LastPositionOfSoundHeard - transform.position);
            newForward.y = 0f;
            newForward.Normalize();

            if (newForward != Vector3.zero) transform.forward = newForward;

            Vector3 directionTo = (currentTargetPosition - enemy.EnemyRigidbody.position).normalized;
            enemy.EnemyRigidbody.velocity = directionTo * moveSpeed;

            yield return new WaitForNavAgentPointReached(enemy.EnemyRigidbody.transform, currentTargetPosition, distnaceOffsetFromPoint);

            enemy.EnemyRigidbody.velocity = Vector3.zero;

            if (patrolCornerLocation.Count == 0)
            {
                yield return new WaitForSeconds(waitTimeBetweenPoints);
                enemy.ChangeState(enemy.PatrolState);
                yield break;
                
            }

            currentTargetPosition = patrolCornerLocation.Dequeue();
        }
    }

    private void CalculatePathToNextPatrolPoint()
    {
        enemy.EnemyNavAgent.CalculatePath(enemy.LastPositionOfSoundHeard, enemy.EnemyNavPath);

        foreach (Vector3 corner in enemy.EnemyNavPath.corners)
        {
            patrolCornerLocation.Enqueue(corner);
        }

    }




}
