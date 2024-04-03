using BepInEx;
using UnityEngine;
using HarmonyLib;
using UnityEngine.SpatialTracking;
using System.Reflection;
using UnityEngine.XR;
using System.IO;
using WeWereHereVR;


namespace WeWereHereVR
{
    class MakePlayer
    {
        public static HandCreator hc { get; private set; }
        public static bool done2 = false;
        public static CharacterController characterControllerInstance;
        public static int photonViewID;
        public static void SetHeight(PlayerMovement __instance)
        {
            FieldInfo normalHeightField = AccessTools.Field(typeof(PlayerMovement), "normalHeight");
            FieldInfo crouchHeightField = AccessTools.Field(typeof(PlayerMovement), "crouchHeight");
            




            string filePath = Path.Combine(Paths.PluginPath, "Settings.txt");
            float normalHeightValue = 1.7f;
        
            string fileContent = FileReader.ReadFromFile(filePath);
            string[] lines = fileContent.Split('\n');
            if (fileContent != null)
            {
                normalHeightValue = float.Parse(lines[0].Trim());
                Debug.Log(normalHeightValue);

            }

            FieldInfo characterControllerField = AccessTools.Field(typeof(PlayerMovement), "characterController");
            CharacterController characterController = (CharacterController)characterControllerField.GetValue(__instance);
            characterController.center = new Vector3(0, 0, 0);
            characterControllerInstance = characterController;

            //GameObject eg = new GameObject();
            //Transform parent = Camera.main.transform.parent;
            //eg.transform.SetParent(parent);
            //Camera.main.transform.SetParent(eg.transform);
            //float realisticHeightOffset = normalHeightValue - (characterController.height / 2f) - parent.position.y;
            //if (parent.position.y <= 0)
            //{
            //    eg.transform.localPosition = new Vector3(0, realisticHeightOffset, 0);
            //}
            //else
            //{
            //    eg.transform.localPosition = new Vector3(0, -realisticHeightOffset, 0);

            //}
            if (GameObject.Find("eg") == null)
            {
                GameObject eg = new GameObject("eg");
                Transform parent = Camera.main.transform.parent;
                Var.playerGameObject = parent.gameObject;
                eg.transform.SetParent(parent);
                float realisticHeightOffset = Camera.main.transform.localPosition.y + (characterController.height / 2f);
                Debug.Log(realisticHeightOffset);
                eg.transform.localPosition = new Vector3(0, -(realisticHeightOffset) + normalHeightValue, 0);
                //eg.transform.localPosition = new Vector3(0.0046f, -1.7019f, -0.2056f);
                Camera.main.transform.SetParent(eg.transform);
                GameObject leftController = new GameObject("LeftController");
                GameObject vralkieTalkie= GameObject.Find("VRalkieTalkie");
                vralkieTalkie.transform.rotation = Quaternion.Euler(-105, 180,0);
                vralkieTalkie.transform.localScale=new Vector3(0.06f, 0.06f, 0.06f);
                vralkieTalkie.transform.localPosition = new Vector3(0, 0, 0);
                vralkieTalkie.transform.SetParent(leftController.transform);


                //TODO: Fix the fucking walkie talkie: beginning:
                //GameObject walkieTalkieGameObject = new GameObject("WalkieTalkie_Voice");
                //WalkieTalkie walkieTalkie = walkieTalkieGameObject.AddComponent<WalkieTalkie>();
                //var onInteractionRPCMethod = AccessTools.Method(typeof(WalkieTalkie), "OnInteractionRPC");

                //onInteractionRPCMethod.Invoke(walkieTalkie, new object[] { eg.transform.parent.GetComponent<PhotonView>().viewID });
                //walkieTalkie.walkietalkieLight = leftController.GetComponentInChildren<Light>();
                //walkieTalkie.OnInteraction(eg.transform.parent.GetComponent<PhotonView>().ownerId);

                leftController.transform.position = new Vector3(0, 0, 0);
                //leftController.transform.rotation = new Quaternion(0, 0, 0,0);
                leftController.transform.localPosition = new Vector3(0, 0, 0);
                GameObject og = new GameObject("offset");
                og.transform.SetParent(eg.transform);
                og.transform.localPosition = new Vector3(-0.0046f, 1.7019f, 0.2056f);
                //leftController.transform.localScale = new Vector3(-0.12f, 0.12f, 0.12f);

                TrackedPoseDriver trackedPoseDriver = leftController.AddComponent<TrackedPoseDriver>();
                trackedPoseDriver.trackingType = TrackedPoseDriver.TrackingType.RotationAndPosition;
                trackedPoseDriver.SetPoseSource(TrackedPoseDriver.DeviceType.GenericXRController, TrackedPoseDriver.TrackedPose.LeftPose);
                
                leftController.transform.SetParent(og.transform);
                leftController.transform.localRotation *= Quaternion.Euler(0f, 180f, 0f);
                leftController.transform.localPosition = new Vector3(-leftController.transform.localPosition.x, leftController.transform.localPosition.y, leftController.transform.localPosition.z);

                hc =eg.AddComponent<HandCreator>();
                //HandCreator hc = eg.AddComponent<HandCreator>();
                hc.CreateHand();
                GameObject rightController = GameObject.Find("Right Controller");
                rightController.transform.SetParent(og.transform);
                CollisionChecker.Initiate(eg.transform.parent.gameObject);
                photonViewID = eg.transform.parent.GetComponent<PhotonView>().viewID;
                Camera.main.transform.GetChild(0).gameObject.SetActive(false);
                Camera.main.transform.GetChild(1).gameObject.transform.position = new Vector3(0, 0, 0);
                Camera.main.transform.GetChild(1).gameObject.transform.localPosition = new Vector3(0, 0, 0);
                Camera.main.transform.GetChild(1).gameObject.transform.SetParent(rightController.transform);
                rightController.transform.GetChild(1).gameObject.transform.localPosition = new Vector3(0, 0, 0);
                rightController.transform.GetChild(1).gameObject.transform.localRotation = Quaternion.Euler(0,0,0);
                Var.rightHand = rightController.transform.GetChild(1).gameObject;
                //GameObject laserPointer = GameObject.Find("LaserPointer");
                //laserPointer.SetActive(false);

                //BoxCollider whiteLever = GameObject.Find("WhiteLever").GetComponentInChildren<BoxCollider>();
                //whiteLever.isTrigger = true;
                //whiteLever.size = new Vector3(2, 2, 2);
                //BoxCollider blueLever = GameObject.Find("Blue Lever").GetComponentInChildren<BoxCollider>();
                //blueLever.isTrigger = true;
                //blueLever.size = new Vector3(2, 2, 2);
                //BoxCollider redLever = GameObject.Find("Red Lever").GetComponentInChildren<BoxCollider>();
                //redLever.isTrigger = true;
                //redLever.size = new Vector3(2, 2, 2);
                //BoxCollider greenLever = GameObject.Find("Green Lever").GetComponentInChildren<BoxCollider>();
                //greenLever.isTrigger = true;
                //greenLever.size = new Vector3(2, 2, 2);
                //BoxCollider MapLever = GameObject.Find("MapLever").GetComponentInChildren<BoxCollider>();
                //MapLever.isTrigger = true;
                //MapLever.size = new Vector3(2, 2, 2);

                //DoorView stageDoor1 = GameObject.Find("TheaterRoom").GetComponentInChildren<DoorView>();
                //BoxCollider stageDoor1Collider = stageDoor1.gameObject.GetComponent<BoxCollider>();

                //BoxCollider barometer = GameObject.Find("Barometer").AddComponent<BoxCollider>();
                //BoxCollider generator = GameObject.Find("generator").GetComponent<BoxCollider>();
                ////barometer.isTrigger = true;
                //generator.isTrigger = true;
                //generator.size = new Vector3(10f, 10f, 10f);
                MazeDoorSwitch[]mazeDoorSwitches = GameObject.FindObjectsOfType<MazeDoorSwitch>();
                BoxCollider generator = GameObject.Find("Canvas").GetComponent<BoxCollider>();
                //barometer.isTrigger = true;
                generator.isTrigger = true;
                generator.size = new Vector3(2f, 2f, 2f);

                //barometer.size = new Vector3(1.5f, 1.5f, 1.5f);
                characterController.radius = 0.2f;
                GameObject controlBoard = GameObject.Find("ControlBoard");

                //This attaches box colliders to the Theater Handles to make interactions with the players hand possible. Same logic as with the UIButtons
                //this is deprecated. A new way to interact with the theater handles has been introduced in 0.3.1
                //TheaterHandleController[] handles = GameObject.FindObjectsOfType<TheaterHandleController>();
                //foreach (TheaterHandleController handle in handles)
                //{
                //    if (handle.gameObject.GetComponent<BoxCollider>() == null)
                //    {
                //        BoxCollider boxCollider = handle.gameObject.AddComponent<BoxCollider>();
                //        boxCollider.size = new Vector3(1f, 1f, 1f);
                //        boxCollider.isTrigger = true;
                //    }

                //}
                DoorView[] doors = GameObject.FindObjectsOfType<DoorView>();
                foreach (DoorView door in doors)
                {
                    
                        BoxCollider boxCollider = door.gameObject.AddComponent<BoxCollider>();
                        boxCollider.size = new Vector3(4, 6, 4);
                        boxCollider.isTrigger = true;
                    

                }
                foreach (MazeDoorSwitch mazeDoorSwitch in mazeDoorSwitches)
                {

                    BoxCollider boxCollider = mazeDoorSwitch.gameObject.AddComponent<BoxCollider>();
                    boxCollider.size = new Vector3(4, 4, 4);
                    boxCollider.isTrigger = true;


                }
                //overlaps with handles. fix asap! Just only create them once the puzzle is completed!
                TrapDoorView[] trapDoors = GameObject.FindObjectsOfType<TrapDoorView>();
                foreach (TrapDoorView trapDoor in trapDoors)
                {
                    //we don't ccheck if it already has a box collider attached to it, because it definetely has. we still want to add another one.
                    BoxCollider boxCollider = trapDoor.gameObject.AddComponent<BoxCollider>();
                    boxCollider.size = new Vector3(3, 3, 3);
                    boxCollider.center = new Vector3(0, 2, 0);
                    boxCollider.isTrigger = true;


                }
                ChessLaser chessLaserInstance = rightController.AddComponent<ChessLaser>();
                chessLaserInstance.Initialize(rightController);
                try
                {
                    Haptics.TriggerHapticFeedback(XRNode.RightHand, 5f);
                    Haptics.TriggerHapticFeedback(XRNode.LeftHand, 5f);
                }
                catch
                {
                    Debug.Log("catched");
                }
                

                done2 = true;
            }


            Debug.Log(normalHeightValue);
            normalHeightField.SetValue(__instance, normalHeightValue);
            crouchHeightField.SetValue(__instance, normalHeightValue / 2);

        }
    }
}
