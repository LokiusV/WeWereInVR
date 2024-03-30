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
using WeWereHereVR;
using System.IO;
using System;

namespace WeWereHereVR
{
    //this class is just responsible for converting any canvas to world space, without spawning the LaserPointer.
    class IngameUIWorldSpace
    {
        public static void setScale(string canvasString,float amount)
        {
            GameObject canvaObject = GameObject.Find(canvasString);
            //canvaObject.transform.localScale = new Vector3(0.001f, 0.001f, 0.001f);
            canvaObject.transform.localScale = new Vector3(amount, amount, amount);
            canvaObject.transform.position = Camera.main.transform.position + new Vector3(0, 0, 2f);
            Canvas canva = canvaObject.GetComponent<Canvas>();
            canva.renderMode = UnityEngine.RenderMode.WorldSpace;

        }

    }
}