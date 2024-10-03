using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class SoundTrigger : MonoBehaviour
{
    public UnityEvent<Vector3> OnHeardSound;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<NoiseMaker>(out NoiseMaker noiseMaker))
        {
            noiseMaker.OnCreateNoise += ListenForNoise;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.TryGetComponent<NoiseMaker>(out NoiseMaker noiseMaker))
        {
            noiseMaker.OnCreateNoise -= ListenForNoise;
        }
    }

    private void ListenForNoise(Vector3 worldPositionOfNoise)
    {
        OnHeardSound.Invoke(worldPositionOfNoise);
    }

}
