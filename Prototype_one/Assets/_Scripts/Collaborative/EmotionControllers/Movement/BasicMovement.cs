using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

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
        transform.position = new Vector3(transform.position.x, 0.0f, transform.position.z);
    }


    private IEnumerator MoveToNextPos()
    {
        float timer = 0.0f;
        while (timer < 1.5f)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        while (true)
        {
            Move();
            yield return new WaitForSeconds(UnityEngine.Random.Range(interval - 0.3f, interval + 0.3f));
        }
    }
    private void Move()
    {
        if (targetPos != null)
        {
            nextPos = targetPos.position;
            this.GetComponent<Rigidbody>().AddForce((nextPos - transform.position).normalized * gameObject.GetComponent<Rigidbody>().mass * speed, ForceMode.Impulse);
            return;
        }
        else
        {
            Vector2 dir = Random.insideUnitCircle.normalized;
            nextPos = new Vector3(dir.x, 0, dir.y);
            this.GetComponent<Rigidbody>().AddForce(nextPos * gameObject.GetComponent<Rigidbody>().mass * speed, ForceMode.Impulse);
            return;
        }
    }
}
