using UnityEngine;
using System.Collections;

public class sliderDrag : MonoBehaviour {

    GLTest gl;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnMouseDrag()
    {
        Debug.Log("I am in UI");
        gl.Calculate();
    }
}
