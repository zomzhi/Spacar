using UnityEngine;
using System;

public class Finger
{
    public enum FingerMotionState {Stationary, Moving, Inactive}
 	
	private int _index = 0;
	//public virtual int index { get {return _index;} set {_index = value;} }
	
	public bool isDown = false;
	public bool hasMoved = false;
	public bool longPressSent = false;
	public bool downAndMoving = false;
	public FingerMotionState motionState = FingerMotionState.Inactive;
	public float timeSpentStationary = 0;
	public float startTime = 0;
	public float downAndMovingStartTime = 0;
	public bool possibleSwipe = true;
	public bool onlyStationary = false;
	public SwipeGesture.SwipeDirection swipeDirection = SwipeGesture.SwipeDirection.None;
	
	public TouchPhase touchPhase = TouchPhase.Ended;
	public Vector2 position = Vector2.zero;
	public Vector2 deltaPosition = Vector2.zero;
	
	public TouchPhase previousTouchPhase = TouchPhase.Ended;
	public Vector2 previousPosition = Vector2.zero;
	public Vector2 previousDeltaPosition = Vector2.zero;
	
	public Vector2 stationaryPosition = Vector2.zero;
	public Vector2 startPosition = Vector2.zero;
	public Vector2 endPosition = Vector2.zero;
	public Vector2 tapStartPosition = Vector2.zero;
	public bool usingMouse = false;
	
	
	public static Vector2 ScreenVectorToGui(Vector2 screenVector)
	{
		return new Vector2(screenVector.x, Screen.height - screenVector.y);
	}
	
	public Finger(int indexIn)
	{
		SetIndex(indexIn);
	}
	
	public virtual int Index()
	{
		return _index;
	}
	public virtual void SetIndex(int index)
	{
		_index = index;
	}
		
	public virtual void Down(Vector2 position)
	{
		//Debug.Log("Finger:Down  " + _index);
	    isDown = true;
	    hasMoved = false;
	    downAndMoving = false;
	    longPressSent = false;
		swipeDirection = SwipeGesture.SwipeDirection.None;
	    motionState = FingerMotionState.Inactive;
	    startPosition = position;
	    stationaryPosition = startPosition;
	    endPosition = startPosition;
	    startTime = Time.time;
	    timeSpentStationary = Time.time;
		downAndMovingStartTime = -1;
	}
	
	public virtual void Up()
	{
		//Debug.Log("Finger:Up  " + _index);
	    isDown = false;
		downAndMoving = false;
	}
	
	public virtual bool SetState()
	{
		previousTouchPhase = touchPhase;
		previousPosition = position;
		previousDeltaPosition = deltaPosition;
		return true;
	}

}

