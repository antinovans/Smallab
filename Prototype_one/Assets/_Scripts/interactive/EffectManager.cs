using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public static EffectManager instance;
    [Header("stars stats")]
    public GameObject starPS;
    [Header("firework effect stats")]
    public GameObject firework;

    public GameObject player1;
    public GameObject player2;
    public GameObject player3;

    private bool isFirework = true;
    private Vector3 v1;
    private Vector3 v2;
    private Vector3 v3;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        isFirework = true;
        StartCoroutine(InitializeFirework());
    }
    private void Update()
    {
        if(GetPlayerDistances() <=0.26f && !isFirework)
        {
            isFirework = true;
            StartCoroutine(InstantiateFirework());
        }

    }
    public void InstantiateStars(Vector3 position)
    {
        var star = Instantiate(starPS, transform);
        star.transform.position = position;
        star.transform.rotation = Quaternion.Euler(-90f, 0f, 0f);
        Destroy(star, 2.0f);
    }
    IEnumerator InitializeFirework()
    {
        yield return new WaitForSeconds(1f);
        isFirework = false;
    }
    IEnumerator InstantiateFirework()
    {
        Vector3 pos = new Vector3((v1.x + v2.x + v3.x) / 3, 1.0f, (v1.z + v2.z + v3.z) / 3);
        var temp = Instantiate(firework, pos, Quaternion.identity);
        yield return new WaitForSeconds(10f);
        Destroy(temp);
        isFirework = false;
    }
    float GetPlayerDistances()
    {
        v1 = player1.transform.position;
        v2 = player2.transform.position;
        v3 = player3.transform.position;
        return Mathf.Abs(Vector3.Distance(v1, v2)) +
            Mathf.Abs(Vector3.Distance(v3, v2));
    }
}
