using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CompoundLineSwipe : LineSwipeBase
{
	
	public List<LineSwipe> lineSwipes = new List<LineSwipe>();
	public List<LineRelationship> lineRelationships = new List<LineRelationship>();
	
	private int maxSegmentCount = 0;
	private float positionDiff;

	public CompoundLineSwipe(string nameIn) : base(nameIn)
	{
	}
	
	public void AddLine(LineSwipe lineSwipe)
	{
		lineSwipes.Add(lineSwipe);
		int segmentCount = lineSwipe.Count();
		if (segmentCount > maxSegmentCount) {
			maxSegmentCount = segmentCount;
		}
	}
	
	public void AddRelationship(LineRelationship lineRelationship)
	{
		lineRelationships.Add(lineRelationship);
	}
	
	public override int GetMaxSegment()
	{
		return maxSegmentCount;
	}
	
	public override int Count()
	{
		return lineSwipes.Count;
	}
	
	public LineSwipe GetLine(int index)
	{
		if (index < 0 || index >= lineSwipes.Count) {
			return null;
		}
		return lineSwipes[index];
	}
	
	public override bool Compare(List<SwipeSegmentList> swipeList, LineGesture.LineSwipeDirection restrictLineSwipeDirection, float positionDiff, float lengthDiffPercent, LineGesture.LineIdentification lineIdentification)
	{
		//Debug.Log("CompoundLineSwipe:Compare " + name);
		if (lineSwipes.Count != swipeList.Count) {
			//Debug.Log("CompoundLineSwipe:Compare " + name + " failed on swipes count got " + lineSwipes.Count + " wanting " + swipeList.Count);
			return false;
		}
		
		compressedSwipeSegments = null;
		compressedSwipeSegmentsList = null;
		
		if (!CompareLocal(swipeList, restrictLineSwipeDirection, positionDiff, lengthDiffPercent, false, lineIdentification)) {
			//Debug.Log("CompoundLineSwipe:Compare " + name + " CompareLocal failed");
			compressedSwipeSegments = null;
			compressedSwipeSegmentsList = null;
			return false;
		}
		
		//
		// matched directions, now the hard part - check lengths and positions
		
		return true;
		
	}
	
	protected bool CompareLocal(List<SwipeSegmentList> swipeList, LineGesture.LineSwipeDirection restrictLineSwipeDirection, float positionDiffIn, float lengthDiffPercent, bool compareLocal, LineGesture.LineIdentification lineIdentification)
	{	
		identificationUsed = lineIdentification;
		restrictLineSwipeDirectionUsed = restrictLineSwipeDirection;
		positionDiff = positionDiffIn;
		int[] founds = new int[swipeList.Count];
		int i;
		for (i = 0; i < swipeList.Count; i++) {
			founds[i] = -1;
		}
		SwipeSegmentList swipeSegments;
		
		bool found;
		int j;
		//Debug.Log("CompoundLineSwipe:CompareLocal " + name + " loop for " + swipeList.Count + " " + lineIdentification);
		for (i = 0; i < swipeList.Count; i++) {
			swipeSegments = swipeList[i];			
			found = false;
			
			for (j = 0; j < lineSwipes.Count; j++) {
				if (founds[j] > -1) {
					//Debug.Log("CompoundLineSwipe:CompareLocal " + name + " skip already found lineSwipes " + j + " using " + founds[j]);
					continue;
				}
				//Debug.Log("CompoundLineSwipe:CompareLocal " + name + " compare swipelist " + i + " with lineSwipes " + j);
				lineSwipes[j].doCompareLengths = compareLocal;
				found  = lineSwipes[j].Compare(swipeSegments, restrictLineSwipeDirection, positionDiff, lengthDiffPercent, lineIdentification);
				
				if (found) {
					if (lineSwipes[j].compressedSwipeSegments != null) {
						lineSwipes[j].matchedSwipeSegments = lineSwipes[j].compressedSwipeSegments;
						//Debug.Log("CompoundLineSwipe:CompareLocal " + name + " compare success on " + j + " returned compressedSwipeSegments " + lineSwipes[j].compressedSwipeSegments.Count + " isFoward " + lineSwipes[j].compressedSwipeSegments.isForwardUsed);
					}
					else {
						lineSwipes[j].matchedSwipeSegments = swipeSegments;
						//Debug.Log("CompoundLineSwipe:CompareLocal " + name + " compare success on " + j + " returned original segments " + swipeSegments.Count + " isFoward " + swipeSegments.isForwardUsed);
					}
					//Debug.Log("CompoundLineSwipe:CompareLocal " + name + " success matched SwipeSegments " + j + " match count " + lineSwipes[j].matchedSwipeSegments.Count + " isFoward " + lineSwipes[j].matchedSwipeSegments.isForwardUsed);
				
					break;
				}
			}
			if (!found) {
				//Debug.Log("CompoundLineSwipe:CompareLocal " + name + " match failed swipeList " + i);
				SetError("Swipe " + j + " was not found", false);
				return false;
			}
			//Debug.Log("CompoundLineSwipe:CompareLocal " + name + " found in compound list at " + j);
			founds[j] = i;
			if (lineSwipes[j].compressedSwipeSegments != null) {
				if (compressedSwipeSegmentsList == null) {
					compressedSwipeSegmentsList = new SwipeSegmentList[lineSwipes.Count];
				}
				compressedSwipeSegmentsList[i] = lineSwipes[j].compressedSwipeSegments;
				//Debug.Log("CompoundLineSwipe:CompareLocal " + name + " add compressedSwipeSegments " + j + " isFoward " + compressedSwipeSegmentsList[i].isForwardUsed);
			}
		}
		
		List<SwipeSegmentList> theSwipeList = new List<SwipeSegmentList>();
		for (i = 0; i < lineSwipes.Count; i++) {
			int swipeListIndex = founds[i];
			if (compressedSwipeSegmentsList != null && compressedSwipeSegmentsList[i] != null) {
				//Debug.Log("CompoundLineSwipe:CompareLocal " + name + " using  compressedSwipeSegmentsList " + i + " length " + compressedSwipeSegmentsList[i].Count);
				theSwipeList.Add(compressedSwipeSegmentsList[i]);
			}
			else {
				if (compressedSwipeSegmentsList != null) {
					//Debug.Log("CompoundLineSwipe:CompareLocal " + name + " no compressedSwipeSegmentsList, using original swipe " + i + " using swipe at " + swipeListIndex + "  length " + swipeList[swipeListIndex].Count);
					//compressedSwipeSegmentsList[i] = swipeList[swipeListIndex];
				}
				theSwipeList.Add(swipeList[swipeListIndex]);
			}
		}
		
		if (!EvaluateRelationships(theSwipeList)) {
			return false;
		}
		return true;
	}
	
	private bool EvaluateRelationships(List<SwipeSegmentList> swipeList)
	{
		
		for (int i = 0; i < lineRelationships.Count; i++) {
			//Debug.Log("CompoundLineSwipe:EvaluateRelationships " + name + " " + identificationUsed + " evaluate relationship  " + i);
			if (!EvaluateRelationship(lineRelationships[i], swipeList)) {
				return false;
			}
			
		}
		
		return true;
	}
			
	private bool EvaluateRelationship(LineRelationship lineRelationship, List<SwipeSegmentList> swipeList)
	{
		SwipeSegmentList targetSegments = lineRelationship.targetLine.matchedSwipeSegments;
		SwipeSegmentList relativeSegments = lineRelationship.relativeLine.matchedSwipeSegments;
		if (targetSegments == null || relativeSegments == null) {
			//Debug.Log("CompoundLineSwipe:EvaluateRelationship " + name + " relationship matched segments are null " + targetSegments + " or " + relativeSegments);
			return false;
		}
		
		bool targetIsForward;
		bool relativeIsForward;
		for (int i = 0; i < swipeList.Count; i++) {
			
			//Debug.Log("CompoundLineSwipe:EvaluateRelationship " + name + " work on swipe " + i + " target pos " + lineRelationship.targetPosition + " relative pos " + lineRelationship.relativePosition);
			
			targetSegments = swipeList[i];
			int isForwardReturn = FindIsForward(lineRelationship.targetLine, swipeList[i]);
			if (isForwardReturn == -1) {
				//Debug.Log("CompoundLineSwipe:EvaluateRelationship " + name + " no targetIsForward isForward " + i);
				continue;
			}
			targetIsForward = (isForwardReturn == 1);
				
			if (!DirectionsMatch(identificationUsed, lineRelationship.targetLine.segments, targetSegments, targetIsForward)) {
				continue;
			}
							
			
			for (int j = 0; j < swipeList.Count; j++) {
				if (i == j ) {
					continue;
				}
				
				relativeSegments = swipeList[j];
				isForwardReturn = FindIsForward(lineRelationship.relativeLine, swipeList[j]);
				if (isForwardReturn == -1) {
					//Debug.Log("CompoundLineSwipe:EvaluateRelationship " + name + " no relativeIsForward isForward " + i + " " + j);
					continue;
				}
				relativeIsForward = (isForwardReturn == 1);
				
				if (!DirectionsMatch(identificationUsed, lineRelationship.relativeLine.segments, swipeList[j], relativeIsForward)) {
					continue;
				}
				
				//Debug.Log("CompoundLineSwipe:EvaluateRelationship " + name + " TestRelationShipPoints " + i + " and " + j +  "  count " + targetSegments.Count + " is targetIsForward=" + targetIsForward + " relativeIsForward=" + relativeIsForward);
				if (TestRelationShipPoints(lineRelationship, targetSegments, targetIsForward, relativeSegments, relativeIsForward)) {
					return true;
				}
			}
		}
		
		return false;
	}
	
	private bool TestRelationShipPoints(LineRelationship lineRelationship, SwipeSegmentList targetSegments, bool targetIsForward, SwipeSegmentList relativeSegments, bool relativeIsForward)
	{
		Vector2 targetPoint = FindRelationShipPoint(targetSegments, targetIsForward, lineRelationship.targetLine,
		                     lineRelationship.targetSegmentNum, lineRelationship.targetPosition, lineRelationship.targetPercentPosition);
		
		Vector2 relativePoint = FindRelationShipPoint(relativeSegments, relativeIsForward, lineRelationship.relativeLine,
		                    lineRelationship.relativeSegmentNum, lineRelationship.relativePosition, lineRelationship.relativePercentPosition);
		//Debug.Log("CompoundLineSwipe:TestRelationShipPoints " + name + " from " + lineRelationship.targetPosition + " at " + targetPoint + " to " + lineRelationship.relativePosition+ " at " + relativePoint);
		

		if (targetPoint == Vector2.zero	||  relativePoint == Vector2.zero) {
			//Debug.Log("CompoundLineSwipe:TestRelationShipPoints a point is zero, false ");
			SetError("Line relationship failed on error - an end point is zero", false);
			return false;
		}
		
		if (lineRelationship.targetPosition == LineRelationship.LinePosition.BetweenTopBottom) {
			//Debug.Log("CompoundLineSwipe:TestRelationShipPoints BetweenTopBottom y " + relativePoint.y + " between " + targetPoint);
			if (relativePoint.y >= targetPoint.x && relativePoint.y <= targetPoint.y) {
				return true;
			}
			SetError("Line is not between top and bottom  with " + lineRelationship.relativePosition + " using range " + targetPoint + " with " + relativePoint , true);
			return false;
		}
		if (lineRelationship.targetPosition == LineRelationship.LinePosition.BetweenLeftRight) {
			//Debug.Log("CompoundLineSwipe:TestRelationShipPoints BetweenLeftRight x " + relativePoint.x + " between " + targetPoint);
			if (relativePoint.x >= targetPoint.x && relativePoint.x <= targetPoint.y) {
				return true;
			}
			SetError("Line is not between left and righ with " + lineRelationship.relativePosition + " using range " + targetPoint + " with " + relativePoint , true);
			return false;
		}
		
		Vector2 diffPos = targetPoint - relativePoint;
		float posDiff = GetMatchPositionDiff(identificationUsed, positionDiff);
		//Debug.Log("CompoundLineSwipe:TestRelationShipPoints " + name + " " + identificationUsed + " calulated pos diff " + posDiff + ", given positionDiff=" + positionDiff + ", base matchPositionDiff=" + matchPositionDiff);
		
		if (Mathf.Abs(diffPos.x) > posDiff || Mathf.Abs(diffPos.y) > posDiff) {
			//Debug.Log("CompoundLineSwipe:TestRelationShipPoints the point are not close enough " + diffPos);
			SetError("Line relationship points are too far apart " + lineRelationship.targetPosition + " with " + lineRelationship.relativePosition + " using " + posDiff + " with " + diffPos , true);
			return false;
		}
		
		if (!VerifyRepationShip(lineRelationship.targetPosition, targetPoint, relativePoint)) {
			//Debug.Log("CompoundLineSwipe:TestRelationShipPoints target postion " + lineRelationship.targetPosition + " failed on points");
			SetError("Target line relationship " + lineRelationship.targetPosition + " not close enough", true);
			return false;
		}
		if (!VerifyRepationShip(lineRelationship.relativePosition, relativePoint, targetPoint)) {
			//Debug.Log("CompoundLineSwipe:TestRelationShipPoints target postion " + lineRelationship.targetPosition + " failed on points");
			SetError("Relative line relationship " + lineRelationship.relativePosition + "  not close enough", true);
			return false;
		}
		
		
		//Debug.Log("CompoundLineSwipe:TestRelationShipPoints point match, true    diff=" + diffPos);
		return true;
	}
	
	private bool VerifyRepationShip(LineRelationship.LinePosition position, Vector2 positionPoint, Vector2 otherPoint)
	{
		switch(position) {
		case LineRelationship.LinePosition.Start:
		case LineRelationship.LinePosition.End:
		case LineRelationship.LinePosition.Percent:
			break;
			
		case LineRelationship.LinePosition.Left:  
			if (positionPoint.x < otherPoint.x) {
				//Debug.Log("CompoundLineSwipe:VerifyRepationShip postion " + position + " failure  " + positionPoint + " is not left of " + otherPoint);
				return CloseEnough(positionPoint.x, otherPoint.x);
			}
			break;
		case LineRelationship.LinePosition.Right:
			if (positionPoint.x > otherPoint.x) {
				//Debug.Log("CompoundLineSwipe:VerifyRepationShip postion " + position + " failure  " + positionPoint + " is not right of " + otherPoint);
				return CloseEnough(positionPoint.x, otherPoint.x);
			}
			break;
		case LineRelationship.LinePosition.Upper:
			if (positionPoint.y < otherPoint.y) {
				//Debug.Log("CompoundLineSwipe:VerifyRepationShip postion " + position + " failure  " + positionPoint + " is not above " + otherPoint);
				return CloseEnough(positionPoint.y, otherPoint.y);
			}
			break;
		case LineRelationship.LinePosition.Lower:
			if (positionPoint.y > otherPoint.y) {
				//Debug.Log("CompoundLineSwipe:VerifyRepationShip postion " + position + " failure  " + positionPoint + " is not below " + otherPoint);
				return CloseEnough(positionPoint.y, otherPoint.y);
			}
			break;
		}
		return true;
	}
	
	private Vector2 FindRelationShipPoint(SwipeSegmentList swipeSegments, bool isForward, LineSwipe lineSwipe, int segmentNum, LineRelationship.LinePosition position, float percentPosition)
	{
		
		SwipeSegment swipeSegment;
		//Debug.Log("CompoundLineSwipe:FindRelationShipPoint " + name + " swipeSegments " + swipeSegments.Count + ", segmentNum " + segmentNum + " " + position + ", isForward=" + isForward);
		if (swipeSegments.Count	!= lineSwipe.Count() || segmentNum >= swipeSegments.Count) {
			//Debug.Log("CompoundLineSwipe:FindRelationShipPoint " + name + " should not happen, failed on count mismatch " + swipeSegments.Count + " vs " + lineSwipe.Count() + " looking for " + segmentNum);
			SetError("Line segment counts do not match, " + swipeSegments.Count + " needing " + lineSwipe.Count(), false);
			return Vector2.zero;
		}
		else if (swipeSegments.Count == 1) {
			swipeSegment = swipeSegments[0];
		}
		else if (isForward) {
			swipeSegment = swipeSegments[segmentNum];
		}
		else {
			swipeSegment = swipeSegments[swipeSegments.Count - segmentNum - 1];
			//Debug.Log("CompoundLineSwipe:FindRelationShipPoint " + name + " backwards use segment " + (swipeSegments.Count - segmentNum - 1));
		}
		
		Vector2 fromPos;
		Vector2 toPos;
		if (isForward) {
			fromPos = swipeSegment.startPosition;
			toPos = swipeSegment.endPosition;
		}
		else {
			fromPos = swipeSegment.endPosition;
			toPos = swipeSegment.startPosition;
		}
		
		//Debug.Log("CompoundLineSwipe:FindRelationShipPoint " + swipeSegment.direction + " " + name + " from " + fromPos + ", to " + toPos);
		switch(position) {
		case LineRelationship.LinePosition.Start:
			//Debug.Log("CompoundLineSwipe:FindRelationShipPoint " + name + " start " + fromPos);
			return fromPos;
		case LineRelationship.LinePosition.End:
			//Debug.Log("CompoundLineSwipe:FindRelationShipPoint " + name + " end " + toPos);
			return toPos;
		case LineRelationship.LinePosition.Left:
			if (fromPos.x > toPos.x) {
				//Debug.Log("CompoundLineSwipe:FindRelationShipPoint " + name + " Left " + toPos);
				return toPos;
			}
			else {
				//Debug.Log("CompoundLineSwipe:FindRelationShipPoint " + name + " Left " + fromPos);
				return fromPos;
			}
		case LineRelationship.LinePosition.Right:
			if (fromPos.x > toPos.x) {
				//Debug.Log("CompoundLineSwipe:FindRelationShipPoint " + name + " Right " + fromPos);
				return fromPos;
			}
			else {
				//Debug.Log("CompoundLineSwipe:FindRelationShipPoint " + name + " Right " + toPos);
				return toPos;
			}
		case LineRelationship.LinePosition.Upper:
			if (fromPos.y > toPos.y) {
				//Debug.Log("CompoundLineSwipe:FindRelationShipPoint " + name + " Upper " + fromPos);
				return fromPos;
			}
			else {
				//Debug.Log("CompoundLineSwipe:FindRelationShipPoint " + name + " Upper " + toPos);
				return toPos;
			}
		case LineRelationship.LinePosition.Lower:
			if (fromPos.y > toPos.y) {
				//Debug.Log("CompoundLineSwipe:FindRelationShipPoint " + name + " Lower " + toPos);
				return toPos;
			}
			else {
				//Debug.Log("CompoundLineSwipe:FindRelationShipPoint " + name + " Lower " + fromPos);
				return fromPos;
			}
		case LineRelationship.LinePosition.Percent:
			Vector2 change = toPos - fromPos;
			change *= percentPosition;
			Vector2 newPos = fromPos + change;
			//Debug.Log("CompoundLineSwipe:FindRelationShipPoint " + name + " Percent " + newPos + " percent " + percentPosition + " between " + fromPos + " and " + toPos);
			return newPos;
		case LineRelationship.LinePosition.BetweenTopBottom:
			if (fromPos.y > toPos.y) {
				//Debug.Log("CompoundLineSwipe:BetweenTopBottom " + toPos.y + " to " + fromPos.y);
				return new Vector2(toPos.y, fromPos.y);
			}
			else {
				//Debug.Log("CompoundLineSwipe:BetweenTopBottom " + fromPos.y + " to " + toPos.y);
				return new Vector2(fromPos.y, toPos.y);
			}
		case LineRelationship.LinePosition.BetweenLeftRight:
			if (fromPos.x > toPos.x) {
				//Debug.Log("CompoundLineSwipe:BetweenLeftRight " + toPos.x + " to " +  fromPos.x);
				return new Vector2(toPos.x, fromPos.x);
			}
			else {
				//Debug.Log("CompoundLineSwipe:BetweenLeftRight " + fromPos.x + " to " + toPos.x);
				return new Vector2(fromPos.x, toPos.x);
			}
		}
		//Debug.Log("CompoundLineSwipe:FindRelationShipPoint " + name + " should not happen, unsupported or bad position " + position);
		SetError("Unable to find relationship point, " + position, false);
		return Vector2.zero;
	}

	private int FindIsForward(LineSwipe lineSwipe, SwipeSegmentList segmentList) {
		
		if (restrictLineSwipeDirectionUsed == LineGesture.LineSwipeDirection.Backward) {
			if (!segmentList.isForwardUsed) {
				//Debug.Log("CompoundLineSwipe:FindIsForward " + name + " on Backward only matched ");
				return 0;
			}
			else {
				return -1;
			}
			
		}
		else if (restrictLineSwipeDirectionUsed == LineGesture.LineSwipeDirection.Forward) {
			if (segmentList.isForwardUsed) {
				//Debug.Log("CompoundLineSwipe:FindIsForward " + name + " on Forward only matched ");
				return 1;
			}
			else {
				return -1;
			}
		}
		else {
			//Debug.Log("CompoundLineSwipe:FindIsForward " + name + " on anywhere CompareDirections isForward is " +  segmentList.isForwardUsed + " " + segmentList.Count);
			return segmentList.isForwardUsed ? 1 : 0;
		}
	}
	

			
	public override bool IsCompound()
	{
		return true;
	}
}

