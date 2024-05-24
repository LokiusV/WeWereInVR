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

            laserObject.SetActive(false);
        }
        public void Update()
        {
            
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

