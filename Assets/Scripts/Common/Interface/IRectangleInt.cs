using System;
using UnityEngine;

/* 
 * 整型矩形定义
 * LevelBlock及LevelBridge将继承此接口
 * Block模型的Pivot统一定义在左下角
 */

namespace MyCompany.Common.Interface
{
	public interface IRectangleInt
	{
		/// <summary>
		/// 宽度
		/// </summary>
		/// <value>The width.</value>
		int width{ get; }

		/// <summary>
		/// 高度
		/// </summary>
		/// <value>The height.</value>
		int height{ get; }

		/// <summary>
		/// 左下角的世界坐标
		/// </summary>
		/// <value>The left bottom.</value>
		Vector3 leftBottom{ get; }
	}
}

