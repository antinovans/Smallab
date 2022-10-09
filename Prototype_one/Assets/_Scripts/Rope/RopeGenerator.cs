using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeGenerator : MonoBehaviour
{
    public GameObject componentPrefab;
    public float linkDis;
    public float maxDis;
    public GameObject end;
    public int numOfComponents;

    private GameObject[] _components;
    private float curDist;
    private bool isConnected;
    private bool shouldUpdatePos;

    // Start is called before the first frame update
    void Start()
    {
        InitializeComponents();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateLine();
    }

    private void UpdateLine()
    {
        curDist = (gameObject.transform.position - end.transform.position).magnitude;
        if (curDist > maxDis)
        {
            isConnected = false;
        }

        if (curDist <= linkDis)
        {
            isConnected = true;
        }

        if (isConnected == false)
        {
            DisableComponent();
            return;
        }
        if (isConnected == true && shouldUpdatePos == true)
        {
            EnableComponent();
        }
    }
    private void EnableComponent()
    {
        GameObject prevObject = gameObject;
        for (int i = 0; i < numOfComponents; i++)
        {
            _components[i].SetActive(true);
            _components[i].transform.position = gameObject.transform.position + (i + 1) * (end.transform.position - gameObject.transform.position) / (numOfComponents + 1);
            AddConfigurableJoint(prevObject.GetComponent<Rigidbody>(), _components[i]);
            prevObject = _components[i];
        }
        AddConfigurableJoint(end.GetComponent<Rigidbody>(), prevObject);
        /*InitializeComponents();*/
        shouldUpdatePos = false;
    }

    private void DisableComponent()
    {
        foreach (var obj in _components)
        {
            Destroy(obj.GetComponent<ConfigurableJoint>());
            obj.SetActive(false);
        }
        shouldUpdatePos = true;
    }

    private void InitializeComponents()
    {
        float scale =  linkDis / numOfComponents;
        _components = new GameObject[numOfComponents];
        GameObject prevObject = gameObject;
        for(int i = 0; i < numOfComponents; i++)
        {
            _components[i] = Instantiate(componentPrefab);
            _components[i].transform.parent = gameObject.transform;
            _components[i].transform.localScale *= scale;
            _components[i].transform.position = gameObject.transform.position + (i + 1) * (end.transform.position - gameObject.transform.position) / (numOfComponents + 1);
            AddConfigurableJoint(prevObject.GetComponent<Rigidbody>(), _components[i]);
            prevObject = _components[i];
        }
        AddConfigurableJoint(end.GetComponent<Rigidbody>(), prevObject);

    }

    private void AddConfigurableJoint(Rigidbody prevRb, GameObject cur)
    {
        ConfigurableJoint cj =  cur.AddComponent<ConfigurableJoint>();
        cj.connectedBody = prevRb;
        JointDrive drive = new JointDrive();
        drive.mode = JointDriveMode.Position;
        drive.positionSpring = 100f;
        drive.positionDamper = 1f;
        drive.maximumForce = 1000f;

        cj.enableCollision = false;
         
        cj.xDrive = drive;
        cj.yDrive = drive;
        cj.zDrive = drive;

        cj.angularXMotion = ConfigurableJointMotion.Limited;
        cj.angularYMotion = ConfigurableJointMotion.Limited;
        cj.angularZMotion = ConfigurableJointMotion.Limited;
    }

}
