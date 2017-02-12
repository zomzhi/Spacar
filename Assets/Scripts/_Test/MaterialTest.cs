using UnityEngine;
using System.Collections;

public class MaterialTest : MonoBehaviour
{

	public Transform cubeTrans;

	public Material testMat;

	Material sharedMat;
	Material mat;
	int index;

	void Start ()
	{
		Material[] mats = cubeTrans.GetComponent<Renderer> ().sharedMaterials;
		for (int i = 0; i < mats.Length; i++)
		{
			if (mats [i].name.Contains ("Red"))
			{
				index = i;
				break;
			}
		}

		sharedMat = mats [index];
		mat = cubeTrans.GetComponent<Renderer> ().materials [index];
		mat.color = Color.yellow;

		testMat.color = Color.black;
	}


	void Update ()
	{
	
		if (Input.GetKeyDown (KeyCode.Space))
		{
			sharedMat.color = Color.blue;
		}

		if (Input.GetKeyDown (KeyCode.R))
		{
			cubeTrans.GetComponent<Renderer> ().material = sharedMat;
		}
	}
}

