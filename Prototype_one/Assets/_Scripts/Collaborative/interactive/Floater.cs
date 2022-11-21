using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floater : MonoBehaviour
{
    public Transform[] Floaters;
    public float UnderWaterDrag;
    public float UnderWaterAngularDrag;
    public float AirDrag;
    public float AirAngularDrag;
    public float FloatingPower;
    public float WaterHeight;
    public float WaveFrequency;
    public float Offset;

    Rigidbody Rb;
    bool Underwater;
    int FloatersUnderWater;
    float InitTimeStamp;
    // Start is called before the first frame update
    void Start()
    {
        UpdateStats();
        Rb = this.GetComponent<Rigidbody>();
        InitTimeStamp = Random.Range(-Mathf.PI, Mathf.PI);
    }

    private void UpdateStats()
    {
        this.UnderWaterDrag = FloaterController.instance.UnderWaterDrag;
        this.UnderWaterAngularDrag = FloaterController.instance.UnderWaterAngularDrag;
        this.AirDrag = FloaterController.instance.AirDrag;
        this.AirAngularDrag = FloaterController.instance.AirAngularDrag;
        this.FloatingPower = FloaterController.instance.FloatingPower;
        this.WaterHeight = FloaterController.instance.WaterHeight;
        this.WaveFrequency = FloaterController.instance.WaveFreq;
        this.Offset = FloaterController.instance.Offset;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        FloatersUnderWater = 0;
        for (int i = 0; i < Floaters.Length; i++)
        {
            float diff = Floaters[i].position.y + Offset - WaterHeight * Mathf.Sin(Time.time * WaveFrequency + InitTimeStamp);
            if (diff < 0)
            {
                Rb.AddForceAtPosition(Vector3.up * FloatingPower * Physics.gravity.magnitude * Mathf.Abs(diff), Floaters[i].position, ForceMode.Force);
                FloatersUnderWater += 1;
                if (!Underwater)
                {
                    Underwater = true;
                    SwitchState(true);
                }
            }
        }
        if (Underwater && FloatersUnderWater == 0)
        {
            Underwater = false;
            SwitchState(false);
        }
    }
    void SwitchState(bool isUnderwater)
    {
        if (isUnderwater)
        {
            Rb.drag = UnderWaterDrag;
            Rb.angularDrag = UnderWaterAngularDrag;
        }
        else
        {
            Rb.drag = AirDrag;
            Rb.angularDrag = AirAngularDrag;
        }
    }
}
