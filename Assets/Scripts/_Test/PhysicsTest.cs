using UnityEngine;
using System.Collections;

public class PhysicsTest : MonoBehaviour
{

	public bool useCustomPhysics = true;

	public float initSpeed = 5f;
	public float gravity;

	Vector3 startPos;
	Rigidbody rigid;
	bool startSimulate = false;
	float verticalVelocity;

	void Awake ()
	{
		rigid = GetComponent<Rigidbody> ();
	}

	void Start ()
	{
		startPos = transform.position;
		Physics.gravity = new Vector3 (0f, gravity, 0f);
	}

	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.R))
		{
			Reset ();
		}

		if (Input.GetKeyDown (KeyCode.Space))
		{
			StartSimulate ();
		}

	}

	void FixedUpdate ()
	{
		if (useCustomPhysics && startSimulate)
		{
			verticalVelocity += gravity * Time.deltaTime;
			transform.position += verticalVelocity * Vector3.up * Time.deltaTime;
		}
	}

	void StartSimulate ()
	{
		if (useCustomPhysics)
		{
			startSimulate = true;
			verticalVelocity = initSpeed;
		}
		else
		{
			rigid.isKinematic = false;
			rigid.velocity = new Vector3 (0f, initSpeed, 0f);
		}
	}

	void Reset ()
	{
		startSimulate = false;
		transform.position = startPos;
		if (rigid != null)
			rigid.isKinematic = true;
	}
}
