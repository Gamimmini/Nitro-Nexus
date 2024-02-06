using System.Collections;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
public class Rifle : MonoBehaviour
{
    public PlayerScript player;
    
    [Header("Rifle")]
    public Camera cam;
    public float giveDamage = 10f;
    public float shootingRange = 100;
    public float fireCharge = 15f;
    public Animator animator;

    [Header("Rifle Ammuntion and Shooting")]
    private float nextTimeToShoot = 0f;
    private int maximumAmmunition = 1000;
    private int mag = 1000;
    private int presentAmmuntion;
    public float reloadingTime = 1.3f;
    private bool setReloading = false;
   
    [Header("Rifle effects")]
    public ParticleSystem muzzuleSpark;
    //public GameObject WoodedEffect;
    public GameObject goreEffect;

    [Header("Sound effects")]
    public AudioSource audioSource;
    public AudioClip shootingSound;
    public AudioClip reloadingSound;


    public bool isShooting = false;
    private void Awake()
    {
        presentAmmuntion = maximumAmmunition;
    }    
    private void Update()
    {
        if (setReloading) return;
        if(presentAmmuntion <= 0)
        {
            StartCoroutine(Reload());
            return;
        }
        /*
        if (isShooting)
        {
            CrossPlatformInputManager.SetButtonDown("Shoot");
        }
        else
        {
            CrossPlatformInputManager.SetButtonUp("Shoot");
        } 
        */
        

        if (player.mobileInputs == true)
        {
            if (CrossPlatformInputManager.GetButton("Shoot") && Time.time >= nextTimeToShoot)
            {
                animator.SetBool("Fire", true);
                animator.SetBool("Idle", false);

                nextTimeToShoot = Time.time + 1f / fireCharge;
                Shoot();
            }
            else if (CrossPlatformInputManager.GetButton("Shoot") && player.currentPlayerSpeed > 0)
            {
                animator.SetBool("Idle", false);
                animator.SetBool("FireWalk", true);
                animator.SetBool("Walk", true);
                animator.SetBool("Fire", false);
                //animator.SetBool("Running", true);
            }
            else if (CrossPlatformInputManager.GetButton("Shoot") && player.currentPlayerSprint > 0)
            {
                animator.SetBool("Idle", false);
                animator.SetBool("FireWalk", true);
                //animator.SetBool("Walk", true);
                animator.SetBool("Running", true);
                animator.SetBool("Fire", false);
            }
            else if (CrossPlatformInputManager.GetButton("Shoot") && CrossPlatformInputManager.GetButton("Aim"))
            {
                animator.SetBool("Idle", false);
                animator.SetBool("IdleAim", true);
                animator.SetBool("FireWalk", true);
                animator.SetBool("Walk", true);
                //animator.SetBool("AimWalk", true);
                animator.SetBool("Reloading", false);
            }
            else
            {
                animator.SetBool("Fire", false);
                animator.SetBool("Idle", true);
                animator.SetBool("FireWalk", false);
            }
        }
        else
        {
            if (Input.GetButtonDown("Fire1") && Time.time >= nextTimeToShoot)
            {
                animator.SetBool("Fire", true);
                animator.SetBool("Idle", false);

                nextTimeToShoot = Time.time + 1f / fireCharge;
                Shoot();
            }
            else if (Input.GetButton("Fire1") && Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                animator.SetBool("Idle", true);
                animator.SetBool("FireWalk", true);
                animator.SetBool("Walk", true);
            }
            else if (Input.GetButton("Fire1") && Input.GetButton("Fire2"))
            {
                animator.SetBool("Idle", false);
                animator.SetBool("IdleAim", true);
                animator.SetBool("FireWalk", true);
                animator.SetBool("Walk", true);
                animator.SetBool("Reloading", false);
            }
            else
            {
                animator.SetBool("Fire", false);
                animator.SetBool("Idle", true);
                animator.SetBool("FireWalk", false);
            }
        }
    }
    void Shoot()
    {
        if(mag == 0)
        {

        }
        presentAmmuntion--;

        if(presentAmmuntion == 0)
        {
            mag--;
        }

        //Update UI
        AmmoCount.occurrence.UpdateAmmoText(presentAmmuntion);
        AmmoCount.occurrence.UpdateMagText(mag);

        muzzuleSpark.Play();
        audioSource.PlayOneShot(shootingSound);
        RaycastHit hitInfo;
        
        //Debug.DrawRay(cam.transform.position, cam.transform.forward * shootingRange, Color.red, 0.1f);
        if (Physics.Raycast(player.playerCamera.transform.position, player.playerCamera.transform.forward, out hitInfo, shootingRange))
        {
            //Debug.Log(hitInfo.transform.name);
            //Debug.Log("Raycast hit something!");
            Enemy enemy = hitInfo.transform.GetComponent<Enemy>();

            Objects objects = hitInfo.transform.GetComponent<Objects>();

            if (objects != null)
            {
                objects.objectHitDamage(giveDamage);
                //Debug.Log("Object Hit! Damage: " + giveDamage);
                //GameObject WoodGo = Instantiate(WoodedEffect, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
                //Destroy(WoodGo, 1f);
            }
            else if(enemy != null)
            {
                enemy.enemyHitDamage(giveDamage);
                //Debug.Log("Enemy Hit! Damage: " + giveDamage);
                GameObject goreGo = Instantiate(goreEffect, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
                Destroy(goreGo, 1f);
            }

            /*
            Vector3 newLookDirection = hitInfo.point + player.playerCamera.transform.forward * shootingRange - player.transform.position;
            newLookDirection.y = 0f; // Giữ nguyên chỉ phần ngang (không đổi theo chiều cao)
            Quaternion newRotation = Quaternion.Euler(0f, player.playerCamera.transform.rotation.eulerAngles.y, 0f);
            player.transform.rotation = Quaternion.Lerp(player.transform.rotation, newRotation, Time.deltaTime * 5f);


            player.transform.rotation = Quaternion.LookRotation(newLookDirection);
            */
            //Debug.Log("PlayerCamera rotation: " + player.playerCamera.transform.rotation.eulerAngles);
            //Debug.Log("Player rotation: " + player.transform.rotation.eulerAngles);

            //Debug.DrawRay(player.playerCamera.transform.position, player.playerCamera.transform.forward * shootingRange, Color.blue, 1f); // Blue line for playerCamera direction
            //Debug.DrawRay(player.transform.position, newLookDirection.normalized * shootingRange, Color.green, 1f); // Green line for newLookDirection

        }

        

    }
    IEnumerator Reload()
    {
        player.playerSpeed = 0f;
        player.playerSprint = 0f;
        setReloading = true;
        Debug.Log("Reloading...");
        animator.SetBool("Reloading", true);
        audioSource.PlayOneShot(reloadingSound);
        yield return new WaitForSeconds(reloadingTime);

        animator.SetBool("Reloading", false);
        presentAmmuntion = maximumAmmunition;
        player.playerSpeed = 1.9f;
        player.playerSprint = 3f;
        setReloading = false;
    }    
}
