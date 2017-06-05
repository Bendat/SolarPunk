using Boo.Lang.Environments;
using Player;
using UnityEngine;
using Utils;

namespace Items.Interactable
{
    public class Chest : Interactable
    {
        public Transform Lid;

        public float OpenTime;

        [SerializeField] public HingeRotations Positions;


        [SerializeField] private HingeStatus _hingeStatus = HingeStatus.Closed;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
           
        }

        public override void Interact(GameObject player)
        {
            switch (_hingeStatus)
            {
                case HingeStatus.Open:
                    Close();
                    break;
                case HingeStatus.Closed:
                    Open();
                    break;
            }
        }

        protected virtual void Open()
        {
            if (_hingeStatus != HingeStatus.Opening)
            {
                _hingeStatus = HingeStatus.Opening;
                StartCoroutine(Lid.LerpToAction(Positions.ClosedOrientation, Positions.OpenOrientation, OpenTime,
                    () => { Debug.Log("opened"); _hingeStatus = HingeStatus.Open; }));
            }
        }

        protected virtual void Close()
        {
            if (_hingeStatus != HingeStatus.Closing)
            {
                _hingeStatus = HingeStatus.Closing;
                StartCoroutine(Lid.LerpToAction(Positions.OpenOrientation, Positions.ClosedOrientation, OpenTime,
                    () => { Debug.Log("closed"); _hingeStatus = HingeStatus.Closed; }));
            }
        }

    }
}
