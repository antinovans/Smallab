using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField]
    private GameObject lotusParent;
    [SerializeField]
    private GameObject[] leaf;
    [SerializeField]
    private GameObject[] lotus;

    [Header("show up setting")]
    public float spawnHeight;
    public Vector3 offset;
    public float rotationDegree;
    public float showUpDuration;
    /*private Vector3 LEAF_Y = new Vector3(0, 0.1f, 0);*/
    // Start is called before the first frame update
    void Start()
    {
        offset = new Vector3(0, spawnHeight, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject SpawnLeaf(Vector3 pos)
    {
        GameObject newLeaf = Instantiate(leaf[Random.Range(0, leaf.Length)]);
        newLeaf.transform.position = pos + offset;
        newLeaf.transform.SetParent(lotusParent.transform);
        StartCoroutine(Rotate(newLeaf, showUpDuration, rotationDegree));
        /*StartCoroutine(Translate(newLeaf, showUpDuration, pos + LEAF_Y));*/
        return newLeaf;
    }

    public GameObject SpawnLotus(Vector3 pos)
    {
        GameObject newLotus = Instantiate(lotus[Random.Range(0, lotus.Length)]);
        newLotus.transform.position = pos + offset;
        newLotus.transform.SetParent(lotusParent.transform);
        StartCoroutine(Rotate(newLotus, showUpDuration, rotationDegree));
        /*StartCoroutine(Translate(newLotus, showUpDuration, pos + LEAF_Y));*/
        return newLotus;
    }

    IEnumerator Rotate(GameObject obj, float duration, float rotationAngle)
    {
        float startRotation = obj.transform.eulerAngles.y;
        float endRotation = startRotation + rotationAngle;
        float t = 0.0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float yRotation = Mathf.Lerp(startRotation, endRotation, t / duration) % rotationAngle;
            obj.transform.eulerAngles = new Vector3(transform.eulerAngles.x, yRotation, transform.eulerAngles.z);
            yield return null;
        }
    }
}
