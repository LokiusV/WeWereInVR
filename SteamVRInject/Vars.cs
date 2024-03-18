using UnityEngine;
using System.Collections;
using BepInEx;
using System.IO;
using WeWereHereVR;
using System;

namespace WeWereHereVR
{
    //This class just stores alot of (semi-)important static/global variables
    //I created this class to make things a bit more organized
    public class Var : MonoBehaviour
    {
        public static bool pickup = false;
        public static GameObject pickupGameObject;
        public static GameObject rightHand;
        //should have chosen a better temporary name... I don't remember what it does anymore....
        public static bool cocd;

        public static bool barometerActive;
        public static BarometerView barometerView;
        public static bool barometerSolved=false;
        public static int leftRight;
        public static bool isInspecting=false;
        public static GameObject inspectedObject;

        //Default Bindings
        public static KeyCode gripButton=KeyCode.Joystick2Button5;
        public static KeyCode indexButton = KeyCode.Joystick2Button15;
        public static KeyCode acceptButton = KeyCode.JoystickButton1;
        public static KeyCode jumpButton = KeyCode.JoystickButton9;
        public static KeyCode cancelButton = KeyCode.JoystickButton0;

        public static Camera mainCamera;
        public static Camera uiCamera;

        //Sets up the correct bindings
        public static void Initialize()
        {
            Debug.Log("Reading Controller Settings");
            string filePath = Path.Combine(Paths.PluginPath, "Settings.txt");
            
            try //to read the Settings.txt file
            {
                string fileContent = FileReader.ReadFromFile(filePath);
                string[] lines = fileContent.Split('\n');
                Debug.Log(lines[2].Trim());
                //Set the bindings to the default Oculus Touch bindings
                if (lines[2].Trim() == "Oculus/Meta")
                {
                    gripButton = KeyCode.Joystick2Button5;
                    indexButton = KeyCode.Joystick2Button15;
                    acceptButton = KeyCode.JoystickButton1;
                    jumpButton = KeyCode.JoystickButton9;
                    cancelButton = KeyCode.JoystickButton0;
                    Debug.Log("Set bindings.");

                }
                //Set the bindings to the default HTC Vive Wand bindings
                else if (lines[2].Trim() == "Vive Wand")
                {
                    gripButton = KeyCode.Joystick2Button5;
                    indexButton = KeyCode.Joystick2Button15;
                    acceptButton = KeyCode.Joystick2Button15;
                    jumpButton = KeyCode.JoystickButton17;
                    cancelButton = KeyCode.Joystick2Button5;
                    Debug.Log("Set bindings for HTC Vive Wands or Pimax Sword Controllers");

                }
                //Set the keybindings to the custom bindings specifiec in Bindings.txt
                else if (lines[2].Trim() == "Custom")
                {
                    string filePath2 = Path.Combine(Paths.PluginPath, "Bindings.txt");
                    try //to read the Bindings.txt file
                    {
                        string fileContent2 = FileReader.ReadFromFile(filePath2);
                        string[] lines2 = fileContent2.Split('\n');
                        gripButton=(KeyCode)Enum.Parse(typeof(KeyCode), lines2[0].Trim());
                        indexButton = (KeyCode)Enum.Parse(typeof(KeyCode), lines2[1].Trim());
                        acceptButton = (KeyCode)Enum.Parse(typeof(KeyCode), lines2[2].Trim());
                        cancelButton = (KeyCode)Enum.Parse(typeof(KeyCode), lines2[3].Trim());
                        jumpButton = (KeyCode)Enum.Parse(typeof(KeyCode), lines2[4].Trim());
                    }
                    catch (Exception e)//if it doesn't work, set it to emergency default values(height gets set somewhere else)
                    {
                        return;
                    }

                }

            }
            catch (Exception e)//if it doesn't work, set it to emergency default values(height gets set somewhere else)
            {
                return;
            }
        }

    }

}