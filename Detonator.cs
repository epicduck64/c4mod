using MSCLoader;
using UnityEngine;

namespace C4Mod
{
    public class Detonator : MonoBehaviour
    {
        public bool detonate = false;
        private Camera camera;
        private GameObject raycasthit;
        private float hitdistance;
        private GameObject detonator;
        private AudioSource click;
        public bool debugging;



        // Use this for initialization
        void Start()
        {
            camera = GameObject.Find("PLAYER").transform.FindChild("Pivot/AnimPivot/Camera/FPSCamera/FPSCamera").GetComponent<Camera>();
            detonator = gameObject;
            click = detonator.transform.Find("Button").GetComponent<AudioSource>();
        }

        // Update is called once per frame
        void Update()
        {

            RaycastHit hit;

            Ray ray = camera.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
            if (Physics.Raycast(ray, out hit))
            {
                raycasthit = hit.collider.gameObject;
                hitdistance = hit.distance;
            }

            if (detonate)
            {
                detonate = false;
            }

            if (cInput.GetKeyDown("Use"))
            {
                if (hitdistance <= 1.2f)
                {
                     if (raycasthit.gameObject == detonator)
                     {
                        click.Play();
                        detonate = true;
                     }
                    
                }
            }
        }
    }
}