using System;
using UnityEngine;

namespace MyCompany.Common
{
	public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
	{
		protected static T _instance;
		public static T Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = GameObject.FindObjectOfType (typeof(T)) as T;
					if (_instance == null)
						_instance = new GameObject ("Singleton of " + typeof(T).ToString (), typeof(T)).GetComponent<T> ();
				}
				return _instance;
			}
		}

		void Awake ()
		{
			if (_instance == null)
				_instance = this as T;
			else
			{
				UnityLog.LogError ("Already exist singleton instance " + typeof(T) + ", destroy new instance!");
				Destroy (this);
			}

			OnAwake ();
		}

		protected virtual void OnAwake ()
		{
			
		}

		void OnApplicationQuit ()
		{
			_instance = null;
		}
	}
}

