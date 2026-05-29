using UnityEngine;
using UnityEngine.AI;

public class EnemigoIA : MonoBehaviour
{
    public GameObject target;
    public int vida = 100;

    NavMeshAgent agent;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (target == null) return;

        NavMeshHit hit;
        Vector3 pos = target.transform.position;
        NavMesh.SamplePosition(pos, out hit, 5, NavMesh.AllAreas);
        agent.SetDestination(hit.position);
    }

    public void GetDamaged(int damage)
    {
        vida -= damage;

        if (vida <= 0)
            Destroy(gameObject);
    }
}
