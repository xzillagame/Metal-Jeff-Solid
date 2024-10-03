using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyState : MonoBehaviour
{
    protected EnemyController enemy;

    public abstract void EnterState(EnemyController enemy);
    public abstract void ExitState(EnemyController enemy);

}
