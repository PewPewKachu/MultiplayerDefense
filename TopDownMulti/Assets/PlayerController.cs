using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;
using System;

public class PlayerController : NetworkBehaviour {
    [SyncVar]
    [SerializeField]
    WeaponStats currWeaponStats;
    [SyncVar]
    [SerializeField]
    WeaponStats primaryGun;
    [SyncVar]
    [SerializeField]
    WeaponStats secondaryGun;

    [SerializeField]
    Camera myCam;

    [SerializeField]
    float walkSpeed = 5;

    [SerializeField]
    GameObject playerObj;
    [SerializeField]
    GameObject currentGun;

    [SerializeField]
    GameObject bulletPrefab;

    Plane plane;

    //Testing Vars

    //UI
    [SerializeField]
    Text ammoText;
    [SerializeField]
    Text primaryText;
    [SerializeField]
    GameObject UpgradeScreen;

    //Ammo Stats
    public int _45MaxAmmo = 150;
    public int _45CurrentAmmo = 150;

    public int _556MaxAmmo = 300;
    public int _556CurrentAmmo = 300;
    [SyncVar]
    [SerializeField]
    public int _9mmMaxAmmo = 300;
    [SyncVar]
    [SerializeField]
    public int _9mmCurrentAmmo = 300;

    [SyncVar]
    int currentClip;
    //Cooldowns
    [SyncVar]
    [SerializeField]
    double fireCooldown = 0;
    [SyncVar]
    [SerializeField]
    double reloadCooldown = 0;
    [SyncVar]
    [SerializeField]
    bool isReloading = false;

    [SerializeField]
    Vector3 mousePos;
    double netDeltaTime;
    double netPrevTime;
	// Use this for initialization
	void Start ()
    {
        plane = new Plane(Vector3.up, transform.position);
        currWeaponStats = primaryGun;
        _9mmCurrentAmmo = 300;
        _45CurrentAmmo = 150;

        currentClip = currWeaponStats.ClipSize;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        Debug.DrawLine(-((transform.position - mousePos).normalized * 100f), playerObj.transform.position, Color.green);
        Debug.DrawLine(mousePos, playerObj.transform.position, Color.yellow);

        #region UI
        switch (currWeaponStats.bulletType) 
        {
            case BulletType._45:
                ammoText.text = currWeaponStats.gunName + ": " + currWeaponStats.CurrentClip.ToString() + "/" + _45CurrentAmmo.ToString();
                break;
            case BulletType._556:
                ammoText.text = currWeaponStats.gunName + ": " + currWeaponStats.CurrentClip.ToString() + "/" + _556CurrentAmmo.ToString();
                break;
            case BulletType._9mm:
                ammoText.text = currWeaponStats.gunName + ": " + currWeaponStats.CurrentClip.ToString() + "/" + _9mmCurrentAmmo.ToString();
                break;
            default:
                break;
        }

        UpdateUpgadesScreen();

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (!UpgradeScreen.activeSelf)
            {
                UpgradeScreen.SetActive(true);
                //UpgradeScreen.GetComponent<UI_Inventory>().UpdatePrimary1(currWeaponStats);
            }
            else
            {
                UpgradeScreen.SetActive(false);
            }
        }
        #endregion

        #region Cooldowns
        //Shooting Cooldown
        CmdFireCooldown();

        //Reloading Cooldown
        CmdReloadCooldown();

        //Get the network's Delta Time
        CmdGetNetDeltaTime();
        #endregion

        #region MouseLook
        Ray ray = myCam.ScreenPointToRay(Input.mousePosition);
        float hitDist = 0.0f;
        if (plane.Raycast(ray, out hitDist))
        {
            Vector3 targetPoint = ray.GetPoint(hitDist);
            if (myCam != null)
            {
                myCam.GetComponent<PlayerCameraScript>().SetMousePos(targetPoint);
                mousePos = targetPoint;
            }
            Quaternion targetRotation = Quaternion.LookRotation(targetPoint - transform.position);
            targetRotation.x = 0;
            targetRotation.z = 0;
            playerObj.transform.rotation = Quaternion.Slerp(playerObj.transform.rotation, targetRotation, 50f * Time.deltaTime);
        }
        #endregion


        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(Vector3.forward * walkSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(Vector3.back * walkSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(Vector3.left * walkSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(Vector3.right * walkSpeed * Time.deltaTime);
        }

        if (Input.GetMouseButton(0))
        {
            CmdShoot();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            CmdReload();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            CmdSwapWeapon(GunType.PRIMARY);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            CmdSwapWeapon(GunType.SECONDARY);
        }
    }

    private void UpdateUpgadesScreen()
    {
        primaryText.text = primaryGun.gunName + "\n" +
                           "Damage:    " + primaryGun.Damage + "\n" +
                           "FireRate:  " + Math.Round((1.0f / primaryGun.FireRate), 2) + " shots/sec" + "\n" +
                           "Accuracy:  " + primaryGun.Accuracy + "\n" +
                           "Clip Size: " + primaryGun.ClipSize + "\n";

    }

    [Command]
    private void CmdSwapWeapon(GunType _gunType)
    {
        switch (_gunType)
        {
            case GunType.PRIMARY:
                if (currWeaponStats.gunType == GunType.PRIMARY)
                {
                    return;
                }

                currWeaponStats = primaryGun;
                break;
            case GunType.SECONDARY:
                if (currWeaponStats.gunType == GunType.SECONDARY)
                {
                    return;
                }

                currWeaponStats = secondaryGun;
                break;
            default:
                break;
        }
    }

    [Command]
    void CmdGetNetDeltaTime()
    {
        netDeltaTime = Network.time - netPrevTime;
        
        netPrevTime = Network.time;
    }

    [Command]
    void CmdFireCooldown()
    {
        if (fireCooldown >= 0)
        {
            fireCooldown -= netDeltaTime;
        }
    }

    [Command]
    void CmdReloadCooldown()
    {
        if (reloadCooldown >= 0)
        {
            reloadCooldown -= netDeltaTime;
        }
        else
        {
            isReloading = false;
        }
    }

    [Command]
    void CmdShoot()
    {
        if (!isServer)
        {
            return;
        }
        if (fireCooldown <= 0 && currWeaponStats.CurrentClip > 0 && !isReloading)
        {
            fireCooldown = currWeaponStats.FireRate;
            currWeaponStats.CurrentClip -= 1;
            currentClip = currWeaponStats.CurrentClip;
            RaycastHit[] hits;
            hits = Physics.RaycastAll(transform.position, playerObj.transform.forward, 100.0f);

            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].transform.tag == "Enemy")
                {
                    hits[i].collider.gameObject.GetComponent<EnemyScript>().TakeDamage(currWeaponStats.Damage);
                }
            }
            GameObject bulletToSpawn = (GameObject)Instantiate(bulletPrefab, transform.position + new Vector3(0, 1.3f, 0), playerObj.transform.rotation);
            //Vector3 bulletTargetPos = -(transform.position - mousePos).normalized;
            //Vector3 bulletTargetPos = -(new Vector3(transform.position.x, 0.0f,transform.position.z) - new Vector3(mousePos.x, 0.0f, mousePos.z)).normalized;
            bulletToSpawn.GetComponent<Bullet>().SetTargetPos(mousePos);
            //float randomAngle = 10 * ((100 - currWeaponStats.Accuracy) / 100f);
            //bulletToSpawn.transform.Rotate(0, UnityEngine.Random.Range(-randomAngle, randomAngle), 0);
            //bulletToSpawn.GetComponent<Bullet>().SetDamage(currWeaponStats.Damage);
            NetworkServer.Spawn(bulletToSpawn);

        }
    }

    
    [Command]
    void CmdReload()
    {
        if (!isServer)
        {
            return;
        }
        switch (currWeaponStats.bulletType)
        {
            case BulletType._45:
                if (!isReloading && _45CurrentAmmo > 0)
                {
                    isReloading = true;
                    _45CurrentAmmo += currWeaponStats.CurrentClip;
                    currWeaponStats.CurrentClip = 0;
                    if (_45CurrentAmmo < currWeaponStats.ClipSize)
                    {
                        currWeaponStats.CurrentClip = _45CurrentAmmo;
                        currentClip = currWeaponStats.CurrentClip;
                        _45CurrentAmmo = 0;
                    }
                    else
                    {
                        _45CurrentAmmo -= currWeaponStats.ClipSize;
                        currWeaponStats.CurrentClip = currWeaponStats.ClipSize;
                        currentClip = currWeaponStats.CurrentClip;
                    }
                    reloadCooldown = currWeaponStats.ReloadTime;
                }
                break;
            case BulletType._556:
                if (!isReloading && _556CurrentAmmo > 0)
                {
                    isReloading = true;
                    _556CurrentAmmo += currWeaponStats.CurrentClip;
                    currWeaponStats.CurrentClip = 0;
                    if (_556CurrentAmmo < currWeaponStats.ClipSize)
                    {
                        currWeaponStats.CurrentClip = _556CurrentAmmo;
                        currentClip = currWeaponStats.CurrentClip;
                        _556CurrentAmmo = 0;
                    }
                    else
                    {
                        _556CurrentAmmo -= currWeaponStats.ClipSize;
                        currWeaponStats.CurrentClip = currWeaponStats.ClipSize;
                        currentClip = currWeaponStats.CurrentClip;
                    }
                    reloadCooldown = currWeaponStats.ReloadTime;
                }
                break;
            case BulletType._9mm:
                if (!isReloading && _9mmCurrentAmmo > 0)
                {
                    isReloading = true;
                    _9mmCurrentAmmo += currWeaponStats.CurrentClip;
                    currWeaponStats.CurrentClip = 0;
                    if (_9mmCurrentAmmo < currWeaponStats.ClipSize)
                    {
                        currWeaponStats.CurrentClip = _9mmCurrentAmmo;
                        currentClip = currWeaponStats.CurrentClip;
                        _9mmCurrentAmmo = 0;
                    }
                    else
                    {
                        _9mmCurrentAmmo -= currWeaponStats.ClipSize;
                        currWeaponStats.CurrentClip = currWeaponStats.ClipSize;
                        currentClip = currWeaponStats.CurrentClip;
                    }
                    reloadCooldown = currWeaponStats.ReloadTime;
                }
                break;
            default:
                break;
        }
    }
}
