using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour
{
    private Controls controls;
    private Vector3 movement;
    // Start is called before the first frame update
    void Start()
    {
        controls = new Controls();
        controls.ControllerMap.Enable();

        controls.ControllerMap.Move.performed += ctx => movement = ctx.ReadValue<Vector2>();
        controls.ControllerMap.Increase.performed += ctx => Increase();
        controls.ControllerMap.Decrease.performed += ctx => Decrease();
    }

    // Update is called once per frame
    void Update()
    {
        print(movement.x + ", " + movement.z);
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
