using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public Transform LookAt;

    private Camera cam;

    private void Start()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        Vector3 pos = cam.WorldToScreenPoint(LookAt.position);

        if (transform.position != pos)
            transform.position = pos;
    }
}
