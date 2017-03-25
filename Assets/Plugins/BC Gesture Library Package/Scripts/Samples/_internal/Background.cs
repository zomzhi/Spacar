using UnityEngine;
using System.Collections;

public class Background : MonoBehaviour
{
	public Transform background;
	public Camera cam;
	
	public int top;
	public int left;
	
	public Material blob;
	
	public Color[] colors;
	public Vector3 topLeft;
	public Vector3 bottomRight;
	
	public Color theColor;
	
	public Vector3[] corners;
	public float height;
	public float width;
	
	private static Background _factory;
	public static Background Factory()
	{
		return _factory;
	}
	
	void Awake()
	{
		_factory = this;
		
		if (Application.loadedLevel > 0) {
			theColor = colors[Application.loadedLevel - 1];
		}
		else {
			theColor = colors[0];
		}
		
		SetBackground(background, theColor, 0f);
		
		SetParticles(theColor);
	}
	
	public void SetParticles(Color aColor)
	{
		//set the color of the particles to match
		Color pColor = aColor;
		if (pColor.g < 0.05f && pColor.r < 0.05f &&  pColor.b < 0.05f) {
			pColor = new Color(0.05f, 0.05f, 0.05f);
		}
		pColor.a = 0.25f;
		blob.SetColor("_TintColor", pColor);
	}
	
	public void SetBackground(Transform surface, float extraZ)
	{
		SetBackground(surface, theColor, extraZ);
	}
	
	public void SetBackground(Transform surface, Color color, float extraZ)
	{
		
		//Reset the background transfrom
		surface.localScale = new Vector3(1, 1, 1);
		surface.rotation = Quaternion.identity;
		surface.position = Vector3.zero;
		
		
		//set the color for the scene
		surface.GetComponent<Renderer>().material.color = color;
		
		//get the mesh information
		Mesh mesh = surface.GetComponent<MeshFilter>().mesh;
       	Vector3[] normals = mesh.normals;
        Vector3[] vertices = mesh.vertices;
		
		//find the inside edge of the FarClipPlane
		float depth = cam.farClipPlane - 0.3f + extraZ;
		
		//Set the corners of the plane.
		corners = new Vector3[4];
		corners[0] = cam.ScreenToWorldPoint(new Vector3(left, 0, depth));
		vertices[0] = corners[0];
		
		corners[1] = cam.ScreenToWorldPoint(new Vector3(left, Screen.height - top, depth));
		vertices[1] = corners[1];
		
		corners[2] = cam.ScreenToWorldPoint(new Vector3(Screen.width, 0, depth));
		vertices[2] = corners[2];
		
		corners[3] = cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height - top, depth));
		vertices[3] = corners[3];
		
		width = corners[2].x - corners[0].x;
		height = corners[1].y - corners[0].y;
		//Debug.Log("Background:SetBackground extraZ=" + extraZ + ", color=" + color + ", depth " + depth + " " + width + " X " + height );
		
		//invert normals
        for (int i = 0; i < normals.Length; i++) {
			normals[i] = -normals[i];
		}
		//apply changes
       	mesh.normals = normals;
       	mesh.vertices = vertices;

        mesh.RecalculateBounds();
	}
}
