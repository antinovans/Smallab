
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class NPCGenerator : MonoBehaviour
{
    public GameObject bottomLeft;
    public GameObject topRight;
    public GameObject Joy;
    public GameObject Sadness;
    public GameObject Anger;

    public static float minX;
    public static float maxX;
    public static float minZ;
    public static float maxZ;
    public static float midX;

    private int curNum;
    // Start is called before the first frame update
    void Start()
    {
        InitializeBoudaries();
        curNum = 0;
/*        StartCoroutine(InstantiatePrefabs());*/
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            Vector3 targetPos = new Vector3(Random.Range(midX, maxX), 0, Random.Range(minZ, maxZ));
            StartCoroutine(GenerateEmotions(Joy, targetPos, true));
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Vector3 targetPos = new Vector3(Random.Range(minX, maxX), 0, Random.Range(minZ, maxZ));
            StartCoroutine(GenerateEmotions(Anger, targetPos, false));
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Vector3 targetPos = new Vector3(Random.Range(minX, maxX), 0, Random.Range(minZ, maxZ));
            StartCoroutine(GenerateEmotions(Sadness, targetPos, false));
        }

    }

    private void InitializeBoudaries()
    {
        minX = bottomLeft.transform.position.x;
        maxX = topRight.transform.position.x;
        minZ = bottomLeft.transform.position.z;
        maxZ = topRight.transform.position.z;
        midX = minX + (maxX - minX) / 2;
    }

    IEnumerator GenerateEmotions(GameObject obj, Vector3 position, bool shouldSkip)
    {
        if (shouldSkip)
        {
            InstantiatePrefab(obj, position);
            yield return null;
        }
        else
        {
            //instantiate particles
            yield return new WaitForSeconds(2.0f);
            //destroy particles
            InstantiatePrefab(obj, position);
        }
    }
    void InstantiatePrefab(GameObject obj, Vector3 position)
    {
        var temp = Instantiate(obj, position, Quaternion.identity, transform);
        if (temp.TryGetComponent(out AngerController ac))
        {
            ac.SetSize(Random.Range(2, 5));
            return;
        }
        if (temp.TryGetComponent(out SadController sc))
        {
            Debug.Log("find sad");
            sc.SetSize(Random.Range(2, 4));
            return;
        }
    }
}
