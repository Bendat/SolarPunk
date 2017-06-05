using UnityEngine;
using UnityEngine.UI;

namespace Player
{
    public class UIManager : MonoBehaviour
    {
        public Text InteractText;
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void EnableInteractText()
        {
            InteractText.gameObject.SetActive(true);
        }

        public void DisableInteractText()
        {
            InteractText.gameObject.SetActive(false);
        }
    }
}
