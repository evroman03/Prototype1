using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class PlayerBehavior : MonoBehaviour
{
    //https://gist.github.com/VanshMillion/9d69fc11f4bb3899ee779e23e7b34abb
    //https://www.youtube.com/watch?v=jr4eb4F9PSQ&list=PLyh3AdCGPTSLg0PZuD1ykJJDnC1mThI42
    [Tooltip("A number = to -1, 0, or 1. NOT SPEED. Determines forward/backward")] public float AccelerationVal = 0;
    [Tooltip("A lower number equals a lower rate of turning")] public float Sensitivity = 1.0f;
    [Tooltip("A lower number equals a higher rate of energy change. Suggest numbers smaller than 3.000")] public float RateOfEnergyGain = .25f, RateOfEnergyLoss=1f;
    [Tooltip("A lower number equals a lower maximum speed without energy.")] public float NoEnergySpeed = .1f;


    private Controls controls;
    private Vector3 movement;
    private Rigidbody rb;
    private bool isCollectingEnergy = true;
    private float energyToRemove = 1, energyToAdd = 1, maxEnergy = 100f, minEnergy = 1f, detectEPadCastDistance = 2f, excessEnergy=0;


    public LayerMask layerMask;
    public float CurrentEnergy = 99f, CurrentSpeed = 0, Power = 75, SpeedEnergyMod = 1, ShieldEnergyMod = 1, AttackEnergyMod = 1;
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
        UIController.GetUIMOD += HandleUIChange;
        rb = GetComponent<Rigidbody>();
        controls = new Controls();
        controls.ControllerMap.Enable();
        controls.ControllerMap.Move.performed += ctx => movement = ctx.ReadValue<Vector2>();
        controls.ControllerMap.Move.canceled += ctx => movement = ctx.ReadValue<Vector2>();
        controls.ControllerMap.Increase.performed += ctx => SelectRight();
        controls.ControllerMap.Decrease.performed += ctx => SelectLeft();
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
    /// <summary>
    /// When the player changes a modifier, the UIBehavior script will use this method to let the 
    /// PlayerBehavior know a value was changed. In the UIScript, it will pass in what modifier 
    /// the PlayerBehavior needs to change and to what value. Theoretically, since this is being
    /// called every time a change is made, modifier will only incrementally change +1/-1, but it 
    /// will still accomodate if game devs wish for instant modifier change, i.e., from 1=>5
    /// </summary>
    public void HandleUIChange(int modSelector, int modifier)
    {
        switch (modSelector)
        {
            case 0:
                SpeedEnergyMod = modifier;
                break;
            case 1:
                ShieldEnergyMod = modifier;
                break;
            case 2:
                AttackEnergyMod = modifier;
                break;
        }
    }
    public bool isOnEnergyPad()
    {
        RaycastHit hit;
        if(Physics.Raycast(transform.position, Vector3.down, out hit, detectEPadCastDistance, layerMask))
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
                print("HERE2");
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
            if(CurrentEnergy > minEnergy)
            {
                wheel.wheelCollider.motorTorque = (AccelerationVal * Power * SpeedEnergyMod) + (excessEnergy*100f);
                // Acceleration (1,0, or -1) * Power (Designer modifier for more speed) * SEM (# between 1-5) + ~250 (about what 3/5 speed is)
            }
            else
            {
                wheel.wheelCollider.motorTorque = (AccelerationVal * Power * SpeedEnergyMod)*NoEnergySpeed;
                //If you dont have enough energy, this else will allow the car at least some speed
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
            if((CurrentEnergy+energy)<=maxEnergy)
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
            if((CurrentEnergy-energy) >= minEnergy)
            {
                CurrentEnergy -= energy;
            }
            EnergyUpdated?.Invoke(CurrentEnergy);

            float noBaseROEL = ((SpeedEnergyMod * .25f) + (ShieldEnergyMod * .25f) + (AttackEnergyMod * .25f)); //This variable gets bigger as mods go up
            float totalROEL = RateOfEnergyLoss / noBaseROEL;                                                    //This variable gets smaller as mods go up
            float finalROEL = Mathf.Clamp(totalROEL, 0.1f, 1f);                                                 //This variable makes sure the rate isn't obnoxious

            //If the mods are all about 1 then a little bonus will be given to the player's top 
            //speed. If they are higher, when added together the result of the following if 
            //will be negative. Flavortext, easy to take out
            if (1f- noBaseROEL>=0) 
            {
                excessEnergy = (float)Math.Round((1-noBaseROEL), 2);
            }
            yield return new WaitForSeconds(finalROEL);
            //The higher these modifiers, the smaller the fraction ROEL/ESM will be, resulting in a
            //faster tick speed
        }
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
    public void SelectRight()
    {
        print("+1");
    }
    public void SelectLeft()
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
        controls.ControllerMap.Increase.performed -= ctx => SelectRight();
        controls.ControllerMap.Decrease.performed -= ctx => SelectLeft();
        controls.ControllerMap.Accelerate.started -= ctx => AccelerateOn();
        controls.ControllerMap.Accelerate.canceled -= ctx => AccelerateOff();
        controls.ControllerMap.Decelerate.started -= ctx => DecelerateOn();
        controls.ControllerMap.Decelerate.canceled -= ctx => DecelerateOff();
        controls.ControllerMap.SelectSpeed.performed -= ctx => SelectSpeed();
        controls.ControllerMap.SelectAttack.performed -= ctx => SelectAttack();
        controls.ControllerMap.SelectShield.performed -= ctx => SelectShield();
    }
}
