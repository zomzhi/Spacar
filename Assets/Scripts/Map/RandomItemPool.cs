using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class RandomItemPool<T> : IDisposable
{
	private List<T> itemPoolList;

	public RandomItemPool ()
	{
		Initialize ();
	}

	void Initialize ()
	{
		itemPoolList = new List<T> ();
	}

	public void AddItem (T item)
	{
		if (itemPoolList == null)
			Initialize ();
	}

	public bool Next (ref T randomItem, bool cullItem = false)
	{
		if (itemPoolList == null || itemPoolList.Count == 0)
			return false;

		randomItem = itemPoolList [UnityEngine.Random.Range (0, itemPoolList.Count)];
		if (cullItem)
			itemPoolList.Remove (randomItem);

		return true;
	}

	public void Dispose ()
	{
		itemPoolList.Clear ();
		itemPoolList = null;
	}
}