using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SpatialTracking;


namespace WeWereHereVR
{
    public class ChessLaser:MonoBehaviour
    {
        private LineRenderer lineRenderer;
        //private TrackedPoseDriver trackedPoseDriver;
        private GameObject laserObject;
        private GameObject rightC;
        public void Initialize(GameObject rightController)
        {
            //create all the new gameObjects
            laserObject = new GameObject("ChessLaser");
            Var.chessLaser = laserObject;

            lineRenderer = laserObject.AddComponent<LineRenderer>();

            lineRenderer.useWorldSpace = true;
            lineRenderer.startWidth = 0.01f;
            lineRenderer.endWidth = 0.01f;
            lineRenderer.positionCount = 2;
            Material newMaterial = new Material(Shader.Find("Sprites/Default"));
            newMaterial.SetFloat("_Mode", 1);
            
            newMaterial.color = Color.white; 


            lineRenderer.material = newMaterial;
            laserObject.transform.SetParent(rightController.transform);

            //TrackedPoseDriver trackedPoseDriver = laserObject.AddComponent<TrackedPoseDriver>();
            //trackedPoseDriver.SetPoseSource(TrackedPoseDriver.DeviceType.GenericXRController, TrackedPoseDriver.TrackedPose.RightPose);
            //trackedPoseDriver=rightController.GetComponent<TrackedPoseDriver>();
            //rightC = rightController;
            laserObject.SetActive(false);
        }
        public void Update()
        {
            
            //Vector3 controllerPosition = trackedPoseDriver.transform.localPosition;
            //Quaternion controllerRotation = trackedPoseDriver.transform.localRotation;


            //trackedPoseDriver.transform.localPosition = controllerPosition;
            //trackedPoseDriver.transform.localRotation = controllerRotation;



            //Vector3 lineStart = trackedPoseDriver.transform.position;
            //Vector3 lineEnd = trackedPoseDriver.transform.position + trackedPoseDriver.transform.forward * 10f;
            Vector3 lineStart = laserObject.transform.position;
            Vector3 lineEnd = laserObject.transform.position + laserObject.transform.forward * 10f;
            lineRenderer.SetPosition(0, lineStart);
            lineRenderer.SetPosition(1, lineEnd);
            
            RaycastHit[] hits = Physics.RaycastAll(lineStart, laserObject.transform.forward, 10f);
            if (Input.GetKeyDown(Var.acceptButton)||TriggerProvider.CheckPressed()==true)
            {


                
                foreach (RaycastHit hit in hits)
                {
                    
                    ChessBoardTileView tile = hit.collider.GetComponent<ChessBoardTileView>();

                    //stricly necessary because a user might press A even when not pointing on a tile
                    if (tile != null)
                    {
                        tile.OnInteraction(MakePlayer.photonViewID);
                        laserObject.SetActive(false);
                        
                    }

                }
            }
        }


        }
    }

