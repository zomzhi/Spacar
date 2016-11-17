using UnityEngine;
using System.Collections;
using MyCompany.MyGame.Level;
using HutongGames.PlayMaker;

namespace MyCompany.MyGame.NPC
{
	public class NPCController : MonoBehaviour
	{
		#region Attribute

		public Vector3 Position
		{
			get{ return thisTrans.position; }
			set{ thisTrans.position = value; }
		}

		protected Transform thisTrans;
		public Transform Trans{ get { return thisTrans; } }

		#endregion

		#region Private Member

		PlayMakerFSM motionPMFsm;
		Fsm motionFsm;

		#endregion

		protected virtual void Awake ()
		{
			motionPMFsm = GetComponent<PlayMakerFSM> ();
			motionFsm = motionPMFsm.Fsm;
			thisTrans = transform;
		}
	}
}

