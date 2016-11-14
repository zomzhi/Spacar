using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UserSegmentList : List<UserSegment>
{
	public void AddSegment(UserSegment segment)
	{
		this.Add(segment);
	}
	
	public void UpdatePoint(int index, Vector3 pos, bool isStart)
	{
		this[index].UpdatePoint(pos, isStart);
		if (isStart) {
			if (index > 1) {
				this[index - 1].UpdatePoint(pos, false);
			}
		}
		else {
			if (index < (this.Count - 1)) {
				this[index + 1].UpdatePoint(pos, true);
			}
		}
	}
}

