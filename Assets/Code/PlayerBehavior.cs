using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine.InputSystem;
using UnityEngine.ProBuilder.MeshOperations;

public class PlayerBehavior : MonoBehaviour
{
    //https://gist.github.com/VanshMillion/9d69fc11f4bb3899ee779e23e7b34abb
    //https://www.youtube.com/watch?v=jr4eb4F9PSQ&list=PLyh3AdCGPTSLg0PZuD1ykJJDnC1mThI42
    [Tooltip("A lower number equals a lower rate of turning")] public float Sensitivity = 1.0f;
    [Tooltip("A lower number equals a higher rate of energy change. Suggest numbers smaller than 3.000")] public float RateOfEnergyGain = .25f, RateOfEnergyLoss = 1f;
    [Tooltip("A lower number equals a lower maximum speed without energy.")] public float NoEnergySpeed = .25f;


    public Controls controls;
    private Rigidbody rb;
    private bool isCollectingEnergy = true, canAttack=true;
    private float ForwardVal = 0, ReverseVal = 0, energyToRemove = 1, energyToAdd = 1, maxEnergy = 100f, minEnergy = 1f, excessEnergy = 0, steerValue = 0;


    public LayerMask layerMask;
    public float DisplaySpeedMultiplier = 10, MaxSteerAngle = 25f, Power = 75, detectEPadCastDistance = 2f, CurrentEnergy = 99f, CurrentSpeed = 0, BrakePower = 50f, MaxSpeed = 237f, SpeedEnergyMod = 1, ShieldEnergyMod = 1, AttackEnergyMod = 1;
    public Action<float, int> EnergyUpdated, SpeedUpdated;
    public Action<int> SelectAttack, SelectShield, SelectSpeed, SelectRight, SelectLeft;
    public PlayerInput PI;

    private UIController[] uiControllers;
    private UIController MyUI;
    private Animator animator;

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

        uiControllers = FindObjectsOfType<UIController>(true);

        for (int i = 0; i < uiControllers.Length; i++)
        {
            if (PI.playerIndex == uiControllers[i].ID)
            {
                uiControllers[i].GetComponent<UIController>().GetUIMOD += HandleUIChange;
                uiControllers[i].playerBehavior = this;
                uiControllers[i].InitUI();
            }
        }
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        
        /*
        controls.ControllerMap.Move.performed += ctx => steerValue = ctx.ReadValue<float>();
        controls.ControllerMap.Move.canceled += ctx => steerValue = 0;
        controls.ControllerMap.Accelerate.started += ctx => AccelerateOn();
        controls.ControllerMap.Accelerate.canceled += ctx => AccelerateOff();
        controls.ControllerMap.Decelerate.started += ctx => DecelerateOn();
        controls.ControllerMap.Decelerate.canceled += ctx => DecelerateOff();
        controls.ControllerMap.SelectSpeed.performed += ctx => SelectSpeed(PI.playerIndex);
        controls.ControllerMap.SelectAttack.performed += ctx => SelectAttack(PI.playerIndex);
        controls.ControllerMap.SelectShield.performed += ctx => SelectShield(PI.playerIndex);
        controls.ControllerMap.Decrease.performed += ctx => SelectLeft(PI.playerIndex);
        controls.ControllerMap.Increase.performed += ctx => SelectRight(PI.playerIndex);
        controls.ControllerMap.Quit.performed += ctx => Quit();
        */
        StartCoroutine(CalcSpeed());
        if (PI.playerIndex == 0)
        {
            tag = "Player1";
        }
        else if (PI.playerIndex == 1)
        {
            tag = "Player2";
        }
    }

    public void activateControls(bool isActive)
    {
        if (isActive && uiControllers != null)
        {
            PI.currentActionMap.FindAction("Move").performed += ctx => steerValue = ctx.ReadValue<float>();
            PI.currentActionMap.FindAction("Move").canceled += ctx => steerValue = 0;
            PI.currentActionMap.FindAction("Accelerate").started += ctx => AccelerateOn();
            PI.currentActionMap.FindAction("Accelerate").canceled += ctx => AccelerateOff();
            PI.currentActionMap.FindAction("Decelerate").started += ctx => DecelerateOn();
            PI.currentActionMap.FindAction("Decelerate").canceled += ctx => DecelerateOff();
            PI.currentActionMap.FindAction("SelectSpeed").performed += ctx => SelectSpeed(PI.playerIndex);
            PI.currentActionMap.FindAction("SelectAttack").performed += ctx => SelectAttack(PI.playerIndex);
            PI.currentActionMap.FindAction("SelectShield").performed += ctx => SelectShield(PI.playerIndex);
            PI.currentActionMap.FindAction("Decrease").performed += ctx => SelectLeft(PI.playerIndex);
            PI.currentActionMap.FindAction("Increase").performed += ctx => SelectRight(PI.playerIndex);
            PI.currentActionMap.FindAction("Quit").performed += ctx => Quit();
            PI.currentActionMap.FindAction("Activate").performed += ctx => Activate();
        }
    }
    void Update()
    {
        MovePlayer();
        SteerPlayer();
        isOnEnergyPad();
        Brake();
    }
    /// <summary>
    /// When the player changes a modifier, the UIBehavior script will use this method to let the 
    /// PlayerBehavior know a value was changed. In the UIScript, it will pass in what modifier 
    /// the PlayerBehavior needs to change and to what value. Theoretically, since this is being
    /// called every time a change is made, modifier will only incrementally change +1/-1, but it 
    /// will still accomodate if game devs wish for instant modifier change, i.e., from 1=>5
    /// </summary>
    void Activate()
    {
        if(canAttack)
        {
            canAttack= false;
            animator.SetTrigger("Attack");
            //ATTACK SOUND HERE
        }
    }
    public void anim_SetCanAttack()
    {
        print("HERE");
        StartCoroutine(HoldUp());
    }
    IEnumerator HoldUp()
    {
        yield return new WaitForSeconds(2f/(AttackEnergyMod+1f));
        canAttack = true;
    }
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
            }
            return false;
        }
    }
    void SteerPlayer()
    {
        foreach (Wheel wheel in wheels)
        {
            if (wheel.AxleType == Axle.Front)
            {
                var steerAngle = steerValue * Sensitivity * MaxSteerAngle; //* (1 - (CurrentSpeed / MaxSpeed));
                var finalAngle = Mathf.Lerp(wheel.wheelCollider.steerAngle, steerAngle, 0.6f);
                //Inverse relationship; as current speed increases, 25 will be multiplied by a smaller decimal to get a smaller angle. (To make it harder to accidently oversteer at high speed)       
                //Lerp is included so that steering isn't instantanious
                wheel.wheelCollider.steerAngle = finalAngle;
                //wheel.wheelModel.transform.localEulerAngles = new Vector3(wheel.wheelModel.transform.eulerAngles.x, finalAngle, wheel.wheelModel.transform.eulerAngles.z);

            }
        }
        foreach (Wheel wheel in wheels)
        {
            Quaternion rot;
            Vector3 pos;
            wheel.wheelCollider.GetWorldPose(out pos, out rot);
            wheel.wheelModel.transform.position = pos;
            wheel.wheelModel.transform.rotation = rot;
        }
    }
    void MovePlayer()
    {   
        foreach(Wheel wheel in wheels) 
        {
            if(CurrentEnergy > minEnergy && CurrentSpeed < MaxSpeed)
            {
                wheel.wheelCollider.motorTorque = ((ForwardVal + ReverseVal) * Power * SpeedEnergyMod) + ((ForwardVal + ReverseVal) * (excessEnergy * 100f));
                // Acceleration (1,0, or -1) * Power (Designer modifier for more speed) * SEM (# between 1-5) + ~25 (about what 3/5 speed is)
                //print(wheel.wheelCollider.motorTorque);
            }
            else if (CurrentEnergy <= minEnergy)
            {
                wheel.wheelCollider.motorTorque = (ForwardVal * Power * SpeedEnergyMod)*NoEnergySpeed;
                //If you dont have enough energy, this else will allow the car at least some speed
            }   
            else
            {
                wheel.wheelCollider.motorTorque = 0;
            }
        }  
    }
    void Brake()
    {
        if (CurrentSpeed/DisplaySpeedMultiplier > 10f && ReverseVal < 0)
        {
            foreach (Wheel wheel in wheels)
            {
                wheel.wheelCollider.brakeTorque = (BrakePower * 100);
                //print("BeingApplied" + wheel.wheelCollider.brakeTorque);
            }
        }
        else if (CurrentSpeed/DisplaySpeedMultiplier < 10f)
        {
            foreach (Wheel wheel in wheels)
            {
                wheel.wheelCollider.brakeTorque = 0;
                //print("NOTBEINGAPPLIED" + wheel.wheelCollider.brakeTorque);
            }
        }
    }
    
    public void ChangeEnergy(float energy)
    {
        CurrentEnergy = Mathf.Clamp(CurrentEnergy+energy, 1, 99);
    }
    
    /*
    public IEnumerator ChangeEnergy(float energyChange)
    {
        float changeDuration = .5f;
        float startEnergy = CurrentEnergy;
        float targetEnergy = Mathf.Clamp(CurrentEnergy + energyChange, minEnergy, maxEnergy);
        float elapsedTime = 0f;
        //print(targetEnergy);
        while (elapsedTime < changeDuration)
        {
            CurrentEnergy = Mathf.Lerp(startEnergy, targetEnergy, elapsedTime / changeDuration);
            elapsedTime += Time.deltaTime;
            print(elapsedTime);
            yield return null;
        }
        print("DONE");
        CurrentEnergy = targetEnergy;
    }
    */
    IEnumerator CalcSpeed()
    {
        while(true)
        {
            Vector3 prevPos = transform.position;
            yield return new WaitForFixedUpdate();
            CurrentSpeed = DisplaySpeedMultiplier * ((float)Math.Round((Vector3.Distance(transform.position, prevPos) / Time.deltaTime), 1));
            SpeedUpdated?.Invoke(CurrentSpeed, PI.playerIndex);
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
            EnergyUpdated?.Invoke(CurrentEnergy, PI.playerIndex);
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
            EnergyUpdated?.Invoke(CurrentEnergy, PI.playerIndex);

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
        ForwardVal = 1;
        //print("ACCON");
    }
    public void AccelerateOff()
    {
        ForwardVal = 0;
        //print("ACCOFF");
    }
    public void DecelerateOn()
    {
        ReverseVal = -1;
        //print("DECON");
    }
    public void DecelerateOff()
    {
        ReverseVal = 0;
        //print("DECOFF");
    }
    public void Quit()
    {
        Application.Quit();
        EditorApplication.ExitPlaymode();
    }
    /// <summary>
    /// We need to unassaign the actions to avoid errors when loading new scenes
    /// </summary>
    public void OnDestroy()
    {
        /*
        controls.ControllerMap.Move.performed -= ctx => steerValue = ctx.ReadValue<float>();
        controls.ControllerMap.Move.canceled -= ctx => steerValue = 0;
        controls.ControllerMap.Increase.performed -= ctx => SelectRight(PI.playerIndex);
        controls.ControllerMap.Decrease.performed -= ctx => SelectLeft(PI.playerIndex);
        controls.ControllerMap.Accelerate.started -= ctx => AccelerateOn();
        controls.ControllerMap.Accelerate.canceled -= ctx => AccelerateOff();
        controls.ControllerMap.Decelerate.started -= ctx => DecelerateOn();
        controls.ControllerMap.Decelerate.canceled -= ctx => DecelerateOff();
        controls.ControllerMap.SelectSpeed.performed -= ctx => SelectSpeed(PI.playerIndex);
        controls.ControllerMap.SelectAttack.performed -= ctx => SelectAttack(PI.playerIndex);
        controls.ControllerMap.SelectShield.performed -= ctx => SelectShield(PI.playerIndex);
        controls.ControllerMap.Quit.performed -= ctx => Quit();
        
        PI.currentActionMap.FindAction("Move").performed -= ctx => steerValue = ctx.ReadValue<float>();
        PI.currentActionMap.FindAction("Move").canceled -= ctx => steerValue = 0;
        PI.currentActionMap.FindAction("Accelerate").started -= ctx => AccelerateOn();
        PI.currentActionMap.FindAction("Accelerate").canceled -= ctx => AccelerateOff();
        PI.currentActionMap.FindAction("Decelerate").started -= ctx => DecelerateOn();
        PI.currentActionMap.FindAction("Decelerate").canceled -= ctx => DecelerateOff();
        PI.currentActionMap.FindAction("SelectSpeed").performed -= ctx => SelectSpeed(PI.playerIndex);
        PI.currentActionMap.FindAction("SelectAttack").performed -= ctx => SelectAttack(PI.playerIndex);
        PI.currentActionMap.FindAction("SelectShield").performed -= ctx => SelectShield(PI.playerIndex);
        PI.currentActionMap.FindAction("Decrease").performed -= ctx => SelectLeft(PI.playerIndex);
        PI.currentActionMap.FindAction("Increase").performed -= ctx => SelectRight(PI.playerIndex);
        PI.currentActionMap.FindAction("Quit").performed -= ctx => Quit();
        for (int i = 0; i < uiControllers.Length; i++)
        {
            uiControllers[i].GetUIMOD -= HandleUIChange;
        }
        */
    }
}