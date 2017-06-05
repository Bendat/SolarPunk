using System;
using Items.Interactable;
using UnityEngine;
using Utils;

namespace Player
{
    public class DetectInteractable : MonoBehaviour
    {
        public Interactable ClosestInteractable;
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            RaycastHit hit;
            Physics.Raycast(PlayerStateManager.Instance.MainCam.transform.position,
                PlayerStateManager.Instance.MainCam.transform.forward, out hit, 3);
            if (hit.collider != null)
            {
                if (hit.collider.gameObject != ClosestInteractable)
                {
                    var interactable = hit.collider.gameObject.GetComponent<Interactable>();
                    if (interactable)
                    {
                        PlayerStateManager.Instance.UIManager.InteractText.gameObject.SetActive(true);
                        ClosestInteractable = interactable;
                    }

                }
            }
            else
            {
                PlayerStateManager.Instance.UIManager.InteractText.gameObject.SetActive(false);
                ClosestInteractable = null;
            }

            if (ClosestInteractable != null && Input.GetButtonDown("Interact"))
            {
                ClosestInteractable.Interact(gameObject);
            }
        }

        

    }
}
