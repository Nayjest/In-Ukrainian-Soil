using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Player : Singleton<Player>
{
    public KeyCode AccelerationKey = KeyCode.UpArrow;

    public Transform Direction;

    public float RotSpeed = 3;
    public float Acceleration = 50;
    public float VelocityToAccelerationPower = 0.17f;

    [InfoBox("Percent of acceleration applied when player dont pressing acceleration (in addition to 100%)")]
    [Range(0,1)]
    public float PassiveMovementPercent = 0.15f;


    [BoxGroup("Burst Energy")]
    [ShowNonSerializedField]
    float burstEnergy = 0.5f;

    [BoxGroup("Burst Energy")]
    [FormerlySerializedAs("BurstSpeedGain")]
    public float VelocityGainPerBurst = 1 / 3f;

    [BoxGroup("Burst Energy")]
    public float MaxBurstEnergy = 3;

    // how much seconds takes regeneration of first, second, etc portion of burst energy
    [BoxGroup("Burst Energy")]
    public float[] BurstRegenTimeTable = new float[] { 0.8f, 1.6f, 3.2f };
    
    [BoxGroup("Burst Energy")]
    [Range(0, 1)]
    public float NoEnergyBurstRegenPercent = 0.1f;

    [BoxGroup("Burst Energy")]
    public float AccelerationEnergyConsumptionByBurst = 0.15f;

    public float BurstEnergyPercent => burstEnergy / MaxBurstEnergy;

    




    [BoxGroup("Acceleration Energy")]
    public float MaxAccelerationEnergy = 1;

    [BoxGroup("Acceleration Energy")]
    public float AccelerationEnergyConsumptionTime = 10f;
    [BoxGroup("Acceleration Energy")]
    public float AccelerationEnergyRegenTime = 5f;
    [BoxGroup("Acceleration Energy")]
    [Range(0, 1)]
    public float NoEnergyMovementPercent = 0.3f;

    [BoxGroup("Acceleration Energy")]
    [ShowNonSerializedField]
    float accelerationEnergy = 1;
       
    public float AccelerationEnergyPercent => accelerationEnergy / MaxAccelerationEnergy;


    [ShowNonSerializedField]
    bool isAccelerating = false;


    
    public float LifePower = 1;
    [ShowNativeProperty]
    public float Timer => LifePower * LifeDecayTime;
    public float LifeDecayTime = 50;
    public float CrystalEnergyRestore = 5 / 30f;
    int restorationCoroutinesQty = 0;
    public bool IsRestoringLifepower => restorationCoroutinesQty > 0;

    int score = 0;

    [ShowNativeProperty]
    public int Score => score;

    public void FindSoul()
    {
        SFXManager.Inst.Play("SFX.Soul");
        score++;
        StartCoroutine(IERestoreLifePower());
    }


    public void ConsumeLifePowerRegen()
    {
        SFXManager.Inst.Play("SFX.Gem");
        StartCoroutine(IERestoreLifePower());
    }

    private IEnumerator IERestoreLifePower()
    {
        restorationCoroutinesQty++;
        float t = 0;
        float dur = 0.3f;
        float i1 = 0;
        float i2 = 0;
        while (t < dur)
        {
            t += Time.deltaTime;
            i2 = Mathf.Pow(t / dur, 0.5f);
            LifePower += (i2 - i1) * CrystalEnergyRestore;
            i1 = i2;
            yield return new WaitForEndOfFrame();            
        }
        restorationCoroutinesQty--;
        yield return null;
    }


    public Rigidbody rb;

    [ShowNativeProperty]
    public float CurrentVelocity => rb.velocity.magnitude;


    private void Reset()
    {
        rb = GetComponent<Rigidbody>();
        Direction = GetComponentInChildren<MouseLook>()?.transform;
    }
    void Start()
    {
        
    }
    private void OnCollisionStay(Collision collision)
    {
        rb.angularVelocity = Vector3.zero;
        rb.MoveRotation(Direction.rotation);
    }

    private void OnCollisionEnter(Collision collision)
    {
        
        
    }

    float Speed => (Acceleration + Mathf.Pow(rb.velocity.magnitude + 1, VelocityToAccelerationPower) - 1);

    

    // Update is called once per frame
    void FixedUpdate()
    {
        UpdateMovement();
        if (!isAccelerating) RegenAccelerationEnergy();
        RegenBurstEnergy();

        LifePower -= Time.fixedDeltaTime / LifeDecayTime;
        if (LifePower <= 0) Game.Inst.GameOver();
    }

    void UpdateMovement()
    {
        //rb.MovePosition(rb.position + Direction.forward * Time.fixedDeltaTime * MovSpeed);
        //rb.velocity = transform.forward * curMovspeed;
        var rot = Direction.rotation;
        rb.MoveRotation(Quaternion.Lerp(rb.rotation, rot, Time.fixedDeltaTime * RotSpeed));

        var speed = Speed * Time.fixedDeltaTime;
        rb.velocity += PassiveMovementPercent * speed * Direction.forward;
        /*
        if (Input.GetKeyDown(AccelerationKey) && !isAccelerating)
        {
            StartAcceleration();
        }
        if (Input.GetKeyUp(AccelerationKey) && isAccelerating)
        {
            StopAcceleration();
        }
        */

        if (isAccelerating)
        {
            var aeConsumed = Time.fixedDeltaTime / AccelerationEnergyConsumptionTime;            
            var speedGain = speed * NoEnergyMovementPercent;
            
            accelerationEnergy -= aeConsumed;
            if (accelerationEnergy < 0)
            {
                accelerationEnergy = 0;
                
            }
            speedGain += speed * (1 - NoEnergyMovementPercent) * AccelerationEnergyPercent;
            if (speedGain > 0)
            {
                rb.velocity += speedGain * Direction.forward;
            }
        }
    }
    void RegenBurstEnergy()
    {
        if (burstEnergy >= MaxBurstEnergy) return;
        int index = Mathf.FloorToInt(burstEnergy);
        if (index >= BurstRegenTimeTable.Length) index = BurstRegenTimeTable.Length - 1;
        var regenTime = BurstRegenTimeTable[index];
        var gain = Time.fixedDeltaTime / regenTime * (NoEnergyBurstRegenPercent + (1-NoEnergyBurstRegenPercent) * AccelerationEnergyPercent);
        burstEnergy += gain;                
    }
    void RegenAccelerationEnergy()
    {
        if (accelerationEnergy > MaxAccelerationEnergy) return;
        var gain = Time.fixedDeltaTime / AccelerationEnergyRegenTime;
        accelerationEnergy += gain;        
    }

    public void StartAcceleration()
    {
        isAccelerating = true;
        var maxBurstSpeed = Speed * VelocityGainPerBurst;
        float burstSpeed = maxBurstSpeed;
        if (burstEnergy < 1)
        {
            burstSpeed *= burstEnergy / 1f;
            burstEnergy = 0;
        } else {
            burstEnergy -= 1;
        }
        accelerationEnergy -= AccelerationEnergyConsumptionByBurst;
        if (accelerationEnergy < 0) accelerationEnergy = 0;
        rb.velocity += burstSpeed * Direction.forward;
    }

    public void StopAcceleration()
    {
        isAccelerating = false;
    }
}
