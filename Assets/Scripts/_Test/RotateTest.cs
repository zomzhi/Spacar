using UnityEngine;
using System.Collections;

public class RotateTest : MonoBehaviour
{

	public float raiseSpeed = 30f;
	public float spinSpeed = 60f;

	bool rotate = false;

	Vector3 curForward;
	Vector3 initRight;

	float spinAngle = 0f;
	float raiseAngle = 0f;

	// Use this for initialization
	void Start ()
	{
		curForward = transform.forward;
		initRight = transform.right;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.R))
		{
			rotate = true;
		}

		if (rotate)
		{
			raiseAngle = Mathf.MoveTowards (raiseAngle, -90f, raiseSpeed * Time.deltaTime);
			Quaternion raiseRot = Quaternion.AngleAxis (raiseAngle, Vector3.right);
			Debug.Log (raiseAngle);

			spinAngle -= spinSpeed * Time.deltaTime;
			Quaternion spinRot = Quaternion.AngleAxis (spinAngle, Vector3.forward);

			transform.rotation = raiseRot * spinRot;
		}
	}
}

