using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeManager : MonoBehaviour
{
    public static CubeManager instance;
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

    public void HandleCubesColor(List<Cube> cubes, Color color, float time)
    {
        StartCoroutine(LerpCubesColor(cubes, color, time));
    }
    IEnumerator LerpCubesColor(List<Cube> cubes, Color color, float time)
    {
        foreach(var c in cubes)
        {
            c.SetColorWithLerp(color, time, 1f);
            yield return new WaitForSeconds(time);
        }
    }
}
