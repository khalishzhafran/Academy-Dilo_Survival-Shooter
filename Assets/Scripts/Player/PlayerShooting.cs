﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public int damagePerShot = 20;
    public float timeBetweenBullets = 0.15f;
    public float range = 100f;

    float timer;
    Ray shootRay = new Ray();
    RaycastHit shootHit;
    int shootableMask;
    ParticleSystem gunParticles;
    LineRenderer gunLine;
    AudioSource gunAudio;
    Light gunLight;
    float effectsDisplayTime = 0.2f;

    private void Awake()
    {
        //Get Mask
        shootableMask = LayerMask.GetMask("Shootable");

        //Mendapatkan reference component
        gunParticles = GetComponent<ParticleSystem>();
        gunLine = GetComponent<LineRenderer>();
        gunAudio = GetComponent<AudioSource>();
        gunLight = GetComponent<Light>();
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (Input.GetButton("Fire1") && timer >= timeBetweenBullets && Time.timeScale != 0) 
        {
            Shoot();
        }

        if (timer >= timeBetweenBullets * effectsDisplayTime)
        {
            DisableEffects();
        }
    }

    public void DisableEffects()
    {
        //Disable line renderer
        gunLine.enabled = false;

        //Disable light
        gunLight.enabled = false;
    }

    public void Shoot()
    {
        timer = 0f;

        //Play audio
        gunAudio.Play();

        //Enable light
        gunLight.enabled = true;

        //Play gun particle
        gunParticles.Stop();
        gunParticles.Play();

        //Enable line renderer dan set position
        gunLine.enabled = true;
        gunLine.SetPosition(0, transform.position);

        //Set posisi ray shoot dan direction
        shootRay.origin = transform.position;
        shootRay.direction = transform.forward;

        //Lakukan raycast jika mendeteksi id enemy hit apapun
        if (Physics.Raycast(shootRay, out shootHit, range, shootableMask))
        {
            //Lakukan raycast hit hace component EnemyHealth
            EnemyHealth enemyHealth = shootHit.collider.GetComponent<EnemyHealth>();

            if (enemyHealth != null)
            {
                //Lakukan take damage
                enemyHealth.TakeDamage(damagePerShot, shootHit.point);
            }

            //Set line end position ke hit position
            gunLine.SetPosition(1, shootHit.point);
        }
        else
        {
            //Set line end position ke range freom barrel
            gunLine.SetPosition(1, shootRay.origin + shootRay.direction * range);
        }
    }
}
