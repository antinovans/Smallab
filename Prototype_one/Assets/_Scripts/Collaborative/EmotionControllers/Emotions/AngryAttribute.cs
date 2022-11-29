using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngryAttribute : EmotionAttribute
{
    public bool isSplitable;

    public static int DEFAULT_MASS = 2;
    public static Vector3 DEFAULT_SCALE = new Vector3(0.08f, 0.08f, 0.08f);
    // Start is called before the first frame update
    private void Start()
    {
        UpdateAttribute();
    }
    public override void SetSize(int size)
    {
        this.size = size;
        UpdateAttribute();
    }
    private void UpdateAttribute()
    {
        isSplitable = size > 1 ? true : false;
        gameObject.transform.localScale = size * DEFAULT_SCALE;
        gameObject.GetComponent<Rigidbody>().mass = size * DEFAULT_MASS;
    }

    private void OnCollisionEnter(Collision collision)
    {
        var collisionObj = collision.gameObject;
        if (collisionObj.CompareTag("Joy") && isSplitable)
        {
            SoundManager.instance.PlaySound("Positive_Collision", false);
            Split(collisionObj.GetComponent<Rigidbody>().velocity, gameObject.GetComponent<Rigidbody>().velocity
                , collisionObj.GetComponent<Rigidbody>().mass, gameObject.GetComponent<Rigidbody>().mass);
            Destroy(collisionObj);
            return;
        }
    }
    private void Split(Vector3 v1, Vector3 v2, float m1, float m2)
    {
        var newVelocity = v2 + v1 * (m1/m2);
        var tempSize = this.size / 2;
        gameObject.GetComponent<AngryAttribute>().SetSize(tempSize);
        var newAnger = Instantiate(gameObject, transform.position, Quaternion.identity, transform.parent);
        gameObject.GetComponent<Rigidbody>().velocity = newVelocity;
        newAnger.GetComponent<Rigidbody>().velocity = newVelocity;
        gameObject.GetComponent<AngryVFXController>().SetColor(tempSize);
    }
}
