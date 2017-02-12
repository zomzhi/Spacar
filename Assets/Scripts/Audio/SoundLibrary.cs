using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MyCompany.MyGame.Audio
{
	public class SoundLibrary : MonoBehaviour
	{
		[System.Serializable]
		public class SoundGroup
		{
			public string groupID;
			public AudioClip[] group;

			public AudioClip GetRandomClip ()
			{
				return group [Random.Range (0, group.Length)];
			}
		}

		public SoundGroup[] soundGroups;

		Dictionary<string, SoundGroup> groupDictionary = new Dictionary<string, SoundGroup> ();

		void Awake ()
		{
			foreach (SoundGroup soundGroup in soundGroups)
			{
				groupDictionary.Add (soundGroup.groupID, soundGroup);
			}
		}

		public AudioClip GetClipFromName (string name)
		{
			if (groupDictionary.ContainsKey (name))
			{
				return groupDictionary [name].GetRandomClip ();
			}
			UnityLog.LogError ("Sound library doesn't exist audio clip " + name);
			return null;
		}
	}

}
