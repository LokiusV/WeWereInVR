using UnityEngine;
using System.Collections;
using BepInEx;
using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine.SpatialTracking;
using WeWereHereVR;

namespace WeWereHereVR
{
    //this class just creates the right hand. It imports assets, creates new gameObjects, etc.
    public class HandCreator : MonoBehaviour
    {
        public static GameObject closedHandInstance;
        public static GameObject indexHandInstance;
        public static GameObject openHandInstance;

        public void CreateHand()
        {
            string pluginFolderPath = Paths.PluginPath;

            
            string assetBundlePath = Path.Combine(pluginFolderPath, "AssetBundle/hands");

            
            AssetBundle assetBundle = AssetBundle.LoadFromFile(assetBundlePath);

            

            
            GameObject closedHandPrefab = assetBundle.LoadAsset<GameObject>("vr_glove_right_model_slim_closed.prefab");
            GameObject indexHandPrefab = assetBundle.LoadAsset<GameObject>("vr_glove_right_model_slim _Index3.prefab");
            GameObject openHandPrefab = assetBundle.LoadAsset<GameObject>("vr_glove_right_model_slim_normal.prefab");



            closedHandInstance= Instantiate(closedHandPrefab);
            indexHandInstance = Instantiate(indexHandPrefab);
            openHandInstance = Instantiate(openHandPrefab);
            closedHandInstance.transform.position = new Vector3(0, 0, 0);
            openHandInstance.transform.position = new Vector3(0, 0, 0);
            indexHandInstance.transform.position = new Vector3(0, 0, 0);

            GameObject rightHandObject = new GameObject("Right Controller");
            rightHandObject.SetActive(true);
            rightHandObject.transform.position = new Vector3(0, 0, 0);
            Debug.Log("Right Controller Position: " + rightHandObject.transform.position);
            GameObject controllerOffset = new GameObject("Controller Offset");
            controllerOffset.transform.SetParent(rightHandObject.transform);

            closedHandInstance.transform.SetParent(controllerOffset.transform);
            openHandInstance.transform.SetParent(controllerOffset.transform);
            indexHandInstance.transform.SetParent(controllerOffset.transform);
            //controllerOffset.transform.localPosition = new Vector3(-0.02059328f, 0.02804078f, -0.1015848f);
            controllerOffset.transform.localPosition = new Vector3(0f, 0f, -0.1015848f);
            controllerOffset.transform.localRotation = Quaternion.Euler(38.802f, 2.465f, 11.122f);//Makes the Controller orientation alot more natur on quest controllers

            TrackedPoseDriver trackedPoseDriver = rightHandObject.AddComponent<TrackedPoseDriver>();
            trackedPoseDriver.trackingType = TrackedPoseDriver.TrackingType.RotationAndPosition;
            trackedPoseDriver.SetPoseSource(TrackedPoseDriver.DeviceType.GenericXRController, TrackedPoseDriver.TrackedPose.RightPose);
            SphereCollider sphereCollider = controllerOffset.AddComponent<SphereCollider>();
            sphereCollider.radius = 0.35f;
            sphereCollider.isTrigger = true;

            HandPose.Initialize(indexHandInstance, openHandInstance, closedHandInstance);

            assetBundle.Unload(false);

        }

        
    }
}
