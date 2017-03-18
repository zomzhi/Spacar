using UnityEngine;
using System.Collections;

public class SwipeTest : MonoBehaviour
{

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	void GestureSwipe (SwipeGesture gesture)
	{
		Debug.Log ("SwipeGesture  swipe happened " + gesture.swipeDirection + " went from " + gesture.startPosition + " to " + gesture.endPosition);
	}

	void GestureSwipeMove (SwipeGesture gesture)
	{	
		Debug.Log ("SwipeGesture  move happened " + gesture.swipePosition + " with direction " + gesture.swipeDirection);
	}

	void GestureSwipeEnd (SwipeGesture gesture)
	{
		Debug.Log ("SwipeGesture  end swipe happened " + gesture.endPosition);
	}
}
