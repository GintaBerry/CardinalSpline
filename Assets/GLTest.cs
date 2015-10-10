using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class GLTest : MonoBehaviour
{

    public Material material;
    private List<Vector3> lineInfo;
    Slider slider1;
    Slider slider2;
    public GameObject carSprite;
    

    float[] m; // the matrix
    Vector3[] Spline;
    bool hasSpline = false;

    // Use this for initialization
    void Start()
    {
        m = new float[16];
        //Spline = new List<Vector3>();
        lineInfo = new List<Vector3>();
        slider1 = GameObject.Find("Slider1").GetComponent<Slider>();
        slider2 = GameObject.Find("Slider2").GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !hasSpline)
        {
            Vector3 a = Input.mousePosition;
            a.z = 10;
            lineInfo.Add(a);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Calculate(lineInfo, 10, slider1.value);
        }
    }

    void OnGUI()
    {
        //GUILayout.Label("Current mouse X position: " + Input.mousePosition.x);
        //GUILayout.Label("Current mouse Y position: " + Input.mousePosition.y);
    }

    void OnPostRender()
    {
        Debug.Log("I am here");
        if (!material)
        {
            Debug.LogError("Need Material");
            return;
        }
        material.SetPass(0);
        GL.LoadOrtho();
        GL.Begin(GL.LINES);
        int size = lineInfo.Count;
        if (!hasSpline)
        {
            for (int i = 0; i < size - 1; i++)
            {
                Vector3 start = lineInfo[i];
                Vector3 end = lineInfo[i + 1];
                DrawLine(start.x, start.y, end.x, end.y);
            }
        }
        else
        {
            for(int i  = 0; i < Spline.Length -1; i++)
            {
                Vector3 start = Spline[i];
                Vector3 end = Spline[i + 1];
                DrawLine(start.x, start.y, end.x, end.y);
            }
        }
        GL.End();
    }

    void DrawLine(float x1, float y1, float x2, float y2)
    {
        GL.Vertex(new Vector3(x1 / Screen.width, y1 / Screen.height, 0));
        GL.Vertex(new Vector3(x2 / Screen.width, y2 / Screen.height, 0));
    }


    //a1 is the parameter of tension, generate the Mc matrix
    void GetCardinalMatrix(float a1)
    {
        m[0] = -a1;
        m[1] = 2 - a1;
        m[2] = a1 - 2;
        m[3] = a1;

        m[4] = 2 * a1;
        m[5] = a1 - 3;
        m[6] = 3 - 2 * a1;
        m[7] = -a1;

        m[8] = -a1;
        m[9] = 0;
        m[10] = a1;
        m[11] = 0;

        m[12] = 0;
        m[13] = 1;
        m[14] = 0;
        m[15] = 0;
    }


    //a ,b ,c , d is the four point (axis x or axis y value and get the Pu's x and y) we use to constrian the spline, alpha is the 0 to 1 parameter
    float Matrix(float a, float b, float c, float d, float alpha)
    {
        float p0, p1, p2, p3;
        p0 = m[0] * a + m[1] * b + m[2] * c + m[3] * d;
        p1 = m[4] * a + m[5] * b + m[6] * c + m[7] * d;
        p2 = m[8] * a + m[9] * b + m[10] * c + m[11] * d;
        p3 = m[12] * a + m[13] * b + m[14] * c + m[15] * d;
        return (p3 + alpha * (p2 + alpha * (p1 + alpha * p0)));
    }

    //Grain is less than 20, it is the number of nodes bwtween control nodes
    void Calculate(List<Vector3> knots, int grain, float tension)
    {
        
        Vector3 k0, k1, k2, k3 = new Vector3();
        int n = lineInfo.Count;
        Spline = new Vector3[(n - 1) * grain + n];
        Vector3[] jd = new Vector3[n + 2];
        float[] alpha = new float[50];
        jd[0] = lineInfo[0];
       
        for (int i = 1; i <= n; i++)
        {
            jd[i] = (lineInfo[i - 1]);
        }
        // jd has 2 more nodes than lineInfo
        jd[n +1] = lineInfo[n  - 1];

        GetCardinalMatrix(tension);

        for (int i = 0; i < grain; i++)
            alpha[i] = (float)i / grain;
        for (int i = 0; i < n - 1; i++)
        {
            k0 = jd[i];
            k1 = jd[i + 1];
            k2 = jd[i + 2];
            k3 = jd[i + 3];
            Spline[i * (grain + 1)] = lineInfo[i];
            for (int j = 0; j < grain; j++)
            {
                float splineX = Matrix(k0.x, k1.x, k2.x, k3.x, alpha[j]);
                float splineY = Matrix(k0.y, k1.y, k2.y, k3.y, alpha[j]);
                Spline[i * (grain + 1) + j + 1] = new Vector3(splineX, splineY, 10);
            }
        }
        Spline[(n - 1) * grain + n - 1] = lineInfo[n - 1];
        hasSpline = true;
    }

    public void Calculate()
    {
        Calculate(lineInfo, (int)slider2.value, slider1.value);
    }

    public void GenerateCar()
    {
        GameObject car = Instantiate(carSprite, Camera.main.ScreenToWorldPoint(lineInfo[0]),Quaternion.identity) as GameObject;
        car.GetComponent<carMove>().Move(Spline);
    }
    
}