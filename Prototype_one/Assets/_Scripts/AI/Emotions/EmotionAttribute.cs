using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EmotionAttribute : MonoBehaviour
{
    public int size;
/*    public void Start()
    {
        InitializeAttribute();
    }*/
/*    private void InitializeAttribute()
    {
        gameObject.transform.localScale *= size;
        gameObject.GetComponent<Rigidbody>().mass *= size;
    }*/
    public int GetSize()
    {
        return size;
    }
    public virtual void SetSize(int size)
    {
        this.size = size;
    }
}
