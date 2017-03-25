using UnityEngine;
using System.Collections;

public class FingerParticle: MonoBehaviour {
	
	public Camera cam;
	public Transform particlePrefab;
	
	public ParticleEmitter[] emitters;
	
	private bool[] showParticles;
	
	private static FingerParticle _factory;
	
	public static FingerParticle Factory()
	{
		return _factory;
	}
	
	public void ShowParticles(bool show)
	{
		for (int i = 0; i < showParticles.Length; i++) {
			ShowParticle(show, i);
		}
	}
	public void ShowParticle(bool show, int fingerNum)
	{
		if (showParticles[fingerNum] == show) {
			return;
		}
		showParticles[fingerNum] = show;
		if (!show) {
			if (emitters[fingerNum] != null) {
				emitters[fingerNum].emit = false;
			}
		}
	}

	void Start ()
	{
		_factory = this;
		string scene = Application.loadedLevelName;
		if(scene == "LineGestures" || scene == "RotateGestures" || scene == "PinchGestures"  || scene == "Splash") {
			Destroy(this);
		}
		emitters = new ParticleEmitter[5];
		showParticles = new bool[5];
		for (int i = 0; i < showParticles.Length; i++) {
			showParticles[i] = true;
		}
	}
	
	void GestureStartTouch(TouchGesture touch)
	{
		InitEmitter(touch);
		xs[touch.finger.Index()] = touch.finger.startPosition.x;
	}
	
	void InitEmitter(TouchGesture touch) 
	{
		if (SampleGUI.sceneSelectionOpen || showParticles == null || !showParticles[touch.finger.Index()] ) {
			return;
		}
		if (emitters[touch.finger.Index()] == null) {
	        Transform clone = Instantiate(particlePrefab, Vector3.zero, Quaternion.identity) as Transform;
			clone.name = "Particles " + touch.finger.Index();
			emitters[touch.finger.Index()] = clone.GetComponent<ParticleEmitter>();
		}
	}
	
	private float[] xs = new float[5];
	void GestureMoveTouch(TouchGesture touch)
	{
		if (SampleGUI.sceneSelectionOpen || showParticles == null || !showParticles[touch.finger.Index()]) {
			return;
		}
//		if (touch.finger.position.x < xs[touch.finger.Index()]) {
//			Debug.Log("FingerParticle:GestureMoveTouch " + touch.finger.Index() + " " + xs[touch.finger.Index()] + " to "   + touch.finger.position);
//		}
		xs[touch.finger.Index()] = touch.finger.position.x;
		InitEmitter(touch);
		Vector3 touchPosition = touch.finger.position;
		touchPosition.z = 15;
		Vector3 position = cam.ScreenToWorldPoint(touchPosition);
		
		emitters[touch.finger.Index()].transform.position = position;
		emitters[touch.finger.Index()].emit = true;
	}
	
	void GestureEndTouch(TouchGesture touch)
	{
		if (showParticles != null && emitters != null && emitters[touch.finger.Index()] != null) {
			emitters[touch.finger.Index()].emit = false;
		}
	}
}
