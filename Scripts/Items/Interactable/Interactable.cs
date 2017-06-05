using UnityEngine;

namespace Items.Interactable
{
    public abstract class Interactable : MonoBehaviour
    {
        public abstract void Interact(GameObject player);
    }

    [System.Serializable]
    public struct HingeRotations
    {
        public Vector3 ClosedOrientation;
        public Vector3 OpenOrientation;

    }

    [System.Serializable]
    public enum HingeStatus
    {
        Open,
        Opening,
        Closing,
        Closed
    }

}
