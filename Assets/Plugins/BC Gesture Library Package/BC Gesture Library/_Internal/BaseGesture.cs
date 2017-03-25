#if (UNITY_ANDROID  || UNITY_IPHONE) && !UNITY_EDITOR && !UNITY_STANDALONE_WIN && !UNITY_STANDALONE_MAC && !UNITY_WEBPLAYER
#define ON_DEVICE
#endif
using UnityEngine;
using System;
using System.Collections.Generic;

public class BaseGesture  : MonoBehaviour
{
	public enum FingerLocation {Over, Always, NotOver, AtLeastOneOver}
	
	public enum FingerCountRestriction
	{
		Any,
		One,
		Two,
		Three,
		Four,
		Five,
		OneOrTwo,
		OneOrTwoOrThree,
		TwoOrThree
	}
	
	public enum XYRestriction {AllDirections, XDirecton, YDirection}
	
	
	public GameObject[] targetMessageObjects;
	public Camera alternateCamera;
	public Collider targetCollider;
	public bool topColliderOnly = false;
	
	public Finger finger;
	public Finger[] fingers;
	public int fingerCount = 0;
	public bool activeChange = true;

	protected static int baseId = 0;
	private int myId = 0;
	private bool setId = true;
	private Camera theCamera;
		
	public static FingerControl fingerControl;
	protected virtual void Start()
	{
		initialize();
		AddGesture();
	}
	
	protected string getId()
	{
		return this.name + "-" + myId;
	}
	
	protected void initialize()
	{

		if (setId) {
			myId = baseId++;
			setId = false;
		}
		if (fingerControl == null) {			
			//Debug.Log("BaseGesture:initialize create fingerControl in " + gameObject.name);
#if ON_DEVICE
//			fingerControl =  (FingerControl) this.gameObject.AddComponent("TouchScreenControl");
			fingerControl =  (FingerControl) this.gameObject.AddComponent<TouchScreenControl>();
#else
			fingerControl = (FingerControl) this.gameObject.AddComponent<MouseControl>();
#endif	
		}
		setCamera();

	}
	
	private void setCamera()
	{
		if (theCamera) {
			return;
		}
		if (alternateCamera) {
			//Debug.Log("BaseGesture:initialize use alternate camera " + alternateCamera.name);
			theCamera = alternateCamera;
		}
		else {
			theCamera = Camera.main;
			if (!theCamera) {
				theCamera = Camera.current;
				if (!theCamera) {
					if (Camera.allCameras.Length > 0) {
						theCamera = Camera.allCameras[0];
					}
				}
			}
		}
	}
	
	private bool added = false;
	void Awake() 
	{
		if (targetMessageObjects != null && targetMessageObjects.Length == 1 && targetMessageObjects[0] == null) {
			targetMessageObjects[0] = gameObject;
		}
		if (targetMessageObjects == null || targetMessageObjects.Length == 0) {
			targetMessageObjects = new GameObject[1];
			targetMessageObjects[0] = gameObject;
		}
		if (targetCollider == null) {
			targetCollider = this.gameObject.GetComponent<Collider>();
		}
	}
	
	public void AddGesture()
	{ 
		if (!added) {
			added = true;
			//Debug.Log("LineGesture:AddGesture " + added + " ----------------------- " + getId() + " ADD " + added);
			FingerControl.AddGesture(this);
		}
	}
	public void RemoveGesture()
	{
		if (added) {
			added = false;
			//Debug.Log("LineGesture:RemoveGesture " + added + " ----------------------- " + getId() + " REMOVE " + added);
			FingerControl.RemoveGesture(this);
		}
	}
	

	
	void OnDestroy()
	{
		RemoveGesture();
	}
	
	public void StartGesture()
	{
		EnableGesture();
	}
	
	public void EndGesture()
	{
		DisableGesture();
	}
	
	
	
	protected virtual void EnableGesture()
	{
		FingerControl._delegateIsDownInternal += GestureIsDown;
	}
	
	protected virtual void DisableGesture()
	{
		FingerControl._delegateIsDownInternal -= GestureIsDown;
	}
	
 	protected void GestureIsDown(Finger fingerIn, bool isDownIn)
	{
		activeChange = true;
	}
	
	protected bool IsOnObject(Vector2 pos)
    {
		if (targetCollider == null) {
			return false;
		}
		
		if (topColliderOnly) {
	        RaycastHit hit;
	        if( Physics.Raycast( theCamera.ScreenPointToRay(pos), out hit ) ) {
				if (hit.collider.gameObject == targetCollider.gameObject) {
					return true;
				}
				//Debug.Log ("BaseGesture:IsOnObject missed and hit " + hit.collider.gameObject.name);
			}
		}
		else {
		   RaycastHit[] hits;
		    hits = Physics.RaycastAll (theCamera.ScreenPointToRay(pos));
		    for (int i = 0; i < hits.Length; i++) {
				if (hits[i].collider.gameObject == targetCollider.gameObject) {
					return true;
				}
			}
			
		}
		//Debug.Log ("BaseGesture:IsOnObject missed and hit  nothing");
        return false;
    }
		
    protected Vector3 ScreenToWorldPosition( Vector2 screenPos )
    {
		return ScreenToWorldPosition( screenPos, float.MinValue);
    }
	
	protected Vector3 ScreenToWorldPosition( Vector2 screenPos, float dist )
    {
		//Debug.Log ("BaseGesture:****** ScreenToWorldPosition ******* " + screenPos + ", dist " + dist + "  " + theCamera.name);
        Ray ray = theCamera.ScreenPointToRay( screenPos );
		//Debug.DrawRay(	ray.origin, ray.direction, Color.red, 0.5f, false);
		//Debug.Log ("BaseGesture:ScreenToWorldPosition the ray " + ray.ToString ());
		if (dist == float.MinValue) {
			if (targetCollider != null) {
				Vector3 distVector = ray.origin - targetCollider.gameObject.transform.position;				
				dist = distVector.magnitude;
 				//Debug.Log ("BaseGesture:ScreenToWorldPosition targetCollider distance = " + dist );
				return ray.GetPoint(dist);
			}
			//Debug.Log ("BaseGesture:ScreenToWorldPosition ray.GetPoint(-ray.origin.z / ray.direction.z) = " + ray.GetPoint(-ray.origin.z / ray.direction.z) );
      		return ray.GetPoint(-ray.origin.z / ray.direction.z);	 // intersection with z is zero plane
 		}
		else  {
			//Debug.Log ("BaseGesture:ScreenToWorldPosition ray.GetPoint(z) = " + ray.GetPoint(dist));
      		return ray.GetPoint(dist);
		}
    }

	
	protected void SetFingers(Finger[] fingersIn, int count)
	{
		finger = fingersIn[0];
		fingers = new Finger[count];
		for (int i = 0; i < count; i++) {
			fingers[i] = fingersIn[i];
		}
	}
	
	private int activeCount = 0;	
	public int ActiveCount()
	{
		if (activeChange) {
			Finger[] fingers = FingerControl.Factory().fingers;
			activeCount = 0;
			for (int i = 0; i < fingers.Length; i++) {
				if (fingers[i].isDown) {
					activeCount++;
				}
			}
			activeChange = false;
			//Debug.Log("LineGesture:ActiveCount " + count);
		}
		return activeCount;
	}
	
	protected void GestureMessage(string gestureName)
	{
		fingerCount = ActiveCount();
		
		for (int i = 0; i < targetMessageObjects.Length; i++) {
			targetMessageObjects[i].SendMessage(gestureName, this, SendMessageOptions.DontRequireReceiver);
		}

	}
	
	protected bool FingerActivated(FingerLocation fLocation)
	{
		if (fLocation == FingerLocation.Always) {
			return true;
		}
		
		Finger[] fingers = FingerControl.Factory().fingers;
		int activatedCount = 0;
		for (int i = 0; i < fingers.Length; i++) {
			if (!fingers[i].isDown) {
				continue;
			}
			
			bool activated = FingerActivated(fLocation, fingers[i].position);
			if (activated) {
				activatedCount++;
			}
			
			if (fLocation == FingerLocation.Over || fLocation == FingerLocation.NotOver) {
				if (!activated) {
					return false;
				}
			}
		}
		if (fLocation == FingerLocation.AtLeastOneOver && activatedCount == 0) {
				//Debug.Log("BaseGesture:FingerActivated over " + IsOnObject(position));
			return false;
		}

		return true;
	}
	
	protected bool FingerActivated(FingerLocation fLocation, Vector2 position)
	{
		if (fLocation == FingerLocation.Always) {
			//Debug.Log("BaseGesture:FingerActivated always  true");
			return true;
		}
		if (fLocation == FingerLocation.Over || fLocation == FingerLocation.AtLeastOneOver) {
			//Debug.Log("BaseGesture:FingerActivated over " + IsOnObject(position) + "  position " + position);
			return IsOnObject(position);
		}
		else if (fLocation == FingerLocation.NotOver) {
			//Debug.Log("BaseGesture:FingerActivated not over " + !IsOnObject(position));
			return !IsOnObject(position);
		}
		//Debug.Log("BaseGesture:FingerActivated default false " + fLocation);
		return false;
	}

				
	protected bool FingerCountGood(int fingerCount, FingerCountRestriction restrictFingerCount)
	{
		if (restrictFingerCount == FingerCountRestriction.Any) {
			return true;
		}
		
		switch(fingerCount) {
		case 1:
			if (restrictFingerCount == FingerCountRestriction.One ||
			    restrictFingerCount == FingerCountRestriction.OneOrTwo ||
			    restrictFingerCount == FingerCountRestriction.OneOrTwoOrThree) {
				return true;
			}
			break;
		case 2:
			if (restrictFingerCount == FingerCountRestriction.Two ||
			    restrictFingerCount == FingerCountRestriction.OneOrTwo ||
			    restrictFingerCount == FingerCountRestriction.OneOrTwoOrThree ||
			    restrictFingerCount == FingerCountRestriction.TwoOrThree) {
				return true;
			}
			break;
		case 3:
			if (restrictFingerCount == FingerCountRestriction.Three ||
			    restrictFingerCount == FingerCountRestriction.OneOrTwoOrThree ||
			    restrictFingerCount == FingerCountRestriction.TwoOrThree) {
				return true;
			}
			break;
		case 4:
			if (restrictFingerCount == FingerCountRestriction.Four) {
				return true;
			}
			break;
		case 5:
			if (restrictFingerCount == FingerCountRestriction.Five) {
				return true;
			}
			break;
		}
		
		return false;
	}
	
	protected void CleanFingers(List<Finger> fingers)
	{
		if (fingers == null || fingers.Count == 0) {
			return;
		}
		Finger finger;
		for (int i = 0; i < fingers.Count; i++) {
			finger = fingers[i];
			if (!finger.isDown || !finger.SetState()) {
				//Debug.Log("BaseGesture:cleanFingers clean finger " + i);
				fingers.RemoveAt(i);
				i--;
			}
		}	
	}
	
	protected Vector3 CalcWorldPosForAllFingers(Vector2 fingerPos)
	{
		Vector3 worldPos;
		worldPos = ScreenToWorldPosition(CalcPosForAllFingers(fingerPos));
		worldPos.z = targetCollider.gameObject.transform.position.z;
		
		return worldPos;
	}
	
	protected Vector2 CalcPosForAllFingers(Vector2 fingerPos)
	{
		Vector2 position;
		int aCount = ActiveCount();
		if (aCount < 2) {
			position = fingerPos;
		}
		else {
			//x = (Ax + Bx + Cx) / 3
			//y = (Ay + By + Cy) / 3
			Finger[] fingers = FingerControl.Factory().fingers;
			float x = 0;
			float y = 0;
			
			for (int i = 0; i < fingers.Length; i++) {
				if (fingers[i].isDown) {
					x += fingers[i].position.x;
					y += fingers[i].position.y;
				}
			}
			x = x / aCount;
			y = y / aCount;
			//Debug.Log("DragGesture:DragGestureDownAndMovingMove Relative over 1 " + worldPos);
			position = new Vector2(x, y);
		}
		
		return position;
	}
	
	public Bounds emptyBounds = new Bounds(Vector3.zero, Vector3.zero);
	
	protected Bounds GetBounds()
	{
		if (gameObject	== null) {
			return emptyBounds;
		}
		Collider[] colliders = gameObject.GetComponentsInChildren<Collider>();
		if (colliders != null && colliders.Length > 0) {
			Bounds bounds = colliders[0].bounds;
			for (int i = 1; i < colliders.Length; i++) {
				bounds.Encapsulate(colliders[i].bounds);
			}
			return bounds;
		}
		else {		
			Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();
			Bounds bounds = renderers[0].bounds;
			if (renderers != null && renderers.Length > 0) {
				for (int i = 1; i < colliders.Length; i++) {
					bounds.Encapsulate(renderers[i].bounds);
				}
				return bounds;
			}
			else {			
				Mesh mesh = GetComponent<MeshFilter>().mesh;
				if (mesh != null) {
					return mesh.bounds;
				}
			}
		}
		
		return emptyBounds;
	}
	
	
}