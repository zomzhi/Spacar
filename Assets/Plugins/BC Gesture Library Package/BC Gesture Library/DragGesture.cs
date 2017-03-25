using UnityEngine;
using System;

public class DragGesture : BaseGesture
{
	public enum DragPosition {Relative, Centred, NoDrag}
	
	private DragGesture dragGesture;
	public DragPosition dragPosition = DragPosition.Relative;
	public FingerLocation fingerLocation = FingerLocation.Over;
	public FingerCountRestriction restrictFingerCount = FingerCountRestriction.Any;
	public bool doDrag = true;
	public XYRestriction restrictDirection = XYRestriction.AllDirections;
	public float restrictScreenMin  = 0f;
	public float restrictScreenMax = 1f;
	
	public int dragFingerCount;
	public Vector3 startPoint = Vector3.zero;
	public Vector3 endPoint = Vector3.zero;
	public Vector3 targetPoint;
	
	private Vector3 worldDelta;
	private Vector3 originalPosition;
	
	protected override void Start()
	{
		base.Start();
		originalPosition = this.transform.position;
	}	
	protected override void EnableGesture()
	{
		base.EnableGesture();
		FingerControl._delegateDownAndMovingBeginInternal += DragGestureDownAndMovingBegin;
		FingerControl._delegateDownAndMovingMoveInternal += DragGestureDownAndMovingMove;
		FingerControl._delegateDownAndMovingEndInternal += DragGestureDownAndMovingEnd;
		if (targetCollider == null) {
			doDrag = false;
		}
		else {
			targetPoint = targetCollider.gameObject.transform.position;
		}
	}
	
	protected override void DisableGesture()
	{
		base.DisableGesture();
		FingerControl._delegateDownAndMovingBeginInternal -= DragGestureDownAndMovingBegin;
		FingerControl._delegateDownAndMovingMoveInternal -= DragGestureDownAndMovingMove;
		FingerControl._delegateDownAndMovingEndInternal -= DragGestureDownAndMovingEnd;
	}
	
	private bool draggingObject = false;
	protected void DragGestureDownAndMovingBegin(Finger fingerIn)
	{
		if ( FingerCountGood(ActiveCount(), restrictFingerCount) &&
		    	FingerActivated(fingerLocation, fingerIn.position) && targetCollider != null && dragPosition != DragPosition.NoDrag) {
			draggingObject = true;
			startPoint = targetCollider.gameObject.transform.localPosition;
			//Debug.Log("DragGesture:DragGestureDownAndMovingBegin " + startPoint + ", finger count " + ActiveCount());
			
			dragFingerCount = 0;
			if (fingerIn.Index() == 0) {
				finger = fingerIn;
			}
			GestureMessage("GestureStartDrag");
		}
	}
	protected void DragGestureDownAndMovingMove(Finger fingerIn, Vector2 fingerPos, Vector2 delta)
	{
		int activeCount = ActiveCount();
		if (draggingObject && finger == fingerIn && FingerCountGood(activeCount, restrictFingerCount) &&
		  			dragPosition != DragPosition.NoDrag ) {
			//Debug.Log("DragGesture:DragGestureDownAndMovingMove " + fingerPos + ", finger count " +activeCount);
			
			if (doDrag) {
				if ((restrictDirection == XYRestriction.XDirecton ||  restrictDirection == XYRestriction.YDirection) && (restrictScreenMin > 0f || restrictScreenMax != 1f)) {
					float min = restrictScreenMin;
					float max = restrictScreenMax;
					if (restrictDirection == XYRestriction.XDirecton) {
						if (min <= 1f) {
							min = Screen.width * min;
						}
						if (max <= 1f) {
							max = Screen.width * max;
						}
						if (fingerPos.x < min) {
							fingerPos.x = min;
						}
						if (fingerPos.x > max) {
							fingerPos.x = max;
						}
					}
					else if (restrictDirection == XYRestriction.YDirection) {
						if (min <= 1f) {
							min = Screen.height * min;
						}
						if (max <= 1f) {
							max = Screen.height * max;
						}
						if (fingerPos.y < min) {
							fingerPos.y = min;
						}
						if (fingerPos.y > max) {
							fingerPos.y = max;
						}
					}
				}
				
				Vector3 worldPos = CalcWorldPosForAllFingers(fingerPos);
				
				if (restrictDirection == XYRestriction.XDirecton)  {
					worldPos.y = targetCollider.gameObject.transform.position.y;
				}
				else if (restrictDirection == XYRestriction.YDirection) {
					worldPos.x = targetCollider.gameObject.transform.position.x;
				}
				
				//Debug.Log("DragGesture:DragGestureDownAndMovingMove  move to " + worldPos);
				if (dragPosition == DragPosition.Relative) {
					if (dragFingerCount != activeCount) {
						dragFingerCount = activeCount;
						worldDelta = new Vector3(targetCollider.gameObject.transform.position.x - worldPos.x, targetCollider.gameObject.transform.position.y - worldPos.y, targetCollider.gameObject.transform.position.z);
					}			
					targetPoint = new Vector3(worldPos.x + worldDelta.x, worldPos.y + worldDelta.y, worldDelta.z);				
				}
				else  {
					targetPoint = worldPos;
				}
				//targetCollider.gameObject.transform.position = targetPoint;
			}
			GestureMessage("GestureMoveDrag");
		}
	}
	
	
	protected void DragGestureDownAndMovingEnd(Finger fingerIn)
	{
		if (draggingObject && finger == fingerIn) {
			draggingObject = false;
			endPoint = targetCollider.gameObject.transform.localPosition;
			//Debug.Log("DragGesture:DragGestureDownAndMovingEnd " + endPoint + ", finger count " + ActiveCount());
			GestureMessage("GestureEndDrag");
		}
		dragFingerCount = 0;
	}
	
	void Update()
	{
		if (doDrag && targetPoint != targetCollider.gameObject.transform.position)  {
			targetCollider.gameObject.transform.position = Vector3.Lerp(targetCollider.gameObject.transform.position, targetPoint, Time.deltaTime * 25f); 
		}
	}

	
	public void RestoreObject()
	{
		this.transform.position = originalPosition;
		targetPoint = targetCollider.gameObject.transform.position;
	}	
}