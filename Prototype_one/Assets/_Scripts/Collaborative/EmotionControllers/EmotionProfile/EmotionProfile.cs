using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEmotionProfile
{
    public void UpdateParent();
    public void OnCollisionUpdate();
    public int GetSize();
    public int GetDefaultValue();
    public int GetDefaultMass();
    public Vector3 GetDefaultScale();
    public GameObject GetParentObject();
    public Rigidbody GetParentRb();
    public IEmotionProfile SetDefaultValue(int value);
    public IEmotionProfile SetDefaultMass(int mass);
    public IEmotionProfile SetDefaultScale(Vector3 scale);
    public IEmotionProfile SetParentObject(GameObject parentObj);
    public IEmotionProfile SetParentRb(Rigidbody parentRb);
}
public class EmotionProfile : IEmotionProfile
{
    //ralative size
    private int size;
    private int defaultValue;
    private int defaultMass;
    private Vector3 defaultScale;
    private GameObject parentObj;
    private Rigidbody parentRb;
    public EmotionProfile(int size)
    {
        this.size = size;
    }
    public int GetSize()
    {
        return this.size;
    }
    public int GetDefaultValue()
    {
        return size;
    }
    public int GetDefaultMass()
    {
        return defaultMass;
    }
    public Vector3 GetDefaultScale()
    {
        return defaultScale;
    }
    public GameObject GetParentObject()
    {
        return parentObj;
    }
    public Rigidbody GetParentRb()
    {
        return parentRb;
    }
    public IEmotionProfile SetDefaultValue(int value)
    {
        this.defaultValue = value;
        return this;
    }
    public IEmotionProfile SetDefaultMass(int mass)
    {
        this.defaultMass = mass;
        return this;
    }
    public IEmotionProfile SetDefaultScale(Vector3 scale)
    {
        this.defaultScale = scale;
        return this;
    }
    public IEmotionProfile SetParentObject(GameObject parentObj)
    {
        this.parentObj = parentObj;
        return this;
    }
    public IEmotionProfile SetParentRb(Rigidbody parentRb)
    {
        this.parentRb = parentRb;
        return this;
    }
    public virtual void UpdateParent()
    {
        parentObj.transform.localScale = size * defaultScale;
        parentRb.mass = size * defaultMass;
    }
    public virtual void OnCollisionUpdate()
    {
        Debug.Log("Do nothing");
    }
}
