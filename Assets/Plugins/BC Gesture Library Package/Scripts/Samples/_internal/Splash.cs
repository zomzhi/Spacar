using UnityEngine;
using System.Collections;

public class Splash : MonoBehaviour
{
	private Rect buttonRect;
	private GUIStyle buttonStyle;
	public Texture2D buttonDownTexture;
	public Texture2D buttonUpTexture;
	
	public static bool splashUp = false;
	
	void Start()
	{
		splashUp = true;
	}
	
	void Awake()
	{
		buttonRect = new Rect( (Screen.width / 2) - 128, Screen.height - 90, 256, 64);
		
	    buttonStyle = new GUIStyle();
        GUIStyleState styleState = new GUIStyleState();
        styleState.background = (Texture2D) buttonDownTexture;
        GUIStyleState styleStateNormal = new GUIStyleState();
        styleStateNormal.background = (Texture2D) buttonUpTexture;
        buttonStyle.normal = styleStateNormal;
        buttonStyle.active = styleState;
        buttonStyle.hover = styleState;
	}

	private void OnGUI () 
	{
		if (GUI.Button(buttonRect, "", buttonStyle)) {
			GoToSamples();
		}
	}
	
	void GestureEndTouch(TouchGesture gesture)
	{		
		if (!buttonRect.Contains( Finger.ScreenVectorToGui(gesture.finger.endPosition)) || !buttonRect.Contains( Finger.ScreenVectorToGui(gesture.finger.startPosition)) ) {
			GoToSamples();  
		}
	}

	private void GoToSamples()
	{
		Application.LoadLevel("SliceGestures");
		splashUp = false;
	}
}

