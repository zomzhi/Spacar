using UnityEngine;
using System.Collections;
using MyCompany.MyGame.Level;
using HutongGames.PlayMaker;
using MyCompany.MyGame.Data.Character;

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

		public float MoveSpeed{ get; set; }

		public float RotateSpeed{ get; set; }

		#endregion

		#region Public Member

		public CharacterAttribute characterAttribute;

		#endregion

		#region Private Member

		protected PlayMakerFSM motionPMFsm;
		protected Fsm motionFsm;

		#endregion

		protected virtual void Awake ()
		{
			motionPMFsm = GetComponent<PlayMakerFSM> ();
			motionFsm = motionPMFsm.Fsm;
			thisTrans = transform;

			if (characterAttribute != null)
			{
				MoveSpeed = characterAttribute.moveSpeed;
				RotateSpeed = characterAttribute.rotateSpeed;
			}
			else
			{
				UnityLog.LogError ("CharacterAttribute is not assigned!");
			}
		}
	}
}

