using UnityEngine;
using System.Collections;
using MyCompany;

public class TurnTest : MonoBehaviour
{
	/// <summary>
	/// 直线运动最大速度
	/// </summary>
	public float forwardMaxSpeed = 5f;

	/// <summary>
	/// 转向速度
	/// </summary>
	public float turnAngleSpeed = 10f;

	/// <summary>
	/// 转向后还未摆正角度的加速度
	/// </summary>
	public float forwardTurnAcce = 3f;

	/// <summary>
	/// 转向后摆正角度后的加速度
	/// </summary>
	public float fowardMoveAcce = 5f;

	/// <summary>
	/// 转向后之前直线运动方向上的减速度
	/// </summary>
	public float turnDeacce = 3f;

	/// <summary>
	/// 转向后目标角度的偏移角度
	/// </summary>
	public float offsetAngle = 30f;

	/// <summary>
	/// 转向后当之前直线上的速度小于这个值时开始摆正角度
	/// </summary>
	public float adjustTurnSpeed = 2f;

	Vector3 forwardDir;
	Vector3 turnDir;

	float forwardSpeed;
	float turnSpeed;
	float targetAngle;
	float turnOffsetAngle;
	float turnAngle;
	float forwardAcce;

	bool turn = false;
	bool adjust = false;

	static Quaternion TURN_ANGLE = Quaternion.Euler (0f, -90f, 0f);

	// Use this for initialization
	void Start ()
	{
		forwardDir = transform.forward;
		forwardSpeed = forwardMaxSpeed;
		turnSpeed = 0f;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.T))
		{
			turn = true;
			adjust = false;
			turnDir = forwardDir;
			forwardDir = TURN_ANGLE * turnDir;

			turnAngle = transform.eulerAngles.y - 90f;
			turnOffsetAngle = turnAngle - offsetAngle;
			targetAngle = turnOffsetAngle;

			turnSpeed = forwardSpeed;				// 按照之前的移动速度进行减速
			forwardSpeed = 0f;
			forwardAcce = forwardTurnAcce;
		}

		if (Input.GetKeyDown (KeyCode.U))
		{
			
		}

		UnityLog.Log ("normal FrameCount : " + Time.frameCount);

		if (turn)
		{
			if (turnSpeed < adjustTurnSpeed)
			{
				targetAngle = turnAngle;
				adjust = true;
				forwardAcce = fowardMoveAcce;
			}
			
			float angle = Mathf.MoveTowardsAngle (transform.eulerAngles.y, targetAngle, turnAngleSpeed * Time.deltaTime);

			transform.eulerAngles = new Vector3 (0f, angle, 0f);
			if (adjust && Mathf.Approximately (transform.eulerAngles.y, targetAngle))
				turn = false;
		}

		forwardSpeed += forwardAcce * Time.deltaTime;
		if (forwardSpeed > forwardMaxSpeed)
			forwardSpeed = forwardMaxSpeed;

		turnSpeed -= turnDeacce * Time.deltaTime;
		if (turnSpeed < 0)
			turnSpeed = 0f;

		transform.position += forwardSpeed * forwardDir * Time.deltaTime + turnSpeed * turnDir * Time.deltaTime;
	}
}

