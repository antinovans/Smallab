using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoyController : MonoBehaviour
{
    [Header("Profile Attributes")]
    public int size;
    //describe the positive value of the joy
    public int defaultValue;
    //default mass of the joy
    public int defaultMass;
    //default local scale of the joy
    public Vector3 defaultScale;

    // Start is called before the first frame update
    void Start()
    {
        UpdateTransform();
    }
    private void UpdateTransform()
    {
        transform.localScale = size * defaultScale;
        gameObject.GetComponent<Rigidbody>().mass = size * defaultMass;
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Gate"))
        {
            collision.gameObject.GetComponent<GateVFXController>().HandleValue(this.size * this.defaultValue);
            Destroy(gameObject);
        }
        if (collision.gameObject.CompareTag("Anger"))
        {
            SoundManager.instance.PlaySound("Positive_Collision", false);
            Destroy(gameObject);
        }
        if (collision.gameObject.CompareTag("Sadness"))
        {
            SoundManager.instance.PlaySound("Positive_Collision", false);
            Destroy(gameObject);
        }
        if (collision.gameObject.CompareTag("Depression"))
        {
            SoundManager.instance.PlaySound("Positive_Collision", false);
            Destroy(gameObject);
        }
    }
}
