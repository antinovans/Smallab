using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BasicMovement : MonoBehaviour
{
    public float interval;
    public float speed;
    public Transform targetPos;
    private Vector3 nextPos;
    // Start is called before the first frame update
    void Start()
    {
        InitializePosition();
        interval = 2.0f;
        StartCoroutine(MoveToNextPos());
    }

    private void InitializePosition()
    {
        transform.position = new Vector3(transform.position.x, NPCGenerator.playerY, transform.position.z);
    }


    private IEnumerator MoveToNextPos()
    {
        while (true)
        {
            FindNextPos();
            Move();
            yield return new WaitForSeconds(UnityEngine.Random.Range(interval - 0.5f, interval + 0.5f));
        }
    }
    private void FindNextPos()
    {
        if (targetPos != null)
        {
            nextPos = targetPos.position;
            return;
        }
        nextPos = new Vector3(UnityEngine.Random.Range(NPCGenerator.minX, NPCGenerator.maxX)
                , transform.position.y, UnityEngine.Random.Range(NPCGenerator.minX, NPCGenerator.maxX));
    }
    private void Move()
    {
        if (TestingController.begin)
        {
            this.GetComponent<Rigidbody>().AddForce((nextPos - transform.position).normalized * gameObject.GetComponent<Rigidbody>().mass * speed, ForceMode.Impulse);
        }
    }
}
