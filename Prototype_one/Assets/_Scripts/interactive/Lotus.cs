using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lotus : MonoBehaviour
{
    public static int num = 0;
    [Header("player setting")]
    public GameObject player1;
    public GameObject player2;
    public GameObject player3;

    private float distance1;
    private float distance2;
    private float distance3;
    private bool interacted;
    private bool isRotating;
    // Start is called before the first frame update
    void Start()
    {
        num++;
        interacted = false;
        isRotating = false;
        player1 = GameObject.FindGameObjectWithTag("Player1");
        player2 = GameObject.FindGameObjectWithTag("Player2");
        player3 = GameObject.FindGameObjectWithTag("Player3");
        float lifetime = UnityEngine.Random.Range(10f, 15f);
        Destroy(gameObject, lifetime);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateDistance();

        if (distance1 < 0.1f || distance2 < 0.1f || distance3 < 0.1f)
        {
            if (interacted)
            {
                FadeOut();
            }
            if (!isRotating)
            {
                isRotating = true;

                int i = UnityEngine.Random.Range(1, 5);
                string soundName = "lotus" + i;
                FindObjectOfType<SoundManager>().PlaySound(soundName, false);

                StartCoroutine(Rotate(2f, 360f));
            }
        }
    }

    private void FadeOut()
    {

        Destroy(gameObject);
    }

    void UpdateDistance()
    {
        Vector3 position = new Vector3(transform.position.x, 0, transform.position.z);
        distance1 = Mathf.Abs(Vector3.Distance(player1.transform.position, position));
        distance2 = Mathf.Abs(Vector3.Distance(player2.transform.position, position));
        distance3 = Mathf.Abs(Vector3.Distance(player3.transform.position, position));
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
        interacted = true;
    }

    private void OnDestroy()
    {
        num--;
    }
}
