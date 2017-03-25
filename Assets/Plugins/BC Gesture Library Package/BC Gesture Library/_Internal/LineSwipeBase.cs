using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LineSwipeBase
{
	public string name;
	public bool closed = false;			// the shape must start and end at the same position
	public bool startAnywhere = true;	// only used with closed
	public bool doCompareLengths = true;
	
	public bool biDirectional = true;
	public bool maintainAspectRatio = true;
	
	public static float SegmentMaxDistanceIgnore = 30f;
	public static float matchPositionDiffSloppyMultiplier = 4f;
	public static float matchLengthDiffPercentSloppyMultiplier = 3.0f;

	protected float matchPositionDiff;
	private float matchLengthDiffPercent;  // decimal per cent
	private bool firstLastMatch = false;
	private int extraSegment = 0;
	protected SegmentList sourceSegments;
	private SwipeSegment[] matchedSegments;
	private float overRideUnitSize = -1f;
	public LineGesture.LineIdentification identificationUsed;
	public LineGesture.LineSwipeDirection restrictLineSwipeDirectionUsed;
	public SwipeSegmentList[] compressedSwipeSegmentsList;
	public bool isForwardUsed = true;
	public SwipeSegmentList compressedSwipeSegments = null;
	public SwipeSegmentList matchedSwipeSegments;
	public static string lastError;
	private int matchScore = 0;
	private int compressedStartIndex;


	public LineSwipeBase(string nameIn)
	{
		name = nameIn;	
	}
	
	public virtual bool Compare(List<SwipeSegmentList> swipeList, LineGesture.LineSwipeDirection restrictLineSwipeDirection, float positionDiff, float lengthDiffPercent, LineGesture.LineIdentification lineIdentification)
	{
		return false;
	}
	public virtual bool Compare(SwipeSegmentList swipeSegments, LineGesture.LineSwipeDirection restrictLineSwipeDirection, float positionDiff, float lengthDiffPercent, LineGesture.LineIdentification lineIdentification)
	{
		return false;
	}
			
	public virtual bool IsCompound()
	{
		return false;
	}
	
	public virtual int Count()
	{
		return 1;
	}

	public virtual int GetMaxSegment()
	{
		return 1;
	}
	public virtual LineSwipeBase GetUsedLineSwipe()
	{
		return this;
	}
	
	public void SetOverRideUnitSize(float size)
	{
		overRideUnitSize = size;
	}
	
	protected void SetError(string str, bool force)
	{
		if (force || lastError == null || lastError.Length == 0) {
			if (str == null || str.Length	== 0) {
				lastError = str;
			}
			else {
				lastError = name + " (" + identificationUsed + ") - " + str;
			}
		}
	}
	
	protected float GetMatchPositionDiff(LineGesture.LineIdentification identification, float defaultValue)
	{
		//Debug.Log("LineSwipeBase:getMatchPositionDiff " + name + " " + identification + " defaultValue " + defaultValue + "  matchPositionDiff=" + matchPositionDiff);
		if (matchPositionDiff <= 0) {
			matchPositionDiff = defaultValue;
			//Debug.Log("LineSwipeBase:getMatchPositionDiff " + name + " use default");
		}
		if (identification == LineGesture.LineIdentification.Sloppy) {
			//Debug.Log("LineSwipeBase:getMatchPositionDiff " + name + " return sloppy " + matchPositionDiff * LineSwipeBase.matchPositionDiffSloppyMultiplier);
			return matchPositionDiff * LineSwipeBase.matchPositionDiffSloppyMultiplier;
		}
		//Debug.Log("LineSwipeBase:getMatchPositionDiff " + name + " return " + matchPositionDiff);
		return matchPositionDiff;
	}
	
	protected bool DirectionsMatch(LineGesture.LineIdentification identification, SegmentList sourceList, SwipeSegmentList swipeSegments, bool isForward)
	{
		if (sourceList.Count != swipeSegments.Count) {
			//Debug.Log("LineSwipeBase:DirectionsMatch " + name + " failed on count " + swipeSegments.Count + " wanting " + sourceList.Count);
			return false;
		}
		int index = (isForward ? 0 : (swipeSegments.Count - 1));
		for (int i = 0; i < sourceList.Count; i++) {
			if (!sourceList[i].CompareDirection(identification,  swipeSegments[index].direction, isForward)) {
				//Debug.Log("LineSwipeBase:CompareDirections " + name + " compared " + i + " " + identification + " failed source " + sourceList[i].DebugStringDirections(isForward) + " to " + swipeSegments[index].direction);
				return false;
			}
			//Debug.Log("LineSwipeBase:DirectionsMatch " + name + " compared " + i + " valid source " + sourceList[i].DebugStringDirections(isForward) + " to " + swipeSegments[index].direction);
			index = incIndex(index, 1, isForward, swipeSegments.Count);
		}
		return true;
	}
	public bool CompareDirections(LineGesture.LineIdentification identification, SwipeSegmentList swipeSegments, bool isForward)
	{
		return DirectionsMatch(identification, sourceSegments, swipeSegments, isForward);
	}

	
	protected bool CloseEnough(float value1, float value2)
	{
		float diff = Mathf.Abs(value1 - value2);
		if (diff < GetMatchPositionDiff(identificationUsed, 60)) {
			return true;
		}
		return false;
	}
	
	
	protected bool CompareSwipeList(SwipeSegmentList swipeSegmentsIn, SegmentList sourceSegmentsIn,  LineGesture.LineSwipeDirection restrictLineSwipeDirection, float positionDiff, float lengthDiffPercent, LineGesture.LineIdentification lineIdentification)
	{	
		sourceSegments = sourceSegmentsIn;
		SwipeSegmentList swipeSegments = swipeSegmentsIn;
		identificationUsed = lineIdentification;
		restrictLineSwipeDirectionUsed = restrictLineSwipeDirection;
		//Debug.Log("LineSwipeBase:Compare identificationUsed " + identificationUsed);
		compressedSwipeSegments = null;
		matchedSwipeSegments = null;
		SetError("", true);

				
		firstLastMatch = false;
		extraSegment = 0;
		matchPositionDiff = positionDiff;
		matchLengthDiffPercent = lengthDiffPercent;
		if (identificationUsed	== LineGesture.LineIdentification.Sloppy) {
			matchPositionDiff = matchPositionDiff * 3.0f;
			matchLengthDiffPercent = matchPositionDiff * 2.0f;
		}
		if (swipeSegments == null || swipeSegments.Count == 0) {
			SetError("Swipe is empty", true);
			return false;
		}
		if (matchedSegments == null) {
			matchedSegments = new SwipeSegment[sourceSegments.Count];
		}
		
		if (closed) {
			return CompareClosed(swipeSegments, restrictLineSwipeDirection);
		}
		
 		if (swipeSegments.Count != sourceSegments.Count && !(tryHarder() && swipeSegments.Count > sourceSegments.Count)) {
			//Debug.Log("LineSwipeBase:Compare failed " + name + " on count,  segments " + sourceSegments.Count + " not equal to drawn " + swipeSegments.Count);
			SetError("Swipe count " + swipeSegments.Count + " does not match source " + sourceSegments.Count, true);
			return false;
		}
		//Debug.Log("LineSwipeBase:Compare on " + name + "  source segments " + sourceSegments.Count + " to input swipeSegments " + swipeSegments.Count + " identification " + identificationUsed);
		
		bool isValid = false;
		SwipeSegmentList[] compressedSave = null;
		if (CompareFoward(swipeSegments)) {
			if (matchScore == 0) {
				return true;
			}
			compressedSave = compressedSwipeSegmentsList;
			isValid = true;
		}
		int lastMatchScore = matchScore;
		if (CompareBackward(swipeSegments)) {
			if (!isValid || matchScore < lastMatchScore) {
				return true;
			}
		}
		if (isValid) {
			isForwardUsed = true;
			compressedSwipeSegmentsList = compressedSave;
			return true;
		}
		
		//Debug.Log("LineSwipeBase:Compare at bottom failed");
		return false;
	}
	
	private bool CompareFoward(SwipeSegmentList swipeSegments)
	{
		if (restrictLineSwipeDirectionUsed == LineGesture.LineSwipeDirection.Forward ||
		    restrictLineSwipeDirectionUsed == LineGesture.LineSwipeDirection.Either ||
		   	restrictLineSwipeDirectionUsed == LineGesture.LineSwipeDirection.Anywhere) {
			//Debug.Log("LineSwipeBase:CompareFoward CompareDirectionFor");
			if (CompareDirectionFor(swipeSegments, 0, true)) {
				//Debug.Log("LineSwipeBase:CompareFoward success forward " + name + " , identificationUsed=" + identificationUsed + " foward  isForward=" + this.isForwardUsed);
				return true;
			}
		}
		
		//Debug.Log("CompareFoward:Compare at bottom failed");
		return false;
		
	}
	private bool CompareBackward(SwipeSegmentList swipeSegments)
	{
		if (!biDirectional) {
			return false;
		}
		if (restrictLineSwipeDirectionUsed == LineGesture.LineSwipeDirection.Backward ||
		    restrictLineSwipeDirectionUsed == LineGesture.LineSwipeDirection.Either ||
		   	restrictLineSwipeDirectionUsed  == LineGesture.LineSwipeDirection.Anywhere) {
			//Debug.Log("LineSwipeBase:CompareBackward CompareDirectionFor");
			if (CompareDirectionFor(swipeSegments, swipeSegments.Count - 1, false)) {
				//Debug.Log("LineSwipeBase:CompareBackward success backwards " + name + " , identificationUsed=" + identificationUsed + " backward  isForward=" + this.isForwardUsed);
				return true;
			}
		}
		
		//Debug.Log("LineSwipeBase:Compare at bottom failed");
		return false;
	}
	
	private bool CompareClosed(SwipeSegmentList swipeSegmentsIn, LineGesture.LineSwipeDirection restrictLineSwipeDirection)
	{
		SwipeSegmentList swipeSegments = swipeSegmentsIn;
		compressedSwipeSegments = null;
		//Debug.Log("LineSwipeBase:CompareClosed " + name + " identification " + identificationUsed);
		
		if (swipeSegments.Count != sourceSegments.Count) {
			if (!startAnywhere || swipeSegments.Count != (sourceSegments.Count + 1)) {
				if (!tryHarder()) {
					//Debug.Log("LineSwipeBase:CompareClosed failed " + name + " on count,  segments " + sourceSegments.Count + " not equal to drawn " + swipeSegments.Count);
					return false;
				}
			}
			if ((swipeSegments.Count == (sourceSegments.Count + 1) || (tryHarder() && swipeSegments.Count > (sourceSegments.Count + 1) ))&& 
			    (swipeSegments[0]).direction == (swipeSegments[swipeSegments.Count - 1]).direction){
				//Debug.Log("LineSwipeBase:CompareClosed count one and ends are equal");
				firstLastMatch = true;
				extraSegment = 1;
			}
			else if (!tryHarder() &&  swipeSegments.Count < sourceSegments.Count) {
				//Debug.Log("LineSwipeBase:CompareClosed count one up but ends not equal, not matched");
				return false;
			}
		}
		
		Vector2 diffPos = swipeSegments[0].startPosition - swipeSegments[swipeSegments.Count - 1].endPosition;
		if (Mathf.Abs(diffPos.x) > matchPositionDiff || Mathf.Abs(diffPos.y) > matchPositionDiff) {
			//Debug.Log("LineSwipeBase:CompareClosed end points do not match " + diffPos);
			SetError("Close line end points to not match ", true);
			return false;
		}
		
		
		bool found = false;
		int tries = swipeSegments.Count;
		if (!startAnywhere || restrictLineSwipeDirection != LineGesture.LineSwipeDirection.Anywhere) {
			tries = 1;
		}
		for (int i = 0; i < tries; i++) {
			//Debug.Log("LineSwipeBase:CompareClosed on " + i);
			if (restrictLineSwipeDirection == LineGesture.LineSwipeDirection.Forward ||
			    restrictLineSwipeDirection == LineGesture.LineSwipeDirection.Either ||
			   	restrictLineSwipeDirection == LineGesture.LineSwipeDirection.Anywhere) {
				found = CompareDirectionFor(swipeSegments, i, true);
				if (found) {
					break;
				}
			}
			if (biDirectional) {
				if (restrictLineSwipeDirection == LineGesture.LineSwipeDirection.Backward ||
				    restrictLineSwipeDirection == LineGesture.LineSwipeDirection.Either ||
				   	restrictLineSwipeDirection == LineGesture.LineSwipeDirection.Anywhere) {
					found = CompareDirectionFor(swipeSegments, i, false);
					if (found) {
						break;
					}
				}
			}
		}
		
		if (found) {
			//Debug.Log("LineSwipeBase:CompareClosed success forward " + name + " , identificationUsed=" + identificationUsed);
			return true;
		}
				
		//Debug.Log("LineSwipeBase:CompareClosed success forward " + name + "  at bottom failed ");
		return false;
	}
	
	
	
	private bool CompareDirectionFor(SwipeSegmentList swipeSegmentsIn, int startIndex, bool isForward)
	{
		//Debug.Log("LineSwipeBase:CompareDirectionFor ******** " + name + " " + identificationUsed + " startIndex=" + startIndex + ", going " + (isForward ? " FOWARD " : " BACKWARD ") + ", extraSegment=" + extraSegment);
		Segment segment;
		SwipeSegmentList swipeSegments = swipeSegmentsIn;
		int index = startIndex;
		int lastIndex = index;
		isForwardUsed = isForward;
		swipeSegments.isForwardUsed = isForwardUsed;
		matchScore = 0;

		//DebugSwipeListDirection("LineSwipeBase:CompareDirectionFor", swipeSegments, isForward, startIndex, "startIndex", -1, "");
		
		for (int i = 0; i < sourceSegments.Count; i++) {
			segment = sourceSegments[i];
			//Debug.Log("LineSwipeBase:CompareDirectionFor " + name + " loop " + i + " swipeSegments at " + index + " swipeSegments.Count " + swipeSegments.Count + ", directions " + segment.DebugStringDirections(isForward)  + " verses " + index + " " + swipeSegments[index].direction);
			
			bool directionCompareSuccess = segment.CompareDirection(identificationUsed, swipeSegments[index].direction, isForward);
			
			if (!directionCompareSuccess || (i == (sourceSegments.Count - 1) &&  swipeSegments.Count  > (sourceSegments.Count + extraSegment)) ) {
				
				//Debug.Log("LineSwipeBase:CompareDirectionFor " + name + " match failed on " + i + " " + segment.DebugStringDirections(isForward)  + " verses " + index + " " + swipeSegments[index].direction);
				if ((swipeSegments.Count - extraSegment) > sourceSegments.Count) {
					if (!TryToCompessSegments(i - 1, 
					                          swipeSegments,  
					                          startIndex, 
					                          lastIndex, 
					                          isForward)) {
						//Debug.Log("LineSwipeBase:CompareDirectionFor TryToCompessSegments failed " + name + " on previous i - 1 " + (i - 1));
						if (!TryToCompessSegments(i, 
					                          swipeSegments,  
					                          startIndex, 
					                          index, 
					                          isForward)) {
							//Debug.Log("LineSwipeBase:CompareDirectionFor TryToCompessSegments " + name + " failed starting on failed current  " + i);
							return false;
						}
					}
					swipeSegments = compressedSwipeSegments;
					//Debug.Log("LineSwipeBase:CompareDirectionFor " + name + " TryToCompessSegments success on " + i + " swipeSegments.Count=" + swipeSegments.Count + ", new start index =" + compressedStartIndex);
					startIndex = compressedStartIndex;   // XXXXXXXX   this it the start given, not the original start of the given matching i = 0
					index = startIndex;
					lastIndex = index;
					matchScore = 0;
					i = -1;
					//DebugSwipeListDirection("LineSwipeBase:CompareDirectionFor", swipeSegments, isForward, startIndex, "startIndex", -1, "");
					continue;
				}
				else {
					return false;
				}
			}
			//Debug.Log("LineSwipeBase:CompareDirectionFor " + name + " segment match succss on " + i + " " + segment.DebugStringDirections(isForward) + " verses " + index + " " + (swipeSegments[index]).direction);
			if (!segment.CompareDirection(LineGesture.LineIdentification.Clean, swipeSegments[index].direction, isForward)) {
				matchScore += 3;
			}
			lastIndex = index;
			index = incIndex(index, 1, isForward, swipeSegments.Count);
		}
		
		if (swipeSegments.Count  != (sourceSegments.Count + extraSegment)) {
			//Debug.Log("LineSwipeBase:CompareDirectionFor did not match " + name + " on count " + sourceSegments.Count + " + " + extraSegment + " != " + swipeSegments.Count);
			return false;
		}
		
		index = startIndex;
		for (int i = 0; i < sourceSegments.Count; i++) {
			matchedSegments[i] =  swipeSegments[index];
			//Debug.Log("LineSwipeBase:CompareDirectionFor " + name + " matching for lengh check " + i + " with " + index + ", " + (SwipeGesture.GetDirection(isForward, matchedSegments[i].direction)) + " with " + (sourceSegments[i].DebugStringDirections(true)));
			index = incIndex(index, 1, isForward, swipeSegments.Count);
		}
		
		//Debug.Log("LineSwipeBase:CompareDirectionForwardFor matched so far, check length");
		if (CheckLengths(matchedSegments, swipeSegments, startIndex)) {
			//Debug.Log("LineSwipeBase:CompareDirectionFor " + name + " matched types and length");
			//DebugSwipeListDirection("LineSwipeBase:CompareDirectionFor success ", swipeSegments, isForward, startIndex, "startIndex", -1, "");
			return true;
		}
		
		//Debug.Log("LineSwipeBase:CompareDirectionFor did not match " + name);
		return false;
	}
	
	
	private bool TryToCompessSegments(int sourceSegmentsIndex, SwipeSegmentList swipeSegments, int startIndex, int currentIndex, bool isForward)
	{

		int extras = swipeSegments.Count - sourceSegments.Count - extraSegment;
		//Debug.Log("LineSwipe:TryToCompessSegments " + name + " " + identificationUsed + " **** TRY TO COMPRESS SEGMENTS **** sourceSegmentsIndex=" + sourceSegmentsIndex + ", swipeSegments count = " + swipeSegments.Count + ", startIndex = " + startIndex + ", currentIndex = " + currentIndex + ", going " + (isForward ? " FOWARD " : " BACKWARD ") + " extras " + extras);
		
		int numberAdded = LookForwardForCompress(sourceSegmentsIndex, swipeSegments, startIndex, currentIndex, isForward, extras);
				
		if (numberAdded	< 2) {
			//Debug.Log("LineSwipeBase:TryToCompessSegments " + name + " none matched, exit ");
			return false;
		}
		int endIndex = incIndex(currentIndex, numberAdded - 1, isForward, swipeSegments.Count);
		Segment sourceSegment = sourceSegments[sourceSegmentsIndex];
		SwipeSegment startSegment = swipeSegments[currentIndex];
		SwipeSegment endSegment = swipeSegments[endIndex];
		SwipeGesture.SwipeDirection calcDirection;
		
		if (isForward) {
			calcDirection = FingerControl.GetSwipeDirection(startSegment.startPosition, endSegment.endPosition);
		}
		else {
			calcDirection = FingerControl.GetSwipeDirection(startSegment.endPosition, endSegment.startPosition);
		}
		//Debug.Log("LineSwipeBase:TryToCompessSegments " + name + " start to end direction " +  calcDirection + " numberAdded=" + numberAdded + ", endIndex=" + endIndex);
		
		if (calcDirection == SwipeGesture.SwipeDirection.None) {
			return false;
		}
		
		//Debug.Log("LineSwipeBase:TryToCompessSegments " + name + " success at segments " + currentIndex + " combining " + numberAdded + " in " + calcDirection);
		SwipeGesture.SwipeDirection combineDirection = calcDirection;
		if (!sourceSegment.CompareDirection(identificationUsed, calcDirection, true)) {
						
			//Debug.Log("LineSwipeBase:TryToCompessSegments " + name + " success->failed swipe direction " + calcDirection + " does not match target direction " + sourceSegment.DebugStringDirections(isForward));
			return false;
		}
		combineDirection =  SwipeGesture.GetDirection(isForward, combineDirection);
		
		//Debug.Log("LineSwipeBase:TryToCompessSegments " + name + " conbine using " + combineDirection + " segment has " + sourceSegment.DebugStringDirections(isForward));
		
		int removeStart;
		int removeEnd;
		int removeStart2;
		int removeEnd2;
		if (isForward) {
			removeStart = currentIndex + 1;
			removeEnd = currentIndex + numberAdded - 1;
			compressedStartIndex = startIndex;
			if (removeEnd > (swipeSegments.Count - 1)) {
				removeStart2 = 0;
				removeEnd2 = removeEnd - swipeSegments.Count - 1;
				removeEnd = swipeSegments.Count - 1;
				compressedStartIndex = startIndex - removeEnd2;
			}
			else {
				removeStart2 = removeStart;
				removeEnd2 = removeEnd; 
			}
		}
		else {
			removeStart = currentIndex - numberAdded + 1;
			removeEnd = currentIndex - 1;
			if (startIndex < removeEnd) {
				compressedStartIndex = startIndex;
			}
			else {
				compressedStartIndex = startIndex - (removeEnd - removeStart + 1);
			}				
			if (removeStart < 0) {
				removeStart2 = swipeSegments.Count +  removeStart;
				removeEnd2 = swipeSegments.Count - 1;
				removeStart = 0;
				compressedStartIndex = 0;
			}
			else {
				removeStart2 = removeStart;
				removeEnd2 = removeEnd; 
			}
		}
		if (compressedStartIndex < 0) {
			compressedStartIndex = 0;
		}
		//Debug.Log("LineSwipeBase:TryToCompessSegments " + name + "compressedStartIndex=" + compressedStartIndex + ", removing " + removeStart + " to " + removeEnd + " and " + removeStart2 + " to " + removeEnd2);
										
		SwipeSegment previousSwipeSegment = null;
		SwipeSegment newSwipeSegment = null;
		compressedSwipeSegments = new SwipeSegmentList();
		compressedSwipeSegments.isForwardUsed = isForward;
		for (int i = 0; i < swipeSegments.Count; i++) {
			if (i == currentIndex) {
				newSwipeSegment  = new SwipeSegment(previousSwipeSegment);
				newSwipeSegment.Copy(startSegment);			
				//Debug.Log("LineSwipeBase:TryToCompessSegments " + name + " CombineToSingleAfter");
				newSwipeSegment.CombineToSingleAfter(isForward, combineDirection, endSegment);
				compressedSwipeSegments.Add(newSwipeSegment);					
				previousSwipeSegment = newSwipeSegment;
			}
			else if ( !((i >= removeStart  &&  i <= removeEnd) || (i >= removeStart2  &&  i <= removeEnd2)) ) {
				newSwipeSegment  = new SwipeSegment(previousSwipeSegment);
				newSwipeSegment.Copy(swipeSegments[i]);
				compressedSwipeSegments.Add(newSwipeSegment);					
				previousSwipeSegment = newSwipeSegment;
			}
		}
		
		if (compressedStartIndex >= compressedSwipeSegments.Count) {
			//Debug.Log("LineSwipeBase:TryToCompessSegments " + name + " problem with compressedStartIndex " + compressedStartIndex + ", compressedSwipeSegments.Count=" + compressedSwipeSegments.Count);
			compressedStartIndex = compressedSwipeSegments.Count - 1;
		}
		//DebugSwipeListDirection("LineSwipeBase:TryToCompessSegments", compressedSwipeSegments, isForward, compressedStartIndex, "compressedStartIndex", -1, "");
			
		return true;
	}
	
	
	private int lookForwardDepth = 0;
	
	private int LookForwardForCompress(int sourceSegmentsIndex, SwipeSegmentList swipeSegments, int startIndex, int currentIndex, bool isForward, int extras)
	{
		lookForwardDepth++;
		int numberAdded = 0;
		try {
			if (extras == 0 || sourceSegmentsIndex < 0) {
				//Debug.Log("LineSwipe:LookForwardForCompress " + name + "-" + lookForwardDepth + " " + identificationUsed + " **** LOOK FORWARD IN SEGMENTS **** fail on no extras=" + extras + ", sourceSegments.Count " + sourceSegments.Count + ". extraSegment " + extraSegment + " or sourceSegmentsIndex " + sourceSegmentsIndex);
				return 0;
			}
			//Debug.Log("LineSwipe:LookForwardForCompress " + name + "-" + lookForwardDepth + " " + identificationUsed + " **** LOOK FORWARD IN SEGMENTS **** sourceSegmentsIndex=" + sourceSegmentsIndex + ", swipeSegments count = " + swipeSegments.Count + ", startIndex = " + startIndex + ", currentIndex = " + currentIndex + ", going " + (isForward ? " FOWARD " : " BACKWARD ") + " extras " + extras);
			
			
			
			if (sourceSegmentsIndex < 0) {
				sourceSegmentsIndex = 0;
			}
			
			Segment sourceSegment = sourceSegments[sourceSegmentsIndex];
			SwipeGesture.SwipeDirection sourceDirection1 = sourceSegment.direction;
			SwipeGesture.SwipeDirection sourceDirection2 = sourceSegment.optionalDirection;
			SwipeGesture.SwipeDirection[] friendlyDirections1 = null;
			SwipeGesture.SwipeDirection[] friendlyDirections2 = null;
			Segment sourceSegmentNext = null;
			int sourceSegmentsIndexNext = -1;
				
			if (sourceSegmentsIndex < (sourceSegments.Count - 1) ) {
				sourceSegmentsIndexNext = sourceSegmentsIndex + 1;
			}
			else if (closed) {
				sourceSegmentsIndexNext = 0;
			}

			if (sourceSegmentsIndexNext >= 0) {
				sourceSegmentNext = sourceSegments[sourceSegmentsIndexNext];
			}
			
			//Debug.Log("LineSwipe:LookForwardForCompress " + name + "-" + lookForwardDepth + " extras=" + extras + " directions " + sourceDirection1 + " and " + sourceDirection2 + ", startIndex=" + startIndex + ", currentIndex=" + currentIndex + ". sourceSegmentsIndexNext=" + sourceSegmentsIndexNext);
			//DebugSwipeListDirection("LineSwipeBase:LookForwardForCompress trace" + "-" + lookForwardDepth, swipeSegments, isForward, startIndex, "startIndex", currentIndex, "currentIndex");
	
			SwipeSegment startSegment = swipeSegments[currentIndex];
			SwipeSegment endSegment = null;
			SwipeGesture.SwipeDirection calcDirection = SwipeGesture.SwipeDirection.None;
			SwipeGesture.SwipeDirection endSegmentDir;
			if (!sourceSegment.CompareDirection(identificationUsed, startSegment.direction, isForward)) {
				//Debug.Log("LineSwipe:LookForwardForCompress " + name + "-" + lookForwardDepth + " failure in base segment directions " + startSegment.direction + " with source " + sourceSegment.DebugStringDirections(true) );
				return 0;
			}
			numberAdded++;
			
			bool didBreak = false;
			bool didBreakIsOk = false;
			for (int i = incIndex(currentIndex, 1, isForward, swipeSegments.Count); numberAdded < (extras + 1) && i != startIndex; i = incIndex(i, 1, isForward, swipeSegments.Count)) {
				didBreak = true;
				
				endSegment = swipeSegments[i];
				endSegmentDir = SwipeGesture.GetDirection(isForward, endSegment.direction);
				//Debug.Log("LineSwipeBase:LookForwardForCompress " + name + "-" + lookForwardDepth + " try swipe segment " + i + " generating direction " + endSegmentDir + " with target " + sourceSegment.DebugStringDirections(true));
	
				if (i != currentIndex && sourceSegmentNext != null && sourceSegmentNext.CompareDirection(identificationUsed, endSegmentDir, true)) {
					//Debug.Log("LineSwipeBase:LookForwardForCompress " + name + "-" + lookForwardDepth + " check for stop LookForwardForCompress on " + i);
					int forwardAdded = LookForwardForCompress(sourceSegmentsIndexNext, swipeSegments, startIndex, i, isForward, extras - numberAdded + 1);
	
					if (forwardAdded > 0) {
						//Debug.Log("LineSwipeBase:LookForwardForCompress " + name + "-" + lookForwardDepth + " stop at segment " + i + " since it matches next direction " + endSegmentDir + " and goes on for " + forwardAdded);
						didBreakIsOk = true;
						break;
					}
					
					if (forwardAdded < 0) {
						//Debug.Log("LineSwipeBase:LookForwardForCompress " + name + "-" + lookForwardDepth + " LookForwardForCompress failed " + forwardAdded);
					}
				}
	
				if (!sourceSegment.CompareDirection(identificationUsed, endSegmentDir, true)) {
					bool directionCompareSuccess = false;
					if (identificationUsed == LineGesture.LineIdentification.Sloppy) {
						if (friendlyDirections1 == null) {
							friendlyDirections1  = FingerControl.GetFriendlyDirections(sourceDirection1);
							if (sourceDirection1 != sourceDirection2) {
								friendlyDirections2  = FingerControl.GetFriendlyDirections(sourceDirection2);						
							}
						}
						for (int j = 0; !directionCompareSuccess && j < friendlyDirections1.Length; j++) {
							if (FingerControl.FriendlySwipeDirections(friendlyDirections1[j], endSegmentDir)) {
								directionCompareSuccess = true;
							}
							if (sourceDirection1 != sourceDirection2) {
								if (FingerControl.FriendlySwipeDirections(friendlyDirections2[j], endSegmentDir)) {
									directionCompareSuccess = true;
								}
							}
							                                                                                  
						}
						//Debug.Log("LineSwipeBase:LookForwardForCompress " + name + "-" + lookForwardDepth + " Sloppy is directions attempt " + friendlyDirections1[0] + "  " + friendlyDirections1[1]);
					}
					
					//Debug.Log("LineSwipeBase:LookForwardForCompress " + name + "-" + lookForwardDepth + " maybe failed " + directionCompareSuccess + " endSegment direction" + endSegmentDir + " on " + i + " does not go with segment " +  sourceSegment.DebugStringDirections(true));
					
					if (!directionCompareSuccess) {
						if (endSegment.distance > SegmentMaxDistanceIgnore) {
							//Debug.Log("LineSwipeBase:LookForwardForCompress " + name + "-" + lookForwardDepth + " direction failure trying direction " + endSegmentDir + " which does not go with segment " + i + " does not go with segment " +  sourceSegment.DebugStringDirections(true) + " distance=" + endSegment.distance);
							SetError("Parsing source direction failure " + endSegmentDir + " does not match " + sourceSegment.DebugStringDirections(true), true);
							break;
						}
						//Debug.Log("LineSwipeBase:LookForwardForCompress " + name + "-" + lookForwardDepth + " segment distance " + endSegment.distance + " is  small enough to ignore, take in " + i + " " + calcDirection);
						numberAdded++;
						didBreak = false;
						continue;
					}
				}
				
				if (isForward) {
					calcDirection = FingerControl.GetSwipeDirection(startSegment.startPosition, endSegment.endPosition);
				}
				else {
					calcDirection = FingerControl.GetSwipeDirection(startSegment.endPosition, endSegment.startPosition);
				}
				//Debug.Log("LineSwipeBase:LookForwardForCompress " + name + "-" + lookForwardDepth + " start to end direction " +  calcDirection);
				
				if (! sourceSegment.CompareDirection(identificationUsed, calcDirection, true)) {
					//Debug.Log("LineSwipeBase:LookForwardForCompress " + name + "-" + lookForwardDepth + " failed " + calcDirection + " on " + i + " does not go with segment ");
	
					if (endSegment.distance > SegmentMaxDistanceIgnore) {
						SetError("Parsing calulated direction failure " + calcDirection + " does not match " + sourceSegment.DebugStringDirections(true), true);
						numberAdded++;
						didBreak = false;
						break;
					}
					//Debug.Log("LineSwipeBase:LookForwardForCompress " + name + "-" + lookForwardDepth + " segment distance " + endSegment.distance + " is  small enough to ignore, take in " + i + " " + calcDirection);
				}
				
				if (numberAdded == extras && sourceSegmentNext != null && 
				    	!sourceSegmentNext.CompareDirection(identificationUsed, swipeSegments[incIndex(i, 1, isForward, swipeSegments.Count)].direction, isForward)) {
					//Debug.Log("LineSwipeBase:LookForwardForCompress " + name + "-" + lookForwardDepth + " next worked but used all extras but next direction does not match " + sourceSegmentNext.DebugStringDirections(isForward) + " with next swipe segemnt direction " + swipeSegments[incIndex(i, 1, isForward, swipeSegments.Count)].direction);
					break;
				}
				
				numberAdded++;
				didBreak = false;
				//Debug.Log("LineSwipeBase:LookForwardForCompress " + name + "-" + lookForwardDepth + " add segment success " + numberAdded + " " + calcDirection + " matches " + i);
				
			}
			
			if (sourceSegmentsIndex == (sourceSegments.Count - 1) && numberAdded !=  (extras + 1)) {
				//Debug.Log("LineSwipeBase:TryToCompessSegments " + name + "-" + lookForwardDepth + " success->failed on last source segment " + sourceSegmentsIndex + " and remaining segments " + (extras + 1) + " were not all used up " + numberAdded);
				SetError("Unable to match source segments on segment " + startIndex, false);
				return 0;
			}
			
			if (numberAdded == extras && sourceSegmentNext != null && !sourceSegmentNext.CompareDirection(identificationUsed, swipeSegments[incIndex(currentIndex, numberAdded, isForward, swipeSegments.Count)].direction, isForward)) {
				//Debug.Log("LineSwipeBase:TryToCompessSegments " + name + "-" + lookForwardDepth + " success->failed at segments but all used " + numberAdded + " and next direction does not match " + ((sourceSegmentNext == null) ? "null" : sourceSegmentNext.DebugStringDirections(isForward)) + " with next swipe segemnt direction " + swipeSegments[incIndex(currentIndex, numberAdded, isForward, swipeSegments.Count)].direction);
				SetError("Unable to match source segments on segment " + startIndex + " ending with matching end", true);
				return 0;
			}
			
			if (numberAdded < extras && sourceSegmentNext == null) {
				//Debug.Log("LineSwipeBase:TryToCompessSegments " + name + "-" + lookForwardDepth + " success->failed last source and did not use them all up " + numberAdded + " of " + extras);
				SetError("Unable to match remaining source segments on segments " + startIndex + " on ending segment", true);
				return 0;
			}
			
			if (didBreak && !didBreakIsOk) {
				numberAdded *= -1;
			}
			
			//Debug.Log("LineSwipeBase:TryToCompessSegments " + name + "-" + lookForwardDepth + " SUCCESS, returning " + numberAdded);
		}
		finally {
			lookForwardDepth--;
		}
		return numberAdded;
	}
	
	
	
	
	
			
	private int incIndex(int index, int numberOfTimes, bool isForward, int indexEnd)
	{
		int inc = isForward ? 1 : -1;
		for (int i = 0; i < numberOfTimes; i++) {		
			index += inc;
					
			if (isForward) {
				if (index >= indexEnd) {
					index = 0;
				}
			}
			else {
				if (index < 0) {
					index = indexEnd - 1;
				}
			}
		}
		return index;
	}

	private void DebugSwipeListDirection(string title, SwipeSegmentList swipeSegments, bool isForward, int index1, string text1, int index2, string text2)
	{
		if (isForward) {			
			for (int k = 0; k < swipeSegments.Count; k++) {
				Debug.Log(title + " -FORWARD- " + name + " segment " + k + "  " + swipeSegments[k].direction + " "  + ((index1 == k) ? text1 : "") + " " + ((index2 == k) ? text2 : ""));
			}
		}
		else {
			for (int k = swipeSegments.Count - 1; k >= 0; k--) {
				Debug.Log(title + " -BACKWARD- " + name + " segment " + k + "  " + SwipeGesture.GetReverseDirection(swipeSegments[k].direction) + " "  + ((index1 == k) ? text1 : "") + " " + ((index2 == k) ? text2 : ""));
			}
		}
	}
	
	private bool CheckLengths(SwipeSegment[] matchedSegments, SwipeSegmentList swipeSegments, int startIndex)
	{
		if (!doCompareLengths) {
			return true;
		}
		//Debug.Log("LineSwipe:CheckLengths " + name + " for " + matchedSegments.Length + " verses " + swipeSegments.Count);
		float[] lengths = new float[matchedSegments.Length];
		float minimumLen = float.MaxValue;
		int minimumRatio = int.MaxValue;
		float maximumLen = float.MinValue;
		for (int i = 0; i < matchedSegments.Length; i++) {
			lengths[i] = getDistance(matchedSegments[i]);
			//Debug.Log("LineSwipe:CheckLengths " + name + "figure lengths " + i + " is " + lengths[i] + ", direction " + matchedSegments[i].direction);
			if (!(i == startIndex && closed && firstLastMatch && extraSegment > 0)) {
				if (lengths[i] < minimumLen) {
					minimumLen = lengths[i];
				}
				if (lengths[i] > maximumLen) {
					maximumLen = lengths[i];
				}
			}
			if (sourceSegments[i].relativeSize > 0 && sourceSegments[i].relativeSize < minimumRatio) {
				minimumRatio = sourceSegments[i].relativeSize;
			}
		}
		if (closed && extraSegment > 0) {
			lengths[startIndex] += getDistance(swipeSegments[swipeSegments.Count - 1]);
			if (lengths[startIndex] < minimumLen) {
				minimumLen = lengths[startIndex];
			}
		}
		if (minimumRatio == int.MaxValue) {
			//Debug.Log("LineSwipe:CheckLengths no minimumRatio found, set to 1");
			minimumRatio = 1;
		}
			
		float unitSize = overRideUnitSize;
		if (unitSize <= 0) {
			unitSize = minimumLen / (float) minimumRatio;
		}
		//Debug.Log("LineSwipe:CheckLengths " + name + " minimumRatio=" + minimumRatio + ", minimumLen = " + minimumLen + ", maximumLen = " + maximumLen + ", unitSize= " + unitSize);
		
		float targetLen;
		float diff;
		float thisMaxDiff;
		for (int i = 0; i < matchedSegments.Length; i++) {
			//Debug.Log("LineSwipe:CheckLengths " + name + " comparing " + i + " using " + sourceSegments[i].sizeSpec + " direction " + sourceSegments[i].DebugStringDirections(true));
			if (sourceSegments[i].sizeSpec == Segment.SizeSpec.Ratio) {
				if (sourceSegments[i].relativeSize > 0) {
					targetLen = unitSize * (float) sourceSegments[i].relativeSize;
					diff = Mathf.Abs(lengths[i] - targetLen);
					thisMaxDiff = unitSize * sourceSegments[i].getMatchLengthDiffPercent(identificationUsed, matchLengthDiffPercent);
					if (Mathf.Abs(diff) > thisMaxDiff) {
						//Debug.Log("LineSwipe:CheckLengths " + name + " length failed on Ratio " + i + ", diff=" + diff + ", targetLen=" + targetLen + ", lengths=" + lengths[i]);
						SetError("Segment length ratio " + sourceSegments[i].relativeSize + " check on " + i, true);
						return false;
					}
				}
			}
			else if (sourceSegments[i].sizeSpec == Segment.SizeSpec.Bigger) {
				targetLen = unitSize * (float) sourceSegments[i].relativeSize;
				if (lengths[i] < targetLen) {
					//Debug.Log("LineSwipe:CheckLengths " + name + " segment " + i + " length failed on Bigger, len=" + lengths[i] + ", targetLen=" + targetLen);
					SetError("Bigger segmentthan " + sourceSegments[i].relativeSize + " failed check on " + i, true);
					return false;
				}					
			}
			else if (sourceSegments[i].sizeSpec == Segment.SizeSpec.Smaller) {
				targetLen = unitSize * (float) sourceSegments[i].relativeSize;
				if (lengths[i] <= targetLen) {
					//Debug.Log("LineSwipe:CheckLengths " + name + " segment " + i + " length failed on Bigger, len=" + lengths[i] + ", targetLen=" + targetLen);
					SetError("Smaller segment than " + sourceSegments[i].relativeSize + " failed check on " + i, true);
					return false;
				}					
			}
			else if (sourceSegments[i].sizeSpec == Segment.SizeSpec.Smallest) {
				if (lengths[i] > minimumLen) {
					//Debug.Log("LineSwipe:CheckLengths " + name + " segment " + i + " length failed on Smallest, len=" + lengths[i]);
					SetError("Smallest segment check failed  failed on " + i, true);
					return false;
				}					
			}
			else if (sourceSegments[i].sizeSpec == Segment.SizeSpec.Biggest) {
				if (lengths[i] > maximumLen) {
					//Debug.Log("LineSwipe:CheckLengths " + name + " segment " + i + " length failed on Biggest, len=" + lengths[i]);
					SetError("Biggest segment check failed  failed on " + i, true);
					return false;
				}					
			}
			else if (sourceSegments[i].sizeSpec == Segment.SizeSpec.Ignore) {
			}
		}
				
		return true;
	}
	
	protected float getDistance(SwipeSegment swipeSegment) 
	{
		Vector2 vectDiff = swipeSegment.startPosition - swipeSegment.endPosition;
		return vectDiff.magnitude;
	}
	
	private bool tryHarder()
	{
		if (identificationUsed == LineGesture.LineIdentification.Precise) {
			return false;
		}
		return true;
	}

}

