using MSCLoader;
using HutongGames.PlayMaker;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace C4Mod
{
    public class BombWaitBehaviour : MonoBehaviour
    {
        private GameObject bomb;
        private Camera camera;
        private GameObject raycasthit;
        private float hitdistance;
        private TextMesh TextDisplay;
        public bool armed = false;
        private GameObject explosion;
        private Detonator script;
        public GameObject detonator;


        // Use this for initialization
        void Start()
        {
            bomb = gameObject;
            TextDisplay = bomb.transform.FindChild("Display").transform.FindChild("Text").GetComponent<TextMesh>();
            camera = GameObject.Find("PLAYER").transform.FindChild("Pivot/AnimPivot/Camera/FPSCamera/FPSCamera").GetComponent<Camera>();
            explosion = bomb.transform.FindChild("Explosion").gameObject;
            explosion.AddComponent<ExplosiveBehaviour>();
            script = detonator.GetComponent<Detonator>();
        }
        private void DetonationListener()
        {
            // Should have just done armed && script.detonate
            if (armed)
            {
               if (script.detonate)
                {
                    explosion.SetActive(true);
                    explosion.gameObject.transform.SetParent(null);
                    Destroy(bomb);
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            DetonationListener();
            explosion.GetComponent<ExplosiveBehaviour>().debugging = detonator.GetComponent<Detonator>().debugging;


            RaycastHit hit;

            Ray ray = camera.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
            if (Physics.Raycast(ray, out hit))
            {
                raycasthit = hit.collider.gameObject;
                hitdistance = hit.distance;
            }

            if (cInput.GetKeyDown("Use"))
            {
                if (hitdistance <= 0.5f)
                {
                    if (raycasthit.gameObject == bomb)
                    {
                        if (armed)
                        {
                            TextDisplay.text = "UNARMED";
                            armed = false;
                            bomb.name = "C4 EXPLOSIVE - UNARMED (Clone)";
                        }
                        else
                        {
                            TextDisplay.text = "ARMED";
                            armed = true;
                            bomb.name = "C4 EXPLOSIVE - ARMED (Clone)";
                        }
                    }
                }
            }
        }
    }
}