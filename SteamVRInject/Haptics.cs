using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.XR;
using System.IO;
using System;
using UnityEngine;

namespace WeWereHereVR
{
    public class Haptics
    {
        public static void TriggerHapticFeedback(XRNode hand, float length)
        {

            
            InputDevice device = InputDevices.GetDeviceAtXRNode(hand);
            
            //HapticCapabilities capabilities;
            try
            {
                device.SendHapticImpulse(0, 0.5f, length);
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }

        }
    }
}
