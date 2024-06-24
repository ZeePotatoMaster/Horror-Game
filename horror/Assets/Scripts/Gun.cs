using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class Gun : NetworkBehaviour
{
    //gun variables
    [SerializeField]
    private float Damage = 50f;
    [SerializeField]
    private float TimeBetweenShots = 0.3f;
    private float TimeUntilShot;
    private bool CanShoot = true;

    [SerializeField]
    private int MaxAmmo = 6;
    private int CurrentAmmo;
    [SerializeField]
    private float ReloadTime = 1f;
    [SerializeField]
    public bool IsReloading = false;
    public int TotalAmmo = 18;

    //main script, player!>!>
    private PlayerBase pb;

    //crosshair effect vars
    private Image Crosshair;
    private bool hitMarker = false;
    private float crosshairTimer = 0f;
    [SerializeField]
    private float crosshairIncrease = 2.0f;
    private Vector3 defCrosshairSize;

    //HUD vars
    [SerializeField] private GameObject PistolUI;
    private Text CurrentAmmoScore;
    private Text TotalAmmoScore;
    private GameObject UI;
    private GameObject canvas;

    //bullet spread
    [SerializeField]
    private float randomLvl = 0;
    private float rand;

    //[SerializeField]
    //private GameObject cube;

    //animation
    [SerializeField]
    private string shootAnim;
    [SerializeField]
    private string reloadAnim;

    // Start is called before the first frame update
    void Start()
    {
        if (!IsOwner) return;

        canvas = GameObject.Find("Canvas");
        UI = Instantiate(PistolUI, Vector3.zero, Quaternion.identity);
        UI.transform.SetParent(canvas.transform, false);
        CurrentAmmoScore = UI.transform.Find("Current Ammo").GetComponent<Text>();
        TotalAmmoScore = UI.transform.Find("Total Ammo").GetComponent<Text>();
        Crosshair = UI.transform.Find("Crosshair").GetComponent<Image>();

        // crosshair start size
        defCrosshairSize = new Vector3(Crosshair.transform.localScale.x, Crosshair.transform.localScale.y, Crosshair.transform.localScale.z);
        
        //ammo
        CurrentAmmo = MaxAmmo;

        //find base
        pb = this.transform.parent.gameObject.GetComponent<PlayerBase>();
    }

    void OnEnable(){
        if (!IsOwner) return;
        UI.gameObject.SetActive(true);
    }

    void OnDisable(){
        if (!IsOwner) return;
        UI.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;

        if (pb.attacked && CanShoot == true && pb != null) Shoot();

        // cooldown
        if (Time.time > TimeUntilShot && CanShoot == false && IsReloading == false)
        {
            Debug.Log(" Ready!");
            CanShoot = true;
        }

        //when out of ammo
        if (CurrentAmmo <= 0 && IsReloading == false && CanShoot && pb != null)
        {
            if (TotalAmmo > 0)
            {
                StartCoroutine(Reload());
            }

            if (TotalAmmo == 0)
            {
                CanShoot = false;
                IsReloading = true;
            }
        }

        //reload manually
        if (pb.reloaded && IsReloading == false && CurrentAmmo != MaxAmmo && pb != null)
        {
            if (TotalAmmo > 0)
            {
                StartCoroutine(Reload());
            }
        }

        //weapon bob
        if (Mathf.Abs(pb.moveDirection.x) > 0.1f || Mathf.Abs(pb.moveDirection.z) > 0.1f)
        {
            this.transform.GetComponent<Animator>().SetBool("walking", true);
        }
        else
        {
            this.transform.GetComponent<Animator>().SetBool("walking", false);
        }

        //hit enemy with crosshair effect
        if (hitMarker)
        {
            crosshairTimer += Time.deltaTime;

            if (crosshairTimer < 0.1)
            {
                Crosshair.transform.localScale = new Vector3(Crosshair.transform.localScale.x, Crosshair.transform.localScale.y, Crosshair.transform.localScale.z) + (new Vector3(crosshairTimer, crosshairTimer, crosshairTimer) * crosshairIncrease);
            }
            if (crosshairTimer >= 0.2)
            {
                Crosshair.transform.localScale = defCrosshairSize;

                crosshairTimer = 0;
                hitMarker = false;
            }
            if (crosshairTimer > 0.1 && crosshairTimer < 0.2)
            {
                Crosshair.transform.localScale = new Vector3(Crosshair.transform.localScale.x, Crosshair.transform.localScale.y, Crosshair.transform.localScale.z) - ((new Vector3(crosshairTimer, crosshairTimer, crosshairTimer) / 2) * crosshairIncrease);
            }
        }

        //UI
        CurrentAmmoScore.text = "" + CurrentAmmo;
        TotalAmmoScore.text = "" + TotalAmmo;
    }

    //shoot gun
    private void Shoot()
    {
        this.transform.GetComponent<Animator>().Play(shootAnim);

        rand = Random.Range(-randomLvl, randomLvl);

        RaycastHit hit;
        if (Physics.Raycast(pb.playerCamera.transform.position, new Vector3(pb.playerCamera.transform.forward.x + rand, pb.playerCamera.transform.forward.y + rand, pb.playerCamera.transform.forward.z + rand), out hit))
        {
            Debug.Log(hit.transform.name);

            if (hit.transform.tag == "Bullet")
            {
                Destroy(hit.transform.gameObject);
            }

            if (hit.transform.tag == "Enemy")
            {
                hit.transform.gameObject.SendMessage("TakeGunDamage", Damage);
                hitMarker = true;
            }

            if (hit.transform.tag == "Player")
            {
                DamageServerRpc(hit.transform.gameObject.GetComponent<NetworkObject>().OwnerClientId);
                hitMarker = true;
            }

            //Instantiate(cube, hit.point, Quaternion.identity);
        }

        CanShoot = false;
        TimeUntilShot = Time.time + TimeBetweenShots;

        CurrentAmmo--;
    }

    [ServerRpc]
    private void DamageServerRpc(ulong id)
    {  
        NetworkManager.ConnectedClients[id].PlayerObject.GetComponent<PlayerHealth>().health.Value -= 25f;
    }


    //reloading

    //reload pistol
    private IEnumerator Reload()
    {
        CanShoot = false;
        IsReloading = true;

        this.transform.GetComponent<Animator>().Play(reloadAnim);

        yield return new WaitForSeconds(ReloadTime);

        if (this.transform.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName(reloadAnim))
        {
            //TotalAmmo -= MaxAmmo;
            TotalAmmo += CurrentAmmo;

            if (TotalAmmo >= MaxAmmo)
            {
                CurrentAmmo = MaxAmmo;

                TotalAmmo -= MaxAmmo;
            }

            else 
            {
                CurrentAmmo = TotalAmmo;

                TotalAmmo = 0;
            }

            Debug.Log(TotalAmmo);
        }

        CanShoot = true;
        IsReloading = false;
    }
}
