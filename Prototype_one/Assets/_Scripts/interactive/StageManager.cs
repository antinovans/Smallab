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

    [Header("lotus setting")]
    public int maxLotusNum;
    public int lotusNum;
    private Vector3 player1Pos;
    private Vector3 player2Pos;
    private Vector3 player3Pos;

    private Spawner spawner;
    private bool isStart;
    private bool instantiating;
    private void Awake()
    {
        spawner = GetComponent<Spawner>();
    }
    // Start is called before the first frame update
    void Start()
    {
        isStart = false;
        instantiating = false;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(Data.positions.Count);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(LightUp());
        }
        if(isStart)
        {
            if (!instantiating)
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
        FindObjectOfType<SoundManager>().PlaySound("Background", true);
        spawner.SpawnLeaf(player1Pos);
        spawner.SpawnLeaf(player2Pos);
        spawner.SpawnLeaf(player3Pos);
        Data.AddPosition(new Vector2(player1Pos.x, player1Pos.z));
        Data.AddPosition(new Vector2(player2Pos.x, player2Pos.z));
        Data.AddPosition(new Vector2(player2Pos.x, player2Pos.z));
        StartCoroutine(ToggleStart());
    }

    IEnumerator ToggleStart()
    {
        yield return new WaitForSeconds(4f);
        isStart = true;
    }
    IEnumerator InstantiateLotus()
    {
        instantiating = true;
        while (Lotus.num < maxLotusNum)
        {
            /*bool shouldContinue = false;*/
            Vector3 location = new Vector3(Random.Range(-1.5f, 1.5f), 0, Random.Range(-1.5f, 1.5f));
/*            foreach (var d in Data.positions)
            {
                if (Vector2.Distance(location, d) < 0.5f)
                {
                    shouldContinue = true;
                    break;
                }
            }
            if (shouldContinue)
                continue;*/
            spawner.SpawnLotus(location);
/*            yield return new WaitForSeconds(10f);
            yield return null;*/
            yield return new WaitForSeconds(0.5f);
        }
        instantiating = false;
    }
    public void UpdatePosition()
    {
        player1Pos = player1.transform.position;
        player2Pos = player1.transform.position;
        player3Pos = player1.transform.position;
    }
}
