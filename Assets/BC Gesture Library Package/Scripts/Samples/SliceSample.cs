using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SliceSample : Sample
{
	
	public float sliceScaleFactor = 0.01f;
	
	private SliceGesture sliceGesture;
	private ComboControl restrictDirectionCombo;

	private Rect settingsLabelRect;
	private Rect restrictDirectionLabelRect;
	private Rect restrictDirectionListRect;
	
	private Rect resultLabelRect;
	private Rect eventLabelRect;
	private Rect eventBoxRect;
	private Rect sliceDirectionLabelRect;
	private Rect sliceDirectionBoxRect;
	private Rect sliceStartPositionLabelRect;
	private Rect sliceStartPositionBoxRect;
	private Rect sliceEndPositionLabelRect;
	private Rect sliceEndPositionBoxRect;
	private Rect sliceFingerLabelRect;
	private Rect sliceFingerBoxRect;
	
	private Rect clearResultButtonRect;
	private bool gestureOccurred = false;
	
	//Physics
	public Transform logo;
	public Transform[] parts;
	
	private bool exploding;
	public float spread;
	public float minVelocity;
	public float randomRange;
	public float drag;
	public float angularDrag;
	private float directionalSpread;
	
	private Vector3 dampVel = Vector3.zero;
	private float smoothSpeed = 0.05f;
	private float maxSpeed = 15f;
	private bool resetting;

	private Transform[] set1;
	private Transform[] set2;
	
	private Vector3 heading;
	private bool positiveHeading;

	void Awake ()
	{
		Renderer[] renderers = logo.GetComponentsInChildren<Renderer> ();
		parts = new Transform[9];
		for(int i=0; i< renderers.Length; i++)
			parts[i] = renderers[i].transform;
		
		foreach (Transform part in parts) {
			if (part != logo) {
				part.gameObject.AddComponent<MeshCollider> ();
				part.GetComponent<MeshCollider> ().convex = true;
				part.gameObject.AddComponent<Rigidbody> ();
				part.GetComponent<Rigidbody>().useGravity = false;
				part.GetComponent<Rigidbody>().isKinematic = false;
				part.GetComponent<Rigidbody>().drag = drag;
				part.GetComponent<Rigidbody>().angularDrag = angularDrag;
			}
		}
	}

	void Update ()
	{
		if(resetting) {	
			int stopCount = 0;
			for(int i=0; i < parts.Length; i++) {
		        parts[i].localPosition = Vector3.SmoothDamp(parts[i].localPosition, Vector3.zero, ref dampVel, smoothSpeed, maxSpeed);
				parts[i].localRotation = Quaternion.Slerp (parts[i].localRotation, Quaternion.identity, 0.1f);
				 
				if (parts[i].localPosition.sqrMagnitude < 0.5) {
					stopCount++;
				}
			}
			if (stopCount == parts.Length) {
				HardReset();
				resetting = false;
			}
		}
	}
	
	protected override void SetupData ()
	{
		sliceGesture = this.gameObject.GetComponentInChildren<SliceGesture>();
				
		settingsLabelRect = new Rect(sectionX, sectionY, 100f, titleHeight);
		
		restrictDirectionLabelRect = new Rect(propLabelX, settingsLabelRect.yMax + titleAfterGap, 120f, boxHeight);
		float boxX = propLabelX + 80f;
		restrictDirectionListRect = new Rect(boxX, restrictDirectionLabelRect.y, 130f, boxHeight);
		
		resultLabelRect = new Rect(sectionX, restrictDirectionListRect.yMax + resultLabelAfterGap, 100f, titleHeight);
		boxX = propLabelX + 100f;
		eventLabelRect = new Rect(propLabelX, resultLabelRect.yMax + titleAfterGap, 100f, boxResultHeight);
		eventBoxRect = new Rect(boxX, eventLabelRect.y, 100f, boxResultHeight);		
		sliceDirectionLabelRect = new Rect(propLabelX, eventBoxRect.yMax + betweenResultsYGap, 100f, boxResultHeight);
		sliceDirectionBoxRect = new Rect(boxX, sliceDirectionLabelRect.y, 100f, boxResultHeight);
		sliceStartPositionLabelRect = new Rect(propLabelX, sliceDirectionBoxRect.yMax + betweenResultsYGap, 100f, boxResultHeight);
		sliceStartPositionBoxRect = new Rect(boxX, sliceStartPositionLabelRect.y, 120f, boxResultHeight);
		sliceEndPositionLabelRect = new Rect(propLabelX, sliceStartPositionBoxRect.yMax + betweenResultsYGap, 100f, boxResultHeight);
		sliceEndPositionBoxRect = new Rect(boxX, sliceEndPositionLabelRect.y, 120f, boxResultHeight);
		sliceFingerLabelRect = new Rect(propLabelX, sliceEndPositionBoxRect.yMax + betweenResultsYGap, 100f, boxResultHeight);
		sliceFingerBoxRect = new Rect(boxX, sliceFingerLabelRect.y, 120f, boxResultHeight);
		
		clearResultButtonRect = new Rect(propLabelX, sliceFingerBoxRect.yMax + betweenYGap, clearButtonWidth, clearButtonHeight);
	}
	
	private bool firstOnGUI = true;
	
	private void OnGUI () 
	{
		if (firstOnGUI) {
			initializeGUI();
			restrictDirectionCombo = new ComboControl();
			restrictDirectionCombo.SetSwipeDirectionList((int) sliceGesture.restrictDirection);
			firstOnGUI = false;
		}
		SampleGUI.Factory().PerformGUI();
		if (!SampleGUI.sceneSelectionOpen) {
			GUI.DrawTexture (settingsLabelRect, SampleGUI.Factory().settingsTextTexture);
			sliceGesture.restrictDirection = (SwipeGesture.SwipeDirection) restrictDirectionCombo.GUIShowList("Direction:", restrictDirectionLabelRect, restrictDirectionListRect);
			//Debug.Log("SwipepingSample:OnGUI fingerActivation " + swipeGesture.fingerActivation);
			
			GUI.DrawTexture (resultLabelRect, SampleGUI.Factory().resultsTextTexture);
			
			GUI.Label(eventLabelRect, "Event:", resultsTextStyle);					
			GUI.Label(sliceDirectionLabelRect, "Direction:", resultsTextStyle);
			GUI.Label(sliceStartPositionLabelRect, "Start position:", resultsTextStyle);
			GUI.Label(sliceEndPositionLabelRect, "End position:", resultsTextStyle);
			GUI.Label(sliceFingerLabelRect, "Finger:", resultsTextStyle);
			
			if (gestureOccurred) {
				GUI.Label(eventBoxRect, "Received Slice", resultsTextStyle);			
				GUI.Label(sliceDirectionBoxRect, sliceGesture.sliceDirection.ToString(), resultsTextStyle);
				GUI.Label(sliceStartPositionBoxRect, sliceGesture.sliceStartPosition.ToString(), resultsTextStyle);
				GUI.Label(sliceEndPositionBoxRect, sliceGesture.sliceEndPosition.ToString(), resultsTextStyle);
				GUI.Label(sliceFingerBoxRect, sliceGesture.finger.Index().ToString(), resultsTextStyle);
				if (GUI.Button(clearResultButtonRect, "", SampleGUI.Factory().clearButtonStyle)) {
					gestureOccurred = false;
				}
			}
		}
	}
	
	private string currentCall;

	void GestureSlice (SliceGesture gesture)
	{
		if (SampleGUI.sceneSelectionOpen || gesture.targetCollider == null) {
			return;
		}
		
		DisplaySlice(gesture.targetCollider.gameObject, gesture.sliceDirection); 
		gestureOccurred = true;
		
		if(!exploding) {
			Explode();
			StartCoroutine(Reset());
		}
	}
	
	protected override void SampleFingerDown()
	{
		gestureOccurred = false;
	}

	void DisplaySlice(GameObject targetObj, SwipeGesture.SwipeDirection direction)
	{
		if (targetObj == null) return;
		
		List<int> pieces;
			
		switch (direction) {
		case SwipeGesture.SwipeDirection.Up:
			pieces = new List<int>(new int[] {5,6,7,8});
			positiveHeading = false;
			heading = Vector3.up;
			break;
		case SwipeGesture.SwipeDirection.Plus45:
			pieces = new List<int>(new int[] {2,3,4,5});
			positiveHeading = true;
			heading = new Vector3(1, 1, 0);
			break;
		case SwipeGesture.SwipeDirection.Right:
			pieces = new List<int>(new int[] {3,4,5,6});
			positiveHeading = true;
			heading = Vector3.right;
			break;
		case SwipeGesture.SwipeDirection.Plus135:
			pieces = new List<int>(new int[] {4,5,6,7});
			positiveHeading = true;
			heading = new Vector3(1, -1, 0);
			break;
		case SwipeGesture.SwipeDirection.Down:
			pieces = new List<int>(new int[] {5,6,7,8});
			positiveHeading = true;
			heading = Vector3.down;
			SplitLogo(pieces);
			break;
		case SwipeGesture.SwipeDirection.Minus135:
			pieces = new List<int>(new int[] {2,3,4,5});
			positiveHeading = false;
			heading = new Vector3(-1, -1, 0);
			break;
		case SwipeGesture.SwipeDirection.Left:
			pieces = new List<int>(new int[] {3,4,5,6});
			positiveHeading = false;
			heading = Vector3.left;
			break;
		case SwipeGesture.SwipeDirection.Minus45:
			pieces = new List<int>(new int[] {4,5,6,7});
			positiveHeading = false;
			heading = new Vector3(-1, 1, 0);
			break;
		default:
			return;
		}
		SplitLogo(pieces);
	}
	
	//Physics
	
	void SplitLogo(List<int> pieces)
	{
		set1 = new Transform[pieces.Count];
		set2 = new Transform[parts.Length - pieces.Count];
		
		for (int i = 0; i < pieces.Count; i++)
			set1[i] = parts[pieces[i]-1];
		
		
		int j = 0;
		for (int i = 0; i < parts.Length; i++) {
			if (!pieces.Contains(i+1))
			{
				set2[j] = parts[i];
				j++;
			}
		}
	}
	
	IEnumerator Reset()
	{
		yield return new WaitForSeconds(1f);
		resetting = true;
		yield return new WaitForSeconds(1f);
		exploding = false;
	}
	
	void HardReset()
	{
		for(int i=0; i<parts.Length; i++)
		{
			parts[i].localPosition = Vector3.zero;
			parts[i].localRotation = Quaternion.identity;
			
			parts[i].GetComponent<Rigidbody>().Sleep();
		}
	}
	
	void Explode()
	{
		resetting = false;
		exploding = true;
		
		if(!positiveHeading) {
			directionalSpread = spread * -1;
		}
		else {
			directionalSpread = spread;
		}
			
		
		//set1
		for(int i=0; i < set1.Length; i++) {
			float random = Random.Range(minVelocity,minVelocity + randomRange);
			set1[i].GetComponent<Rigidbody>().velocity = Vector3.Cross(heading, Vector3.forward) * directionalSpread * random;
		}
		
		//set2
		for(int i=0; i < set2.Length; i++) {
			float random = Random.Range(minVelocity,minVelocity + randomRange);
			set2[i].GetComponent<Rigidbody>().velocity = Vector3.Cross(heading, Vector3.forward) * -directionalSpread * random;
		}
	}
}

