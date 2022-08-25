using MSCLoader;
using System;
using UnityEngine;

namespace C4Mod
{
    public class C4Mod : Mod
    {
        public override string ID => "C4Mod"; //Your mod ID (unique)
        public override string Name => "C4 Mod"; //You mod name
        public override string Author => "epicduck410"; //Your Username
        public override string Version => "0.3"; //Version
        private GameObject bomb;
        private GameObject prefabbomb;
        private GameObject prefabdetonator;
        private AssetBundle ab;
        private bool delete = false;
        private GameObject detonator;
        Settings arm = new Settings("armbutton", "Arm", armbombs);
        Settings disarm = new Settings("disarmbutton", "Disarm", disarmbombs);
        Settings debug = new Settings("debugger", "Debug Mode", false);


        private static void armbombs()
        {
            var array = UnityEngine.Object.FindObjectsOfType<GameObject>();
            foreach (GameObject obj in array)
            {
                if (obj.name == "C4 EXPLOSIVE - UNARMED (Clone)")
                {
                    obj.transform.GetComponent<BombWaitBehaviour>().armed = true;
                    obj.name = "C4 EXPLOSIVE - ARMED (Clone)";
                    obj.transform.Find("Display/Text").GetComponent<TextMesh>().text = "ARMED";
                }
            }
        }

        private static void disarmbombs()
        {
            var array = UnityEngine.Object.FindObjectsOfType<GameObject>();
            foreach (GameObject obj in array)
            {
                if (obj.name == "C4 EXPLOSIVE - ARMED (Clone)")
                {
                    obj.transform.GetComponent<BombWaitBehaviour>().armed = false;
                    obj.name = "C4 EXPLOSIVE - UNARMED (Clone)";
                    obj.transform.Find("Display/Text").GetComponent<TextMesh>().text = "UNARMED";
                }
            }
        }

        // Set this to true if you will be load custom assets from Assets folder.
        // This will create subfolder in Assets folder for your mod.
        public override bool UseAssetsFolder => true;

        public override void OnNewGame()
        {
            // Called once, when starting a New Game, you can reset your saves here
        }

        public override void OnLoad()
        {
            ab = LoadAssets.LoadBundle(this, "bomb.unity3d");
            prefabbomb = ab.LoadAsset("C4 EXPLOSIVE.prefab") as GameObject;
            prefabdetonator = ab.LoadAsset("DETONATOR.prefab") as GameObject;
            ab.Unload(false);
            detonator = UnityEngine.Object.Instantiate(prefabdetonator); //Instantiate object in the world
            Vector3 player = GameObject.Find("PLAYER").transform.position;
            detonator.transform.localPosition = player;
            detonator.AddComponent<Detonator>();
            LoadAssets.MakeGameObjectPickable(detonator);
        }

        public override void ModSettings()
        {
            Settings.AddButton(this, arm, "Arm all bombs");
            Settings.AddButton(this, disarm, "Disarm all bombs");
            Settings.AddCheckBox(this, debug);
        }

        public override void OnSave()
        {
            // Called once, when save and quit
            // Serialize your save file here.
        }

        public override void OnGUI()
        {
            // Draw unity OnGUI() here
        }

        public override void Update()
        {
            
            detonator.GetComponent<Detonator>().debugging = (bool)debug.GetValue();

            if (Input.GetKey(KeyCode.LeftControl))
            {
                if (Input.GetKey(KeyCode.Backspace))
                {
                    delete = true;
                }
            }



            if (delete)
            {
                // If you are using this example, do this instead: if (GameObject.Find("item1") || GameObject.Find("item2")) and then do an else. This will make sure all objects get deleted.
                if (GameObject.Find("C4 EXPLOSIVE - UNARMED (Clone)"))
                {
                    GameObject tempbomb = GameObject.Find("C4 EXPLOSIVE - UNARMED (Clone)");
                    GameObject.Destroy(tempbomb);
                } else
                {
                    delete = false;
                }
                if (GameObject.Find("C4 EXPLOSIVE - ARMED (Clone)"))
                {
                    GameObject tempbomb = GameObject.Find("C4 EXPLOSIVE - ARMED (Clone)");
                    GameObject.Destroy(tempbomb);
                }
                else
                {
                    delete = false;
                }
            }


            // Spawns a bomb
            if (Input.GetKeyDown("l"))
            {
                bomb = GameObject.Instantiate(prefabbomb); //Instantiate object in the world
                Vector3 player = GameObject.Find("PLAYER").transform.position;
                bomb.name = "C4 EXPLOSIVE - UNARMED (Clone)";
                bomb.transform.localPosition = player;
                bomb.AddComponent<BombWaitBehaviour>();
                LoadAssets.MakeGameObjectPickable(bomb);
                bomb.GetComponent<BombWaitBehaviour>().detonator = detonator;
            }
        }
    }
}
