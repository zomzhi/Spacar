using UnityEngine;
using System;

public class MouseFinger : Finger
{
	private int _virtualIndex = 0;
	
	public MouseFinger(int index) : base (index)
	{
		usingMouse = true;
	}
	
	public override int Index()
	{
		return _virtualIndex;
	}	
	public override void SetIndex(int index)
	{
		_virtualIndex = index;
	}	
	
	public override bool SetState()
	{
		base.SetState();
		
		Vector2 mousePos = Input.mousePosition;
		deltaPosition = mousePos - position;
		position = mousePos;
		touchPhase = GetTouchPhase();
		//Debug.Log("MouseFinger:setState " + touchPhase);
		return true;
	}
	
	private TouchPhase setTouchPhase(TouchPhase touchPhasein)
	{
		//Debug.Log("MouseFinger:setTouchPhase " + touchPhase);
		touchPhase = touchPhasein;
		return touchPhase;
	}
	
	public TouchPhase GetTouchPhase ()
	{
		
		// look for first button that is active
		bool aButtonIsActive = false;
		for (int buttonIndex = 0; buttonIndex < MouseControl.mouseButtons; buttonIndex++) {
			if (!Input.GetMouseButton (buttonIndex)) {
				continue;
			}
			//Debug.Log("MouseFinger:GetTouchPhase " + buttonIndex);
			aButtonIsActive = true;
			SetIndex(buttonIndex);
						
			if (previousTouchPhase != TouchPhase.Began && Input.GetMouseButtonDown (buttonIndex)) {
				return setTouchPhase(TouchPhase.Began);
			}
						
			if (deltaPosition.sqrMagnitude < 1.0f) {
				return setTouchPhase(TouchPhase.Stationary);
			}
			
			return setTouchPhase(TouchPhase.Moved);
		}
		
		if (!aButtonIsActive && (touchPhase != TouchPhase.Ended && touchPhase != TouchPhase.Canceled)) {
			//Debug.Log("MouseFinger:GetTouchPhase set to ended aButtonIsActive=" + aButtonIsActive + ", touchPhase=" + touchPhase);
			return setTouchPhase(TouchPhase.Ended);
		}
		
		
		return TouchPhase.Canceled;
	}

}

