using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngryProfile : ProfileDecorator
{
    public bool isSplitable
    {
        get { return isSplitable; }
        set { isSplitable = value; }
    }
    /*    public static int DEFAULT_MASS = 2;
        public static Vector3 DEFAULT_SCALE = new Vector3(0.08f, 0.08f, 0.08f);*/

    public AngryProfile(IEmotionProfile profile) : base(profile) { 
        this.isSplitable = this.GetBaseProfile().GetSize() > 1 ? true : false;
    }

    public override void UpdateParent()
    {
        this.GetBaseProfile().UpdateParent();
        this.isSplitable = this.GetBaseProfile().GetSize() > 1 ? true : false;
    }
    public override void OnCollisionUpdate()
    {
        //call parent obj to scale down
    }
}
