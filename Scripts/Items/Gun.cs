using System.Runtime.Remoting.Messaging;
using Player;
using UnityEngine;

namespace Items
{
    public class Gun : MonoBehaviour
    {
        public int MaxAmmo;
        
        public int CurrentAmmo;

        public int MaxClipSize; 
        public int CurrentClipSize;

//        public int 
        public bool CanFire {
            get { return CurrentClipSize > 0; }
        }

        public bool CanReload {
            get { return CurrentAmmo > CurrentClipSize; }
        }
        public int CurrentCapacity {
            get { return CurrentAmmo + CurrentClipSize; }
        }

        public int FreeSlots {
            get { return MaxAmmo - (CurrentAmmo + CurrentClipSize); }
        }

        public bool CanShoot {
            get { return CurrentClipSize > 0; }
        }
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Shoot()
        {
            var target = GetTarget();

        }

        private Vector3 GetTarget()
        {
            RaycastHit hit;
            Physics.Raycast(PlayerStateManager.Instance.MainCam.transform.position,
                PlayerStateManager.Instance.MainCam.transform.forward, out hit, 500);
            
            return hit.point;
            
        }
    }

    public enum ShootStatus
    {
        Empty,
        NeedsReload,
        CanFire
    }
}
