using UnityEngine;
using System.Collections;

public class TouchScreenControl : FingerControl
{
	public int fingersUsed = 5;

	public override int FingerCount ()
	{
		return fingersUsed;
	}

	protected override void Start ()
	{
		fingerBeingUsed = new int[FingerCount ()];
		
		base.Start ();
	}
	
	public override bool IsFingers()
	{
		return true;
	}

	protected override Finger NewFinger (int index)
	{
		return (Finger) new TouchScreenFinger (index);
	}


	Touch nullTouch = new Touch ();

	int[] fingerBeingUsed;
	// fingers used 

	bool HasValidTouch (Finger finger)
	{
		return fingerBeingUsed[finger.Index()] != -1;
	}

	Touch GetTouch (Finger finger)
	{
		int touchIndex = fingerBeingUsed[finger.Index()];
		
		if (touchIndex == -1)
			return nullTouch;
		
		return Input.touches[touchIndex];
	}




}
