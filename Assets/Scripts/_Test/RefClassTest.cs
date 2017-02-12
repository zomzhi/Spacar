using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RefClassTest : MonoBehaviour
{

	public class RefClass
	{
		public List<int> intList;

		public RefClass ()
		{
			intList = new List<int> ();
		}

		public void AddInt (int a)
		{
			intList.Add (a);
		}

		public void Log ()
		{
			string str = "";
			foreach (int i in intList)
				str += i.ToString ();
			Debug.Log (str);
		}
	}

	// Use this for initialization
	void Start ()
	{
		RefClass rc = new RefClass ();
		SetupRefClass (rc, 1);
		SetupRefClass (rc, 3);
		rc.Log ();
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	void SetupRefClass (RefClass rf, int a)
	{
//		rf.AddInt (a);
		rf.intList.Add (a);
	}
}

