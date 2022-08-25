using UnityEngine;
using UnityStandardAssets.ImageEffects;
using MSCLoader;
using HutongGames.PlayMaker;
using System;

namespace C4Mod
{
    public class ExplosiveBehaviour : MonoBehaviour
    {
        private GameObject explosion;
        private Light light;
        private GameObject player;
        private AudioSource sound;
        public bool debugging;
        public static GameObject death { get; internal set; }

        // Use this for initialization
        void Start()
        {
            death = GameObject.Find("Systems").transform.Find("Death").gameObject;
            explosion = gameObject;
            light = explosion.transform.Find("ExplosionLight").GetComponent<Light>();
            player = GameObject.Find("PLAYER");
            sound = gameObject.GetComponent<AudioSource>();

            float dist = Vector3.Distance(player.transform.position, transform.position);

            float radius = 60F;
            float power = 9999F;
            Vector3 explosionPos = transform.position;

            // For Player
            if (dist <= 19)
            {
                killCustom("Young male" + Environment.NewLine + "dies from" + Environment.NewLine + "explosion", "Nuorimies kuoli" + Environment.NewLine + "räjähdyksessä");
                GameObject.Find("PLAYER/Pivot/AnimPivot/Camera/FPSCamera/DeadBody").SetActive(true);
                Rigidbody[] rigbody = GameObject.Find("PLAYER/Pivot/AnimPivot/Camera/FPSCamera/DeadBody").GetComponentsInChildren<Rigidbody>();
                Destroy(GameObject.Find("PLAYER/Pivot/AnimPivot/Camera/FPSCamera/DeadBody").GetComponent<FixedJoint>());
                foreach (Rigidbody body in rigbody)
                {
                    body.AddExplosionForce(power / 2, explosionPos, radius, 0.1f);
                }
            }

            Collider[] colliders = Physics.OverlapSphere(explosionPos, radius);
            foreach (Collider hit in colliders)
            {
                if (debugging)
                {
                    ModConsole.Print(hit.name);
                }

                Rigidbody rb = hit.GetComponent<Rigidbody>();

                // For AICars
                if (hit.gameObject.name == "CarColliderAI")
                {
                    GameObject car = hit.transform.parent.gameObject;
                    // We found a car!
                    if (hit.transform.parent.Find("CrashEvent"))
                    {
                        // We found something to murder!
                        if (hit.transform.parent.Find("CrashEvent/DeathForce").GetComponent<FixedJoint>())
                        {
                            // Don't want errors now, do we
                            FixedJoint joint = hit.transform.parent.Find("CrashEvent/DeathForce").GetComponent<FixedJoint>();
                            Destroy(joint);
                            // Destroying the joint is probably easier. If the game notices the joint is at all missing, it kills the driver. (hopefully)
                        }
                    }
                    // Welp, even if we can't kill them, lets add a force to them.
                    Rigidbody rigidbody = car.GetComponent<Rigidbody>();
                    rigidbody.AddExplosionForce(power * 25, explosionPos, radius, 0.1f);
                }
                // For Humans
                if (hit.gameObject.name == "HumanTriggerCrime")
                {
                    float distance = Vector3.Distance(hit.transform.parent.position, transform.position);
                    if (distance <= 20)
                    {
                        Transform root = hit.transform.parent.parent;
                        PlayMakerFSM[] playmakers = root.GetComponents<PlayMakerFSM>();
                        foreach (PlayMakerFSM fsm in playmakers)
                        {
                            fsm.enabled = false;
                        }
                        hit.transform.parent.Find("Char").gameObject.SetActive(false);
                        hit.transform.parent.Find("RagDoll").gameObject.SetActive(true);
                        Rigidbody[] rigidbodies = hit.transform.parent.GetComponentsInChildren<Rigidbody>();
                        foreach (Rigidbody body in rigidbodies)
                        {
                            body.AddExplosionForce(power / 4, explosionPos, radius, 1);
                        }
                    }
                }
                // For ETC Cars
                if (hit.gameObject.name == "StagingWheel")
                {
                    Transform car = hit.transform.parent;
                    Rigidbody rigidbody = car.gameObject.GetComponent<Rigidbody>();
                    rigidbody.AddExplosionForce(power * 15, explosionPos, radius, 0.5f);
                    // Lets see if there is a windshield on this car.
                    if (car.Find("LOD").Find("Windshield").Find("windshield"))
                    {
                        // Lets break it.
                        if (car.Find("LOD").Find("Windshield").Find("windshield").GetComponent<FixedJoint>())
                        {
                            FixedJoint joint = car.Find("LOD").Find("Windshield").Find("windshield").GetComponent<FixedJoint>();
                            Destroy(joint);
                            // Same thing for killing the drivers, I guess.
                        }
                    }

                }


                if (rb != null)
                {
                    rb.AddExplosionForce(power, explosionPos, radius, 0.1f);
                }

            }
        }

        public static void killCustom(string en, string fi)
        {
            death.SetActive(true);
            Transform array = GameObject.Find("MasterAudio/Death").GetComponentInChildren<Transform>();
            int pickedsound = UnityEngine.Random.Range(0, array.childCount);
            GameObject.Find("MasterAudio/Death").transform.GetChild(pickedsound).GetComponent<AudioSource>().Play();

            Destroy(GameObject.Find("PLAYER").GetComponent<SimpleSmoothMouseLook>());
            Destroy(GameObject.Find("PLAYER").GetComponent<MouseLook>());
            Destroy(GameObject.Find("PLAYER").GetComponent<CharacterController>());
            Destroy(GameObject.Find("PLAYER").GetComponent<CharacterMotor>());
            Destroy(GameObject.Find("PLAYER").GetComponent<FPSInputController>());
            Destroy(GameObject.Find("PLAYER/Pivot/AnimPivot/Camera/FPSCamera").GetComponent<SimpleSmoothMouseLook>());
            Destroy(GameObject.Find("PLAYER/Pivot/AnimPivot/Camera/FPSCamera").GetComponent<MouseLook>());

            GameObject.Find("PLAYER/Pivot/AnimPivot/Camera/FPSCamera/FPSCamera").GetComponent<ScreenOverlay>().enabled = true;

            death.transform.Find("GameOverScreen/Paper/Fatigue/TextEN").GetComponent<TextMesh>().text = en;
            death.transform.Find("GameOverScreen/Paper/Fatigue/TextFI").GetComponent<TextMesh>().text = fi;

        }



        // Update is called once per frame
        void Update()
        {
            light.intensity -= 0.05f;
            if (light.intensity == 0)
            {
                if (!sound.isPlaying)
                {
                    Destroy(explosion);
                }
            }
        }
    }
}