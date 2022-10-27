using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lotus : MonoBehaviour
{
    [Header("player setting")]
    public GameObject player1;
    public GameObject player2;
    public GameObject player3;

    private float distance1;
    private float distance2;
    private float distance3;
    // Start is called before the first frame update
    void Start()
    {
        player1 = GameObject.FindGameObjectWithTag("Player1");
        player2 = GameObject.FindGameObjectWithTag("Player2");
        player3 = GameObject.FindGameObjectWithTag("Player3");
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 position = new Vector3(transform.position.x, 0, transform.position.z);
        distance1 = Mathf.Abs(Vector3.Distance(player1.transform.position, position));
        distance2 = Mathf.Abs(Vector3.Distance(player2.transform.position, position));
        distance3 = Mathf.Abs(Vector3.Distance(player3.transform.position, position));

        if(distance1 < 0.1f || distance2 < 0.1f || distance3 < 0.1f)
        {
            StartCoroutine(Rotate(2f, 360f));
        }
    }

    IEnumerator Rotate(float duration, float rotationAngle)
    {
        float startRotation = transform.eulerAngles.y;
        float endRotation = startRotation + rotationAngle;
        float t = 0.0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float yRotation = Mathf.Lerp(startRotation, endRotation, t / duration) % rotationAngle;
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, yRotation, transform.eulerAngles.z);
            yield return null;
        }
    }
}
