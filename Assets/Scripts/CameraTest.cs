using UnityEngine;
using System.Collections;

public class CameraTest : MonoBehaviour
{

	public Vector3 rotateAngle = Vector3.zero;
	public Vector3 relativeDir = Vector3.one;
	public Transform parent;


	void Awake ()
	{
		
	}

	void Update ()
	{
		parent.rotation = Quaternion.Euler (rotateAngle);

		transform.position = parent.position + parent.localRotation * relativeDir;
		transform.rotation = parent.rotation;
	}
}

