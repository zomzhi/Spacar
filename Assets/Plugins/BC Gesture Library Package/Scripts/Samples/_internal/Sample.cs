using System;
using UnityEngine;
using System.Collections;

public class Sample : MonoBehaviour
{
	public bool addTouchGesture = true;
	
	protected GUIStyle settingsTextStyle;
	protected GUIStyle resultsTextStyle;
	
	protected const float sectionX = 60f;
	protected const float sectionY = 65f;
	protected const float propLabelX = 60f;
	protected const float propLabelAfterGap = 8f;
	protected const float titleAfterGap = 12f;
	protected const float resultLabelAfterGap = 20f;
	protected const float betweenYGap = 6f;
	protected const float betweenResultsYGap = 6f;
	protected const float labelCenterOffset = 5f;
	protected const float titleHeight = 40f;
	protected const float boxHeight = 28f;
	protected float boxResultHeight = 28f;
	protected const float clearButtonHeight = 40f;
	protected const float clearButtonWidth = 73f;
	protected const float checkboxSize = 28f;
	
    protected virtual void Start( )
    {
		if (addTouchGesture) {
			this.gameObject.AddComponent<TouchGesture>();
		}
	}
	
	void Update ()
	{
		if (Input.GetKeyUp(KeyCode.Escape)) {
			Application.Quit();
		}
	}	
	
 	protected void initializeGUI()
	{
		if (settingsTextStyle != null) {
			return;
		}
		if (Screen.height < 500f) {
			boxResultHeight = 20f;
		}
		SetupData();
		if (SampleGUI.Factory() != null) {
			settingsTextStyle = SampleGUI.Factory().settingsTextStyle;
			resultsTextStyle = SampleGUI.Factory().resultsTextStyle;
		}
	}
	
	protected virtual void SetupData ()
	{
	}
	
	protected bool GuiCheckBox(Rect placementRect, bool currentValue)
	{
		if (GUI.Button(placementRect, SampleGUI.Factory().checkboxOffTexture)) {
			return !currentValue;
		}
		if (currentValue) {
			GUI.DrawTexture (placementRect, SampleGUI.Factory().checkboxOnTexture);
		}
		
		return currentValue;
	}

	
	void GestureStartTouch(TouchGesture gesture)
	{
		SampleFingerDown();
	}
	
	private GUIStyle CreatePreDoneGUIStyle(GUIStyle style)
	{
		return style;
	}
	
	protected virtual void SampleFingerDown() {}

	protected void DisplaySuccess (GameObject obj, string text)
	{
		EmitSuccess(obj);
		SampleGUI.clearStatus();
		SampleGUI.StatusText = text;
	}
	
	protected void EmitSuccess(GameObject obj)
	{
		ParticleEmitter emitter = obj.GetComponentInChildren<ParticleEmitter> ();
		if (emitter) {
			emitter.Emit ();
		}
	}

    public static Vector3 ScreenToWorldPosition( Vector2 screenPos, float z )
    {
        Ray ray = Camera.main.ScreenPointToRay( screenPos );
        Vector3 point = ray.GetPoint(-ray.origin.z / ray.direction.z);	// intersection with z is zero plane
		point.z = 0;
		return point;
    }
	
    public static Vector2 WorldToScreenPosition( Vector3 worldPos) {
		return Camera.main.WorldToScreenPoint(worldPos);
	}
	
	public static void SetVector(Vector3 targetVector, Vector3 sourceVector)
	{
		targetVector.x = sourceVector.x;
		targetVector.y = sourceVector.y;
		targetVector.z = sourceVector.z;
	}
	
	public static Vector2 ScreenToGuiPoint(Vector2 screenPoint)
	{		
		return new Vector2(screenPoint.x, Screen.height - screenPoint.y);
	}

    public static GameObject ScreenPointFindsCollider( Vector2 screenPos )
    {
        Ray ray = Camera.main.ScreenPointToRay( screenPos );
        RaycastHit hit;

        if( Physics.Raycast( ray, out hit ) ) {
            return hit.collider.gameObject;
		}

        return null;
    }
	
	public int GUITextPositiveNaturalIntIncludingZero(Rect rect, int val,int size)
	{
		int defaultVal = val;
		string str = GUITextField(rect, val.ToString(), size);
		return ConvertToPositiveNaturalInt(str, defaultVal, true);
	}	
	
	public int GUITextPositiveNaturalInt(Rect rect, int val,int size)
	{
		int defaultVal = val;
		string str = GUITextField(rect, val.ToString(), size);
		return ConvertToPositiveNaturalInt(str, defaultVal, false);
	}
	public float GUITextPositiveNonZeroFloat(Rect rect, float val, int size)
	{
		float defaultVal = val;
		string str = GUITextField(rect, val.ToString(), size);
		return ConvertToPositiveNonZeroFloat(str, defaultVal);
	}
	
	private string GUITextField(Rect rect, string val, int size)
	{
		GUI.DrawTexture (rect, SampleGUI.Factory().textBoxTexture);
		return GUI.TextField(new Rect(rect.x + 5, rect.y + 2, rect.width - 5, rect.height - 2), val.ToString(), size, SampleGUI.Factory().editTextStyle);
	}
	
	public int ConvertToPositiveNaturalInt(string strVal, int defaultVal, bool allowZero)
	{
		
		int j;
		bool result = Int32.TryParse(strVal, out j);
		if (result) {
			if (j > 0 || (allowZero && j == 0)) {
				return j;
			}
			else {
				if (allowZero) {
					SampleGUI.ErrorText = "Value entered number a positive integer zero or bigger.";
				}
				else {
					SampleGUI.ErrorText = "Value entered number a positive integer one or bigger.";
				}
			}
		}
		else {
		   SampleGUI.ErrorText = "Value entered is not an integer.";
		}
		return defaultVal;
	}
	
	public float ConvertToPositiveNonZeroFloat(string strVal, float defaultVal)
	{
		
		float j;
		bool result = float.TryParse(strVal, out j);
		if (result) {
			if (j >= 0) {
				return j;
			}
			else {
				SampleGUI.ErrorText = "Value entered number a non-zero real number bigger than zero.";
			}
		}
		else {
		   SampleGUI.ErrorText = "Value entered is not an real number.";
		}
		return defaultVal;
	}

}
