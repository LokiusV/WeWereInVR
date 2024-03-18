using BepInEx;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;
using HarmonyLib;
using UnityEngine.SpatialTracking;
using UnityEngine.EventSystems;
using UnityEngine.XR;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Reflection;
using System.Reflection.Emit;

namespace WeWereHereVR
{
    //Responsible for the LaserSpawner, UIInteractions and more
    public class SPC : MonoBehaviour
    {

        private LineRenderer lineRenderer;
        private TrackedPoseDriver trackedPoseDriver;
        private GraphicRaycaster graphicRaycaster;
        public EventSystem eventSystem;
        public GraphicRaycaster raycaster;
        public string canvasName;
        public float raycastDistance = 20f;

        public void Initialize(LineRenderer line, TrackedPoseDriver tracked, GraphicRaycaster raycaster, string canvas)
        {
            lineRenderer = line;
            trackedPoseDriver = tracked;
            graphicRaycaster = raycaster;
            canvasName = canvas;
            GameObject existingObject = GameObject.Find(canvasName);
            Canvas canva = existingObject.GetComponent<Canvas>();
            //Canvas canva = FindObjectOfType<Canvas>();
            //GameObject existingObject=canva.gameObject;
            raycaster = canva.GetComponent<GraphicRaycaster>();
            GameObject existingManager = GameObject.Find("EventSystem");
            eventSystem = existingManager.GetComponent<EventSystem>();
            canva.renderMode = UnityEngine.RenderMode.WorldSpace;
            existingObject.transform.localScale = new Vector3(0.001f, 0.001f, 0.001f);
            existingObject.transform.position = new Vector3(0, 1.5f, 0.5f);


        }

        void Update()
        {
            if (trackedPoseDriver != null && lineRenderer != null)
            {
                
                Vector3 controllerPosition = trackedPoseDriver.transform.localPosition;
                Quaternion controllerRotation = trackedPoseDriver.transform.localRotation;

                
                trackedPoseDriver.transform.localPosition = controllerPosition;
                trackedPoseDriver.transform.localRotation = controllerRotation;

                Vector3 lineStart = trackedPoseDriver.transform.position; 
                Vector3 lineEnd = trackedPoseDriver.transform.position + trackedPoseDriver.transform.forward * 10f; 

                
                lineRenderer.SetPosition(0, lineStart); 
                lineRenderer.SetPosition(1, lineEnd); 
                
                
                Button[] uiButtons = FindObjectsOfType<Button>();
                foreach (Button uiButton in uiButtons)
                {
                    if (uiButton.gameObject.GetComponent<BoxCollider>() == null && gameObject.activeSelf)
                    {
                        
                        RectTransform buttonRectTransform = uiButton.GetComponent<RectTransform>();

                        
                        Vector2 buttonSize = buttonRectTransform.sizeDelta;

                        
                        BoxCollider boxCollider = uiButton.gameObject.AddComponent<BoxCollider>();

                        
                        boxCollider.size = new Vector3(buttonSize.x, buttonSize.y, 1f);
                        //boxCollider.enabled = true;
                        //boxCollider.isTrigger = true;
                    }
                    else if (uiButton.gameObject.GetComponent<BoxCollider>() != null && gameObject.activeSelf == false)
                    {
                        uiButton.gameObject.GetComponent<BoxCollider>().enabled = false;
                    }
                }
                RaycastHit hit;
                if (Physics.Raycast(lineStart, trackedPoseDriver.transform.forward, out hit, 10f)) 
                {
                    BoxCollider collider = hit.collider.GetComponent<BoxCollider>(); 

                    if (collider != null) 
                    {
                        
                        if (Input.GetKeyDown(Var.acceptButton))
                        {
                            GameObject parentObject = collider.gameObject;
                            Button hitButton = parentObject.GetComponent<Button>();
                            if (hitButton != null && parentObject.GetComponent<ServerListItem>() == null)
                            {
                                hitButton.onClick.Invoke();
                                Debug.Log("Click");
                            }
                            else if (hitButton != null && parentObject.GetComponent<ServerListItem>() != null)
                            {
                                ServerListItem join = parentObject.GetComponent<ServerListItem>();
                                ServerListItem serverListItemInstance = new ServerListItem();
                                var joinServerMethod = AccessTools.Method(typeof(ServerListItem), "JoinServer");
                                joinServerMethod.Invoke(serverListItemInstance, new object[] { join.roomName });

                            }
                        }
                    }
                }




            }
        }
    }
}
