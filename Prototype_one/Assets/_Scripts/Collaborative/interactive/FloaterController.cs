using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloaterController : MonoBehaviour
{
    public static FloaterController instance;
    [Header("water and air stats")]
    public float UnderWaterDrag;
    public float UnderWaterAngularDrag;
    public float AirDrag;
    public float AirAngularDrag;
    public float FloatingPower;
    public float WaterHeight;
    public float WaveFreq;
    //the distance between gameobject's bottom part with its anchor point
    public float Offset;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }
}
