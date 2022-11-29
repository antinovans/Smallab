using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierMovement : MonoBehaviour
{
    public Transform targetTransform;
    public float randRange;
    public float curveRatio;
    public float minSpeed;
    public float maxSpeed;
    public AnimationCurve speedCurve;
    //private fields
    private float speed;
    private Vector2 startPos;
    private Vector2 midPos;
    private Vector2 endPos;
    private float percent;
    private float percentSpeed;
    private bool isUnlock;
    private Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        isUnlock = false;
        rb = GetComponent<Rigidbody>();
        rb.AddForce(2 * rb.mass * new Vector3(Random.Range(-1, 1), 0, Random.Range(-1, 1)).normalized,
            ForceMode.Impulse);
        StartCoroutine(Unlock());
    }

    // Update is called once per frame
    void Update()
    {
        if (isUnlock)
        {
            endPos = new Vector2(targetTransform.position.x, targetTransform.position.z);
            speed = maxSpeed * speedCurve.Evaluate(percent) + minSpeed * (1 - speedCurve.Evaluate(percent));
            percentSpeed = speed / (endPos - startPos).magnitude;
            percent += percentSpeed * Time.deltaTime;
            if (percent > 1)
                percent = 1;
            var tempPos = Bezier(percent, startPos, midPos, endPos);
            transform.position = new Vector3(tempPos.x, 0, tempPos.y);
        }
    }

    void Init()
    {
        isUnlock = true;
        endPos = new Vector2(targetTransform.position.x, targetTransform.position.z);
        percent = 0;
        startPos = new Vector2(transform.position.x, transform.position.z);
        midPos = GetMidPos(startPos, endPos);
    }
    private Vector2 GetMidPos(Vector2 a, Vector2 b)
    {
        Vector2 m = Vector2.Lerp(a, b, 0.1f);
        Vector2 normal = Vector2.Perpendicular(a - b).normalized;
        float rd = Random.Range(-randRange, randRange);
        return m + (a - b).magnitude * curveRatio * rd * normal;
    }

    private Vector2 Bezier(float t, Vector2 a, Vector2 b, Vector2 c)
    {
        var ab = Vector2.Lerp(a, b, t);
        var bc = Vector2.Lerp(b, c, t);
        return Vector2.Lerp(ab, bc, t);
    }

    IEnumerator Unlock()
    {
        float timer = 0.0f;
        while(timer < 0.5f)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        Init();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Depression"))
            Destroy(gameObject);
    }
    public void SetTarget(GameObject target)
    {
        this.targetTransform = target.transform;
    }
}
