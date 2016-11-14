using UnityEngine;
using System.Collections;

namespace MyCompany.MyGame.Player
{
	public class PlayerPhysics : MonoBehaviour
	{
		/// <summary>
		/// 水平移动最大速度
		/// </summary>
		public float horizonSpeedMax = 5f;

		/// <summary>
		/// 水平移动加速度
		/// </summary>
		public float horizonAcce = 5f;

		/// <summary>
		/// 当前水平移动速度
		/// </summary>
		float curHorizonVelocity;

		/// <summary>
		/// 目标水平移动速度
		/// </summary>
		float targetHorizonSpeed;

		private Transform thisTrans;

		void Awake ()
		{
			thisTrans = transform;
		}

		/// <summary>
		/// 由PlayerInput调用，每一帧Update都会执行的函数
		/// 角色的所有物理计算都在这里
		/// </summary>
		/// <param name="moveX">Move x.</param>
		public void Move (float moveX)
		{
			
			if (Mathf.Approximately (moveX, 0f))
				targetHorizonSpeed = 0f;
			else
				targetHorizonSpeed = moveX > 0 ? horizonSpeedMax : -horizonSpeedMax;
			

			if (!Mathf.Approximately (curHorizonVelocity, targetHorizonSpeed))
			{
				if (curHorizonVelocity < targetHorizonSpeed)
				{
					curHorizonVelocity += horizonAcce * Time.deltaTime;
					if (curHorizonVelocity > targetHorizonSpeed)
						curHorizonVelocity = targetHorizonSpeed;
				}
				else if (curHorizonVelocity > targetHorizonSpeed)
				{
					curHorizonVelocity -= horizonAcce * Time.deltaTime;
					if (curHorizonVelocity < targetHorizonSpeed)
						curHorizonVelocity = targetHorizonSpeed;
				}
			}
				
			thisTrans.Translate (curHorizonVelocity * Time.deltaTime, 0f, 0f);
		}
	}
}

