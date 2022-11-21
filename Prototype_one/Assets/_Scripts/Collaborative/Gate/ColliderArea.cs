using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderArea : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        var collisionObj = collision.gameObject;
        if (!collisionObj.CompareTag("Joy") && !collisionObj.CompareTag("Anger"))
        {
            SoundManager.instance.PlaySound("Positive_Consume", false);
            return;
        }
        if (collisionObj.CompareTag("Anger"))
        {
            if(collisionObj.GetComponent<AngryAttribute>().GetSize() > 1)
            {
                EventManager.current.GateColliderEnter();
                EventManager.current.GateColorChange(collision.gameObject.GetComponent<EmotionAttribute>().GetSize());
            }
            Destroy(collisionObj);
            return;
        }
        if (collisionObj.CompareTag("Joy"))
        {
            EventManager.current.GateColorChange(collision.gameObject.GetComponent<EmotionAttribute>().GetSize());
            Destroy(collisionObj);
            return;
        }
    }
}
