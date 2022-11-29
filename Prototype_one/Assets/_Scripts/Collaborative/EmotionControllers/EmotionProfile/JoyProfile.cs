using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoyProfile : ProfileDecorator
{
    public JoyProfile(IEmotionProfile profile) : base(profile) { }
    public override void UpdateParent()
    {
        this.GetBaseProfile().UpdateParent();
    }
    public override void OnCollisionUpdate()
    {
        SoundManager.instance.PlaySound("Positive_Collision", false);
        //call parent obj to destroy itself
    }
}
