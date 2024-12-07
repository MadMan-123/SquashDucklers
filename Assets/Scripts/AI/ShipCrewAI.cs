using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SocialPlatforms.Impl;

public class ShipCrewAI : MonoBehaviour
{
    public NavMeshAgent agent;
    public bool walking;
    public bool gocannon;
    public Transform posA;
    public Transform posB;


    private void Awake()
    {
        walking = true;
  CanonBall();
        agent = GetComponent<NavMeshAgent>();
    }

        private void OnTriggerEnter(Collider collision)
    {

        if (collision.gameObject.CompareTag("canonball"))
        {
            Canon();
            gocannon = true;
            Debug.Log("cannonballed");
        }
        else if (collision.gameObject.CompareTag("canon"))
        {
            gocannon = false;
            CanonBall();
        }
    }


    private void CanonBall()
    {
        if (!gocannon) 
        { 
        agent.SetDestination(posA.position);
    }

    
    }

    private void Canon()
    {
        if (gocannon)
        {
            agent.SetDestination(posB.position);
        }
       
    }

}
