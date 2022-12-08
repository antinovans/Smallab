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
        interval += UnityEngine.Random.Range(-interval / 4, interval / 4);
        InitializePosition();
        _timer = 0;
    }

    private void Update()
    {
        /*if (TestingController.begin)
        {
            Move();
        }*/
    }

    /*    private void Move()
        {
            _timer += Time.deltaTime;
            if (_timer > interval)
            {
                FindNextPos();
                _movingDir = (_nextPos - transform.position).normalized;
                _timer = 0f;
            }
            _speed = speedCurve.Evaluate(_timer) * maxSpeed;
            gameObject.GetComponent<Rigidbody>().velocity = _speed * _movingDir;
        }*/
    private void Move()
    {
        _timer += Time.deltaTime;
        if (_timer > interval)
        {
            FindNextPos();
            _movingDir = (_nextPos - transform.position).normalized;
            _timer = 0f;
        }
        _speed = speedCurve.Evaluate(_timer) * maxSpeed;
        gameObject.GetComponent<Rigidbody>().AddForce(_speed * _movingDir, ForceMode.Acceleration);
    }

    private void InitializePosition()
    {
        /*transform.position = new Vector3(transform.position.x, NPCGenerator.playerY, transform.position.z);*/
    }
    protected void FindNextPos()
    {
        _nextPos = new Vector3(UnityEngine.Random.Range(NPCGenerator.minX, NPCGenerator.maxX)
                , transform.position.y, UnityEngine.Random.Range(NPCGenerator.minX, NPCGenerator.maxX));
    }
    public void SetInterval(float input)
    {
        this.interval = input;
    }

    public float GetInterval()
    {
        return interval;
    }
    public void SetSpeed(float input)
    {
        this.maxSpeed = input;
    }

    public float GetMaxSpeed()
    {
        return maxSpeed;
    }
}
