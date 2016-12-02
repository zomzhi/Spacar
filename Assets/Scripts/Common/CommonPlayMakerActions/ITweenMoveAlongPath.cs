using UnityEngine;
using System.Collections;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory (ActionCategory.iTween)]
	public class ITweenMoveAlongPath : iTweenFsmAction
	{
		[RequiredField]
		public FsmOwnerDefault gameObject;

		[Tooltip ("The time in seconds the animation will take to complete.")]
		public FsmFloat time;
		[Tooltip ("The time in seconds the animation will wait before beginning.")]
		public FsmFloat delay;
		[Tooltip ("Can be used instead of time to allow animation based on speed. When you define speed the time variable is ignored.")]
		public FsmFloat speed;

		[Tooltip ("Whether to animate in local or world space.")]
		public Space space = Space.World;

		[Tooltip ("The shape of the easing curve applied to the animation.")]
		public iTween.EaseType easeType = iTween.EaseType.linear;
		[Tooltip ("The type of loop to apply once the animation has completed.")]
		public iTween.LoopType loopType = iTween.LoopType.none;

		[ActionSection ("LookAt")]
		[Tooltip ("Whether or not the GameObject will orient to its direction of travel. False by default.")]
		public FsmBool orientToPath;
		[Tooltip ("A target object the GameObject will look at.")]
		public FsmGameObject lookAtObject;
		[Tooltip ("A target position the GameObject will look at.")]
		public FsmVector3 lookAtVector;
		[Tooltip ("The time in seconds the object will take to look at either the Look Target or Orient To Path. 0 by default")]
		public FsmFloat lookTime;
		[Tooltip ("Restricts rotation to the supplied axis only.")]
		public iTweenFsmAction.AxisRestriction axis = iTweenFsmAction.AxisRestriction.none;

		[ActionSection ("Path")]
		[Tooltip ("Whether to automatically generate a curve from the GameObject's current position to the beginning of the path. True by default.")]
		public FsmBool moveToPath;
		[Tooltip ("Path array")]
		[ArrayEditor (VariableType.Vector3, "path")]
		public FsmArray pathArray;
		[Tooltip ("How much of a percentage (from 0 to 1) to look ahead on a path to influence how strict Orient To Path is and how much the object will anticipate each curve.")]
		public FsmFloat lookAhead;
		[Tooltip ("Reverse the path so object moves from End to Start node.")]
		public FsmBool reverse;

		private Vector3[] tempVec3;
		GameObject go;

		public override void OnDrawActionGizmos ()
		{
			if (tempVec3 == null)
				tempVec3 = GetPath ();
			iTween.DrawPathGizmos (tempVec3, Color.yellow);
		}

		public override void Reset ()
		{
			base.Reset ();
			gameObject = null;
			time = 1f;
			delay = 0f;
			speed = null;
			space = Space.World;

			orientToPath = true;
			lookAtObject = new FsmGameObject { UseVariable = true };
			lookAtVector = new FsmVector3 { UseVariable = true };
			lookTime = new FsmFloat { UseVariable = true };
			axis = iTweenFsmAction.AxisRestriction.none;

			moveToPath = true;
			lookAhead = new FsmFloat { UseVariable = true };
			pathArray = new FsmArray { UseVariable = true };
			reverse = false;
		}

		public override void OnEnter ()
		{
			base.OnEnteriTween (gameObject);
			if (loopType != iTween.LoopType.none)
				base.IsLoop (true);

			go = Fsm.GetOwnerDefaultTarget (gameObject);
			DoiTween ();
		}

		public override void OnExit ()
		{
			base.OnExitiTween (gameObject);
		}

		void DoiTween ()
		{
			if (go == null)
				return;

			tempVec3 = GetPath ();
			Hashtable hash = new Hashtable ();
			hash.Add ("path", tempVec3);
			hash.Add ("movetopath", moveToPath.IsNone ? true : moveToPath.Value);
			if (!orientToPath.IsNone)
				hash.Add ("orienttopath", orientToPath.Value);
			if (!lookAtObject.IsNone)
				hash.Add ("looktarget", lookAtObject.Value.transform);
			else if (!lookAtVector.IsNone)
				hash.Add ("looktarget", lookAtVector.Value);
			if (!lookAtObject.IsNone || !lookAtVector.IsNone)
				hash.Add ("looktime", lookTime.IsNone ? 0f : lookTime.Value);
			hash.Add ("lookahead", lookAhead.IsNone ? 1f : lookAhead.Value);
			hash.Add ("axis", axis == AxisRestriction.none ? "" : System.Enum.GetName (typeof(iTweenFsmAction.AxisRestriction), axis));

			hash.Add ("islocal", space == Space.Self);
			hash.Add (speed.IsNone ? "time" : "speed", speed.IsNone ? time.IsNone ? 1f : time.Value : speed.Value);
			hash.Add ("delay", delay.IsNone ? 0f : delay.Value);
			hash.Add ("easetype", easeType);
			hash.Add ("looptype", loopType);
			hash.Add ("ignoretimescale", realTime.IsNone ? false : realTime.Value);
			hash.Add ("oncomplete", "iTweenOnComplete");
			hash.Add ("oncompleteparams", itweenID);
			hash.Add ("onstart", "iTweenOnStart");
			hash.Add ("onstartparams", itweenID);

			itweenType = "move";
			iTween.MoveTo (go, hash);
		}

		Vector3[] GetPath ()
		{
			Vector3[] path = new Vector3[pathArray.Length];
			for (int i = 0; i < pathArray.Length; i++)
			{
				path [i] = ((Vector3)pathArray.Get (i));
			}
			return path;
		}
	}
}


