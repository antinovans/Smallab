using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public static EffectManager instance;
    [Header("stars stats")]
    public GameObject starPS;
    [Header("Firework stats")]
    public GameObject firework;
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

    public void InstantiateStars(Vector3 position)
    {
        var star = Instantiate(starPS, transform);
        star.transform.position = position;
        star.transform.rotation = Quaternion.Euler(-90f, 0f, 0f);
        Destroy(star, 2.0f);
    }

}
