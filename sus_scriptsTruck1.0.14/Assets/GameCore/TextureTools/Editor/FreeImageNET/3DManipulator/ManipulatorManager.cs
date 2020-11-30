using UnityEngine;
using System.Collections;

public class ManipulatorManager : MonoBehaviour {
	
	private WebCamTexture webcamTexture;
	private KDManipulator theManipulator;
	
	private float prevX;
	private float prevY;
	private float prevZ;
	
	// Use this for initialization
	void Start () {
	
		webcamTexture = new WebCamTexture();        
		
	    webcamTexture.requestedFPS = 30;
		webcamTexture.requestedWidth = 320;
		webcamTexture.requestedHeight = 240;
		
        webcamTexture.Play();
		
		theManipulator = new KDManipulator(webcamTexture.width, webcamTexture.height );		
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
	
		theManipulator.ClearData();		
		theManipulator.ProcessImage( ref webcamTexture );
		
		// Now get coordinates
		float x = ( theManipulator.GetMinX(theManipulator.BiggestAreaID) + theManipulator.GetMaxX(theManipulator.BiggestAreaID)) * 0.0015625f; //Normalized coeff
        float y = 1f - (( theManipulator.GetMinY(theManipulator.BiggestAreaID) + theManipulator.GetMaxY(theManipulator.BiggestAreaID)) * 0.00208333333333333333333333333333f); //Normalized coeff
        float z = theManipulator.GetArea(theManipulator.BiggestAreaID) * 1.3020833333333333333333333333333e-5f; // Normalize on 320x240 area
		
		x = ( x + prevX ) / 2f;
		y = ( y + prevY ) / 2f;
		z = ( z + prevZ ) / 2f;
		
		transform.position = new Vector3(x,y,z);		
		
		prevX = x;
		prevY = y;
		prevZ = z;
		
	}
}
