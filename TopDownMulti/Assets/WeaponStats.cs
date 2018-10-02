using UnityEngine;
using System.Collections;

[System.Serializable]
public enum GunType
{
    PRIMARY,
    SECONDARY
}

[System.Serializable]
public enum BulletType
{
    _45,
    _556,
    _9mm
}

[System.Serializable]
public class WeaponStats
{
    //Pistol Stats
    [Header("WeaponStats")]
    public GunType gunType;
    public BulletType bulletType;
    public string gunName;
    public int CurrentClip;
    public int ClipSize;
    public float FireRate;
    public float Damage;
    public float Accuracy;
    public float ReloadTime;

    
}
