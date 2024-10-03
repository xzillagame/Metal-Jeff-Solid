using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrolState : EnemyState
{
    [SerializeField] protected float moveSpeed = 5f;
    [SerializeField] protected float waitTimeBetweenPoints = 1f;

    [SerializeField] protected List<Transform> patrolGameObjectLocations = new List<Transform>();
    protected Queue<Vector3> patrolCornerLocation = new Queue<Vector3>();
    private List<Vector3> cornerDebugList = new List<Vector3>();

    protected Vector3 currentTargetPosition = Vector3.zero;

    protected int currentPatrolPointIteration = 0;
    protected float distnaceOffsetFromPoint = 0;

    public override void EnterState(EnemyController enemy)
    {
        this.enemy = enemy;
        distnaceOffsetFromPoint = enemy.EnemyNavAgent.stoppingDistance;

        if (currentTargetPosition == Vector3.zero)
        {
            CalculatePathToNextPatrolPoint();
            currentTargetPosition = patrolCornerLocation.Dequeue();
        }
        else
        {
            CalculatePathToSelectedPoint(currentTargetPosition);
        }


        StartCoroutine(MoveToLocationRoutine());

    }

    public override void ExitState(EnemyController enemy)
    {
        StopCoroutine(MoveToLocationRoutine());
        patrolCornerLocation.Clear();
        cornerDebugList.Clear();
    }

    private IEnumerator MoveToLocationRoutine()
    {
        while (true)
        {
            Vector3 newForward = (currentTargetPosition - transform.position);
            newForward.y = 0f;
            newForward.Normalize();

            if (newForward != Vector3.zero) transform.forward = newForward;

            Vector3 directionTo = (currentTargetPosition - enemy.EnemyRigidbody.position).normalized;
            enemy.EnemyRigidbody.velocity = directionTo * moveSpeed;

            yield return new WaitForNavAgentPointReached(enemy.EnemyRigidbody.transform, currentTargetPosition, distnaceOffsetFromPoint);

            enemy.EnemyRigidbody.velocity = Vector3.zero;

            if (patrolCornerLocation.Count == 0)
            {
                CalculatePathToNextPatrolPoint();
                yield return new WaitForSeconds(waitTimeBetweenPoints);
            }

            currentTargetPosition = patrolCornerLocation.Dequeue();
        }
    }

    private void CalculatePathToNextPatrolPoint()
    {
        enemy.EnemyNavAgent.CalculatePath(patrolGameObjectLocations[currentPatrolPointIteration].position, enemy.EnemyNavPath);

        cornerDebugList.Clear();

        foreach (Vector3 corner in enemy.EnemyNavPath.corners)
        {
            patrolCornerLocation.Enqueue(corner);
            cornerDebugList.Add(corner);
        }

        currentPatrolPointIteration = (currentPatrolPointIteration + 1) % patrolGameObjectLocations.Count;
    }

    private void CalculatePathToSelectedPoint(Vector3 point)
    {
        enemy.EnemyNavAgent.CalculatePath(currentTargetPosition, enemy.EnemyNavPath);

        cornerDebugList.Clear();

        foreach(Vector3 corner in enemy.EnemyNavPath.corners)
        {
            patrolCornerLocation.Enqueue(corner);
            cornerDebugList.Add(corner);
        }

    }


    private void OnDrawGizmos()
    {
        if (patrolCornerLocation == null) return;

        Gizmos.color = Color.red;

        foreach(Vector3 corner in cornerDebugList)
        {
            Gizmos.DrawWireSphere(corner, 0.5f);
        }
    }



}
