using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class deadParts : MonoBehaviour
{
    [SerializeField] private Material sprites;
    [SerializeField] private ParticleSystem ps;
    private ParticleSystemRenderer rend;
    private Vector3 scal;
    private ParticleSystem.TextureSheetAnimationModule anim;
    private ParticleSystem.EmissionModule emis;
    private ParticleSystem.Burst burs;
    private ParticleSystem.MainModule man;
    private void Start()
    {
        rend = ps.GetComponent<ParticleSystemRenderer>();
        scal = ps.shape.scale;
        anim = ps.textureSheetAnimation;
        emis = ps.emission;
        burs = emis.GetBurst(0);
        man = ps.main;
    }
    public void spawnParticles()
    {
        man.loop = false;
        rend.material = sprites;
        anim.enabled = true;
        anim.numTilesX = 5;
        anim.frameOverTime = new ParticleSystem.MinMaxCurve(0);
        emis.rateOverTime = 0;
        emis.burstCount = 1;
        burs.count = 1;
        ps.emission.SetBurst(0, burs);
        scal.y = 0;
        transform.position = new Vector3(transform.position.x, transform.position.y, -0.1f);
        transform.rotation=new Quaternion(0,0,0,0);
        ParticleSystem.Particle[] particleArray = new ParticleSystem.Particle[man.maxParticles];
        int particles = ps.GetParticles(particleArray);
        for (int i = 0; i < particles; i++)
        {
            Vector3 vel = particleArray[i].velocity;
            vel.z = 0;
            particleArray[i].velocity = vel;
        }
        ps.SetParticles(particleArray, particles);
        for (int i = 0; i < anim.numTilesX; i++)
        {
            if (i >= anim.numTilesX - 2)
            {
                burs.count = 2;
                ps.emission.SetBurst(0, burs);
            }
            ps.Play();
            anim.startFrame = i / (float)anim.numTilesX;
            Instantiate(ps, transform.position, ps.transform.rotation);
        }
    }
}