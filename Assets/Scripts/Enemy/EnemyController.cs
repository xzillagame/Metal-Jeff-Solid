
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{

    [SerializeField] private SoundTrigger enemySoundTrigger;


    [field:SerializeField] public Rigidbody EnemyRigidbody { get; private set; }
    [field: SerializeField] public NavMeshAgent EnemyNavAgent { get; private set; }

    public NavMeshPath EnemyNavPath { get; private set; }

    [field: SerializeField] public EnemyPatrolState PatrolState { get; private set; }
    [field: SerializeField] public EnemyInvistigateState InvistigateState { get; private set; }

    private EnemyState currentState;

    [SerializeField] private LayerMask playerCollisonLayer;


    public Vector3 LastPositionOfSoundHeard { get; private set; } = Vector3.zero;

    public void ChangeState(EnemyState state)
    {
        switch (state)
        {
            case EnemyPatrolState:
                TransitionState(PatrolState); break;
            case EnemyInvistigateState:
                TransitionState(InvistigateState); break;
            default:
                break;
        }

    }

    private void TransitionState(EnemyState state)
    {
        currentState.ExitState(this);
        currentState = state;
        currentState.EnterState(this);
    }


    private void OnEnable()
    {
        enemySoundTrigger.OnHeardSound.AddListener(EnemyReactionToSound);
    }

    private void Start()
    {
        EnemyNavPath = new NavMeshPath();

        currentState = PatrolState;

        currentState.EnterState(this);

    }

    private void EnemyReactionToSound(Vector3 soundPosition)
    {
        LastPositionOfSoundHeard = soundPosition;
        currentState.ExitState(this);

        EnemyRigidbody.velocity = Vector3.zero;

        currentState = InvistigateState;
        currentState.EnterState(this);

    }

    private void OnCollisionEnter(Collision collision)
    {
        int collidingLayer = 1 << collision.gameObject.layer;


        if( (collidingLayer & playerCollisonLayer) != 0 )
        {
            PatrolState.ExitState(this);
            EnemyRigidbody.velocity = Vector3.zero;
        }


    }





}

