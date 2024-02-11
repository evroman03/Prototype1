using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackScript : MonoBehaviour
{
    private PlayerBehavior pbSource;
    private PlayerBehavior pbTarget;
    private float attackPower, targetShield;

    void OnEnable()
    {
        pbSource = transform.root.gameObject.GetComponent<PlayerBehavior>();
        attackPower = pbSource.AttackEnergyMod;
    }
    void Beam()
    {
        pbTarget.ChangeEnergy(attackPower * (-10f / targetShield));
        pbSource.ChangeEnergy(10f/targetShield);
        //StartCoroutine(pbSource.ChangeEnergy(attackPower * (-10f / targetShield)));
        //StartCoroutine(pbSource.ChangeEnergy((10f / targetShield)));
    }
    private void OnTriggerEnter(Collider other)
    {
        if ((other.gameObject.CompareTag("Player1")) || (other.gameObject.CompareTag("Player2")))
        {
            if(pbTarget != null)
            {
                Beam();
            }
            else
            {
                pbTarget = other.gameObject.GetComponent<PlayerBehavior>();
                targetShield = pbTarget.ShieldEnergyMod;
                Beam();
            }
        }
    }
}
