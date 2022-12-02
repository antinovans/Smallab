
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCGenerator : MonoBehaviour
{
    public GameObject bottomLeft;
    public GameObject topRight;
    public GameObject prefabs;
    public int maxNum;
    public float interval;

    public static float minX;
    public static float maxX;
    public static float minZ;
    public static float maxZ;

    private int curNum;
    private bool isStart = false;
    // Start is called before the first frame update
    void Start()
    {
        InitializeBoudaries();
        curNum = 0;
/*        StartCoroutine(InstantiatePrefabs());*/
    }
    private void Update()
    {
        if (TestingController.begin && !isStart)
        {
            StartCoroutine(InstantiatePrefabs());
            isStart = true;
        }
    }

    private void InitializeBoudaries()
    {
        minX = bottomLeft.transform.position.x;
        maxX = topRight.transform.position.x;
        minZ = bottomLeft.transform.position.z;
        maxZ = topRight.transform.position.z;
    }
    IEnumerator InstantiatePrefabs()
    {
        while (curNum <= maxNum)
        {
            var obj = Instantiate(prefabs, transform.position,
                Quaternion.identity);
            if (obj.CompareTag("Anger"))
                obj.GetComponent<AngerController>().SetSize(UnityEngine.Random.Range(2, 4));
            if (obj.CompareTag("Sadness"))
                obj.GetComponent<SadController>().SetSize(UnityEngine.Random.Range(2, 4));
            curNum++;
            yield return new WaitForSeconds(UnityEngine.Random.Range(interval - 1, interval + 1));
        }
    }
}
