using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.Controls;


public class EnemyMove : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask whatIsGround, whatIsPlayer;

    //attack
    private Animator m_animator;
    public float timeBetweenAttacks;
    bool alreadyAttacked;

    [SerializeField] private float slapForce = 2.5f;
    [SerializeField] private float slapRadius = 0.75f;
    [SerializeField] private float howMuchUp = 0.75f;
    [SerializeField] private float slapDamage = 5;
    [SerializeField] PhysicMaterial slapMat;

    //States
    public float sightRange, attackRange;
    public bool InSightRange, InAttackRange;
    public bool spawned;




    // Start is called before the first frame update
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        spawned = true;
    }

    // Update is called once per frame
    void Update()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        InSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        InAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (InSightRange && !InAttackRange) Chase();
        if (InSightRange && InAttackRange) Attack();
        if (spawned) Jump();

    }


    private void Jump()
    {

        spawned = false;
    }

    private void Chase()
    {
       

        agent.SetDestination(player.position);
    }

    private void Attack()
    {
        //stop movement while slapping
        agent.SetDestination(transform.position);



        if (!alreadyAttacked)
        {
            ///Attack code here
            Collider[] colliders = new Collider[10];
            int count = Physics.OverlapSphereNonAlloc(transform.position + transform.forward, slapRadius, colliders);
            bool canSlapSfx = false;
            //for each collider in colliders, if can get rigidbody, add force
            for (int i = 0; i < count; i++)
            {
                float extraForce = 0f;
                if (colliders[i].gameObject == gameObject || !colliders[i].TryGetComponent(out Rigidbody rb)) continue; //if same object continue
                canSlapSfx = true;
                if (colliders[i].TryGetComponent(out Health health))         //if has component health then
                {
                    health.TakeDamage(gameObject, slapDamage);
                    extraForce = (health.GetHealth() / health.GetMaxHealth()) * slapForce;
                }
                if (colliders[i].TryGetComponent(out PlayerControler pc))
                {
                    StartCoroutine(ReduceFriction(colliders[i].gameObject, pc, extraForce / 5));
                    pc.Ragdoll();
                }
                if (colliders[i].TryGetComponent(out AIBrain brain))
                {
                    //disable the agent and enable kinematic
                    StartCoroutine(brain.ReenableAgent(brain.knockDownTime));
                    brain.ChangeState(AIBrain.State.Chase);

                }
                rb.AddForce(((transform.forward) * (slapForce + extraForce / 5)) + ((transform.up * howMuchUp) * slapForce / 5), ForceMode.Impulse);

            }
            if (canSlapSfx)
                SoundManager.PlayAudioClip("Slap", transform.position + transform.forward, 1f);

            ///End of attack code

            alreadyAttacked = true;

            if (alreadyAttacked)
            {

                Invoke(nameof(ResetAttack), timeBetweenAttacks);
            }

        }
    }
    private void ResetAttack()
    {

        alreadyAttacked = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    IEnumerator ReduceFriction(GameObject player, PlayerControler pc, float slapForce)
    {
        pc.deceleration = 0.001f;
        PhysicMaterial temp = null;
        gameObject.GetComponent<CapsuleCollider>().material = slapMat;
        yield return new WaitForSeconds(1 + slapForce);
        pc.deceleration = 0.1f;
        gameObject.GetComponent<CapsuleCollider>().material = temp;
    }


}

