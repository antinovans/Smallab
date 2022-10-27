using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    [Header("light setting")]
    public Light sceneLight;
    public float startIntensity;
    public float endIntensity;

    [Header("timer setting")]
    public float fadeInTimer;
    public float lotusSpawnTimer;

    [Header("player setting")]
    public GameObject player1;
    public GameObject player2;
    public GameObject player3;

    private int maxLotusNum;
    private int lotusNum;
    private Vector3 player1Pos;
    private Vector3 player2Pos;
    private Vector3 player3Pos;

    private Spawner spawner;
    private bool isStart;

    private void Awake()
    {
        spawner = GetComponent<Spawner>();
    }
    // Start is called before the first frame update
    void Start()
    {
        maxLotusNum = 10;
        lotusNum = 0;
        isStart = false;
        StartCoroutine(LightUp());
    }

    // Update is called once per frame
    void Update()
    {
        if(isStart)
        {
            StartCoroutine(InstantiateLotus());
        }
    }

    IEnumerator LightUp()
    {
        float elapsedTime = 0.0f;
        while(elapsedTime < fadeInTimer)
        {
            sceneLight.intensity = Mathf.Lerp(startIntensity, endIntensity, elapsedTime/fadeInTimer);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        sceneLight.intensity = endIntensity;
        isStart = true;
        spawner.SpawnLeaf(player1Pos);
        spawner.SpawnLeaf(player2Pos);
        spawner.SpawnLeaf(player3Pos);
    }

    IEnumerator InstantiateLotus()
    {
        while(lotusNum < maxLotusNum)
        {
            Vector3 location = new Vector3(Random.Range(-1.2f, 1.2f), 0, Random.Range(-1.2f, 1.2f));
            spawner.SpawnLotus(location);
            lotusNum++;
            yield return new WaitForSeconds(10f);
        }
    }
    public void UpdatePosition()
    {
        player1Pos = player1.transform.position;
        player2Pos = player1.transform.position;
        player3Pos = player1.transform.position;
    }
}
