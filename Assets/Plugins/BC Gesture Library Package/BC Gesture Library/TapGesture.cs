using UnityEngine;
using System;
using System.Collections.Generic;
using System.Timers;

public class TapGesture : BaseGesture
{
	public int taps;
	public float maxTimeBetweensTaps = 0.4f;
	public float maxTapDistance = 25f;
	public int tapRateTapsCount = 2;
	public FingerLocation startsOnObject = FingerLocation.Over;
	public FingerLocation movesOnObject = FingerLocation.Over;
	public FingerLocation endsOnObject = FingerLocation.Over;
	public bool enforceStationary = false;
	
	public float tapsPerMinute = 0f;
	private int tapsReceived = 0;
	private float lastTime = 0f;
	private float tapStartTime;
	private List<float> tapTimes;
	private Timer tapTimer = new System.Timers.Timer();
	private float _maxTimeBetweensTaps;
	private bool gestureIsGoing = false;
		
	protected override void EnableGesture()
	{
		base.EnableGesture();
		tapTimer.AutoReset = false;
		tapTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
		
		tapTimes = new List<float>();
		FingerControl._delegateFingerUpInternal += TapGestureFingerUp;
		FingerControl._delegateFingerMovingInternal += TapGestureDownAndMovingMove;
		FingerControl._delegateFingerDownInternal += TapGestureFingerDown;
	}
	
	protected override void DisableGesture()
	{
		base.DisableGesture();
		FingerControl._delegateFingerUpInternal -= TapGestureFingerUp;
		FingerControl._delegateFingerMovingInternal -= TapGestureDownAndMovingMove;
		FingerControl._delegateFingerDownInternal -= TapGestureFingerDown;
	}
	
	protected void TapGestureFingerDown (Finger fingerIn)
	{
	_maxTimeBetweensTaps = maxTimeBetweensTaps;
		if (_maxTimeBetweensTaps < 0.0001f) {
			_maxTimeBetweensTaps = 0.0001f;
		}
		if (FingerActivated(startsOnObject, fingerIn.position) ) {
			gestureIsGoing = true;
		}
		//Debug.Log("TapGesture:TapGestureFingerDown " + _maxTimeBetweensTaps);
	}

	protected void TapGestureFingerUp (Finger fingerIn)
	{
		//Debug.Log("TapGesture:TapGestureFingerUp validate end =" + FingerActivated(endsOnObject, fingerIn.position));
		if (gestureIsGoing && FingerActivated(endsOnObject, fingerIn.position) && (!enforceStationary || fingerIn.onlyStationary) ) {
			gestureIsGoing = false;
			
			if (lastTime > 0 && (Time.time - lastTime) > _maxTimeBetweensTaps) {
				clearTaps();
				//Debug.Log("TapGesture:TapGestureFingerUp clearTaps " + (Time.time - lastTime));
			}
			
			if (tapsReceived == 0) {
				//Debug.Log("TapGesture:TapGestureFingerUp tapsReceived is zero ");
				tapTimes.Clear();
			}
			Vector2 distVect = fingerIn.position - fingerIn.startPosition;
			//Debug.Log("TapGesture:TapGestureFingerUp distVect=" + distVect + ", magnitude=" + distVect.magnitude);
			if (distVect.magnitude > maxTapDistance) {
				tapTimes.Clear();
				return;
			}
			
			tapsReceived++;
			tapTimes.Add(Time.time);
			
			if (tapsReceived == 1) {
				tapsPerMinute = 0;
			}
			else if (tapsReceived < tapRateTapsCount) {
				tapsPerMinute = (1 / ((Time.time - tapTimes[0]) / tapsReceived)) * 60;
			}
			else {
				//Debug.Log("TapGesture:TapGestureFingerUp count=" + tapRateTapsCount + ", tapTimes[0]=" + tapTimes[0] + ", time=" + (Time.time - tapTimes[0]) );
				tapsPerMinute =  (1 / ((Time.time - tapTimes[0]) / tapRateTapsCount)) * 60;
				tapTimes.RemoveAt(0);
			}
					
			lastTime = Time.time;
			//Debug.Log("TapGesture:TapGestureFingerUp tapsReceived " + tapsReceived + " taps=" + taps );
			if (tapsReceived == taps || taps == 0) {
				finger = fingerIn;
				GestureMessage("GestureTap");
				//Debug.Log("TapGesture:TapGestureFingerUp GestureTap");
			}
			
			tapTimer.Stop();
			tapTimer.Interval = _maxTimeBetweensTaps * 1000f;
			tapTimer.Start();

		}
		else  {
			clearTaps();
		}
	}
	
	protected void TapGestureDownAndMovingMove(Finger fingerIn)
	{
	
		if (!enforceStationary && gestureIsGoing && !FingerActivated(movesOnObject, fingerIn.position)) {
			gestureIsGoing = false;
		}
	}
 
	// Specify what you want to happen when the Elapsed event is raised.
	private void OnTimedEvent(object source, ElapsedEventArgs e)
	{
		clearTaps();
	}
	
	private void clearTaps()
	{
		tapTimer.Stop();
		tapTimes.Clear();
		tapsPerMinute = 0;
		tapsReceived = 0;
		lastTime = 0;
	}
}