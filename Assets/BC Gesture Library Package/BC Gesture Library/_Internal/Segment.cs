using UnityEngine;
using System.Collections;

public class Segment
{
	public enum SizeSpec {Ratio, Bigger, Biggest, Smaller, Smallest, Ignore};
	
	public SwipeGesture.SwipeDirection direction;
	public SwipeGesture.SwipeDirection optionalDirection;
	public int relativeSize;
	public SizeSpec sizeSpec = SizeSpec.Ratio;
	private float _matchLengthDiffPercent = 0;  // decimal per cent
	
	public SwipeGesture.SwipeDirection directionReverse;
	public SwipeGesture.SwipeDirection optionalDirectionReverse;
	

	public float matchLengthDiffPercent {
		set  {
			 _matchLengthDiffPercent = value; 
		}
	}
	

	public float getMatchLengthDiffPercent(LineGesture.LineIdentification identification, float defaultValue)
	{
		if (_matchLengthDiffPercent <= 0) {
			_matchLengthDiffPercent = defaultValue;
		}
		if (identification == LineGesture.LineIdentification.Sloppy) {
			return _matchLengthDiffPercent * LineSwipeBase.matchLengthDiffPercentSloppyMultiplier;
		}
		return _matchLengthDiffPercent;
	}
	
	public Segment()
	{
	}
	
		
	public Segment(SwipeGesture.SwipeDirection directionIn, int relativeSizeIn, SizeSpec sizeSpecIn)
	{
		relativeSize = relativeSizeIn;
		relativeSize = relativeSizeIn;
		direction = directionIn;
		sizeSpec = sizeSpecIn;
		optionalDirection = directionIn;		
		directionReverse = SwipeGesture.GetReverseDirection(direction);
		optionalDirectionReverse =  directionReverse;
	}
	
	public Segment(SwipeGesture.SwipeDirection directionIn, int relativeSizeIn, SwipeGesture.SwipeDirection optionalDrectionIn, SizeSpec sizeSpecIn)
	{
		direction = directionIn;
		optionalDirection = optionalDrectionIn;		
		sizeSpec = sizeSpecIn;
		relativeSize = relativeSizeIn;
		directionReverse = SwipeGesture.GetReverseDirection(direction);
		optionalDirectionReverse =  SwipeGesture.GetReverseDirection(optionalDirection);
	}
	
	public string DebugStringDirections(bool isForward)
	{
		if (direction == optionalDirection) {
			if (isForward) {
				return direction.ToString();
			}
			else {
				return directionReverse.ToString();
			}
		}
		else {
			if (isForward) {
				return direction.ToString() + " or " + optionalDirection.ToString();
			}
			else {
				return directionReverse.ToString() + " or " + optionalDirectionReverse.ToString();
			}
		}
	}
	
	public bool CompareDirection(LineGesture.LineIdentification identification, SwipeGesture.SwipeDirection swipeDirection, bool isForward)
	{
		if (isForward) {
			return CompareDirectionForward(identification, swipeDirection);
		}
		else {
			return CompareDirectionReverse(identification, swipeDirection);
		}
	}
	
	
	public bool CompareDirectionForward(LineGesture.LineIdentification identification, SwipeGesture.SwipeDirection swipeDirection)
	{
		if (swipeDirection == direction || swipeDirection == optionalDirection) {
			return true;
		}
		if (identification == LineGesture.LineIdentification.Sloppy) {
			if (FingerControl.FriendlySwipeDirections(direction, swipeDirection) ||  FingerControl.FriendlySwipeDirections(optionalDirection, swipeDirection)) {
				return true;
			}
			
		}
		//Debug.Log("Segment:Compare failed " + swipeDirection + " not equal to " + direction + " or " + optionalDirection);
		return false;
	}
	public bool CompareDirectionReverse(LineGesture.LineIdentification identification, SwipeGesture.SwipeDirection swipeDirection)
	{
		
		if (swipeDirection == directionReverse ||
		    swipeDirection == optionalDirectionReverse) {
			return true;
		}
		if (identification == LineGesture.LineIdentification.Sloppy) {
			if (FingerControl.FriendlySwipeDirections(directionReverse, swipeDirection) ||  FingerControl.FriendlySwipeDirections(optionalDirectionReverse, swipeDirection)) {
				return true;
			}
			
		}
		//Debug.Log("Segment:CompareReverse failed " + swipeDirection + " not equal to " + directionReverse + " or " + optionalDirectionReverse);
		return false;
	}
		
}

