using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurvedMovement : MonoBehaviour
{
    public AnimationCurve speedCurve;
    public float maxSpeed;
    public float interval;

    private Vector3 _nextPos;
    private float _timer;
    private float _speed;
    private Vector3 _movingDir;
    void Start()
    {
        InitializePosition();
        interval = 3.0f;
        maxSpeed = 1.0f;
        _timer = interval;
    }

    private void Update()
    {
        _timer += Time.deltaTime;
        if(_timer > interval)
        {
            FindNextPos();
            _movingDir = (_nextPos - transform.position).normalized;
            _timer = 0f;
        }
        _speed = speedCurve.Evaluate(_timer)* maxSpeed;
        gameObject.GetComponent<Rigidbody>().velocity = _speed * _movingDir;
    }

    private void InitializePosition()
    {
        transform.position = new Vector3(transform.position.x, NPCGenerator.playerY, transform.position.y);
    }
    protected void FindNextPos()
    {
        _nextPos = new Vector3(UnityEngine.Random.Range(NPCGenerator.minX, NPCGenerator.maxX)
                , transform.position.y, UnityEngine.Random.Range(NPCGenerator.minX, NPCGenerator.maxX));
    }
}
