using UnityEngine;
using System.Collections;

namespace Proj2
{
    public class Dim : MonoBehaviour
    {
        public float timer = 0.2f;
        public bool canKill = true;

        private Light mLight;
        private float inten;


        void Start()
        {
            if (gameObject.GetComponent<Light>())
            {
                mLight = gameObject.GetComponent<Light>();
                inten = mLight.intensity;
            }
        }


        void Update()
        {
            if (gameObject.GetComponent<Light>())
            {
                mLight.intensity -= inten * (Time.deltaTime / timer);
                if (canKill && mLight.intensity <= 0)
					Destroy(gameObject.GetComponent<Light>());
            }
        }
    }
}