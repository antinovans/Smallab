using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineConnector : MonoBehaviour
{
    public GameObject[] _objects;

    private LineRenderer _lr;
    // Start is called before the first frame update
    void Start()
    {
        _lr = gameObject.GetComponent<LineRenderer>();
        _lr.positionCount = _objects.Length;
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < _objects.Length; i++)
        {
            _lr.SetPosition(i, _objects[i].transform.position);
        }
    }
}
