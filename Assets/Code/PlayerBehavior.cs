using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class PlayerBehavior : MonoBehaviour
{
    [Tooltip("A number = to -1, 0, or 1. NOT SPEED. Determines forward/backward")] public float AccelerationVal = 0;
    [Tooltip("A lower number equals a lower rate of turning")] public float Sensitivity = 1.0f;
    [Tooltip("A lower number equals a higher rate of energy change")] public float RateOfEnergyGain = .25f, RateOfEnergyLoss=1f;
    [Tooltip("A lower number equals a lower maximum speed w/out energy.")] public float NoEnergySpeed = .1f;

    private Controls controls;
    private Vector3 movement;
    private Rigidbody rb;
    private bool isCollectingEnergy = true;
    private float energyToRemove = 1, energyToAdd = 1, MaxEnergy = 100f, MinEnergy = 1f, DetectEPadCastDistance = 2f;


    public LayerMask layerMask;
    public float CurrentEnergy = 2f, CurrentSpeed = 0, Power = 60, EnergySpeedMod=1;
    public static Action<float> EnergyUpdated;

  
    /// <summary>
    /// We need the enum (named integer) do diffrentiate between front and rear so steering 
    /// can be applied correctly
    /// </summary>
    public enum Axle
    {
        Front,
        Rear
    }
    /// <summary>
    /// A struct (structure) is a custom datatype. It allows you to make a list of wheels.
    /// It NEEDS to be serializeable so that we can see our custom datatype in the inspector.
    /// </summary>
    [Serializable]
    public struct Wheel
    {
        public GameObject wheelModel;
        public WheelCollider wheelCollider;
        public Axle AxleType;
    }
    public List<Wheel> wheels; 


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        controls = new Controls();
        controls.ControllerMap.Enable();

        controls.ControllerMap.Move.performed += ctx => movement = ctx.ReadValue<Vector2>();
        controls.ControllerMap.Move.canceled += ctx => movement = ctx.ReadValue<Vector2>();
        controls.ControllerMap.Increase.performed += ctx => Increase();
        controls.ControllerMap.Decrease.performed += ctx => Decrease();
        controls.ControllerMap.Accelerate.started += ctx => AccelerateOn();
        controls.ControllerMap.Accelerate.canceled += ctx => AccelerateOff();
        controls.ControllerMap.Decelerate.started += ctx => DecelerateOn();
        controls.ControllerMap.Decelerate.canceled += ctx => DecelerateOff();
        controls.ControllerMap.SelectSpeed.performed += ctx => SelectSpeed();
        controls.ControllerMap.SelectAttack.performed += ctx => SelectAttack();
        controls.ControllerMap.SelectShield.performed += ctx => SelectShield();


        StartCoroutine(CalcSpeed());
    }
    void Update()
    {
        MovePlayer();
        SteerPlayer();
        isOnEnergyPad();
    }
    public bool isOnEnergyPad()
    {
        RaycastHit hit;
        if(Physics.Raycast(transform.position, Vector3.down, out hit, DetectEPadCastDistance, layerMask))
        {
            if(!isCollectingEnergy)
            {
                isCollectingEnergy = true;
                StartCoroutine(AddEnergy(energyToAdd));
            }
            return true;
        }
        else
        {
            if(isCollectingEnergy)
            {
                isCollectingEnergy = false;
                StartCoroutine(RemoveEnergy(energyToRemove));
            }
            return false;
        }
    }
    void SteerPlayer()
    {
        foreach (var wheel in wheels)
        {
            if (wheel.AxleType == Axle.Front)
            {
                var steerAngle = movement.x * Sensitivity * 30f;
                wheel.wheelCollider.steerAngle = Mathf.Lerp(wheel.wheelCollider.steerAngle, steerAngle, 0.6f);
            }
        }
    }
    void MovePlayer()
    {   
        foreach(Wheel wheel in wheels) 
        {
            if(CurrentEnergy > MinEnergy)
            {
                wheel.wheelCollider.motorTorque = AccelerationVal * Power * EnergySpeedMod;
            }
            else
            {
                wheel.wheelCollider.motorTorque = (AccelerationVal * Power * EnergySpeedMod)*NoEnergySpeed;
                //If you dont have enough energy,
            }
            
        }
        
    }
    IEnumerator CalcSpeed()
    {
        while(true)
        {
            Vector3 prevPos = transform.position;
            yield return new WaitForFixedUpdate();
            CurrentSpeed = (float)Math.Round((Vector3.Distance(transform.position, prevPos) / Time.deltaTime), 0);
        }     
    }
    IEnumerator AddEnergy(float energy)
    {
        while(isCollectingEnergy)
        {
            if((CurrentEnergy+energy)<=MaxEnergy)
            {
                CurrentEnergy += energy;
            }
            EnergyUpdated?.Invoke(CurrentEnergy);
            yield return new WaitForSeconds(RateOfEnergyGain);
        }
    }
    IEnumerator RemoveEnergy(float energy)
    {
        while (!isCollectingEnergy)
        {
            if((CurrentEnergy-energy) >= MinEnergy)
            {
                CurrentEnergy -= energy;
            }
            EnergyUpdated?.Invoke(CurrentEnergy);
            yield return new WaitForSeconds(RateOfEnergyLoss / (Mathf.Clamp((EnergySpeedMod*0.5f), 1, 2)));
            //The higher the SpeedMod, the smaller the fraction ROEL/ESM will be, resulting in a 
            //faster tick speed
        }
    }
    public void SelectSpeed()
    {
        print("SPDSLCT");
    }
    public void SelectAttack()
    {
        print("ATKSLCT");
    }
    public void SelectShield()
    {
        print("SHLDSLCT");
    }
    public void AccelerateOn()
    {
        AccelerationVal = 1;
        //print("ACCON");
    }
    public void AccelerateOff()
    {
        AccelerationVal = 0;
        //print("ACCOFF");
    }
    public void DecelerateOn()
    {
        AccelerationVal = -1;
        //print("DECON");
    }
    public void DecelerateOff()
    {
        AccelerationVal = 0;
        //print("DECOFF");
    }
    public void Increase()
    {
        print("+1");
    }
    public void Decrease()
    {
        print("-1");
    }
    /// <summary>
    /// We need to unassaign the actions to avoid errors when loading new scenes
    /// </summary>
    public void OnDestroy()
    {
        controls.ControllerMap.Move.performed -= ctx => movement = ctx.ReadValue<Vector2>();
        controls.ControllerMap.Move.canceled -= ctx => movement = ctx.ReadValue<Vector2>();
        controls.ControllerMap.Increase.performed -= ctx => Increase();
        controls.ControllerMap.Decrease.performed -= ctx => Decrease();
        controls.ControllerMap.Accelerate.started -= ctx => AccelerateOn();
        controls.ControllerMap.Accelerate.canceled -= ctx => AccelerateOff();
        controls.ControllerMap.Decelerate.started -= ctx => DecelerateOn();
        controls.ControllerMap.Decelerate.canceled -= ctx => DecelerateOff();
        controls.ControllerMap.SelectSpeed.performed -= ctx => SelectSpeed();
        controls.ControllerMap.SelectAttack.performed -= ctx => SelectAttack();
        controls.ControllerMap.SelectShield.performed -= ctx => SelectShield();
    }
}
