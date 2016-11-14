
using System;
using System.Collections.Generic;
using UnityEngine;

// fixed by roger
// change DynamicInvoke into delegate
// ten times faster


namespace MyCompany.Common.Signal
{
	public delegate void funsig ();
	public delegate void funsig<T> (T t1);
	public delegate void funsig<T1, T2> (T1 t1, T2 t2);
	public delegate void funsig<T1, T2, T3> (T1 t1, T2 t2, T3 t3);
	public delegate void funsig<T1, T2, T3, T4> (T1 t1, T2 t2, T3 t3, T4 t4);
	public delegate void funsig<T1, T2, T3, T4, T5> (T1 t1, T2 t2, T3 t3, T4 t4, T5 t5);

	public delegate T funsigret<T> ();
	public delegate T funsigret<T, T1> (T1 t1);
	public delegate T funsigret<T, T1, T2> (T1 t1, T2 t2);
	public delegate T funsigret<T, T1, T2, T3> (T1 t1, T2 t2, T3 t3);
	public delegate T funsigret<T, T1, T2, T3, T4> (T1 t1, T2 t2, T3 t3, T4 t4);
	public delegate T funsigret<T, T1, T2, T3, T4, T5> (T1 t1, T2 t2, T3 t3, T4 t4, T5 t5);

	// define event name here
	public class SignalMgr
	{

		public static SignalMgr instance = new SignalMgr ();
		List<System.Delegate> _msgMap = null;

		public SignalMgr ()
		{
			_msgMap = new List<Delegate> ();

			for (int i = 0; i < (int)GAME_EVT.MAX; ++i)
			{
				_msgMap.Add (null);
			}
		}

		public void Dispose ()
		{
			_msgMap.Clear ();
		}

		public void Subscribe (GAME_EVT signal, Delegate listener)
		{			
			if (listener == null)
			{
				return;
			}

			Delegate func = _msgMap [(int)signal];
			if (func != null)
			{
				func = Delegate.Combine (func, listener);
			}
			else
			{
				func = listener;
			}
			_msgMap [(int)signal] = func;
		}

		public void Unsubscribe (GAME_EVT signal, Delegate listener)
		{		
			if (listener == null)
			{
				return;
			}

			Delegate func = _msgMap [(int)signal];
			if (func != null)
			{
				func = Delegate.RemoveAll (func, listener);
			}
			_msgMap [(int)signal] = func;
		}

		public void Raise (GAME_EVT signal)
		{
			Delegate func = _msgMap [(int)signal];
			if (func != null)
			{
				funsig tmp = (funsig)func;
				tmp ();
			}			
		}

		public void Raise<T1> (GAME_EVT signal, T1 args)
		{
			Delegate func = _msgMap [(int)signal];
			if (func != null)
			{
				funsig<T1> tmp = (funsig<T1>)func;
				tmp (args);
			}			
		}


		public void Raise<T1, T2> (GAME_EVT signal, T1 arg1, T2 arg2)
		{
			Delegate func = _msgMap [(int)signal];
			if (func != null)
			{
				funsig<T1, T2> tmp = (funsig<T1, T2>)func;
				tmp (arg1, arg2);
			}			
		}

		public void Raise<T1, T2, T3> (GAME_EVT signal, T1 arg1, T2 arg2, T3 arg3)
		{
			Delegate func = _msgMap [(int)signal];
			if (func != null)
			{
				funsig<T1, T2, T3> tmp = (funsig<T1, T2, T3>)func;
				tmp (arg1, arg2, arg3);
			}			
		}


		public void Raise<T1, T2, T3, T4> (GAME_EVT signal, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
		{
			Delegate func = _msgMap [(int)signal];
			if (func != null)
			{
				funsig<T1, T2, T3, T4> tmp = (funsig<T1, T2, T3, T4>)func;
				tmp (arg1, arg2, arg3, arg4);
			}			
		}

		public void Raise<T1, T2, T3, T4, T5> (GAME_EVT signal, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
		{
			Delegate func = _msgMap [(int)signal];
			if (func != null)
			{
				funsig<T1, T2, T3, T4, T5> tmp = (funsig<T1, T2, T3, T4, T5>)func;
				tmp (arg1, arg2, arg3, arg4, arg5);
			}			
		}


		public T RaiseRet<T> (GAME_EVT signal) where T : new()
		{
			Delegate func = _msgMap [(int)signal];
			if (func != null)
			{
				funsigret<T> tmp = (funsigret<T>)func;
				return tmp ();
			}		
			return new T ();	
		}

		public T RaiseRet<T, T1> (GAME_EVT signal, T1 args) where T : new()
		{
			Delegate func = _msgMap [(int)signal];
			if (func != null)
			{
				funsigret<T, T1> tmp = (funsigret<T, T1>)func;
				return tmp (args);
			}		
			return new T ();
		}


		public T RaiseRet<T, T1, T2> (GAME_EVT signal, T1 arg1, T2 arg2) where T : new()
		{
			Delegate func = _msgMap [(int)signal];
			if (func != null)
			{
				funsigret<T, T1, T2> tmp = (funsigret<T, T1, T2>)func;
				return tmp (arg1, arg2);
			}		
			return new T ();
		}

		public T RaiseRet<T, T1, T2, T3> (GAME_EVT signal, T1 arg1, T2 arg2, T3 arg3) where T : new()
		{
			Delegate func = _msgMap [(int)signal];
			if (func != null)
			{
				funsigret<T, T1, T2, T3> tmp = (funsigret<T, T1, T2, T3>)func;
				return tmp (arg1, arg2, arg3);
			}		
			return new T ();
		}


		public T RaiseRet<T, T1, T2, T3, T4> (GAME_EVT signal, T1 arg1, T2 arg2, T3 arg3, T4 arg4) where T : new()
		{
			Delegate func = _msgMap [(int)signal];
			if (func != null)
			{
				funsigret<T, T1, T2, T3, T4> tmp = (funsigret<T, T1, T2, T3, T4>)func;
				return tmp (arg1, arg2, arg3, arg4);
			}		
			return new T ();
		}

		public T RaiseRet<T, T1, T2, T3, T4, T5> (GAME_EVT signal, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) where T : new()
		{
			Delegate func = _msgMap [(int)signal];
			if (func != null)
			{
				funsigret<T, T1, T2, T3, T4, T5> tmp = (funsigret<T, T1, T2, T3, T4, T5>)func;
				return tmp (arg1, arg2, arg3, arg4, arg5);
			}		
			return new T ();
		}
	}
}
