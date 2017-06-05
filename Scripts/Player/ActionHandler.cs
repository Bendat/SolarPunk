using System;
using UnityEngine;

namespace Player
{
    public class ActionHandler : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            Shoot();
        }

        void Shoot()
        {
            var isFiring = Input.GetAxis("Fire1") > 0;
            PlayerStateManager.Instance.AnimSetBool("IsFiringBool", isFiring);
        }
    }
}
