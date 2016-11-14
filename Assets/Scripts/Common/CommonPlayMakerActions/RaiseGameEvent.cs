using System;
using UnityEngine;
using MyCompany.Common.Signal;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory ("MyCompany Action")]
	public class RaiseGameEvent : FsmStateAction
	{

		//		[RequiredField]
		//		[Tooltip ("Game Event Name")]
		//		public FsmString eventName;

		public GAME_EVT eventEnum;

		public FsmBool enterSend;

		public FsmBool everyFrame;

		public FsmBool exitSend;

		//		GAME_EVT sendEvent;

		public override void OnEnter ()
		{
//			sendEvent = (GAME_EVT)Enum.Parse (typeof(GAME_EVT), eventName.Value);
//			if (sendEvent == null)
//			{
//				Debug.LogError ("Cannot parse game event : " + eventName.Value);
//				Finish ();
//				return;
//			}

			if (eventEnum == GAME_EVT.MAX)
			{
				Finish ();
				return;
			}

			if (!enterSend.IsNone && enterSend.Value)
				SignalMgr.instance.Raise (eventEnum);

			if ((everyFrame.IsNone || !everyFrame.Value) && (exitSend.IsNone || !exitSend.Value))
				Finish ();
		}

		public override void OnUpdate ()
		{
			if (!everyFrame.IsNone && everyFrame.Value)
				SignalMgr.instance.Raise (eventEnum);
		}

		public override void OnExit ()
		{
			if (!exitSend.IsNone && exitSend.Value)
				SignalMgr.instance.Raise (eventEnum);
		}
	}

}

