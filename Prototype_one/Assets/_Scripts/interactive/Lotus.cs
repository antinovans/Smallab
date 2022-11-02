using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Data
{
    public static SortedSet<Vector2> positions = new SortedSet<Vector2>(new Vector2Comparer());
    public static void AddPosition(Vector2 pos)
    {
        positions.Add(pos);
    }
    public static void removePosition(Vector2 pos)
    {
        positions.Remove(pos);
    }
}
public class Vector2Comparer : IComparer<Vector2>
{
    public int Compare(Vector2 v1, Vector2 v2)
    {
        float sum1 = v1.x + v1.y;
        float sum2 = v2.x + v2.y;
        if (sum1 < sum2)
            return -1;
        else if (sum1 > sum2)
            return 1;
        else
            return 0;
    }
}
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

    private Vector2 initPos;
    private List<Material> mats;
    private Color fadeOutColor = new Color(0.0f, 0.0f, 0.0f, 0.0f);
    // Start is called before the first frame update
    void Start()
    {
        initPos = new Vector2(transform.position.x, transform.position.z);
        Data.AddPosition(initPos);
        num++;
        interacted = false;
        isRotating = false;
        player1 = GameObject.FindGameObjectWithTag("Player1");
        player2 = GameObject.FindGameObjectWithTag("Player2");
        player3 = GameObject.FindGameObjectWithTag("Player3");
        float lifetime = UnityEngine.Random.Range(15f, 30f);
        var renderers = GetComponentsInChildren<MeshRenderer>();
        mats = new List<Material>();
        foreach (var r in renderers)
        {
            if(r.material == null)
            {
                Debug.Log("unable to find mat");
                continue;
            }
            mats.Add(r.material);
        }
        StartCoroutine(FadeOut(lifetime));
        /* Destroy(gameObject, lifetime);*/
    }

    // Update is called once per frame
    void Update()
    {
        UpdateDistance();

        if (distance1 < 0.2f || distance2 < 0.2f || distance3 < 0.2f)
        {
            if (interacted)
            {
                StartCoroutine(FadeOut(0));
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

    IEnumerator FadeOut(float timer)
    {
        yield return new WaitForSeconds(timer);
        foreach (var m in mats)
        {
            StartCoroutine(alphaFades(m));
        }
        /*Destroy(gameObject);*/
    }

    IEnumerator alphaFades(Material m)
    {
        Color c = m.color;
        float fadeTime = 2.0f;
        float timer = 0.0f;
        while(timer <= fadeTime)
        {
            m.color = Color.Lerp(c, fadeOutColor, timer / fadeTime);
            timer += Time.deltaTime;
            yield return null;
        }
        if (gameObject != null)
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
        Data.removePosition(initPos);
    }
}
