using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class PlayerBehavior : MonoBehaviour
{
    [Tooltip("A number = to -1, 0, or 1. NOT SPEED.")] public float AccelerationNum = 0;
    [Tooltip("Sensitivity of steering")] public float Sensitivity = 1.0f;
    private Controls controls;
    private Vector3 movement, moveDirection;
    private Rigidbody rb;
    private bool AccelerationOn = false, DecelerationOn = false;
    public float Energy, CastDistance = 1f, Speed = 60, TopSpeed = 0;

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
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();
        SteerPlayer();
        isOnEnergyPad();
    }
    public bool isOnEnergyPad()
    {
        RaycastHit hit;
        if(Physics.Raycast(transform.position, Vector3.down, out hit, CastDistance))
        {
            print("ONPAD");
            return true;
        }
        else
        {
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
            wheel.wheelCollider.motorTorque = AccelerationNum * Speed;
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
        AccelerationNum = 1;
        print("ACCON");
    }
    public void AccelerateOff()
    {
        AccelerationNum = 0;
        print("ACCOFF");
    }
    public void DecelerateOn()
    {
        AccelerationNum = -1;
        print("DECON");
    }
    public void DecelerateOff()
    {
        AccelerationNum = 0;
        print("DECOFF");
    }
    public void Increase()
    {
        print("+1");
    }
    public void Decrease()
    {
        print("-1");
    }
}
