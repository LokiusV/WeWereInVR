using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SpatialTracking;
using UnityEngine.UI;

namespace WeWereHereVR
{
    public class KeyboardController : MonoBehaviour
    {
        public InputField inputField;
        // Start is called before the first frame update
        void Start()
        {
            GameObject keyboard = GameObject.Find("Keyboard");
            TrackedPoseDriver trackedPoseDriver = keyboard.AddComponent<TrackedPoseDriver>();
            trackedPoseDriver.SetPoseSource(TrackedPoseDriver.DeviceType.GenericXRController, TrackedPoseDriver.TrackedPose.LeftPose);
            trackedPoseDriver.trackingType = TrackedPoseDriver.TrackingType.PositionOnly;
            Button[] buttons = keyboard.GetComponentsInChildren<Button>();

            
            foreach (Button button in buttons)
            {
                
                button.onClick.AddListener(() =>
                {
                    
                    keyboard.GetComponent<KeyboardController>().Add(button.name);
                });

            }

        }

        // Update is called once per frame
        
        public void Add(string x)
        {
            if (inputField != null)
            {
                inputField.text += x.ToLower();
            }

        }
        public void Sub()
        {
            if (inputField != null && inputField.text.Length > 0)
            {
                inputField.text = inputField.text.Substring(0, inputField.text.Length - 1);
            }

        }
    }
}
