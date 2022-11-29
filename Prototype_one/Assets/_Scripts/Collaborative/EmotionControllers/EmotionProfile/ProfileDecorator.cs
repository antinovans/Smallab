using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ProfileDecorator : IEmotionProfile
{
    private IEmotionProfile baseProfile;
    public ProfileDecorator(IEmotionProfile profile)
    {
        this.baseProfile = profile;
    }
    public abstract void UpdateParent();
    public abstract void OnCollisionUpdate();
    public int GetSize()
    {
        return this.baseProfile.GetSize();
    }
    public int GetDefaultValue()
    {
        return baseProfile.GetDefaultValue();
    }
    public int GetDefaultMass()
    {
        return baseProfile.GetDefaultMass();
    }
    public Vector3 GetDefaultScale()
    {
        return baseProfile.GetDefaultScale();
    }
    public GameObject GetParentObject()
    {
        return baseProfile.GetParentObject();
    }
    public Rigidbody GetParentRb()
    {
        return baseProfile.GetParentRb();
    }
    public IEmotionProfile SetDefaultValue(int value)
    {
        return baseProfile.SetDefaultValue(value);
    }
    public IEmotionProfile SetDefaultMass(int mass)
    {
        return baseProfile.SetDefaultMass(mass);
    }
    public IEmotionProfile SetDefaultScale(Vector3 scale)
    {
        return baseProfile.SetDefaultScale(scale);
    }
    public IEmotionProfile SetParentObject(GameObject parentObj)
    {
        return baseProfile.SetParentObject(parentObj);
    }
    public IEmotionProfile SetParentRb(Rigidbody parentRb)
    {
        {
            return baseProfile.SetParentRb(parentRb);
        }
    }
    public IEmotionProfile GetBaseProfile()
    {
        return this.baseProfile;
    }
}
