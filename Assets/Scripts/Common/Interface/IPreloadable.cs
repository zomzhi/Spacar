using System;
using UnityEngine;

namespace MyCompany.Common.Interface
{
	public interface IPreloadable
	{
		/// <summary>
		/// 是否需要预加载
		/// </summary>
		/// <value><c>true</c> if this instance is preload; otherwise, <c>false</c>.</value>
		bool IsPreload{ get; }

		/// <summary>
		/// 预加载的数目
		/// </summary>
		/// <value>The preload amount.</value>
		int PreloadAmount{ get; }

		/// <summary>
		/// 加载需要的帧数
		/// </summary>
		/// <value>The preload frames.</value>
		int PreloadFrames{ get; }
	}
}