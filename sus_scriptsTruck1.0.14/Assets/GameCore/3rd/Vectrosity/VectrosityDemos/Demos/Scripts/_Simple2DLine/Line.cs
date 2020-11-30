// Use this method if you need more control than you get with Vector.SetLine
using UnityEngine;
using System.Collections.Generic;
using Vectrosity;

public class Line : MonoBehaviour {
    public Canvas canvas;
    public RectTransform lineTrans;

    public int segments = 50;
    void Start () {
        VectorLine.canvas = canvas;
        // Make a Vector2 list; in this case we just use 2 elements...
        var linePoints = new List<Vector2>();
        //linePoints.Add (new Vector2(0, Random.Range(0, Screen.height)));				// ...one on the left side of the screen somewhere
        //linePoints.Add (new Vector2(Screen.width-1, Random.Range(0, Screen.height)));	// ...and one on the right
        linePoints.Add(new Vector2(Screen.width * 0.5f, Screen.height * 0.5f));
        linePoints.Add(new Vector2(Screen.width * 0.5f, Screen.height * 0.5f + 100));
        linePoints.Add(new Vector2(Screen.width * 0.5f - 500, Screen.height * 0.5f));

        // Make a VectorLine object using the above points, with a width of 2 pixels
#if true
        var line = new VectorLine("LineTest", lineTrans, linePoints, 2.0f, LineType.Continuous);
		
#else
        // Make Vector2 list where the size is the number of segments plus one, since it's for a continuous line
        // (A discrete line would need the size to be segments*2)
        var _tempPoints = new List<Vector2>(segments + 1);

        // Make a VectorLine object using the above points and the default material,
        // with a width of 2 pixels, an end cap of 0 pixels, and depth 0
        var line = new VectorLine("Curve", linePoints, 2.0f, LineType.Continuous, Joins.Weld);
        // Create a curve in the VectorLine object using the curvePoints array as defined in the inspector
        line.MakeCurve(linePoints.ToArray(), segments);
#endif
        // Draw the line
        line.Draw();
    }
}