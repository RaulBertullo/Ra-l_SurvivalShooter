using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviour : MonoBehaviour
{
    public GameObject target;

    [HideInInspector] public NavMeshAgent agent;

    public int life = 100;
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    void Update()
    {
        NavMeshHit navHit;
        NavMesh.SamplePosition(target.transform.position, out navHit, 5, NavMesh.AllAreas);
        agent.SetDestination(navHit.position);
    }

    public void GetDamaged(int damage)
    {
        life -= damage;
        if (life <= 0) Destroy(gameObject);
    }

    float timer;

    private void OnTriggerStay(Collider other)
    {
        PlayerController player = other.GetComponentInParent<PlayerController>();

        if (player != null)
        {
            timer += Time.deltaTime;

            if (timer >= 1f)
            {
                player.vidaActual -= 10;
                timer = 0f;
            }
        }
    }
}
