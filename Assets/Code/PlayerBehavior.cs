using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour
{
    private Controls controls;
    private Vector3 movement, speed;
    private float topSpeed;
    // Start is called before the first frame update
    void Start()
    {
        controls = new Controls();
        controls.ControllerMap.Enable();

        controls.ControllerMap.Move.performed += ctx => movement = ctx.ReadValue<Vector2>();
        controls.ControllerMap.Increase.performed += ctx => Increase();
        controls.ControllerMap.Decrease.performed += ctx => Decrease();
        //controls.ControllerMap.Accelerate.performed += ctx => speed = ctx.ReadValue<Vector2>();
       controls.ControllerMap.Accelerate.performed += ctx => Accelerate();
       controls.ControllerMap.Decelerate.performed += ctx => Decelerate();
    }

    // Update is called once per frame
    void Update()
    {
        //print(movement.x + ", " + movement.z);
        print(speed);
    }
    void Accelerate()
    {
        print("+2");
    }
    void Decelerate()
    {
        print("-2");
    }
    void Increase()
    {
        print("+1");
    }
    void Decrease()
    {
        print("-1");
    }
}
