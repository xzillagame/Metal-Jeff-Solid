using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{

    public bool IsSneaking { get; private set; } = false;

    [SerializeField] private float collisonOffset = 0.75f;
    [SerializeField] private NoiseMaker playerNoiseMaker;
    

    [SerializeField] private NavMeshAgent playerAgent;
    [SerializeField] private float normalMaxSpeed = 10.5f;
    [SerializeField] private float sneakingMaxSpeed = 5.5f;

    [SerializeField] private float NoiseBroadcastInterval = 0.25f;

    [SerializeField] LayerMask layerCollisions;
    [SerializeField] LayerMask clickableArea;

    Camera mainCamera;
    SneakingPlayerInput playerInput;

    private void Start()
    {
        mainCamera = Camera.main;

        playerAgent.speed = normalMaxSpeed;

        playerInput = new SneakingPlayerInput();

        playerInput.Enable();
        playerInput.SneakInput.PointInput.performed += PerformMovePlayer;
        playerInput.SneakInput.Sneak.performed += PerformSneak;

    }

    private void PerformSneak(InputAction.CallbackContext context)
    {
        if(context.action.WasPressedThisFrame())
        {
            playerAgent.speed = sneakingMaxSpeed;
            IsSneaking = true;
        }

        else if (context.action.WasReleasedThisFrame())
        {
            playerAgent.speed = normalMaxSpeed;
            IsSneaking = false;
        }
    }

    private void PerformMovePlayer(InputAction.CallbackContext context)
    {
        Ray mouseInputHit = mainCamera.ScreenPointToRay( Mouse.current.position.ReadValue() );

        if(Physics.Raycast(mouseInputHit,out RaycastHit rayHit, Mathf.Infinity, clickableArea))
        {
            playerAgent.SetDestination(rayHit.point);

            StopCoroutine(BroadcastNoiseInInterval());
            StartCoroutine(BroadcastNoiseInInterval());
        }
    }



    private IEnumerator BroadcastNoiseInInterval()
    {

        WaitForSeconds timedInterval = new WaitForSeconds(NoiseBroadcastInterval);

        //Wait a frame for the set destiation function to calculate the remaining distance
        yield return null;


        while(playerAgent.remainingDistance >= 0.001f )
        {
            if (!IsSneaking)
            {
                Debug.Log("Creating noise");
                playerNoiseMaker.CreateNoise();
            }
            yield return timedInterval;
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        int collidedLayer = 1 << collision.gameObject.layer;

        if ((collidedLayer & layerCollisions) != 0)
        {
            Vector3 pointOfCollison = collision.GetContact(0).point;
            Vector3 collisionNormal = collision.GetContact(0).normal;

            Vector3 offsetFromCollision = collisionNormal * collisonOffset;

            transform.position = pointOfCollison + offsetFromCollision;

            playerAgent.SetDestination(transform.position);

            StopCoroutine(BroadcastNoiseInInterval());
        }
    }

}


