using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GLTest : MonoBehaviour {

    public Material material;
    private List<Vector3> lineInfo;

    // Use this for initialization
    void Start () {
        lineInfo = new List<Vector3>();
	}
	
	// Update is called once per frame
	void Update () {
        if(Input.GetMouseButtonDown(0))
        lineInfo.Add(Input.mousePosition);
	}

    void OnGUI()
    {
        GUILayout.Label("Current mouse X position: " + Input.mousePosition.x);
        GUILayout.Label("Current mouse Y position: " + Input.mousePosition.y);
    }

    void OnPostRender()
    {
        Debug.Log("I am here");
        if(!material)
        {
            Debug.LogError("Need Material");
            return;
        }
        material.SetPass(0);
        GL.LoadOrtho();
        GL.Begin(GL.LINES);
        int size = lineInfo.Count;
        for (int i = 0; i < size - 1; i++)
        {
            Vector3 start = lineInfo[i];
            Vector3 end = lineInfo[i+1];
            DrawLine(start.x, start.y, end.x, end.y);
        }
        GL.End();
    }

    void DrawLine(float x1, float y1, float x2, float y2)
    {
        GL.Vertex(new Vector3(x1 / Screen.width, y1 / Screen.height, 0));
        GL.Vertex(new Vector3(x2 / Screen.width, y2 / Screen.height, 0));
    }
}