using UnityEngine;
using System.Collections;

namespace MyCompany.MyGame.Audio
{
	public class AudioManager : MonoBehaviour
	{
		private int volume = 0;

		public bool Mute
		{
			get
			{
				return volume == 0;
			}
			set
			{
				volume = value ? 0 : 1;
				musicSources [0].volume = (float)volume;
				musicSources [1].volume = (float)volume;
				sfx2DSource.volume = (float)volume;
				PlayerPrefs.SetInt ("mute", volume);		// 0 - mute
				PlayerPrefs.Save ();
			}
		}

		public BeatDetection beatDetection;

		SoundLibrary library;

		AudioSource sfx2DSource;
		AudioSource[] musicSources;
		int activeMusicSourceIndex;

		void Awake ()
		{
			beatDetection = GetComponent<BeatDetection> ();
			if (beatDetection == null)
				beatDetection = gameObject.AddComponent<BeatDetection> ();
			library = GetComponent<SoundLibrary> ();

			// Get player preference
			volume = PlayerPrefs.GetInt ("mute", 1);

			musicSources = new AudioSource[2];
			for (int i = 0; i < 2; i++)
			{
				GameObject newMusicSource = new GameObject ("Music source " + (i + 1));
				musicSources [i] = newMusicSource.AddComponent<AudioSource> ();
//				musicSources [i].volume = (float)volume;
				musicSources [i].volume = 0;
				newMusicSource.transform.parent = transform;
			}
			GameObject newSfx2DSource = new GameObject ("2D sfx source");
			sfx2DSource = newSfx2DSource.AddComponent<AudioSource> ();
			sfx2DSource.volume = (float)volume;
			newSfx2DSource.transform.parent = transform;
		}

		public void PlayMusic (AudioClip clip, float fadeDuration = 1f)
		{
			activeMusicSourceIndex = 1 - activeMusicSourceIndex;
			musicSources [activeMusicSourceIndex].clip = clip;
			musicSources [activeMusicSourceIndex].Play ();
			beatDetection.SetAudioSource (musicSources [activeMusicSourceIndex]);

			StopCoroutine (AnimateMusicCrossfade (fadeDuration));
			StartCoroutine (AnimateMusicCrossfade (fadeDuration));
		}

		public void StopPlayMusic ()
		{
			musicSources [activeMusicSourceIndex].Stop ();
		}

		public void PlaySound (AudioClip clip, Vector3 pos)
		{
			if (clip != null)
			{
				AudioSource.PlayClipAtPoint (clip, pos, (float)volume);
			}
		}

		public void PlaySound (string soundName, Vector3 pos)
		{
			PlaySound (library.GetClipFromName (soundName), pos);
		}

		public void PlaySound2D (string soundName, float volumeScale = 1f)
		{
			sfx2DSource.PlayOneShot (library.GetClipFromName (soundName), volumeScale * volume);
		}

		IEnumerator AnimateMusicCrossfade (float duration)
		{
			float percent = 0f;
			float activeSourceVolume = musicSources [activeMusicSourceIndex].volume;
			float inactiveSourceVolume = musicSources [1 - activeMusicSourceIndex].volume;

			while (percent < 1)
			{
				percent += Time.deltaTime * 1 / duration;
				musicSources [activeMusicSourceIndex].volume = Mathf.Lerp (activeSourceVolume, (float)volume, percent);
				musicSources [1 - activeMusicSourceIndex].volume = Mathf.Lerp (inactiveSourceVolume, 0f, percent);
				yield return null;
			}
		}
	}
}

