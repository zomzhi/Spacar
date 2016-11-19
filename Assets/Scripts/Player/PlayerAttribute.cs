using System;
using UnityEngine;

namespace MyCompany.MyGame.Data.Character
{
	[CreateAssetMenu (menuName = "Attribute/PlayerAttribute Attribute")]
	public class PlayerAttribute : ScriptableObject
	{
		/// <summary>
		/// 水平移动最大速度
		/// </summary>
		public float horizonSpeedMax = 5f;

		/// <summary>
		/// 水平移动加速度
		/// </summary>
		public float horizonAcceleration = 5f;

		/// <summary>
		/// 向前移动最大速度
		/// </summary>
		public float forwardSpeedMax = 5f;

		/// <summary>
		/// 向前移动加速度
		/// </summary>
		public float forwardAcceleration = 5f;

		/// <summary>
		/// 转向角速度
		/// </summary>
		public float turnAngleSpeed = 60f;

		/// <summary>
		/// 转向后还未摆正角度的加速度
		/// </summary>
		public float forwardTurnAcce = 3f;

		/// <summary>
		/// 转向时直线运动方向上的减速度
		/// </summary>
		public float turnDeceleration = 3f;

		/// <summary>
		/// 转向时直线方向速度减为0需要耗费的时间
		/// 用时间而不是用加速度来减速是为了保证速度过快时转向也能迅速完成
		/// </summary>
		public float turnDecelerationTime = 1.25f;

		/// <summary>
		/// 转向后目标角度的偏移角度
		/// </summary>
		public float turnOffsetAngle = 20f;

		/// <summary>
		/// 转向后当之前直线上的速度小于这个值时开始摆正角度
		/// </summary>
		public float adjustTurnSpeed = 2f;

		/// <summary>
		/// 从任意朝向旋转到正常朝向的速度
		/// </summary>
		public float toNormalAngleSpeed = 120f;

		#region Gravity stuff

		/// <summary>
		/// 距离地面的高度
		/// </summary>
		public float touchGroundHeight = 0.5f;

		/// <summary>
		/// 重力加速度
		/// </summary>
		public float gravity = 10f;

		/// <summary>
		/// 最大下落速度
		/// </summary>
		public float fallMaxSpeed = 20f;

		#endregion

		#region Jump To Wall

		/// <summary>
		/// 跃起的初速度
		/// </summary>
		public float jumpSpeed = 10f;

		/// <summary>
		/// Forward抬起变为Up的角速度
		/// </summary>
		public float raiseAngleSpeed = 60f;

		/// <summary>
		/// 空中旋转的角速度
		/// </summary>
		public float spinAngleSpeed = 60f;

		/// <summary>
		/// 跳跃到墙上后其初始向前速度为之前跳跃方向的速度，
		/// 最小为此值
		/// </summary>
		public float jumpWallMinSpeed = 5f;

		#endregion

		#region ExceedBridge stuff

		/// <summary>
		/// 超出bridge后最高可达到的高度
		/// </summary>
		public float exceedMaxHeight = 10f;

		/// <summary>
		/// 当超出bridge后头部会逐渐往下低
		/// 到达最高点时头部角度偏移为maxHeightHeadAngle
		/// 其后下降时使用此旋转角速度
		/// </summary>
		public float lowHeadAngleSpeed = 30f;

		/// <summary>
		/// 到达最高点时头部角度偏移
		/// </summary>
		public float maxHeightHeadAngle = 45f;

		/// <summary>
		/// 头部下低偏移的最低值
		/// </summary>
		public float minFallHeadAngle = 135f;

		/// <summary>
		/// 超出bridge后其下一个bridge的forward初速度为
		/// 之前速度与此乘数的积
		/// </summary>
		public float exceedForwardSpeedMultiplier = 0.25f;

		/// <summary>
		/// 翻滚一周花费的时间
		/// 在超出bridge至下落过程中根据总时间算出
		/// 翻滚的周数并得到旋转角速度
		/// </summary>
		public float rollOneCircleTime = 1f;


		#endregion
	}
}
 
