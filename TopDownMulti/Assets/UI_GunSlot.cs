using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class UI_GunSlot : MonoBehaviour {

    WeaponStats myWeaponStats;

    [Header("Damage")]
    //Damage
    [SerializeField]
    Image damageFill;
    [SerializeField]
    Text damageText;
    float maxDamage = 9999;

    [Header("FireRate")]
    //Fire Rate
    [SerializeField]
    Image fireRateFill;
    [SerializeField]
    Text fireRateText;
    float maxFireRate = 9999;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SetWeaponStats(WeaponStats _WeaponStats)
    {
        myWeaponStats = _WeaponStats;
        UpdateUI();
    }

    private void UpdateUI()
    {
        //damage
        float damageRatio = myWeaponStats.Damage / maxDamage;
        damageFill.rectTransform.localScale = new Vector3(damageRatio, 1, 1);
        damageText.text = "" + Math.Floor(myWeaponStats.Damage);

        //FireRate
        float RPM = ((1 / myWeaponStats.FireRate) * 60);
        float fireRateRatio = RPM / maxFireRate;
        fireRateFill.rectTransform.localScale = new Vector3(fireRateRatio, 1, 1);
        fireRateText.text = "" + Math.Floor(RPM);
    }
}
