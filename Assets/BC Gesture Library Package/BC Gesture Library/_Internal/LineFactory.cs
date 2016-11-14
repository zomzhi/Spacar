using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class LineFactory
{
	public enum LineType {None, A, E, F, H, I, K, L, M, N, T, V, W, X, Y, Z, 
		Number1, Number4, Number7, Plus, Minus, RightCheck, LeftCheck, Square, Rectangle}
	
	private LineSwipeBase[] lineSwipes;
	
	private static LineFactory _factory;
	
	public static LineFactory Factory ()
	{
		if (_factory == null) {
			_factory = new LineFactory();
		}
		return _factory;
	}
	
	public LineFactory()
	{
		lineSwipes = new LineSwipeBase[Enum.GetNames(typeof( LineType )).Length];
		
		LineSwipe lineSwipe;
		LineSwipe lineSwipe2;
		LineSwipe lineSwipe3;
		LineSwipe lineSwipe4;
		CompoundLineSwipe compoundLineSwipe;
		MultipleLineSwipe multipleLineSwipe;
		
		//A
		multipleLineSwipe = new MultipleLineSwipe(LineType.A.ToString());
		lineSwipes[(int)LineType.A] = multipleLineSwipe;
		// A with 2 lines - angle around and middle
		compoundLineSwipe = new CompoundLineSwipe(LineType.A.ToString());
		lineSwipe = new LineSwipe(LineType.A.ToString());
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Plus45, 1, Segment.SizeSpec.Bigger));		
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Plus135, 1, Segment.SizeSpec.Bigger));		
		compoundLineSwipe.AddLine(lineSwipe);		
		lineSwipe2 = new LineSwipe(LineType.A.ToString());
		lineSwipe2.AddSegment(new Segment(SwipeGesture.SwipeDirection.Right, 1, Segment.SizeSpec.Smallest));
		compoundLineSwipe.AddLine(lineSwipe2);
		compoundLineSwipe.AddRelationship(new LineRelationship(lineSwipe, 0, 0.5f, lineSwipe2, 0, LineRelationship.LinePosition.Left));
		compoundLineSwipe.AddRelationship(new LineRelationship(lineSwipe, 1, 0.5f, lineSwipe2, 0, LineRelationship.LinePosition.Right));
		compoundLineSwipe.AddRelationship(new LineRelationship(lineSwipe, 0, LineRelationship.LinePosition.BetweenTopBottom, lineSwipe2, 0, 0.5f));
		multipleLineSwipe.AddLineSwipe(compoundLineSwipe);
		// A with 3 lines - up, down, left
		compoundLineSwipe = new CompoundLineSwipe(LineType.A.ToString());
		lineSwipe = new LineSwipe(LineType.A.ToString());
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Plus45, 1, Segment.SizeSpec.Bigger));		
		compoundLineSwipe.AddLine(lineSwipe);		
		lineSwipe2 = new LineSwipe(LineType.A.ToString());
		lineSwipe2.AddSegment(new Segment(SwipeGesture.SwipeDirection.Plus135, 1, Segment.SizeSpec.Bigger));		
		compoundLineSwipe.AddLine(lineSwipe2);		
		lineSwipe3 = new LineSwipe(LineType.A.ToString());
		lineSwipe3.AddSegment(new Segment(SwipeGesture.SwipeDirection.Right, 1, Segment.SizeSpec.Smallest));
		compoundLineSwipe.AddLine(lineSwipe3);
		compoundLineSwipe.AddRelationship(new LineRelationship(lineSwipe, 0, LineRelationship.LinePosition.Upper, lineSwipe2, 0, LineRelationship.LinePosition.Upper));
		compoundLineSwipe.AddRelationship(new LineRelationship(lineSwipe, 0, 0.5f, lineSwipe3, 0, LineRelationship.LinePosition.Left));
		compoundLineSwipe.AddRelationship(new LineRelationship(lineSwipe, 0, 0.5f, lineSwipe3, 0, LineRelationship.LinePosition.Right));
		compoundLineSwipe.AddRelationship(new LineRelationship(lineSwipe, 0, LineRelationship.LinePosition.BetweenTopBottom, lineSwipe3, 0, 0.5f));
		multipleLineSwipe.AddLineSwipe(compoundLineSwipe);
		
		// E
		multipleLineSwipe = new MultipleLineSwipe(LineType.E.ToString());
		lineSwipes[(int)LineType.E] = multipleLineSwipe;
		// E with 4 lines
		compoundLineSwipe = new CompoundLineSwipe(LineType.E.ToString());
		lineSwipe = new LineSwipe(LineType.E.ToString());
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Down, 0, Segment.SizeSpec.Biggest));		
		compoundLineSwipe.AddLine(lineSwipe);		
		lineSwipe2 = new LineSwipe(LineType.E.ToString());
		lineSwipe2.AddSegment(new Segment(SwipeGesture.SwipeDirection.Right, 1, Segment.SizeSpec.Ratio));
		compoundLineSwipe.AddLine(lineSwipe2);
		lineSwipe3 = new LineSwipe(LineType.E.ToString());
		lineSwipe3.AddSegment(new Segment(SwipeGesture.SwipeDirection.Right, 1, Segment.SizeSpec.Ratio));
		compoundLineSwipe.AddLine(lineSwipe3);
		lineSwipe4 = new LineSwipe(LineType.E.ToString());
		lineSwipe4.AddSegment(new Segment(SwipeGesture.SwipeDirection.Right, 1, Segment.SizeSpec.Ratio));
		compoundLineSwipe.AddLine(lineSwipe4);		
		compoundLineSwipe.AddRelationship(new LineRelationship(lineSwipe, 0, LineRelationship.LinePosition.Upper, lineSwipe2, 0, LineRelationship.LinePosition.Left));
		compoundLineSwipe.AddRelationship(new LineRelationship(lineSwipe, 0, .5f, lineSwipe3, 0, LineRelationship.LinePosition.Start));
		compoundLineSwipe.AddRelationship(new LineRelationship(lineSwipe, 0, LineRelationship.LinePosition.Lower, lineSwipe4, 0, LineRelationship.LinePosition.Left));
		compoundLineSwipe.AddRelationship(new LineRelationship(lineSwipe, 0, LineRelationship.LinePosition.BetweenTopBottom, lineSwipe3, 0,LineRelationship.LinePosition.Left));
		multipleLineSwipe.AddLineSwipe(compoundLineSwipe);
		// E with 2 lines - one outside and one middle
		compoundLineSwipe = new CompoundLineSwipe(LineType.E.ToString());
		lineSwipe = new LineSwipe(LineType.E.ToString());
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Left, 1, Segment.SizeSpec.Ratio));		
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Down, 0, Segment.SizeSpec.Biggest));		
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Right, 1, Segment.SizeSpec.Ratio));		
		compoundLineSwipe.AddLine(lineSwipe);		
		lineSwipe2 = new LineSwipe(LineType.E.ToString());
		lineSwipe2.AddSegment(new Segment(SwipeGesture.SwipeDirection.Right, 1, Segment.SizeSpec.Ratio));
		compoundLineSwipe.AddLine(lineSwipe2);
		compoundLineSwipe.AddRelationship(new LineRelationship(lineSwipe, 1, .5f, lineSwipe2, 0, LineRelationship.LinePosition.Start));
		compoundLineSwipe.AddRelationship(new LineRelationship(lineSwipe, 1, LineRelationship.LinePosition.BetweenTopBottom, lineSwipe2, 0,LineRelationship.LinePosition.Left));
		multipleLineSwipe.AddLineSwipe(compoundLineSwipe);
		// E with 3 lines - top/back,  middle, bottom
		compoundLineSwipe = new CompoundLineSwipe(LineType.E.ToString());
		lineSwipe = new LineSwipe(LineType.E.ToString());
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Left, 1, Segment.SizeSpec.Ratio));		
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Down, 0, Segment.SizeSpec.Biggest));		
		compoundLineSwipe.AddLine(lineSwipe);		
		lineSwipe2 = new LineSwipe(LineType.E.ToString());
		lineSwipe2.AddSegment(new Segment(SwipeGesture.SwipeDirection.Right, 1, Segment.SizeSpec.Ratio));
		compoundLineSwipe.AddLine(lineSwipe2);
		lineSwipe3 = new LineSwipe(LineType.E.ToString());
		lineSwipe3.AddSegment(new Segment(SwipeGesture.SwipeDirection.Right, 1, Segment.SizeSpec.Ratio));
		compoundLineSwipe.AddLine(lineSwipe3);
		compoundLineSwipe.AddRelationship(new LineRelationship(lineSwipe, 1, .5f, lineSwipe2, 0, LineRelationship.LinePosition.Start));
		compoundLineSwipe.AddRelationship(new LineRelationship(lineSwipe, 1, LineRelationship.LinePosition.Lower, lineSwipe3, 0, LineRelationship.LinePosition.Left));
		compoundLineSwipe.AddRelationship(new LineRelationship(lineSwipe, 1, LineRelationship.LinePosition.BetweenTopBottom, lineSwipe2, 0,LineRelationship.LinePosition.Left));
		multipleLineSwipe.AddLineSwipe(compoundLineSwipe);
		// E with 3 lines - back/bottom,  top, middle
		compoundLineSwipe = new CompoundLineSwipe(LineType.E.ToString());
		lineSwipe = new LineSwipe(LineType.E.ToString());
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Down, 1, Segment.SizeSpec.Ratio));		
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Right, 0, Segment.SizeSpec.Biggest));		
		compoundLineSwipe.AddLine(lineSwipe);		
		lineSwipe2 = new LineSwipe(LineType.E.ToString());
		lineSwipe2.AddSegment(new Segment(SwipeGesture.SwipeDirection.Right, 1, Segment.SizeSpec.Ratio));
		compoundLineSwipe.AddLine(lineSwipe2);
		lineSwipe3 = new LineSwipe(LineType.E.ToString());
		lineSwipe3.AddSegment(new Segment(SwipeGesture.SwipeDirection.Right, 1, Segment.SizeSpec.Ratio));
		compoundLineSwipe.AddLine(lineSwipe3);
		compoundLineSwipe.AddRelationship(new LineRelationship(lineSwipe, 0, LineRelationship.LinePosition.Upper, lineSwipe2, 0, LineRelationship.LinePosition.Left));
		compoundLineSwipe.AddRelationship(new LineRelationship(lineSwipe, 1, .5f, lineSwipe3, 0, LineRelationship.LinePosition.Start));
		compoundLineSwipe.AddRelationship(new LineRelationship(lineSwipe, 0, LineRelationship.LinePosition.BetweenTopBottom, lineSwipe2, 0 ,LineRelationship.LinePosition.Left));
		multipleLineSwipe.AddLineSwipe(compoundLineSwipe);
		

		// F
		multipleLineSwipe = new MultipleLineSwipe(LineType.F.ToString());
		lineSwipes[(int)LineType.F] = multipleLineSwipe;
		
		// F with 3 lines
		compoundLineSwipe = new CompoundLineSwipe(LineType.F.ToString());
		lineSwipe = new LineSwipe(LineType.F.ToString());
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Down, 0, Segment.SizeSpec.Biggest));		
		compoundLineSwipe.AddLine(lineSwipe);		
		lineSwipe2 = new LineSwipe(LineType.F.ToString());
		lineSwipe2.AddSegment(new Segment(SwipeGesture.SwipeDirection.Right, 1, Segment.SizeSpec.Ratio));
		compoundLineSwipe.AddLine(lineSwipe2);
		lineSwipe3 = new LineSwipe(LineType.F.ToString());
		lineSwipe3.AddSegment(new Segment(SwipeGesture.SwipeDirection.Right, 0, Segment.SizeSpec.Smallest));
		compoundLineSwipe.AddLine(lineSwipe3);
		compoundLineSwipe.AddRelationship(new LineRelationship(lineSwipe, 0, LineRelationship.LinePosition.Upper, lineSwipe2, 0, LineRelationship.LinePosition.Left));
		compoundLineSwipe.AddRelationship(new LineRelationship(lineSwipe, 0, .4f, lineSwipe3, 0, LineRelationship.LinePosition.Left));
		multipleLineSwipe.AddLineSwipe(compoundLineSwipe);
		// F with 2 lines - top/down and middle
		compoundLineSwipe = new CompoundLineSwipe(LineType.F.ToString());
		lineSwipe = new LineSwipe(LineType.F.ToString());
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Left, 1, Segment.SizeSpec.Ratio));		
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Down, 0, Segment.SizeSpec.Biggest));		
		compoundLineSwipe.AddLine(lineSwipe);		
		lineSwipe2 = new LineSwipe(LineType.F.ToString());
		lineSwipe2.AddSegment(new Segment(SwipeGesture.SwipeDirection.Right, 1, Segment.SizeSpec.Ratio));
		compoundLineSwipe.AddLine(lineSwipe2);
		compoundLineSwipe.AddRelationship(new LineRelationship(lineSwipe, 1, .4f, lineSwipe2, 0, LineRelationship.LinePosition.Left));
		compoundLineSwipe.AddRelationship(new LineRelationship(lineSwipe, 1, LineRelationship.LinePosition.BetweenTopBottom, lineSwipe2, 0,LineRelationship.LinePosition.Left));
		multipleLineSwipe.AddLineSwipe(compoundLineSwipe);

		// H
		compoundLineSwipe = new CompoundLineSwipe(LineType.H.ToString());
		lineSwipe = new LineSwipe(LineType.H.ToString());
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Down, 1, Segment.SizeSpec.Bigger));		
		compoundLineSwipe.AddLine(lineSwipe);		
		lineSwipe2 = new LineSwipe(LineType.H.ToString());
		lineSwipe2.AddSegment(new Segment(SwipeGesture.SwipeDirection.Down, 1, Segment.SizeSpec.Bigger));
		compoundLineSwipe.AddLine(lineSwipe2);
		lineSwipe3 = new LineSwipe(LineType.H.ToString());
		lineSwipe3.AddSegment(new Segment(SwipeGesture.SwipeDirection.Right, 1, Segment.SizeSpec.Smallest));
		compoundLineSwipe.AddLine(lineSwipe3);
		compoundLineSwipe.AddRelationship(new LineRelationship(lineSwipe, 0, 0.5f, lineSwipe3, 0, LineRelationship.LinePosition.Left));
		compoundLineSwipe.AddRelationship(new LineRelationship(lineSwipe2, 0, 0.5f, lineSwipe3, 0, LineRelationship.LinePosition.Right));
		compoundLineSwipe.AddRelationship(new LineRelationship(lineSwipe, 0, LineRelationship.LinePosition.BetweenTopBottom, lineSwipe3, 0, LineRelationship.LinePosition.Left));
		lineSwipes[(int)LineType.H] = compoundLineSwipe;

		// I
		lineSwipe = new LineSwipe(LineType.I.ToString());
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Down, 1, Segment.SizeSpec.Ratio));
		lineSwipes[(int)LineType.I] = lineSwipe;

		// K
		multipleLineSwipe = new MultipleLineSwipe(LineType.K.ToString());
		lineSwipes[(int)LineType.K] = multipleLineSwipe;
		// K with 3 lines
		compoundLineSwipe = new CompoundLineSwipe(LineType.K.ToString());
		lineSwipe = new LineSwipe(LineType.K.ToString());
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Down, 0, Segment.SizeSpec.Biggest));		
		compoundLineSwipe.AddLine(lineSwipe);		
		lineSwipe2 = new LineSwipe(LineType.K.ToString());
		lineSwipe2.AddSegment(new Segment(SwipeGesture.SwipeDirection.Plus45, 1, Segment.SizeSpec.Bigger));
		compoundLineSwipe.AddLine(lineSwipe2);
		lineSwipe3 = new LineSwipe(LineType.K.ToString());
		lineSwipe3.AddSegment(new Segment(SwipeGesture.SwipeDirection.Plus135, 1, Segment.SizeSpec.Smallest));
		compoundLineSwipe.AddLine(lineSwipe3);
		compoundLineSwipe.AddRelationship(new LineRelationship(lineSwipe, 0, 0.6f, lineSwipe2, 0, LineRelationship.LinePosition.Start));
		compoundLineSwipe.AddRelationship(new LineRelationship(lineSwipe2, 0, .1f, lineSwipe3, 0, LineRelationship.LinePosition.Start));
		compoundLineSwipe.AddRelationship(new LineRelationship(lineSwipe, 0, LineRelationship.LinePosition.BetweenTopBottom, lineSwipe2, 0, LineRelationship.LinePosition.Left));
		multipleLineSwipe.AddLineSwipe(compoundLineSwipe);
		// K with 2 lines - 1 vertical 1 angle
		compoundLineSwipe = new CompoundLineSwipe(LineType.K.ToString());
		lineSwipe = new LineSwipe(LineType.K.ToString());
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Down, 0, Segment.SizeSpec.Biggest));		
		compoundLineSwipe.AddLine(lineSwipe);		
		lineSwipe2 = new LineSwipe(LineType.K.ToString());
		lineSwipe2.AddSegment(new Segment(SwipeGesture.SwipeDirection.Minus135, 1, Segment.SizeSpec.Ratio));
		lineSwipe2.AddSegment(new Segment(SwipeGesture.SwipeDirection.Plus135, 1, Segment.SizeSpec.Ratio));
		compoundLineSwipe.AddLine(lineSwipe2);
		compoundLineSwipe.AddRelationship(new LineRelationship(lineSwipe, 0, .5f, lineSwipe2, 0, LineRelationship.LinePosition.End));
		compoundLineSwipe.AddRelationship(new LineRelationship(lineSwipe, 0, LineRelationship.LinePosition.BetweenTopBottom, lineSwipe2, 0, LineRelationship.LinePosition.Left));
		multipleLineSwipe.AddLineSwipe(compoundLineSwipe);

		// L
		lineSwipe = new LineSwipe(LineType.L.ToString());
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Down, 1, Segment.SizeSpec.Bigger));
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Right, 1, Segment.SizeSpec.Smallest));
		lineSwipe.GetSegment(0).matchLengthDiffPercent = 0.5f;
		lineSwipes[(int)LineType.L] = lineSwipe;
		
		// M
		lineSwipe = new LineSwipe(LineType.M.ToString());
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Up, 1, SwipeGesture.SwipeDirection.Plus45, Segment.SizeSpec.Ratio));
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Plus135, 1, SwipeGesture.SwipeDirection.Down, Segment.SizeSpec.Ratio));
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Plus45, 1, SwipeGesture.SwipeDirection.Up, Segment.SizeSpec.Ratio));
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Down, 1, SwipeGesture.SwipeDirection.Plus135, Segment.SizeSpec.Ratio));
		lineSwipes[(int)LineType.M] = lineSwipe;
		
		// N
		multipleLineSwipe = new MultipleLineSwipe(LineType.N.ToString());
		lineSwipes[(int)LineType.N] = multipleLineSwipe;
		// N with 1 line
		lineSwipe = new LineSwipe(LineType.N.ToString());
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Up, 1, Segment.SizeSpec.Ratio));
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Plus135, 1, Segment.SizeSpec.Ratio));
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Up, 1, Segment.SizeSpec.Ratio));
		lineSwipe.GetSegment(1).matchLengthDiffPercent = 0.6f;
		multipleLineSwipe.AddLineSwipe(lineSwipe);		
		// N with 2 lines - 1 angle, 1 vertical
		compoundLineSwipe = new CompoundLineSwipe(LineType.N.ToString());
		lineSwipe = new LineSwipe(LineType.N.ToString());
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Up, 1, Segment.SizeSpec.Ratio));		
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Plus135, 1, Segment.SizeSpec.Ratio));		
		compoundLineSwipe.AddLine(lineSwipe);		
		lineSwipe2 = new LineSwipe(LineType.N.ToString());
		lineSwipe2.AddSegment(new Segment(SwipeGesture.SwipeDirection.Up, 1, Segment.SizeSpec.Ratio));
		compoundLineSwipe.AddLine(lineSwipe2);
		compoundLineSwipe.AddRelationship(new LineRelationship(lineSwipe, 1, LineRelationship.LinePosition.Lower, lineSwipe2, 0, LineRelationship.LinePosition.Lower));
		multipleLineSwipe.AddLineSwipe(compoundLineSwipe);
		// N with 2 lines - 1 vertical, 1 angle
		compoundLineSwipe = new CompoundLineSwipe(LineType.N.ToString());
		lineSwipe = new LineSwipe(LineType.N.ToString());
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Up, 1, Segment.SizeSpec.Ratio));		
		compoundLineSwipe.AddLine(lineSwipe);		
		lineSwipe2 = new LineSwipe(LineType.N.ToString());
		lineSwipe2.AddSegment(new Segment(SwipeGesture.SwipeDirection.Plus135, 1, Segment.SizeSpec.Ratio));		
		lineSwipe2.AddSegment(new Segment(SwipeGesture.SwipeDirection.Up, 1, Segment.SizeSpec.Ratio));
		compoundLineSwipe.AddLine(lineSwipe2);
		compoundLineSwipe.AddRelationship(new LineRelationship(lineSwipe, 0, LineRelationship.LinePosition.Upper, lineSwipe2, 0, LineRelationship.LinePosition.Upper));
		multipleLineSwipe.AddLineSwipe(compoundLineSwipe);
		// N with 3 lines
		compoundLineSwipe = new CompoundLineSwipe(LineType.N.ToString());
		lineSwipe = new LineSwipe(LineType.N.ToString());
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Up, 1, Segment.SizeSpec.Ratio));		
		compoundLineSwipe.AddLine(lineSwipe);		
		lineSwipe2 = new LineSwipe(LineType.N.ToString());
		lineSwipe2.AddSegment(new Segment(SwipeGesture.SwipeDirection.Plus135, 1, Segment.SizeSpec.Ratio));		
		compoundLineSwipe.AddLine(lineSwipe2);
		lineSwipe3 = new LineSwipe(LineType.N.ToString());
		lineSwipe3.AddSegment(new Segment(SwipeGesture.SwipeDirection.Up, 1, Segment.SizeSpec.Ratio));		
		compoundLineSwipe.AddLine(lineSwipe3);
		compoundLineSwipe.AddRelationship(new LineRelationship(lineSwipe, 0, LineRelationship.LinePosition.Upper, lineSwipe2, 0, LineRelationship.LinePosition.Upper));
		compoundLineSwipe.AddRelationship(new LineRelationship(lineSwipe2, 0, LineRelationship.LinePosition.Lower, lineSwipe3, 0, LineRelationship.LinePosition.Lower));
		multipleLineSwipe.AddLineSwipe(compoundLineSwipe);

		// T
		compoundLineSwipe = new CompoundLineSwipe(LineType.T.ToString());
		lineSwipe = new LineSwipe(LineType.T.ToString());
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Right, 1, Segment.SizeSpec.Ratio));
		compoundLineSwipe.AddLine(lineSwipe);
		lineSwipe2 = new LineSwipe(LineType.T.ToString());
		lineSwipe2.AddSegment(new Segment(SwipeGesture.SwipeDirection.Down, 1, Segment.SizeSpec.Bigger));
		compoundLineSwipe.AddLine(lineSwipe2);
		compoundLineSwipe.AddRelationship(new LineRelationship(lineSwipe, 0, .5f, lineSwipe2, 0, LineRelationship.LinePosition.Upper));
		compoundLineSwipe.AddRelationship(new LineRelationship(lineSwipe, 0, LineRelationship.LinePosition.BetweenLeftRight, lineSwipe2, 0, LineRelationship.LinePosition.Upper));
		lineSwipes[(int)LineType.T] = compoundLineSwipe;
		
		// V
		lineSwipe = new LineSwipe(LineType.V.ToString());
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Plus135, 1, Segment.SizeSpec.Ratio));
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Plus45, 1, Segment.SizeSpec.Ratio));
		lineSwipes[(int)LineType.V] = lineSwipe;
		                     
		// W
		lineSwipe = new LineSwipe(LineType.W.ToString()); 
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Plus135, 1, SwipeGesture.SwipeDirection.Down, Segment.SizeSpec.Bigger));
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Plus45, 1, SwipeGesture.SwipeDirection.Up, Segment.SizeSpec.Ratio));
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Plus135, 1, SwipeGesture.SwipeDirection.Down, Segment.SizeSpec.Ratio));
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Plus45, 1, SwipeGesture.SwipeDirection.Up, Segment.SizeSpec.Bigger));
		lineSwipes[(int)LineType.W] = lineSwipe;

		// X
		compoundLineSwipe = new CompoundLineSwipe(LineType.X.ToString());
		lineSwipe = new LineSwipe(LineType.X.ToString());
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Plus45, 1, Segment.SizeSpec.Ratio));
		compoundLineSwipe.AddLine(lineSwipe);
		lineSwipe2 = new LineSwipe(LineType.X.ToString());
		lineSwipe2.AddSegment(new Segment(SwipeGesture.SwipeDirection.Plus135, 1, Segment.SizeSpec.Ratio));
		compoundLineSwipe.AddLine(lineSwipe2);
		compoundLineSwipe.AddRelationship(new LineRelationship(lineSwipe, 0, .5f, lineSwipe2, 0, 0.5f));
		lineSwipes[(int)LineType.X] = compoundLineSwipe;
		
		// Y
		multipleLineSwipe = new MultipleLineSwipe(LineType.Y.ToString());
		lineSwipes[(int)LineType.Y] = multipleLineSwipe;
		
		// Y with 2 - V and down
		compoundLineSwipe = new CompoundLineSwipe(LineType.Y.ToString());
		lineSwipe = new LineSwipe(LineType.Y.ToString());
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Plus135, 1, Segment.SizeSpec.Ratio));
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Plus45, 1, Segment.SizeSpec.Ratio));
		compoundLineSwipe.AddLine(lineSwipe);
		lineSwipe2 = new LineSwipe(LineType.Y.ToString());
		lineSwipe2.AddSegment(new Segment(SwipeGesture.SwipeDirection.Down, 1, Segment.SizeSpec.Ratio));
		compoundLineSwipe.AddLine(lineSwipe2);
		compoundLineSwipe.AddRelationship(new LineRelationship(lineSwipe, 0, LineRelationship.LinePosition.End, lineSwipe2, 0, LineRelationship.LinePosition.Start));
		multipleLineSwipe.AddLineSwipe(compoundLineSwipe);
		// Y with 3
		compoundLineSwipe = new CompoundLineSwipe(LineType.Y.ToString());
		lineSwipe = new LineSwipe(LineType.Y.ToString());
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Plus135, 1, Segment.SizeSpec.Ratio));
		compoundLineSwipe.AddLine(lineSwipe);
		lineSwipe2 = new LineSwipe(LineType.Y.ToString());
		lineSwipe2.AddSegment(new Segment(SwipeGesture.SwipeDirection.Minus135, 1, Segment.SizeSpec.Ratio));
		compoundLineSwipe.AddLine(lineSwipe2);
		lineSwipe3 = new LineSwipe(LineType.Y.ToString());
		lineSwipe3.AddSegment(new Segment(SwipeGesture.SwipeDirection.Down, 1, Segment.SizeSpec.Ratio));
		compoundLineSwipe.AddLine(lineSwipe3);
		compoundLineSwipe.AddRelationship(new LineRelationship(lineSwipe, 0, LineRelationship.LinePosition.Lower, lineSwipe2, 0, LineRelationship.LinePosition.Upper));
		compoundLineSwipe.AddRelationship(new LineRelationship(lineSwipe, 0, LineRelationship.LinePosition.Lower, lineSwipe3, 0, LineRelationship.LinePosition.Upper));
		multipleLineSwipe.AddLineSwipe(compoundLineSwipe);
		// Y with 2 - down and side
		compoundLineSwipe = new CompoundLineSwipe(LineType.Y.ToString());
		lineSwipe = new LineSwipe(LineType.Y.ToString());
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Down, 1, Segment.SizeSpec.Ratio));
		compoundLineSwipe.AddLine(lineSwipe);
		lineSwipe2 = new LineSwipe(LineType.Y.ToString());
		lineSwipe2.AddSegment(new Segment(SwipeGesture.SwipeDirection.Minus135, 1, Segment.SizeSpec.Ratio));
		compoundLineSwipe.AddLine(lineSwipe2);
		compoundLineSwipe.AddRelationship(new LineRelationship(lineSwipe, 0, 0.5f, lineSwipe2, 0, LineRelationship.LinePosition.Lower));
		multipleLineSwipe.AddLineSwipe(compoundLineSwipe);

		// Z
		lineSwipe = new LineSwipe(LineType.Z.ToString());
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Right, 1, Segment.SizeSpec.Ratio));
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Minus135, 1, Segment.SizeSpec.Bigger));
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Right, 1, Segment.SizeSpec.Ratio));
		lineSwipes[(int)LineType.Z] = lineSwipe;
		                     
		// 1
		lineSwipe = new LineSwipe(LineType.Number1.ToString());
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Down, 1, Segment.SizeSpec.Ratio));
		lineSwipes[(int)LineType.Number1] = lineSwipe;
		                     
		// 4
		multipleLineSwipe = new MultipleLineSwipe(LineType.Number4.ToString());
		lineSwipes[(int)LineType.Number4] = multipleLineSwipe;
		// 4 with 3 segment
		lineSwipe = new LineSwipe(LineType.Number4.ToString());
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Up, 1, Segment.SizeSpec.Bigger));
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Minus135, 1, Segment.SizeSpec.Ratio));
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Right, 1, Segment.SizeSpec.Ratio));
		multipleLineSwipe.AddLineSwipe(lineSwipe);
		// 4 with 2 segments
		compoundLineSwipe = new CompoundLineSwipe(LineType.Y.ToString());
		lineSwipe = new LineSwipe(LineType.Y.ToString());
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Down, 1, SwipeGesture.SwipeDirection.Minus135, Segment.SizeSpec.Ratio));
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Right, 1, Segment.SizeSpec.Ratio));
		compoundLineSwipe.AddLine(lineSwipe);
		lineSwipe2 = new LineSwipe(LineType.Y.ToString());
		lineSwipe2.AddSegment(new Segment(SwipeGesture.SwipeDirection.Down, 0, Segment.SizeSpec.Biggest));
		compoundLineSwipe.AddLine(lineSwipe2);
		compoundLineSwipe.AddRelationship(new LineRelationship(lineSwipe, 1, 0.7f, lineSwipe2, 0, 0.5f));
		compoundLineSwipe.AddRelationship(new LineRelationship(lineSwipe, 1, LineRelationship.LinePosition.BetweenLeftRight, lineSwipe2, 0, 0.5f));
		multipleLineSwipe.AddLineSwipe(compoundLineSwipe);
		                     
		// 7
		lineSwipe = new LineSwipe(LineType.Number7.ToString());
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Right, 1, Segment.SizeSpec.Ratio));
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Minus135, 1, Segment.SizeSpec.Bigger));
		lineSwipes[(int)LineType.Number7] = lineSwipe;
		
		// Plus
		compoundLineSwipe = new CompoundLineSwipe(LineType.Plus.ToString());
		lineSwipe = new LineSwipe(LineType.Plus.ToString());
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Right, 1, Segment.SizeSpec.Ratio));
		compoundLineSwipe.AddLine(lineSwipe);
		lineSwipe2 = new LineSwipe(LineType.Plus.ToString());
		lineSwipe2.AddSegment(new Segment(SwipeGesture.SwipeDirection.Down, 1, Segment.SizeSpec.Ratio));
		compoundLineSwipe.AddLine(lineSwipe2);
		compoundLineSwipe.AddRelationship(new LineRelationship(lineSwipe, 0, .5f, lineSwipe2, 0, 0.5f));
		compoundLineSwipe.AddRelationship(new LineRelationship(lineSwipe, 0, LineRelationship.LinePosition.BetweenLeftRight, lineSwipe2, 0, 0.5f));
		compoundLineSwipe.AddRelationship(new LineRelationship(lineSwipe2, 0, LineRelationship.LinePosition.BetweenTopBottom, lineSwipe, 0, 0.5f));
		lineSwipes[(int)LineType.Plus] = compoundLineSwipe;
		
		// Minus
		lineSwipe = new LineSwipe(LineType.Minus.ToString());
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Right, 1, Segment.SizeSpec.Ratio));
		lineSwipes[(int)LineType.Minus] = lineSwipe;
		                     
		// Right Check
		lineSwipe = new LineSwipe(LineType.RightCheck.ToString());
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Plus135, 1, Segment.SizeSpec.Smallest));
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Plus45, 1, Segment.SizeSpec.Bigger));
		lineSwipes[(int)LineType.RightCheck] = lineSwipe;

		// Left Check
		lineSwipe = new LineSwipe(LineType.LeftCheck.ToString());
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Minus135, 1, Segment.SizeSpec.Smallest));
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Minus45, 1, Segment.SizeSpec.Bigger));
		lineSwipes[(int)LineType.LeftCheck] = lineSwipe;

		// Square
		lineSwipe = new LineSwipe(LineType.Square.ToString());
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Down, 1, Segment.SizeSpec.Ratio));
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Right, 1, Segment.SizeSpec.Ratio));
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Up, 1, Segment.SizeSpec.Ratio));
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Left, 1, Segment.SizeSpec.Ratio));
		lineSwipe.closed = true;
		lineSwipes[(int)LineType.Square] = lineSwipe;

		// Box
		lineSwipe = new LineSwipe(LineType.Rectangle.ToString());
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Down, 0, Segment.SizeSpec.Ignore));
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Right, 0, Segment.SizeSpec.Ignore));
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Up, 0, Segment.SizeSpec.Ignore));
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Left, 0, Segment.SizeSpec.Ignore));
		lineSwipe.closed = true;
		lineSwipe.maintainAspectRatio = false;
		lineSwipes[(int)LineType.Rectangle] = lineSwipe;
		                     
		// Circle
//		lineSwipe = new LineSwipe(LineType.Circle.ToString());
//		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Right, 1, Segment.SizeSpec.Ratio));
//		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Plus135, 1, Segment.SizeSpec.Ratio));
//		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Down, 1, Segment.SizeSpec.Ratio));
//		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Minus135, 1, Segment.SizeSpec.Ratio));
//		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Left, 1, Segment.SizeSpec.Ratio));
//		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Minus45, 1, Segment.SizeSpec.Ratio));
//		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Up, 1, Segment.SizeSpec.Ratio));
//		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Plus45, 1, Segment.SizeSpec.Ratio));
//		lineSwipe.closed = true;
//		lineSwipes[(int)LineType.Circle] = lineSwipe;
	}
	
	public bool IsCompound(LineType lineType)
	{
		return lineSwipes[(int)lineType].IsCompound();
	}
	
	public int GetCount(LineType lineType)
	{
		return lineSwipes[(int)lineType].Count();
	}
	public int GetMaxSegmentCount(LineType lineType)
	{
		return lineSwipes[(int)lineType].GetMaxSegment();
	}
	
	public LineSwipeBase GetLineSwipe(LineType lineType)
	{
		//Debug.Log("LineFactory:GetLineSwipe " + lineType + " return " + lineSwipes[(int)lineType]);
		return lineSwipes[(int)lineType];
	}
	
	public LineType FindLineType(LineType[] possibleTypes, List<SwipeSegmentList> swipeList, LineGesture.LineIdentification lineIdentification, LineGesture.LineSwipeDirection restrictLineSwipeDirection, float positionDiff, float lengthDiffPercent)
	{
		
		if (lineSwipes == null || lineSwipes.Length	== 0) {
			Debug.LogError("LineFactory:FindLineType *** ERROR *** LineFactory not initialized");
			return LineType.None;
		}
		
		LineType lineType = doFindLineType(possibleTypes, swipeList, LineGesture.LineIdentification.Precise, restrictLineSwipeDirection, positionDiff, lengthDiffPercent);
		if (lineType != LineType.None) {
			return lineType;
		}
		
		if (lineIdentification == LineGesture.LineIdentification.Clean || lineIdentification == LineGesture.LineIdentification.Sloppy) {
			lineType = doFindLineType(possibleTypes, swipeList, LineGesture.LineIdentification.Clean, restrictLineSwipeDirection, positionDiff, lengthDiffPercent);
			if (lineType != LineType.None) {
				return lineType;
			}
		}
		
		if (lineIdentification == LineGesture.LineIdentification.Sloppy) {
			lineType = doFindLineType(possibleTypes, swipeList, LineGesture.LineIdentification.Sloppy, restrictLineSwipeDirection, positionDiff, lengthDiffPercent);
			if (lineType != LineType.None) {
				return lineType;
			}
		}
	
		//Debug.Log("LineFactory:FindLineType none found");		
		return LineType.None;
	}
	
	private LineType doFindLineType(LineType[] possibleTypes, List<SwipeSegmentList> swipeList, LineGesture.LineIdentification lineIdentification, LineGesture.LineSwipeDirection restrictLineSwipeDirection, float positionDiff, float lengthDiffPercent)
	{
		
		for (int i = 0; i < possibleTypes.Length; i++) {
			if (lineSwipes[(int)possibleTypes[i]] == null) {
				Debug.LogError("LineFactory:FindLineType *** ERROR *** LineFactory item " + possibleTypes[i] + " is not defined.");
				return LineType.None;
			}
			if (lineSwipes[(int)possibleTypes[i]].Compare(swipeList, restrictLineSwipeDirection, positionDiff, lengthDiffPercent, lineIdentification)) {
				//Debug.Log("LineFactory:doFindLineType found " + possibleTypes[i]);
				return possibleTypes[i];
			}
		}
		return LineType.None;
	}
}

