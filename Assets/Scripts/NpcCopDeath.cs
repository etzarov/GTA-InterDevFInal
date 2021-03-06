﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcCopDeath : MonoBehaviour
{

    [SerializeField] int health;


    // Start is called before the first frame update
    void Start()
    {
        //set up the health of this NPC
        //health = this.GetComponent<NpcCivPersonalityManager>().personality.health;
       
    }


    /// <summary>
    /// Reduces the health of the NPC for when they get shot. Takes in the damage dealt. After damage is dealt, checks if the NPC dies. If they are still alive, change their emotion to scared.
    /// </summary>
    /// <param name="damage">Damage.</param>
    public void ReduceHealth(int damage)
    {
        health -= damage;
        CheckForDeath();
      
    }

    /// <summary>
    /// Checks for the death of the NPC.
    /// </summary>
    private bool CheckForDeath()
    {
        if (health <= 0)
        {
            NpcCivManager.Instance.RemoveNpc(this.gameObject);
            this.gameObject.GetComponent<NpcCopMoveWalk>().enabled = false;
            this.gameObject.GetComponent<NpcCopShoot>().enabled = false;
            this.gameObject.transform.Translate(new Vector3(0, -1, 0));
            this.gameObject.transform.Rotate(new Vector3(70, 20, 0));
            //ScoreManager.Instance.IncreaseScore(10);
            NpcCopManager.Instance.RemoveNpc(this.gameObject);
            NpcCopManager.Instance.CopDeath();
            Invoke("StopForces", .5f);
            Invoke("Despawn", 10f);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Stops the forces acting on the NPC.
    /// </summary>
    private void StopForces()
    {
        this.gameObject.GetComponent<Collider>().enabled = false;
        this.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    private void Despawn ()
    {
        if (Vector3.Distance(this.transform.position, PlayerManager.Instance.gameObject.transform.position) > 30f)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Invoke("Despawn", 10f);
        }
    }
}
