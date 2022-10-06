using UnityEngine;
using System.Collections;

public class LineSegmentConnector : MonoBehaviour {
	
	public Transform point1, point2;
	float distance;	
	
	// Use this for initialization
	void Start () {
		

	}
	

	
	// Update is called once per frame
	void Update () {
		
		distance = Vector3.Distance(point2.position, point1.position); // determine how far apart they are
		
		transform.position = point1.position; // start it at position 1
		
		transform.position = point1.position + ((point2.position - point1.position) * 0.5f); // move the line length
		
		transform.localScale = new Vector3(transform.localScale.x, distance * 0.5f, transform.localScale.z); // scale the line length
		
		transform.up = point2.position - point1.position; // rotate the line
	
	}
}
