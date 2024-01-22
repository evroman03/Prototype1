using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour
{
    private Controls controls;
    private Vector3 movement, speed;
    private float topSpeed;
    private bool on = true;
    public float Energy;
    // Start is called before the first frame update
    void Start()
    {
        controls = new Controls();
        controls.ControllerMap.Enable();

        controls.ControllerMap.Move.performed += ctx => movement = ctx.ReadValue<Vector2>();
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
        if(on)
        {
            StartCoroutine(PrintSpeed());
            on= false;
        }
        

        //
        //print(speed);
    }
    IEnumerator PrintSpeed()
    {
        print(movement.x + ", " + movement.z);
        yield return new WaitForSeconds(.5f);
        on = true;
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
        print("ACCON");
    }
    public void AccelerateOff()
    {
        print("ACCOFF");
    }
    public void DecelerateOn()
    {
        print("DECON");
    }
    public void DecelerateOff()
    {
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
