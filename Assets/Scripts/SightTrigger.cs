using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class SightTrigger : MonoBehaviour
{

    public UnityEvent OnEntitySpotted;

    [SerializeField] private float sightlineThreshold = 0.75f;

    [SerializeField] private float sighChecktInterval = 0.25f;
    private PlayerMovement player;

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<PlayerMovement>(out player))
        {
            StopCoroutine(CalculateDotProductForSight());
            StartCoroutine(CalculateDotProductForSight());
        }

    }


    private void OnTriggerExit(Collider other)
    {
        if(other.TryGetComponent<PlayerMovement>(out player) == player) player = null;
    }



    private IEnumerator CalculateDotProductForSight()
    {
        WaitForSeconds sightInterval = new WaitForSeconds(sighChecktInterval);


        while (player != null)
        {

            Vector3 directionToTarget = (player.transform.position - transform.position).normalized;

            float dotValue = Vector3.Dot(transform.forward, directionToTarget);

            if (dotValue >= sightlineThreshold)
            {
                Debug.Log("Player spotted");
            }
            else
            {
                Debug.Log("Player in sight area, but not seen");
            }

            yield return sightInterval;



        }



    }








}
