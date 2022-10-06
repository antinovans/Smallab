using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorVisibilityController : MonoBehaviour {

    float inactiveMouseTime = 0.0f;
    float secondsToWaitBeforeHidingCursor = 3.0f;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (DetectedMouseMovement())
        {
            inactiveMouseTime = 0;
            Cursor.visible = true;

        }
        else
        {

            inactiveMouseTime += Time.deltaTime;
            if (inactiveMouseTime >= secondsToWaitBeforeHidingCursor)
            {
                //not increment
                inactiveMouseTime = secondsToWaitBeforeHidingCursor;
                Cursor.visible = false;
            }
        }
    }


    bool DetectedMouseMovement()
    {
        
        if (Application.isMobilePlatform)
        {
            if (Input.touches.Length >= 1)
                return true;
            else
                return false;
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
                return true;
            return (Input.GetAxis("Mouse X") != 0) || (Input.GetAxis("Mouse Y") != 0);
        }
        

    }
}
