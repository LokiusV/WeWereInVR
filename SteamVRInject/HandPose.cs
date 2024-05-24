using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using WeWereHereVR;
using HarmonyLib;
namespace WeWereHereVR
{
    public class HandPose : MonoBehaviour
    {
        
        private static List<KeyCode> heldKeys = new List<KeyCode>();
        public static GameObject indexHand;
        public static GameObject normalHand;
        public static GameObject closedHand;
        private static bool done = false;
        public static void Initialize(GameObject iH, GameObject nH, GameObject cH)
        {
            indexHand = iH;
            closedHand = cH;
            normalHand = nH;

        }
        public static void UpdateHeldKeys()
        {

            List<KeyCode> currentKeys = new List<KeyCode>();

            foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKey(keyCode))
                {
                    currentKeys.Add(keyCode);
                }
            }
            
            if (heldKeys.Count > currentKeys.Count)
            {
                done = false;

            }
            else if (heldKeys.Count < currentKeys.Count)
            {
                done = false;
            }
            heldKeys = currentKeys;
        }

        public static void CheckKeyCombinations()
        {
            if (!Var.pickup && (!Var.barometerActive))
            {

                if (heldKeys.Contains(Var.gripButton) && heldKeys.Contains(Var.indexButton))
                {
                    if (!done)
                    {
                        Var.leftRight = 1;
                        CollisionChecker.CheckCollisionWithSphere(indexHand);
                        if (CollisionChecker.held)
                        {
                            indexHand.SetActive(false);
                            closedHand.SetActive(false);
                            normalHand.SetActive(false);

                        }
                        else
                        {
                            indexHand.SetActive(false);
                            closedHand.SetActive(true);
                            normalHand.SetActive(false);

                        }
                        done = true;

                    }

                    if (heldKeys.Contains(Var.cancelButton) && CollisionChecker.held)
                    {
                        CollisionChecker.heldObject.transform.localRotation = Quaternion.Euler(180, 0, 0);
                    }
                    else if (!heldKeys.Contains(Var.cancelButton) && CollisionChecker.held)
                    {
                        CollisionChecker.heldObject.transform.localRotation = Quaternion.Euler(0, 0, 0);

                    }

                }
                else if (heldKeys.Contains(Var.gripButton) && !(heldKeys.Contains(Var.indexButton)))
                {
                    if (!done)
                    {
                        Var.leftRight = 0;
                        CollisionChecker.CheckCollisionWithSphere(indexHand);
                        if (CollisionChecker.held)
                        {
                            indexHand.SetActive(false);
                            closedHand.SetActive(false);
                            normalHand.SetActive(false);

                        }
                        else
                        {
                            indexHand.SetActive(true);
                            closedHand.SetActive(false);
                            normalHand.SetActive(false);

                        }



                    }
                    done = true;
                    if (heldKeys.Contains(Var.cancelButton) && CollisionChecker.held)
                    {
                        CollisionChecker.heldObject.transform.localRotation = Quaternion.Euler(180, 0, 0);
                    }
                    else if (!heldKeys.Contains(Var.cancelButton) && CollisionChecker.held)
                    {
                        CollisionChecker.heldObject.transform.localRotation = Quaternion.Euler(0, 0, 0);

                    }

                    //}
                    if (!CollisionChecker.held)
                    {
                        CollisionChecker.CheckCollisionWithIndex(indexHand);

                    }

                }
                else
                {

                    if (CollisionChecker.held)
                    {


                        CollisionChecker.heldObject.transform.SetParent(CollisionChecker.heldObjectParent);
                        CollisionChecker.heldObject.transform.position = CollisionChecker.heldObjectOriginalTransformPosition;
                        CollisionChecker.heldObject.transform.rotation = CollisionChecker.heldObjectOriginalTransformRotation;
                        CollisionChecker.held = false;
                    }
                    indexHand.SetActive(false);
                    closedHand.SetActive(false);
                    normalHand.SetActive(true);
                    CollisionChecker.sameObject = null;

                }
            }
            else if (!Var.pickup && Var.barometerActive)
            {
                indexHand.SetActive(false);
                closedHand.SetActive(false);
                normalHand.SetActive(true);

                if (Input.GetKeyDown(Var.acceptButton) || TriggerProvider.CheckPressed() == true)
                {
                    BarometerView barometerView = GameObject.Find("generator").GetComponentInChildren<BarometerView>();
                    var barometerMethod = AccessTools.Method(typeof(BarometerView), "OnSkillShot");
                    barometerMethod.Invoke(Var.barometerView, new object[] {  });
                    
                }
                else if (Input.GetKeyDown(Var.cancelButton))
                {
                    CollisionChecker.ActivateBarometer(GameObject.Find("generator").GetComponentInChildren<BarometerController>().gameObject);

                }
            }
            else
            {
                if (heldKeys.Contains(Var.gripButton))
                {
                    CollisionChecker.CheckCollisionWithSphere(indexHand);
                }
                else
                {
                    Var.rightHand.GetComponent<PlayerRightHand>().ClearHand();
                    Var.pickup = false;
                    done = true;
                }

            }
        }
    }
}
