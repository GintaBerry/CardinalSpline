using UnityEngine;
using System.Collections;

public class carMove : MonoBehaviour {

    int splinePointNum = 0;
    Vector3[] spline;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    
    
    public void Move(Vector3[] splineVectors)
    {
        spline = splineVectors;
        if(splinePointNum < splineVectors.Length)
        {
            this.transform.position = Camera.main.ScreenToWorldPoint(splineVectors[splinePointNum]);
            splinePointNum++;
            Invoke("Move", 0.1f);
        }
    }

    public void Move()
    {
        Move(spline);
    }
}
