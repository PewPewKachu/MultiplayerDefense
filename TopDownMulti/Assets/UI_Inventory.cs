using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UI_Inventory : MonoBehaviour {

    [SerializeField]
    Image PrimarySlot1;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void UpdatePrimary1(WeaponStats _weaponStats)
    {
        PrimarySlot1.GetComponent<UI_GunSlot>().SetWeaponStats(_weaponStats);
    }
}
