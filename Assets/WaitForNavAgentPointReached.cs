using UnityEngine;

public class WaitForNavAgentPointReached : CustomYieldInstruction
{

    //Ai
    private Transform ai;
    //Point
    //Transform point;
    private Vector3 worldPoint;

    private readonly float pointOffset;

    public override bool keepWaiting
    {
        get 
        {
            Vector3 magnitude = worldPoint - ai.position;

            if (magnitude.magnitude <= pointOffset) return false;
            else return true;
        }
    }

    public WaitForNavAgentPointReached(Transform aiTransform, Transform pointTranform, float pointReachedOffset = 0f) 
    {
        ai = aiTransform;
        worldPoint = pointTranform.position;
        pointOffset = pointReachedOffset;
    }

    public WaitForNavAgentPointReached(Transform aiTransform, Vector3 destination, float pointReachedOffset = 0f)
    {
        ai = aiTransform;
        worldPoint = destination;
        pointOffset = pointReachedOffset;
    }



}
