#if (UNITY_ANDROID  || UNITY_IPHONE) && !UNITY_EDITOR && !UNITY_STANDALONE_WIN && !UNITY_STANDALONE_MAC && !UNITY_WEBPLAYER
#define ON_DEVICE
#endif

using UnityEngine;
using System.Collections;

public class SampleGUI : MonoBehaviour
{
	public string instructionText = "";
	public const float instructionTextWidth = 400f;
	public const float sceneBarWidth = 185f;
	public const float guiBarWidth = 325f;
	public const float topBarHeight = 50f;
	public string sampleTitleName = "";
	
	public Texture2D topBarTexture;
	public Texture2D settingsBarTexture;
	public Texture2D menuButtonArrowTexture;
	public Texture2D menuButtonTexture;
	public Texture2D menuButtonDownTexture;
	public Texture2D buttonTexture;
	public Texture2D buttonDownTexture;
	public Texture2D clearButtonTexture;
	public Texture2D clearButtonDownTexture;
	public Texture2D navigateButtonTexture;
	public Texture2D navigateButtonDownTexture;
	public Texture2D instructionBackgroundBeginTexture;
	public Texture2D instructionBackgroundTexture;
	public Texture2D instructionBackgroundEndTexture;
	
	public Texture2D comboBoxListTop;
	public Texture2D comboBoxListRepeater;
	public Texture2D comboBoxListBottom;
	public Texture2D comboBoxListPointer;
	
	public Texture2D settingsTextTexture;
	public Texture2D resultsTextTexture;
	public Texture2D checkboxOffTexture;
	public Texture2D checkboxOnTexture;
	public Texture2D textBoxTexture;

	public Color titleColor = Color.red;
	public GUIStyle titleStyle;
	public GUIStyle buttonStyle;
	public GUIStyle navigateButtonStyle;
	public GUIStyle navigateThisButtonStyle;
	public GUIStyle statusStyle;
	public GUIStyle settingsTextStyle;
	public GUIStyle resultsTextStyle;
	public GUIStyle instructionStyle;
	public GUIStyle comboBoxButtonStyle;
	public GUIStyle comboBoxListButtonStyle;
	
	//public bool showStatusText = true;

	private Rect topBarRect;
	private Rect topBarTextRect;
	private Rect backArrowButtonRect;
#if ON_DEVICE
	private Rect exitButtonRect;
#endif
	public Rect settingsBarRect;
//	private Rect errorTextRect;
//	private Rect statusTextRect;
//	private Rect status2TextRect;
	private Rect instructionTextRect;
	private Rect instructionBackgroundBeginRect;
	private Rect instructionBackgroundRect;
	private Rect instructionBackgroundEndRect;
	
	private GUIStyle topBarStyle;
	private GUIContent topBarContent;
	private GUIStyle menuButtonStyle;
	private GUIStyle exitButtonStyle;
	public GUIStyle clearButtonStyle;
	public GUIStyle editTextStyle;
	
	public static bool sceneSelectionOpen = true;
	public static bool aSceneHasBeenPicked = false;
	public static bool sceneIsChanging = false;
	public static bool doMenuButtons = true;
	
	private Rect sceneClosedButtonRect;
	private Rect sceneBarRect;
	private Rect[] sceneButtonRects;
	private static string[] buttonNames;
	private static string[] sceneNames;

	static string statusText = "";
	static string statusTextObj = "";
	static string errorText = "";
	static float clearErrorTextTime = 0;
	

	
	private static SampleGUI _factory;
	
	public static void clearStatus()
	{
		statusText = "";
		statusTextObj = "";
	}
	
	public static string ErrorText
    {
        get  { return errorText; }
        set  {
			clearErrorTextTime = Time.time + 5f;
			errorText = value;
		}
    }

    public static string StatusText
    {
        get  { return statusText; }
        set  {			
			statusText = value;
		}
    }
    public static string StatusTextObj
    {
        get { return statusTextObj; }
        set {
			statusTextObj = value;
		}
    }
	
	
	void Start()
	{
		_factory = this;
		if (aSceneHasBeenPicked) {
			sceneSelectionOpen = false;
		}
	}
	
	public static SampleGUI Factory()
	{
		return _factory;
	}
	
	public static int GetSceneNumber()
	{
		SetNames();
		for (int i = 0; i < sceneNames.Length; i++) {
			if (Application.loadedLevelName == sceneNames[i]) {
				return i;
			}
		}
		return 0;
	}

	
	protected static  void SetNames()
	{
		if (buttonNames != null) {
			return;
		}
		
		buttonNames = new string[9];
		sceneNames = new string[9];
		
		buttonNames[0] = "Dragging";
		sceneNames[0] = "DragGestures";
		buttonNames[1] = "Line Swipe";
		sceneNames[1] = "LineGestures";
		buttonNames[2] = "Long Press";
		sceneNames[2] = "LongPressGestures";
//		buttonNames[3] = "Motion";
//		sceneNames[3] = "MotionGestures";
		buttonNames[3] = "Pinch";
		sceneNames[3] = "PinchGestures";
		buttonNames[4] = "Rotation";
		sceneNames[4] = "RotateGestures";
		buttonNames[5] = "Slice";
		sceneNames[5] = "SliceGestures";
		buttonNames[6] = "Swipe";
		sceneNames[6] = "SwipeGestures";
		buttonNames[7] = "Tapping";
		sceneNames[7] = "TapGestures";
		buttonNames[8] = "Touch";
		sceneNames[8] = "TouchGestures";
	}

	void Awake ()
	{
		statusText = "";
		
		SetNames();
								
		topBarRect = new Rect (0f, 0f, Screen.width, topBarHeight);
		topBarTextRect = new Rect(topBarRect.x, topBarRect.y, topBarRect.width, topBarRect.height);
#if ON_DEVICE
		exitButtonRect = new Rect (topBarRect.xMax - 70f, 8f, 60f, 30f);
#endif		
//		errorTextRect = new Rect (instructionTextRect.x, instructionTextRect.yMax + 5, instructionTextRect.width, instructionTextRect.height);
//		statusTextRect = new Rect (instructionTextRect.x, Screen.height - 50f, instructionTextRect.width, instructionTextRect.height);
//		status2TextRect = new Rect (instructionTextRect.x, statusTextRect.yMax + 5f, instructionTextRect.width, instructionTextRect.height);
		
		sceneClosedButtonRect = new Rect(0f, topBarRect.yMax, 50f, Screen.height - topBarRect.yMax);
		backArrowButtonRect = new Rect(15f, (sceneClosedButtonRect.height / 2), 26, 47);
		settingsBarRect = new Rect(0, topBarRect.yMax, guiBarWidth, sceneClosedButtonRect.height);
		
		float workingWidth = Screen.width - settingsBarRect.xMax;
		instructionTextRect = new Rect (settingsBarRect.xMax + (workingWidth / 2f) - (instructionTextWidth / 2f), Screen.height - 40f, instructionTextWidth, 20f);
		instructionBackgroundBeginRect = new Rect (instructionTextRect.x - 15f, instructionTextRect.y - 5f, 5f, 30);
		instructionBackgroundRect = new Rect (instructionBackgroundBeginRect.xMax, instructionBackgroundBeginRect.y, instructionTextWidth + 20f, instructionBackgroundBeginRect.height);
		instructionBackgroundEndRect = new Rect (instructionBackgroundRect.xMax, instructionBackgroundBeginRect.y, 5f, instructionBackgroundBeginRect.height);
		
		sceneButtonRects = new Rect[9];
		sceneBarRect = new Rect(sceneClosedButtonRect.x, sceneClosedButtonRect.y, sceneBarWidth, sceneClosedButtonRect.height);
		sceneButtonRects[0] = new Rect(sceneClosedButtonRect.x, sceneClosedButtonRect.y, sceneBarRect.width, sceneBarRect.height / sceneButtonRects.Length);
		for (int i = 1; i < sceneButtonRects.Length; i++) {
			sceneButtonRects[i] = new Rect(sceneButtonRects[0].x, sceneButtonRects[i - 1].yMax, sceneButtonRects[0].width, sceneButtonRects[0].height);
		}
				
	    menuButtonStyle = new GUIStyle();
        GUIStyleState styleState = new GUIStyleState();
		styleState.textColor = menuButtonStyle.normal.textColor;
        styleState.background = (Texture2D) menuButtonDownTexture;
        GUIStyleState styleStateNormal = new GUIStyleState();
		styleStateNormal.textColor = menuButtonStyle.normal.textColor;
        styleStateNormal.background = (Texture2D) menuButtonTexture;
        menuButtonStyle.normal = styleStateNormal;
        menuButtonStyle.active = styleState;
        menuButtonStyle.hover = styleState;
		
        styleState = new GUIStyleState();
 		styleState.textColor = buttonStyle.normal.textColor;
        styleState.background = (Texture2D) buttonDownTexture;
        styleStateNormal = new GUIStyleState();
 		styleStateNormal.textColor = buttonStyle.normal.textColor;
      	styleStateNormal.background = (Texture2D) buttonTexture;
//        buttonStyle.normal = styleStateNormal;
//        buttonStyle.active = styleState;
//        buttonStyle.hover = styleState;
		
		
        styleState = new GUIStyleState();
        styleState.background = (Texture2D) clearButtonDownTexture;
        styleStateNormal = new GUIStyleState();
       	styleStateNormal.background = (Texture2D) clearButtonTexture;
        clearButtonStyle.normal = styleStateNormal;
        clearButtonStyle.active = styleState;
        clearButtonStyle.hover = styleState;
		
        styleState = new GUIStyleState();
 		styleState.textColor = navigateButtonStyle.normal.textColor;
        styleState.background = (Texture2D) navigateButtonDownTexture;
        styleStateNormal = new GUIStyleState();
 		styleStateNormal.textColor = navigateButtonStyle.normal.textColor;
      	styleStateNormal.background = (Texture2D) navigateButtonTexture;
		
        navigateButtonStyle.normal = styleStateNormal;
        navigateButtonStyle.active = styleState;
        navigateButtonStyle.hover = styleStateNormal;
		
		navigateThisButtonStyle.alignment = navigateButtonStyle.alignment;
        navigateThisButtonStyle.normal = styleState;
        navigateThisButtonStyle.active = styleState;
        navigateThisButtonStyle.hover = styleState;
		
		sceneIsChanging = false;
	}
	
	public void PerformGUI ()
	{		
		if (Splash.splashUp) {
			return;
		}
		GUI.DrawTexture(topBarRect, topBarTexture);
		if (aSceneHasBeenPicked) {
			GUI.Label (topBarTextRect, "BC Gesture Library - " + sampleTitleName, titleStyle);
		}
		else {
			GUI.Label (topBarTextRect, "BC Gesture Library - by BlackCherry", titleStyle);
		}
		
#if ON_DEVICE
		if (GUI.Button (exitButtonRect, "Exit", buttonStyle)) {
			Application.Quit();
			aSceneHasBeenPicked = false;
			sceneSelectionOpen = true;
			sceneIsChanging = true;
			Application.LoadLevel("SliceGestures");
		}
#endif		
		if (sceneSelectionOpen) {
			GUI.Box (sceneBarRect, "");
			
			for (int i = 0; i < buttonNames.Length; i++) { 
				if (aSceneHasBeenPicked && Application.loadedLevelName == sceneNames[i] && !sceneIsChanging) {
					if (GUI.Button (sceneButtonRects[i], buttonNames[i], navigateThisButtonStyle)) {
						ChangeScene(sceneNames[i]);
					}
				}
				else {			
					if (GUI.Button (sceneButtonRects[i], buttonNames[i], navigateButtonStyle)) {
						ChangeScene(sceneNames[i]);
						aSceneHasBeenPicked = true;
					}
				}
			}
		}
		else {
			GUI.DrawTexture (settingsBarRect, settingsBarTexture);

			if (doMenuButtons) {
				if (GUI.Button (sceneClosedButtonRect, "", menuButtonStyle)) {
					sceneSelectionOpen = true;
				}
				GUI.DrawTexture (backArrowButtonRect, menuButtonArrowTexture);
			}
			
			GUI.DrawTexture (instructionBackgroundBeginRect, instructionBackgroundBeginTexture);
			GUI.DrawTexture (instructionBackgroundRect, instructionBackgroundTexture);
			GUI.DrawTexture (instructionBackgroundEndRect, instructionBackgroundEndTexture);
			if (aSceneHasBeenPicked) {
				GUI.Label (instructionTextRect, instructionText, instructionStyle);
			}
			else {
				GUI.Label (instructionTextRect, "Select a Gesture Type", instructionStyle);
			}
				
			
//			if (showStatusText) {				
//				GUI.Label (errorTextRect, errorText, statusStyle);
//				GUI.Label (statusTextRect, statusText, statusStyle);
//				GUI.Label (status2TextRect, statusTextObj, statusStyle);
//				//Debug.Log("SampleGUI:OnGUI statusText = " + statusText);
//			}
		}
			
	}
	
	private void ChangeScene(string sceneName)
	{
		if (Application.loadedLevelName != sceneName) {
			sceneSelectionOpen = true;
			sceneIsChanging = true;
			Application.LoadLevel(sceneName);
		}
		else {
			sceneSelectionOpen = false;
		}

	}
	
	void Update()
	{
		if (clearErrorTextTime > 0) {
			if (Time.time > clearErrorTextTime) {
				clearErrorTextTime = 0;
				errorText = "";
			}
		}
	}
}
