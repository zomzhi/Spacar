using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LineRelationship
{
	public enum LinePosition {Percent, Start, End, Upper, Lower, Left, Right, BetweenTopBottom, BetweenLeftRight}
	
	public LineSwipe targetLine;
	public int targetSegmentNum;
	public LinePosition targetPosition = LinePosition.Percent;
	public float targetPercentPosition;
	public LineSwipe relativeLine;
	public int relativeSegmentNum;
	public LinePosition relativePosition = LinePosition.Percent;
	public float relativePercentPosition;
	
	public LineRelationship(LineSwipe targetLineIn,
	                        int targetSegmentNumIn,
	                        LinePosition targetPositionIn,
	                        LineSwipe relativeLineIn,
	                        int relativeSegmentNumIn,
	                        LinePosition relativePositionIn)
	{
		targetLine = targetLineIn;
		targetSegmentNum = targetSegmentNumIn;
		targetPosition = targetPositionIn;
		relativeLine = relativeLineIn;
		relativeSegmentNum = relativeSegmentNumIn;
		relativePosition = relativePositionIn;
	}
	
	public LineRelationship(LineSwipe targetLineIn,
	                        int targetSegmentNumIn,
	                        float targetPercentPositionIn,
	                        LineSwipe relativeLineIn,
	                        int relativeSegmentNumIn,
	                        LinePosition relativePositionIn)
	{
		targetLine = targetLineIn;
		targetSegmentNum = targetSegmentNumIn;
		targetPercentPosition = targetPercentPositionIn;
		relativeLine = relativeLineIn;
		relativeSegmentNum = relativeSegmentNumIn;
		relativePosition = relativePositionIn;
	}
	
	public LineRelationship(LineSwipe targetLineIn,
	                        int targetSegmentNumIn,
	                        float targetPercentPositionIn,
	                        LineSwipe relativeLineIn,
	                        int relativeSegmentNumIn,
	                        float relativePercentPositionIn)
	{
		targetLine = targetLineIn;
		targetSegmentNum = targetSegmentNumIn;
		targetPercentPosition = targetPercentPositionIn;
		relativeLine = relativeLineIn;
		relativeSegmentNum = relativeSegmentNumIn;
		relativePercentPosition = relativePercentPositionIn;
	}
	
	public LineRelationship(LineSwipe targetLineIn,
	                        int targetSegmentNumIn,
	                        LinePosition targetPositionIn,
	                        LineSwipe relativeLineIn,
	                        int relativeSegmentNumIn,
	                        float relativePercentPositionIn)
	{
		targetLine = targetLineIn;
		targetSegmentNum = targetSegmentNumIn;
		targetPosition = targetPositionIn;
		relativeLine = relativeLineIn;
		relativeSegmentNum = relativeSegmentNumIn;
		relativePercentPosition = relativePercentPositionIn;
	}}

