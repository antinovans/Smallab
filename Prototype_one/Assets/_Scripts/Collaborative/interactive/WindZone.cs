using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindZone : MonoBehaviour
{
    public static WindZone instance;
    public float windForce;
    public ForceMode mode;
    public Vector3 windDir;
    private GameObject[] players;
    private Vector3[] lastPositions;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        lastPositions = new Vector3[players.Length];
        for(int i = 0; i < players.Length; i++)
        {
            lastPositions[i] = players[i].transform.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        windDir = new Vector3(0, 0, 0);
        for (int i = 0; i < players.Length; i++)
        {
            windDir += players[i].transform.position - lastPositions[i];
            lastPositions[i] = players[i].transform.position;
        }
    }
}
