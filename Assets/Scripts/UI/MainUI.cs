using UnityEngine;
using System.Collections;

namespace MyCompany.MyGame.UI
{
	public class MainUI : MonoBehaviour
	{

		#region Public Member



		#endregion


		#region Private Member



		#endregion


		#region Private Methods

		void HideUI ()
		{
			
		}

		#endregion


		#region Public Methods

		public IEnumerator ShowLoadImage ()
		{
			yield return new WaitForSeconds (1f);
		}

		public void StartPlay ()
		{
			HideUI ();
			GameSystem.Instance.StartPlay ();
		}

		#endregion
	}
}

