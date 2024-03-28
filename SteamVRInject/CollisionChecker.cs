using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HarmonyLib;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine.XR;

namespace WeWereHereVR
{
    public static class CollisionChecker
    {
        public static GameObject heldObject = null;
        public static Vector3 heldObjectOriginalTransformPosition;
        public static Quaternion heldObjectOriginalTransformRotation;
        public static bool held = false;
        public static Transform heldObjectParent = null;
        public static List<GameObject> badBoyList = new List<GameObject>();
        public static bool first = false;
        public static GameObject sameObject;
        public static GameObject pickupObject;
        private static float interactionCooldown = 1.0f;
        private static float lastInteractionTime;


        // Start is called before the first frame update
        public static GameObject CheckCollisionWithSphere(GameObject Hand)
        {
            //Debug.Log("Started");

            Collider parentCollider = Hand.GetComponentInParent<SphereCollider>();
            //Debug.Log(parentCollider);

            //badBoyList.Add(Hand.transform.parent.gameObject);
            //badBoyList.Add(GameObject.Find())
            Collider[] colliders = Physics.OverlapSphere(parentCollider.transform.position, parentCollider.bounds.extents.magnitude);
            //if (!first)
            //{             //Debug.Log(Hand.transform.parent.parent.parent);
            //    Initiate("asd");
            //}
            //Debug.Log(badBoyList.Count);
            if (colliders.Length > 0)
            {
                GameObject closestObject = colliders[0].gameObject;
                if (!Var.pickup)
                {
                    
                    float closestDistance = Vector3.Distance(parentCollider.transform.position, closestObject.transform.position);

                    for (int i = 1; i < colliders.Length; i++)
                    {
                        float distance = Vector3.Distance(parentCollider.transform.position, colliders[i].transform.position);

                        if (distance < closestDistance && !(badBoyList.Contains(colliders[i].gameObject)))
                        {
                            closestObject = colliders[i].gameObject;
                            closestDistance = distance;
                        }
                    }
                }
                else
                {
                    closestObject = null;
                    for(int i = 0; i < colliders.Length; i++)
                    {
                        if (colliders[i].gameObject.GetComponent<BaseSnapLocation>() != null)
                        {
                            closestObject = colliders[i].gameObject;
                            break;
                        }
                        
                    }
                    if (closestObject != null)
                    {
                        CollisionAction(Hand, closestObject);
                    }
                    return closestObject;
                    
                }
                
                if (!held)
                {
                    CollisionAction(Hand, closestObject);
                }
                return closestObject;
            }
            else
            {
                Debug.Log("Whoops, no colliders");
            }

            return null;
        }
        public static void CollisionAction(GameObject hand, GameObject gameObject)
        {
            //if we have an object with the type of pickup in our hand
            if (Var.pickup)
            {
                
                //Debug.Log("Snap");
                //we identify it
                if (gameObject.GetComponent<FilmRollSlotView>() != null)//and snap it to the correct socket
                {
                    gameObject.GetComponent<FilmRollSlotView>().OnPointedAt();
                    gameObject.GetComponent<FilmRollSlotView>().OnInteraction(MakePlayer.photonViewID);
                    Var.pickup = false;
                }
                else if (gameObject.GetComponent<HieroglyphSlabSlotView>() != null)//and snap it to the correct socket
                {
                    if (!(Time.time - lastInteractionTime < interactionCooldown))
                    {
                        gameObject.GetComponent<HieroglyphSlabSlotView>().OnPointedAt();
                        gameObject.GetComponent<HieroglyphSlabSlotView>().OnInteraction(MakePlayer.photonViewID);
                        Var.pickup = false;
                    }



                    
                    
                }
                
            }
            else
            {
                //this checks whether the gameObject the collider is attached to contains any of the desired game components we want to interact with
                //TODO: make that switch case instead.
                if (gameObject.GetComponent<InspectableBookView>() != null)//that handles the book.
                {
                    heldObject = gameObject;
                    heldObjectParent = gameObject.transform.parent;
                    heldObjectOriginalTransformPosition = gameObject.transform.position;
                    heldObjectOriginalTransformRotation = gameObject.transform.rotation;

                    held = true;


                    gameObject.transform.position = new Vector3(0, 0, 0);
                    gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
                    gameObject.transform.localPosition = new Vector3(0, 0, 0);
                    gameObject.transform.localRotation = Quaternion.Euler(0, 0, 0);

                    gameObject.transform.SetParent(hand.transform.parent);
                    gameObject.transform.localPosition = new Vector3(0, 0, 0);
                    gameObject.transform.localRotation = Quaternion.Euler(0, 0, 0);
                    InputDevice device = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
                    HapticCapabilities capabilities;
                    if (device.TryGetHapticCapabilities(out capabilities))
                    {
                        if (capabilities.supportsImpulse)
                        {
                            device.SendHapticImpulse(0, 0.5f, 0.5f);
                        }
                    }

                }
                else if (gameObject.GetComponent<InteractableTorches>() != null)//this the torches
                {
                    InteractableTorches torch = gameObject.GetComponent<InteractableTorches>();
                    torch.OnInteraction(MakePlayer.photonViewID);

                }
                else if (gameObject.GetComponent<GeneratorHandleController>() != null)//this the generator handle
                {
                    if (Var.barometerSolved)
                    {
                        GeneratorHandleController handle = gameObject.GetComponent<GeneratorHandleController>();
                        handle.OnInteraction(MakePlayer.photonViewID);
                    }

                }
                else if (gameObject.GetComponent<TheaterSolutionLever>() != null)//theater solution lever
                {
                    TheaterSolutionLever lever = gameObject.GetComponent<TheaterSolutionLever>();
                    lever.OnInteraction(MakePlayer.photonViewID);

                }
                else if (gameObject.GetComponent<TheaterRedLightSwitch>() != null)//blood light switch
                {
                    TheaterRedLightSwitch lightswitch = gameObject.GetComponent<TheaterRedLightSwitch>();
                    lightswitch.OnInteraction(MakePlayer.photonViewID);

                }
                else if (gameObject.GetComponent<TheaterHandleController>() != null)//the theater handle
                {
                    
                        TheaterHandleDirectionView[] directionViews = gameObject.GetComponentsInChildren<TheaterHandleDirectionView>();
                        foreach (TheaterHandleDirectionView directionView in directionViews)
                        {
                            if (directionView.gameObject.name == "Left" && Var.leftRight == 0)//depending on the buttons the player pressed we move it counter-clockwise
                            {
                                var onMouseDownMethod = AccessTools.Method(typeof(TheaterHandleDirectionView), "OnMouseDown");
                                onMouseDownMethod.Invoke(directionView, new object[] { });
                            }
                            else if (directionView.gameObject.name == "Right" && Var.leftRight == 1)//or clockwise
                            {
                                var onMouseDownMethod = AccessTools.Method(typeof(TheaterHandleDirectionView), "OnMouseDown");
                                onMouseDownMethod.Invoke(directionView, new object[] { });
                            }
                        }
                    

                }
                //else if (gameObject.GetComponent<TheaterControlPanelView>() != null)
                //{
                //    Var.isInspecting = true;
                //    Var.inspectedObject = gameObject;
                //    var lockMethod = AccessTools.Method(typeof(TheaterControlPanelView), "LockAtPosition");
                //    lockMethod.Invoke(gameObject.GetComponent<TheaterControlPanelView>(), new object[] { });
                //}
                else if (gameObject.GetComponent<WaterValveView>() != null)//the valves
                {
                    WaterValveView valve = gameObject.GetComponent<WaterValveView>();
                    valve.OnInteraction(MakePlayer.photonViewID);

                }
                else if (gameObject.GetComponent<TheaterRecordPlayerView>() != null)//the grammophone
                {
                    TheaterRecordPlayerView record = gameObject.GetComponent<TheaterRecordPlayerView>();
                    record.OnInteraction(MakePlayer.photonViewID);

                }
                //else if (gameObject.name == "Canvas")//the generator
                //{
                //    if (!Var.barometerSolved)//if we didn't solve the barometer puzzle yet
                //    {
                //        //Debug.Log("Activated Barometer");
                //        ActivateBarometer(GameObject.Find("generator").GetComponentInChildren<BarometerController>().gameObject);//we activate it.
                //    }
                //    else//otherwise we just activate the generator
                //    {
                //        FieldInfo isInteractableField = AccessTools.Field(typeof(GeneratorHandleController), "isInteractable");
                //        bool isInteractable = (bool)isInteractableField.GetValue(gameObject.GetComponentInChildren<GeneratorHandleController>());
                //        if (isInteractable)
                //        {
                //            gameObject.GetComponentInChildren<GeneratorHandleController>().OnInteraction(MakePlayer.photonViewID);
                //        }
                //    }


                //}
                else if (gameObject.GetComponent<BarometerView>())//the generator
                {
                    if (!Var.barometerSolved)//if we didn't solve the barometer puzzle yet
                    {
                        //Debug.Log("Activated Barometer");
                        ActivateBarometer(GameObject.Find("generator").GetComponentInChildren<BarometerController>().gameObject);//we activate it.
                    }
                    //else//otherwise we just activate the generator
                    //{
                    //    FieldInfo isInteractableField = AccessTools.Field(typeof(GeneratorHandleController), "isInteractable");
                    //    bool isInteractable = (bool)isInteractableField.GetValue(gameObject.GetComponentInChildren<GeneratorHandleController>());
                    //    if (isInteractable)
                    //    {
                    //        gameObject.GetComponentInChildren<GeneratorHandleController>().OnInteraction(MakePlayer.photonViewID);
                    //    }
                    //}


                }
                else if (gameObject.GetComponent<ChandelierSwitch>() != null)//the white lever in the librarians room
                {
                    ChandelierSwitch switcharoo = gameObject.GetComponent<ChandelierSwitch>();//cannot call it switch for obvious reasons.
                    switcharoo.OnInteraction(MakePlayer.photonViewID);

                }
                else if (gameObject.GetComponent<FilmRollPickUp>() != null)//the film roll
                {

                    gameObject.GetComponent<FilmRollPickUp>().OnInteraction(MakePlayer.photonViewID);
                    Var.pickup = true;


                }
                else if (gameObject.GetComponent<HieroglyphSlabPickup>() != null)//symbols in the painting room
                {
                    if (!(Time.time - lastInteractionTime < interactionCooldown))
                    {
                        gameObject.GetComponent<HieroglyphSlabPickup>().OnInteraction(MakePlayer.photonViewID);
                        Var.pickup = true;
                    }
                }
                else if (gameObject.GetComponent<ProjectorHandleController>() != null)//Chess film projector handle
                {

                    ProjectorHandleController handle = gameObject.GetComponent<ProjectorHandleController>();
                    if (handle.IsInteractable())
                    {
                        handle.OnInteraction(MakePlayer.photonViewID);
                    }

                }
                else if (gameObject.GetComponent<FilmProjectorPlayButtonView>() != null)//chess film projector play button
                {

                    FilmProjectorPlayButtonView button = gameObject.GetComponent<FilmProjectorPlayButtonView>();
                    button.OnInteraction(MakePlayer.photonViewID);

                }
                else if (gameObject.GetComponent<DoorView>() != null)//door
                {

                    DoorView door = gameObject.GetComponent<DoorView>();
                    door.OnInteraction(MakePlayer.photonViewID);

                }
                else if (gameObject.GetComponent<MazeDoorSwitch>() != null)//different door
                {
                    MazeDoorSwitch switcharoo = gameObject.GetComponent<MazeDoorSwitch>();//cannot call it switch for obvious reasons.
                    switcharoo.OnInteraction(MakePlayer.photonViewID);

                }
                else if (gameObject.GetComponent<ChessPiece>() != null)//any chess piece
                {
                    ChessPiece piece = gameObject.GetComponent<ChessPiece>();
                    piece.OnInteraction(MakePlayer.photonViewID);
                    if (Var.chessLaser != null)
                    {
                        Debug.Log("Hewwo");
                        Var.chessLaser.SetActive(true);
                    }
                    else
                    {
                        Debug.Log("whoops");
                    }

                }
                else if (gameObject.GetComponent<ChessBoardTileView>() != null)//any chess board tile
                {
                    ChessBoardTileView tile = gameObject.GetComponent<ChessBoardTileView>();
                    tile.OnInteraction(MakePlayer.photonViewID);

                }
                else if (gameObject.GetComponent<HieroglyphSlabSlotView>() != null)//symbol slots
                {
                    if (gameObject.GetComponentInChildren<HieroglyphSlabPickup>() != null)//if tehy're not empty
                    {
                        HieroglyphSlabPickup pickup = gameObject.GetComponentInChildren<HieroglyphSlabPickup>();
                        if (pickup != null)//we wanna pick it back up
                        {

                            pickup.OnInteraction(MakePlayer.photonViewID);
                            Var.pickup = true;
                            lastInteractionTime = Time.time;//cooldown so it doesn't immediately snap back into the slot



                        }
                    }




                }
                    //doesn't work yet. Probably never will. If else isn't toooooo bad...
                    //switch (gameObject.GetComponent<Component>())
                    //{
                    //    case InspectableBookView bookView:
                    //        heldObject = gameObject;
                    //        heldObjectParent = gameObject.transform.parent;
                    //        heldObjectOriginalTransformPosition = gameObject.transform.position;
                    //        heldObjectOriginalTransformRotation = gameObject.transform.rotation;

                    //        held = true;

                    //        gameObject.transform.position = new Vector3(0, 0, 0);
                    //        gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
                    //        gameObject.transform.localPosition = new Vector3(0, 0, 0);
                    //        gameObject.transform.localRotation = Quaternion.Euler(0, 0, 0);

                    //        gameObject.transform.SetParent(hand.transform.parent);
                    //        gameObject.transform.localPosition = new Vector3(0, 0, 0);
                    //        gameObject.transform.localRotation = Quaternion.Euler(0, 0, 0);

                    //        break;

                    //    case InteractableTorches torch:
                    //        torch.OnInteraction(MakePlayer.photonViewID);
                    //        break;

                    //    case WaterValveView valve:
                    //        valve.OnInteraction(MakePlayer.photonViewID);
                    //        break;

                    //    case TheaterRecordPlayerView record:
                    //        record.OnInteraction(MakePlayer.photonViewID);
                    //        break;
                    //    case ChandelierSwitch switcharoo://can't name it switch for obvious reasons.
                    //        switcharoo.OnInteraction(MakePlayer.photonViewID);
                    //        break;
                    //    default:
                    //        break;
                    //}
                }
        }
        
        public static GameObject CheckCollisionWithIndex(GameObject Hand)
        {
            //Debug.Log("Started");

            Collider parentCollider = Hand.GetComponentInChildren<SphereCollider>();
            //Debug.Log(parentCollider);

            //badBoyList.Add(Hand.transform.parent.gameObject);
            //badBoyList.Add(GameObject.Find())
            Collider[] colliders = Physics.OverlapSphere(parentCollider.transform.position, parentCollider.bounds.extents.magnitude);
            //if (!first)
            //{             //Debug.Log(Hand.transform.parent.parent.parent);
            //    Initiate("asd");
            //}
            //Debug.Log(badBoyList.Count);
            if (colliders.Length > 0)
            {
                GameObject closestObject = colliders[0].gameObject;
                

                float closestDistance = Vector3.Distance(parentCollider.transform.position, closestObject.transform.position);

                for (int i = 1; i < colliders.Length; i++)
                {
                    float distance = Vector3.Distance(parentCollider.transform.position, colliders[i].transform.position);

                    if (distance < closestDistance && !(badBoyList.Contains(colliders[i].gameObject)))
                    {
                        closestObject = colliders[i].gameObject;
                        closestDistance = distance;
                    }
                }
                
                

                if (!held)
                {
                    CollisionActionIndex(Hand, closestObject);
                }
                return closestObject;
            }
            else
            {
                //Debug.Log("Whoops, no colliders");
            }

            return null;
        }
        public static void CollisionActionIndex(GameObject hand, GameObject gameObject)
        {
            if (gameObject.GetComponent<SymbolInteractableView>() != null)
            {
                if (gameObject != sameObject)
                {
                    SymbolInteractableView symbol = gameObject.GetComponent<SymbolInteractableView>();
                    symbol.OnInteraction(MakePlayer.photonViewID);
                    sameObject = gameObject;

                }

            }
        }
        //this makes sure that the script above will not interact with any colliders that are attached to the player
        public static void Initiate(GameObject gameObject)
        {
            Transform[] children = gameObject.GetComponentsInChildren<Transform>(true);


            foreach (Transform child in children)
            {
                //Debug.Log(child.gameObject);

                badBoyList.Add(child.gameObject);
                first = true;
            }

        }
        public static void ActivateBarometer(GameObject gameObject)
        {
            //BarometerController barometer = gameObject.GetComponent<BarometerController>();
            BarometerController barometer = GameObject.Find("Barometer").GetComponent<BarometerController>();
            var startBarometer = AccessTools.Method(typeof(BarometerController), "ToggleMiniGame");
            startBarometer.Invoke(barometer, new object[] { new object[] { } });
            Var.barometerActive = true;
        }
    }
}
