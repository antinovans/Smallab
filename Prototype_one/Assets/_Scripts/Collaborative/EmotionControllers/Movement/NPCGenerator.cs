
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCGenerator : MonoBehaviour
{
    public GameObject bottomLeft;
    public GameObject topRight;
    public GameObject[] prefabs;
    public int maxNum;
    public float interval;

    public static float minX;
    public static float maxX;
    public static float minZ;
    public static float maxZ;
    public static float playerY;

    private int curNum;
    // Start is called before the first frame update
    void Start()
    {
        InitializeBoudaries();
        curNum = 5;
        StartCoroutine(InstantiatePrefabs());
    }

    private void InitializeBoudaries()
    {
        playerY = GameObject.FindGameObjectWithTag("Player").transform.position.y;
        minX = bottomLeft.transform.position.x;
        maxX = topRight.transform.position.x;
        minZ = bottomLeft.transform.position.z;
        maxZ = topRight.transform.position.z;
    }
    IEnumerator InstantiatePrefabs()
    {
        while (curNum <= maxNum)
        {
            var obj = Instantiate(prefabs[UnityEngine.Random.Range(0, prefabs.Length)]);
            obj.transform.position =  new Vector3(UnityEngine.Random.Range(minX, maxX)
                , transform.position.y, UnityEngine.Random.Range(minX, maxX));
            curNum++;
            yield return new WaitForSeconds(UnityEngine.Random.Range(interval - 1, interval + 1));
        }
    }
}
