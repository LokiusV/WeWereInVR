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
using System.Linq;

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
            existingObject.transform.position = Camera.main.transform.position + new Vector3(0, 0, 2f);//new Vector3(0, 1.5f, 0.5f);


        }

        void Update()
        {
            //joy_1_axis_2: trigger
            //joy_1_axis_3: horizontal
            //joy_1_axis_4: vertical
            //if (Input.GetKeyDown(Var.jumpButton))
            //{
            //    Debug.Log("pressed");
            //    Var.debugInt++;
            //}
            //Debug.Log("Axis: " + Var.debugInt.ToString());
            //Debug.Log(Input.GetAxis("joy_1_axis_" + Var.debugInt.ToString()));
            //Debug.Log(TriggerProvider.CheckPressed());
            //Debug.Log(Input.GetAxis("joy_1_axis_3"));
            //Debug.Log("Left: "+SnapTurnProvider.SnapLeft());
            //Debug.Log("Right: "+SnapTurnProvider.SnapRight());
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

                //all of this is obviously awful for performance but necessary, because the server buttons, fields, etc. Get created after the initialization of the laser pointer(when the server list refreshes)
                //I could just hook into the server browser refresh method but this was easier for the time.
                //TODO: put in server browser refresh and LaserSpawner.Initialize() instead
                Button[] uiButtons = FindObjectsOfType<Button>();
                InputField[] inputFields= FindObjectsOfType<InputField>();
                Toggle[] toggles = FindObjectsOfType<Toggle>();
                Dropdown[] dropdowns = FindObjectsOfType<Dropdown>();
                Scrollbar[] sliders = FindObjectsOfType<Scrollbar>();
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
                foreach (InputField inputField in inputFields)
                {
                    if (inputField.gameObject.GetComponent<BoxCollider>() == null && gameObject.activeSelf)
                    {

                        RectTransform buttonRectTransform = inputField.GetComponent<RectTransform>();


                        Vector2 inputSize = buttonRectTransform.sizeDelta;


                        BoxCollider boxCollider = inputField.gameObject.AddComponent<BoxCollider>();


                        boxCollider.size = new Vector3(inputSize.x, inputSize.y, 1f);
                        //boxCollider.enabled = true;
                        //boxCollider.isTrigger = true;
                    }
                    else if (inputField.gameObject.GetComponent<BoxCollider>() != null && gameObject.activeSelf == false)
                    {
                        inputField.gameObject.GetComponent<BoxCollider>().enabled = false;
                    }
                }
                foreach (Toggle toggle in toggles)
                {
                    if (toggle.gameObject.GetComponent<BoxCollider>() == null && gameObject.activeSelf)
                    {

                        RectTransform buttonRectTransform = toggle.GetComponent<RectTransform>();


                        Vector2 toggleSize = buttonRectTransform.sizeDelta;


                        BoxCollider boxCollider = toggle.gameObject.AddComponent<BoxCollider>();


                        boxCollider.size = new Vector3(toggleSize.x, toggleSize.y, 1f);
                    }
                    else if (toggle.gameObject.GetComponent<BoxCollider>() != null && gameObject.activeSelf == false)
                    {
                        toggle.gameObject.GetComponent<BoxCollider>().enabled = false;
                    }
                }
                foreach (Dropdown dropdown in dropdowns)
                {
                    if (dropdown.gameObject.GetComponent<BoxCollider>() == null && gameObject.activeSelf)
                    {

                        RectTransform buttonRectTransform = dropdown.GetComponent<RectTransform>();


                        Vector2 buttonSize = buttonRectTransform.sizeDelta;


                        BoxCollider boxCollider = dropdown.gameObject.AddComponent<BoxCollider>();


                        boxCollider.size = new Vector3(buttonSize.x, buttonSize.y, 1f);
                    }
                    else if (dropdown.gameObject.GetComponent<BoxCollider>() != null && gameObject.activeSelf == false)
                    {
                        dropdown.gameObject.GetComponent<BoxCollider>().enabled = false;
                    }
                }
                
                float axisValue = Input.GetAxis("joy_0_axis_1");

                if (Mathf.Abs(axisValue) >= 0.3f)
                {
                    foreach (Scrollbar slider in sliders)
                    {
                        slider.value = Mathf.Clamp01(slider.value -axisValue * Time.deltaTime);
                        
                    }
                }
                RaycastHit hit;
                if (Physics.Raycast(lineStart, trackedPoseDriver.transform.forward, out hit, 10f)) 
                {
                    BoxCollider collider = hit.collider.GetComponent<BoxCollider>(); 

                    if (collider != null) 
                    {
                        
                        if (Input.GetKeyDown(Var.acceptButton)||TriggerProvider.CheckPressed())
                        {
                            GameObject parentObject = collider.gameObject;
                            Button hitButton = parentObject.GetComponent<Button>();
                            InputField hitField=parentObject.GetComponent<InputField>();
                            Toggle hitToggle= parentObject.GetComponent<Toggle>();
                            Dropdown hitDropdown = parentObject.GetComponent<Dropdown>();
                            Slider hitSlider = parentObject.GetComponent<Slider>();
                            if (hitToggle != null)
                            {
                                hitToggle.isOn=!hitToggle.isOn;
                            }
                            else if (hitSlider != null)
                            {
                                Var.slider=hitSlider;
                            }
                            else if (hitDropdown != null)
                            {
                                if (hitDropdown.value+1 >= hitDropdown.options.Count)
                                {
                                    hitDropdown.value = 0;
                                }
                                else
                                {
                                    hitDropdown.value++;
                                }
                                hitDropdown.RefreshShownValue();
                            }
                            else if (hitField != null && GameObject.Find("Keyboard") != null)
                            {
                                GameObject.Find("Keyboard").GetComponent<KeyboardController>().inputField = hitField;
                                hitField.Select();
                            }
                            else if (hitButton != null && parentObject.GetComponent<ServerListItem>() == null)
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
                if (Var.slider!= null)
                {
                    Var.slider.value = Mathf.Clamp01(Var.slider.value - axisValue * Time.deltaTime);

                }




            }
        }
    }
}
