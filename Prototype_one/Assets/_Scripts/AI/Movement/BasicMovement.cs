using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BasicMovement : MonoBehaviour
{
    public float interval;
    public float speed;
    protected Vector3 _nextPos;
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


    protected IEnumerator MoveToNextPos()
    {
        while (true)
        {
            FindNextPos();
            Move();
            yield return new WaitForSeconds(UnityEngine.Random.Range(interval - 1, interval + 1));
        }
    }
    protected void FindNextPos()
    {
        _nextPos = new Vector3(UnityEngine.Random.Range(NPCGenerator.minX, NPCGenerator.maxX)
                , transform.position.y, UnityEngine.Random.Range(NPCGenerator.minX, NPCGenerator.maxX));
    }
    protected void Move()
    {
        this.GetComponent<Rigidbody>().AddForce((_nextPos - transform.position).normalized * gameObject.GetComponent<Rigidbody>().mass * speed, ForceMode.Impulse);
    }
}
