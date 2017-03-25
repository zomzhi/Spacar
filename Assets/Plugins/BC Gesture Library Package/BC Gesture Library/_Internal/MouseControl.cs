using UnityEngine;
using System.Collections;

public class MouseControl : FingerControl
{
	
	public static int mouseButtons = 5;	

	protected override void Start ()
	{
		base.Start ();		
	}

	protected override Finger NewFinger (int index)
	{
		return (Finger) new MouseFinger (index);
	}

	public override int FingerCount ()
	{
		return 1;
	}
	public override bool IsFingers()
	{
		return false;
	}
	

	public float wheelPinchThreshold = 0.01f;	
	private bool pinchingMouseWheel = false;
	
	protected override void Update()
	{
		base.Update ();
		
		if (!trackPinch) {
			return;
		}
		
		float scrollWheelAxis = Input.GetAxis ("Mouse ScrollWheel");
		//Debug.Log("MouseControl:Update axis " + scrollWheelAxis);
		if (Mathf.Abs(scrollWheelAxis) > wheelPinchThreshold) {
			float moveDistance = scrollWheelAxis * 200f;
			if (!pinchingMouseWheel) {
				//Debug.Log("MouseControl:Update SendEventPinchBegin ");
				SendEventPinchBegin (fingers[0], fingers[0], PinchGesture.PinchDirection.Vertical,  moveDistance);
				SendEventPinchMove (fingers[0], fingers[0], PinchGesture.PinchDirection.Vertical, moveDistance);
				pinchingMouseWheel = true;
			}
			else {
				//Debug.Log("MouseControl:Update SendEventPinchMove " + moveDistance);
				SendEventPinchMove (fingers[0], fingers[0], PinchGesture.PinchDirection.Vertical,  moveDistance);
			}
		}
		else if (pinchingMouseWheel) {
			//Debug.Log("MouseControl:Update SendEventPinchEnd");
			SendEventPinchEnd (fingers[0], fingers[0]);
			pinchingMouseWheel = false;
		}			
	}
	
	
}
