using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GUILine : MonoBehaviour
{	
	public enum DrawColor {Normal, Good, Bad, Selected, Drawing};
	
	private Material lineMaterial;
	private ArrayList lineList = new ArrayList();
	private ArrayList goodLineList = new ArrayList();
	float lineWidth = 1;
	Color normalColor = Color.white;
	Color goodColor = Color.green;
	Color badColor = Color.red;
	Color lineColor = Color.white;
	Color selectedColor = Color.green;
	Color drawingColor = Color.yellow;
	private int oneOffIndex = -1;
	private Color oneOffColor;
	
	private static GUILine _factory;
	
	public static GUILine Factory()
	{
		return _factory;
	}

	public void Awake () {
		_factory = this;
	    lineMaterial = new Material( "Shader \"Lines/Colored Blended\" {" +
	        "SubShader { Pass {" +
	        "   BindChannels { Bind \"Color\",color }" +
	        "   Blend SrcAlpha OneMinusSrcAlpha" +
	        "   ZWrite Off Cull Off Fog { Mode Off }" +
	        "} } }");
	    lineMaterial.hideFlags = HideFlags.HideAndDontSave;
	    lineMaterial.shader.hideFlags = HideFlags.HideAndDontSave;

    }

	
	public void SetLineColor(DrawColor drawcolor)
	{
		lineColor = GetColor(drawcolor);
	}
	
	public Color GetColor(DrawColor drawcolor)
	{
		switch (drawcolor) {
		case DrawColor.Normal:
			return normalColor;
		case DrawColor.Good:
			return goodColor;
		case DrawColor.Bad:
			return badColor;
		case DrawColor.Selected:
			return selectedColor;
		case DrawColor.Drawing:
			return drawingColor;
		}
		
		return normalColor;
	}
	
	public void SetOneOffLineColor(int index, DrawColor drawcolor)
	{
		oneOffIndex = index;
		oneOffColor = GetColor(drawcolor);
	}
	
	
	public void AddLinePoints(List<Vector3> points)
	{
		lineList.Add(points);
	}
	
	public List<Vector3> GetLine(int listIndex)
	{
		if (listIndex >= lineList.Count) {
			return null;
		}
		return (List<Vector3>) lineList[listIndex];
	}
	
	public Vector3 GetPoint(int listIndex, int pointIndex)
	{
		if (listIndex >= lineList.Count) {
			return Vector3.zero;
		}
		List<Vector3> pointList = (List<Vector3>) lineList[listIndex];
		if (pointIndex >= pointList.Count) {
			return Vector3.zero;
		}
		return pointList[pointIndex];
	}
	
	public void PopPoints()
	{
		if (lineList.Count  > 0) {
			lineList.RemoveAt(lineList.Count - 1);
		}
	}
	
	public void ClearAllPoints()
	{
		lineList.Clear();
	}
	public void AddGoodPoints(List<Vector3> points)
	{
		goodLineList.Add(points);
	}
	
	public void ClearAllGoodPoints()
	{
		goodLineList.Clear();
	}
	
	public void OnPostRender(  ) {	        
		for (var l = 0; l < lineList.Count; l++) {
			if (l == oneOffIndex) {
				DrawLines((List<Vector3> ) lineList[l], oneOffColor);
			}
			else {
				DrawLines((List<Vector3> ) lineList[l], lineColor);
			}
		}
		for (var l = 0; l < goodLineList.Count; l++) {
			DrawLines((List<Vector3> ) goodLineList[l], goodColor);
		}
	}
	
	private void DrawLines(List<Vector3> linePoints, Color theColor)
	{
		if (linePoints == null || linePoints.Count == 0) {
			return;
		}
	    lineMaterial.SetPass(0);
		lineMaterial.SetColor("_SpecColor", theColor);
	    
	    if (lineWidth == 1) {
				
		   	GL.PushMatrix();			
	        GL.Begin(GL.LINES);
  			GL.Color(theColor);
	        for (int i = 1; i < linePoints.Count; i++) {
	  			//Debug.Log("GUILine:OnPostRender 1    " + linePoints.Count + "  " + (i - 1) + " - " + linePoints[i - 1] + " and " + i + " - " + linePoints[i]);
				GL.Vertex(linePoints[i - 1]);
	            GL.Vertex(linePoints[i]);
	        }
			GL.End();
		    GL.PopMatrix();
	    }
	    else {
	   		float thisWidth = 1.0f/((float)Screen.width) * lineWidth * .5f;
		
       		GL.Begin(GL.QUADS);
    		GL.Color(theColor);
	        for (var i = 1; i < linePoints.Count; i++) {
	  			//Debug.Log("GUILine:OnPostRender " + linePoints.Count);
	            Vector3 perpendicular = (new Vector3(linePoints[i].y, linePoints[i - 1].x, linePoints[i].z) -
	                                 new Vector3(linePoints[i - 1].y, linePoints[i].x, linePoints[i].z)).normalized * thisWidth;
	            Vector3 v1 = new Vector3(linePoints[i - 1].x, linePoints[i - 1].y, linePoints[i].z);
	            Vector3 v2 = new Vector3(linePoints[i].x, linePoints[1].y, linePoints[i].z);
	            GL.Vertex(v1 - perpendicular);
	            GL.Vertex(v1 + perpendicular);
	            GL.Vertex(v2 + perpendicular);
	            GL.Vertex(v2 - perpendicular);
	        }
			GL.End();

	    }
	}
	
}