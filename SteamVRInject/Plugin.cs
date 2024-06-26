﻿

using BepInEx;
using UnityEngine;
using UnityEngine.Video;
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
using System.IO;
using System;
using WeWereHereVR;
using Valve.VR.Extras;
using Valve.VR.InteractionSystem;


[HarmonyPatch(typeof(MainMenuController), "Start")]
public class MainMenuPatch
{
    [HarmonyPrefix]
    public static void Postfix(LoadingController __instance)//attaching to the MainMenuController sometimes works and sometimes doesn't...wtf? LoadingController always works though
    {
        Debug.Log("MainMenuController patched!");
        
        CreateMenuController("Canvas");
        float horizontalThumbstick = Input.GetAxis("joy_0_axis_0");
        float verticalThumbstick = Input.GetAxis("joy_0_axis_1");
        verticalThumbstick = -verticalThumbstick;
        Debug.Log(horizontalThumbstick);



    }




    public static void CreateMenuController(string canvasName)
    {
        
            GameObject emptyObject = new GameObject("LaserPointer");

            LineRenderer lineRenderer = emptyObject.AddComponent<LineRenderer>();
            GraphicRaycaster raycaster = emptyObject.AddComponent<GraphicRaycaster>();
            lineRenderer.useWorldSpace = true;
            lineRenderer.startWidth = 0.01f; 
            lineRenderer.endWidth = 0.01f;   
            lineRenderer.positionCount = 2;

            //removed because I don't have the willpower to mess with layers right now
            //Material newMaterial = new Material(Shader.Find("Sprites/Default"));
            //newMaterial.SetFloat("_Mode", 1);

            //newMaterial.color = Color.white; 

        
            //lineRenderer.material = newMaterial;
            



            TrackedPoseDriver trackedPoseDriver = emptyObject.AddComponent<TrackedPoseDriver>();
            trackedPoseDriver.SetPoseSource(TrackedPoseDriver.DeviceType.GenericXRController, TrackedPoseDriver.TrackedPose.RightPose);
            SPC spcScript = emptyObject.AddComponent<SPC>();
            spcScript.Initialize(lineRenderer, trackedPoseDriver, raycaster,canvasName);
    }
}
[HarmonyPatch(typeof(InGameOptionsController), "OnVideoPlay")]
public class IngameUIPatch
{
    
    [HarmonyPrefix]
    public static void Postfix(InGameOptionsController __instance, object[] arg0)
    {
        Debug.Log("Video Start");




        MainMenuPatch.CreateMenuController("IngameUI");
        GameObject laserPointer = GameObject.Find("LaserPointer");
        GameObject lp = new GameObject("lp");
        lp.transform.position = Camera.main.transform.parent.position;
        lp.transform.parent = Camera.main.transform.parent;

        lp.transform.localPosition = new Vector3(0.0046f, 1.7f, -0.2056f);
        laserPointer.transform.parent = lp.transform;
        //laserPointer.transform.parent = Camera.main.transform.parent;
        Canvas canvas = GameObject.Find("IngameUI").GetComponent<Canvas>();
        canvas.gameObject.transform.position = Camera.main.transform.position+new Vector3(0,2,2);
        
        GameObject rawImageObject = new GameObject("VideoRawImage");
        rawImageObject.transform.SetParent(canvas.transform);
        RawImage rawImage = rawImageObject.AddComponent<RawImage>();
        RectTransform canvasRectTransform = canvas.GetComponent<RectTransform>();

        RectTransform rectTransform = rawImageObject.GetComponent<RectTransform>();
        rectTransform.sizeDelta = canvasRectTransform.sizeDelta;
        rectTransform.localPosition = new Vector3(0f, 2f, 5f);
        rectTransform.localScale = new Vector3(1, 1, 1);
        rectTransform.SetAsFirstSibling();




        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            VideoPlayer videoPlayer = mainCamera.GetComponent<VideoPlayer>();
            Debug.Log(videoPlayer);
            //videoPlayer.targetCamera = uiCamera;//3
            if (videoPlayer != null)
            {
                
                RenderTexture renderTexture = new RenderTexture(1920, 1080, 24);
                videoPlayer.targetTexture = renderTexture;
                videoPlayer.renderMode = VideoRenderMode.RenderTexture;

                
                rawImage.texture = renderTexture;

                
            }
            
        }
        

    }

}
[HarmonyPatch(typeof(OptionController), "Start")]
public class GameOptionsPatch
{
    [HarmonyPostfix]
    public static void Postfix(OptionController __instance)
    {
        Debug.Log("attached");
        MainMenuPatch.CreateMenuController("Canvas");

    }
}
[HarmonyPatch(typeof(MatchmakingController), "Start")]
public class LobbyPatch:MonoBehaviour
{
    [HarmonyPostfix]
    public static void Postfix(MatchmakingController __instance)
    {
        string pluginFolderPath = Paths.PluginPath;


        string assetBundlePath = Path.Combine(pluginFolderPath, "AssetBundle/keyboard");


        AssetBundle assetBundle = AssetBundle.LoadFromFile(assetBundlePath);




        GameObject keyboardPrefab = assetBundle.LoadAsset<GameObject>("kbd.prefab");
        GameObject keyboard = Instantiate(keyboardPrefab);
        keyboard.name = "Keyboard";
        keyboard.transform.position = new Vector3(0, 0, 0);
        keyboard.AddComponent<KeyboardController>();
        assetBundle.Unload(false);
        MainMenuPatch.CreateMenuController("LobbyPrefab");

    }
}
[HarmonyPatch(typeof(BarometerController), "Start")]
public class BarometerVar
{
    public static BarometerController barometer;
    [HarmonyPostfix]
    public static void Postfix(BarometerController __instance)
    {
        barometer = __instance;

    }
}



[HarmonyPatch(typeof(PlayerMovement), "DetectMovement")]
public class MovementPatch
{
    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        //get rid of all dis shit, no clue why I didn't just use a prefix though
        return new List<CodeInstruction>();
    }

    [HarmonyPostfix]
    public static void Postfix(PlayerMovement __instance)
    {
        
        Quaternion hmdRotation = Camera.main.transform.rotation;

        
        float horizontalThumbstick = Input.GetAxis("joy_0_axis_0");
        float verticalThumbstick = Input.GetAxis("joy_0_axis_1");
        verticalThumbstick = -verticalThumbstick;
        
        
        Vector3 thumbstickDirection = hmdRotation * new Vector3(horizontalThumbstick, 0f, verticalThumbstick);

        
        FieldInfo directionField = AccessTools.Field(typeof(PlayerMovement), "direction");//direction is private so I have to get it using AccessTools.Field
        if (directionField != null)
        {
            directionField.SetValue(__instance, thumbstickDirection.normalized);
        }
    }
}



[HarmonyPatch(typeof(MatchmakingController))]
[HarmonyPatch("RoomCreationStatus")]
class RoomCreationStatusPatch:MonoBehaviour
{
    static void Postfix(object[] value)
    {
        
        bool roomCreationStatus = (bool)value[0];

        
        MatchmakingController matchmakingController = FindObjectOfType<MatchmakingController>();
        if (matchmakingController != null)
        {
            FieldInfo lobbyPanelField = AccessTools.Field(typeof(MatchmakingController), "lobbyPanel");
            GameObject lobbyPanel = (GameObject)lobbyPanelField.GetValue(matchmakingController);

            if (lobbyPanel != null)
            {
                
                if (roomCreationStatus)
                {
                    lobbyPanel.SetActive(value: false);
                }
            }
            else
            {
                Debug.LogError("lobbyPanel is null.");//necessary because when brute forcing a load to this scene, this sometimes happens
            }
        }
        else
        {
            Debug.LogError("MatchmakingController instance not found.");//necessary because when brute forcing a load to this scene, this sometimes happens
        }
    }
}
[HarmonyPatch(typeof(MatchmakingController))]
[HarmonyPatch("InputPasswordStatus")]
class PasswordPatch : MonoBehaviour
{
    static void Postfix(object[] value)
    {

        bool roomCreationStatus = (bool)value[0];


        MatchmakingController matchmakingController = FindObjectOfType<MatchmakingController>();
        if (matchmakingController != null)
        {
            
            Button[] buttons = GameObject.Find("ServerBrowser").GetComponentsInChildren<Button>();
            foreach (Button button in buttons)
            {
                button.gameObject.GetComponent<BoxCollider>().enabled = false;
            }
        }
        else
        {
            Debug.LogError(".");//necessary because when brute forcing a load to this scene, this sometimes happens
        }
    }
}
[HarmonyPatch(typeof(PlayerMovement), "Start")]
public class MousePatch:MonoBehaviour
{

    [HarmonyPostfix]
    static void Postfix(PlayerMovement __instance)
    {
        MouseControls mouseControls = __instance.gameObject.GetComponent<MouseControls>();//accessing the MouseControls component on the player
        Destroy(mouseControls);//And fucking nuking this little shit into oblivion
        ObjectCloner.CloneObject("walkieTalkieExplorer","VRalkieTalkie");//since the walkie talkie has to be picked up to start the first puzzle, we'll have to clone it to attach it to the left controller
        GameObject clonedObject = GameObject.Find("VRalkieTalkie");
        MakePlayer.characterControllerInstance.center = new Vector3(0, 0, 0);
        







    }
}
[HarmonyPatch(typeof(BarometerView), "Start")]
public class BarometerPatch : MonoBehaviour
{
    [HarmonyPostfix]
    static void Postfix(BarometerView __instance)
    {
        Var.barometerView = __instance;
    }
}

[HarmonyPatch(typeof(BarometerView), "OnBarometerSolved")]
public class BarometerEndPatch : MonoBehaviour
{
    [HarmonyPostfix]
    static void Postfix(BarometerView __instance, object[] arg0)
    {
        Var.barometerSolved = true;
        Var.barometerActive = false;
    }
}



[HarmonyPatch(typeof(PlayerMovement), "TogglePlayerControls")]
public class MovementTogglePatch
{
    [HarmonyPostfix]
    static void Postfix(PlayerMovement __instance)
    {
        
        GameObject laserPointer = null;
        Canvas canva = null;
        try
        {
            GameObject canvaObject = GameObject.Find("IngameUI");
            Debug.Log("Found");
            canva = canvaObject.GetComponent<Canvas>();
            GameObject rawImageObject = GameObject.Find("VideoRawImage");
            rawImageObject.SetActive(false);
            laserPointer = GameObject.Find("lp");
            laserPointer.SetActive(false);
            GameObject.Destroy(laserPointer);
        }
        catch(Exception e)
        {
            Debug.Log(e.ToString());
            
        }
        try
        {
            laserPointer = GameObject.Find("LaserPointer");
            laserPointer.SetActive(false);
            GameObject.Destroy(laserPointer);
            canva.renderMode = UnityEngine.RenderMode.ScreenSpaceOverlay;
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
        
        MakePlayer.SetHeight(__instance);
    }
}

[HarmonyPatch(typeof(SpikePuzzleEndTriggerView),"OnTriggerEnter")]
public class ActivateTheater
{
    [HarmonyPostfix]
    static void Postfix(SpikePuzzleEndTriggerView __instance)
    {
        FieldInfo boxColliderField = AccessTools.Field(typeof(TheaterControlPanelView), "boxCollider");
        BoxCollider boxCollider = (BoxCollider)boxColliderField.GetValue(GameObject.FindObjectOfType<TheaterControlPanelView>());
        FieldInfo isInspectingField = AccessTools.Field(typeof(TheaterControlPanelView), "playerIsInspecting");
        bool playerIsInspecting = (bool)isInspectingField.GetValue(GameObject.FindObjectOfType<TheaterControlPanelView>());
        FieldInfo controllerFocusIndexField = AccessTools.Field(typeof(TheaterControlPanelView), "controllerFocusIndex");
        int controllerFocusIndex = (int)controllerFocusIndexField.GetValue(GameObject.FindObjectOfType<TheaterControlPanelView>());
        EventManager.TriggerEvent("optionsIsPlayerMinigame", true);
        EventManager.TriggerEvent("THEATER_PUZZLE_CONTROL_PANEL_INSPECTED", true);
        GameObject.FindObjectOfType<TheaterControlPanelView>().handleDirections[controllerFocusIndex].ToggleHighlight(enable: true);
        boxCollider.enabled = false;
        playerIsInspecting = true;
        //Var.isInspecting = true;
        

    }
}



[HarmonyPatch(typeof(PlayerMovement), "UpdateCharacterControllerHeight")]
public class CharacterCenterPatch
{
    [HarmonyPrefix]
    static bool Prefix(PlayerMovement __instance, float height)
    {
        FieldInfo characterControllerField = AccessTools.Field(typeof(PlayerMovement), "characterController");
        CharacterController characterController = (CharacterController)characterControllerField.GetValue(__instance);
        characterController.height = height;
        return false;
    }
}

[HarmonyPatch(typeof(GameOverSceneController), "Start")]
public class GameOverPatch:MonoBehaviour
{
    [HarmonyPostfix]
    static void Postfix(GameOverSceneController __instance)
    {
        Destroy(GameObject.Find("Main Camera"));
        GameObject eventSystemGameObject = GameObject.Find("EventSystem");
        Camera camera = eventSystemGameObject.AddComponent<Camera>();
        camera.tag = "MainCamera";
        MainMenuPatch.CreateMenuController("Canvas");


    }
}
[HarmonyPatch(typeof(InfoScreenController), "Start")]
public class InfoScreenPatch : MonoBehaviour
{
    [HarmonyPostfix]
    static void Postfix(InfoScreenController __instance)
    {
        //IngameUIWorldSpace.setScale("Canvas",0.010f);
        IngameUIWorldSpace.setScale("Canvas", 0.001f);

    }
}
[HarmonyPatch(typeof(OptionController), "Start")]
public class OptionPatch : MonoBehaviour
{
    [HarmonyPostfix]
    static void Postfix(OptionController __instance)
    {
        Haptics.TriggerHapticFeedback(XRNode.RightHand, 1f);
        //disabling unneccesarry panels
        GameObject.Find("GameOptionsButton").SetActive(false);
        GameObject.Find("KeybindingsButton").SetActive(false);
        Var.graphicsButton = GameObject.Find("GraphicsOptionsButton");
        Var.audioButton = GameObject.Find("AudioOptionsButton");
        Var.mainMenuButton = GameObject.Find("MainMenuButton");
        Var.leaveButton = GameObject.Find("LeaveGameButton");
        

    }
}
[HarmonyPatch(typeof(OptionController), "GraphicsOptions")]
public class GraphicsOptionsPatch : MonoBehaviour
{
    [HarmonyPostfix]
    static void Postfix(OptionController __instance)
    {
        Var.audioButton.SetActive(false);
        Var.graphicsButton.SetActive(false);
        Var.mainMenuButton.SetActive(false);
        Var.leaveButton.SetActive(false);


    }
}
[HarmonyPatch(typeof(OptionController), "AudioOptions")]
public class AudioOptionsPatch : MonoBehaviour
{
    [HarmonyPostfix]
    static void Postfix(OptionController __instance)
    {
        Var.audioButton.SetActive(false);
        Var.graphicsButton.SetActive(false);
        Var.mainMenuButton.SetActive(false);
        try
        {
            Var.leaveButton.SetActive(false);
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
        Slider[] sliders = FindObjectsOfType<Slider>();
        foreach (Slider slider in sliders)
        {
            if (slider.gameObject.GetComponent<BoxCollider>() == null && slider.gameObject.activeSelf)
            {

                RectTransform buttonRectTransform = slider.GetComponent<RectTransform>();


                Vector2 buttonSize = buttonRectTransform.sizeDelta;


                BoxCollider boxCollider = slider.gameObject.AddComponent<BoxCollider>();


                boxCollider.size = new Vector3(buttonSize.x, buttonSize.y, 1f);
            }
            else if (slider.gameObject.GetComponent<BoxCollider>() != null && slider.gameObject.activeSelf == false)
            {
                slider.gameObject.GetComponent<BoxCollider>().enabled = false;
            }
        }



    }
}
[HarmonyPatch(typeof(OptionController), "ClosePanel")]
public class ClosePanelPatch : MonoBehaviour
{
    [HarmonyPostfix]
    static void Postfix(OptionController __instance)
    {
        Var.audioButton.SetActive(true);
        Var.graphicsButton.SetActive(true);
        Var.mainMenuButton.SetActive(true);
        try
        {
            Var.leaveButton.SetActive(true);
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }

        Var.slider = null;
        


    }
}

[HarmonyPatch(typeof(PlayerMovement), "FixedUpdate")]
public class UpdatePatch
{
    [HarmonyPrefix]
    static void Prefix(PlayerMovement __instance)
    {
        if (Input.GetKeyDown(Var.jumpButton))
        {
            FieldInfo velocityField = AccessTools.Field(typeof(PlayerMovement), "verticalVelocity");
            FieldInfo characterControllerField = AccessTools.Field(typeof(PlayerMovement), "characterController");
            CharacterController characterController = (CharacterController)characterControllerField.GetValue(__instance);
            if (characterController.isGrounded)
            {
                velocityField.SetValue(__instance, __instance.jumpSpeed);
            }



        }
        



        if (MakePlayer.done2)
        {
            MakePlayer.characterControllerInstance.center = new Vector3(Camera.main.transform.localPosition.x, MakePlayer.characterControllerInstance.center.y, Camera.main.transform.localPosition.z);
            HandPose.UpdateHeldKeys();
            HandPose.CheckKeyCombinations();
            SnapTurnProvider.Snap();
        }
        else
        {
            MakePlayer.characterControllerInstance.center = new Vector3(0, 0, 0);

        }
        



    }
}
[HarmonyPatch(typeof(BarometerController), "ToggleMiniGame")]
public class BarometerControllerPatch
{
    
    public static bool Prefix(BarometerController __instance)
    {
        
        FieldInfo minigameIsActiveField = typeof(BarometerController).GetField("minigameIsActive", BindingFlags.NonPublic | BindingFlags.Instance);
        bool minigameIsActive = (bool)minigameIsActiveField.GetValue(__instance);

        
        minigameIsActive = !minigameIsActive;

        if (minigameIsActive)
        {

            EventManager.TriggerEvent("CHESS_PUZZLE_GENERATOR_BAROMETER_TOGGLE", true);
            EventManager.TriggerEvent("playerTogglePlayerMovement", false);
            EventManager.TriggerEvent("optionsIsPlayerMinigame", true);
        }
        else
        {
            EventManager.TriggerEvent("CHESS_PUZZLE_GENERATOR_BAROMETER_TOGGLE", false);

            EventManager.TriggerEvent("playerTogglePlayerMovement", true);
            EventManager.TriggerEvent("optionsIsPlayerMinigame", false);
            
            __instance.Invoke("EnableMenu", 0.3f);
        }

        
        minigameIsActiveField.SetValue(__instance, minigameIsActive);

        
        

        
        return false;
    }
}

[HarmonyPatch(typeof(WalkieTalkie), "OnInteraction")]
public class WalkieTalkiePatch
{
    [HarmonyPostfix]
    static void Prefix(WalkieTalkie __instance)
    {
        __instance.WalkieTalkieObject.SetActive(true);




    }
}

[HarmonyPatch(typeof(EyeStartingSlabView), "OnTriggerEnter")]
public class EyePatch//lmao
{
    [HarmonyPrefix]
    static void Prefix(EyeStartingSlabView __instance,Collider collider)
    {
        MethodInfo walkieTalkiePickedUpMethod = typeof(EyeStartingSlabView).GetMethod("WalkieTalkiePickedUp", BindingFlags.NonPublic | BindingFlags.Instance);
        //FieldInfo pickedUpField = typeof(bool).GetField("walkieTalkiePickup", BindingFlags.NonPublic | BindingFlags.Instance);
        //pickedUpField.SetValue(__instance, true);
        ParameterInfo[] parametersInfo = walkieTalkiePickedUpMethod.GetParameters();

        
        object[] parameters = new object[parametersInfo.Length];

        
        walkieTalkiePickedUpMethod.Invoke(__instance, parameters);



    }
}
[HarmonyPatch(typeof(WalkieTalkie), "OnInteractionRPC")]
public class WalkieTalkieDestroyPatch
{
    static bool Prefix(WalkieTalkie __instance, int playerViewId) 
    {

        FieldInfo pickedUpField = typeof(BasePickup).GetField("pickedUp", BindingFlags.NonPublic | BindingFlags.Instance);
        bool pickedUp = (bool)pickedUpField.GetValue(__instance);
        if (!pickedUp)
        {
            PhotonView.Find(playerViewId).gameObject.GetComponent<PlayerController>().PickupLeftHandItem(GameObject.Find("walkieTalkieLibrarian").GetComponent<WalkieTalkie>()); // Replace YourCustomPickupLeftHandItem with your custom method
            __instance.interactionDistance = 0f;
            return false;
        }
        return false;
    }

}

[BepInPlugin("com.Lokius.WeWereInVR", "WeWereInVR", "1.0.0")]
public class SteamVRSetup : BaseUnityPlugin
{
    void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Screen.SetResolution(1920, 1080, true);
        string filePath = Path.Combine(Paths.PluginPath, "Settings.txt");
        float targetTimeStep;
        try //to read the Settings.txt file
        {
            string fileContent= FileReader.ReadFromFile(filePath);
            string[] lines = fileContent.Split('\n');
            Debug.Log("Line1:" + lines[0].Trim());
            Debug.Log("Line2:" + lines[1].Trim());
            float normalHeightValue = float.Parse(lines[0].Trim());
            Debug.Log(normalHeightValue);
            targetTimeStep = 1f/float.Parse(lines[1].Trim());
            Debug.Log(targetTimeStep);

        }
        catch(Exception e)//if it doesn't work, set it to emergency default values(height gets set somewhere else)
        {
            Debug.LogError("Made a fucky wucky while reading the file, oopsie! Error: " + e);
            targetTimeStep = 1f / 90f;
        }
        Time.fixedDeltaTime = targetTimeStep;
        Debug.Log(Time.fixedDeltaTime);
        Harmony harmony = new Harmony("com.Lokius.WeWereInVRPlugin");
        harmony.PatchAll();



        //MainMenuPatch.CreateMenuController("Canvas");
        Var.Initialize();
        IngameUIWorldSpace.setScale("Canvas", 0.001f);
        Var.started = true;
        
        


    }
    


}








