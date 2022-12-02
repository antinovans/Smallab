using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GateVFXController :MonoBehaviour
{
    public Color baseColorBegin;
    public Color baseColorEnd;
    public int maxHealth;

    private int initHealth; 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Joy"))
        {
            Destroy(collision.gameObject);
            return;
        }
        if (collision.gameObject.CompareTag("Anger"))
        {
            Destroy(collision.gameObject);
            return;
        }
        if (collision.gameObject.CompareTag("Sadness"))
        {
            Destroy(collision.gameObject);
            return;
        }
        if (collision.gameObject.CompareTag("Depression"))
        {
            Destroy(collision.gameObject);
            return;
        }
    }

}
