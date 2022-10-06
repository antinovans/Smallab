using UnityEngine;
//using System.Collections;
using System;

public class TrackedObject {

    /*
     * VERSION 3.0
     *
     */

	public int id = -1; // make sure it has a default value that is bogus so we can check for it
	public Vector3 position;
	public Quaternion rotation;
    public float meanMarkerError;
	
	
	public void setID(int value){
		id = value;
	}
}
