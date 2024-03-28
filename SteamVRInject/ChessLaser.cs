using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SpatialTracking;


namespace WeWereHereVR
{
    public class ChessLaser
    {
        private LineRenderer lineRenderer;
        private TrackedPoseDriver trackedPoseDriver;
        public void Initialize()
        {
            //create all the new gameObjects
            GameObject laserObject = new GameObject("ChessLazer");

            lineRenderer = laserObject.AddComponent<LineRenderer>();

            lineRenderer.useWorldSpace = true;
            lineRenderer.startWidth = 0.01f;
            lineRenderer.endWidth = 0.01f;
            lineRenderer.positionCount = 2;
            laserObject.transform.SetParent(GameObject.Find("offset").transform);

            TrackedPoseDriver trackedPoseDriver = laserObject.AddComponent<TrackedPoseDriver>();
            trackedPoseDriver.SetPoseSource(TrackedPoseDriver.DeviceType.GenericXRController, TrackedPoseDriver.TrackedPose.RightPose);
        }
        void Update()
        { 
            if (trackedPoseDriver != null && lineRenderer != null)
            {

                

                Vector3 lineStart = trackedPoseDriver.transform.position;
                Vector3 lineEnd = trackedPoseDriver.transform.position + trackedPoseDriver.transform.forward * 10f;


                lineRenderer.SetPosition(0, lineStart);
                lineRenderer.SetPosition(1, lineEnd);
            }
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray); //did not know this function existed... thanks ChatGPT!
            if (Input.GetKeyDown(Var.acceptButton))
            {


                
                foreach (RaycastHit hit in hits)
                {
                    
                    ChessBoardTileView tile = hit.collider.GetComponent<ChessBoardTileView>();

                    //stricly necessary because a user might press A even when not pointing on a tile
                    if (tile != null)
                    {
                        tile.OnInteraction(MakePlayer.photonViewID);
                    }

                }
            }
        }


        }
    }

