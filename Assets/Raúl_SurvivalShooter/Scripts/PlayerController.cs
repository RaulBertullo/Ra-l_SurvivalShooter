using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] LayerMask sueloMask;
    [SerializeField] LayerMask enemigoMask;

    [SerializeField] GameData datosJuego;

    EnemyBehaviour enemigoActual;

    public int vidaMax = 100;
    public int vidaActual;

    bool enemigoCerca;

    [HideInInspector] public NavMeshAgent navAgent;

    PlayerMouseInput controles;

    InputAction clickPrincipal;
    InputAction[] cambioArmas;
    InputAction guardarPartida;
    InputAction borrarPartida;

    [SerializeField] GameObject[] listaArmas = new GameObject[3];

    IWeapon armaActual;

    Vector3 posicionMouse;

    float tiempoDisparo;

    void Awake()
    {
        armaActual = listaArmas[0].GetComponent<IWeapon>();
        navAgent = GetComponent<NavMeshAgent>();
        vidaActual = vidaMax;
        controles = new PlayerMouseInput();
        controles.Player.Enable();

        clickPrincipal = controles.Player.MainClick;

        cambioArmas = new InputAction[3]
        {
        controles.Player.First,
        controles.Player.Second,
        controles.Player.Third
        };

        guardarPartida = controles.Player.SaveQuit;
        borrarPartida = controles.Player.Reset;

        if (SaveSystem.Load(datosJuego, gameObject))
        {
            transform.position = datosJuego.playerPosition;

            for (int i = 0; i < datosJuego.enemyPositions.Length; i++)
            {
                FindAnyObjectByType<EnemySpawner>().RestoreEnemy(
                    datosJuego.enemyPositions[i],
                    datosJuego.enemyHealths[i]
                );
            }
        }
    }

    void Update()
    {
        Debug.Log("VIDA: " + vidaActual);

        tiempoDisparo += Time.deltaTime;

        if (enemigoCerca && tiempoDisparo >= armaActual.GetCooldown())
        {
            tiempoDisparo = 0;
            Disparar();
        }

        posicionMouse = Mouse.current.position.value;

        if (clickPrincipal.WasPressedThisFrame())
        {
            MoverJugador();
        }

        if (enemigoActual != null)
        {
            float distancia =
                Vector3.Distance(transform.position, enemigoActual.transform.position);

            if (distancia <= armaActual.GetRange())
            {
                if (!enemigoCerca)
                {
                    tiempoDisparo = armaActual.GetCooldown();
                }

                enemigoCerca = true;

                navAgent.SetDestination(transform.position);
            }
            else
            {
                enemigoCerca = false;

                navAgent.SetDestination(enemigoActual.transform.position);
            }
        }
        else
        {
            enemigoCerca = false;
        }

        for (int i = 0; i < cambioArmas.Length; i++)
        {
            if (cambioArmas[i].WasPressedThisFrame())
            {
                for (int j = 0; j < listaArmas.Length; j++)
                {
                    listaArmas[j].SetActive(false);
                }

                listaArmas[i].SetActive(true);

                armaActual = listaArmas[i].GetComponent<IWeapon>();
            }
        }

        if (guardarPartida.WasPressedThisFrame())
        {
            GuardarSalir();
        }

        if (borrarPartida.WasPressedThisFrame())
        {
            ReiniciarJuego();
        }

     
    }

    void GuardarSalir()
    {
        datosJuego.playerPosition = transform.position;

        EnemyBehaviour[] enemigos =
            FindObjectsByType<EnemyBehaviour>(FindObjectsSortMode.None);

        datosJuego.enemyPositions = new Vector3[enemigos.Length];
        datosJuego.enemyHealths = new int[enemigos.Length];

        for (int i = 0; i < enemigos.Length; i++)
        {
            datosJuego.enemyPositions[i] = enemigos[i].transform.position;
            datosJuego.enemyHealths[i] = enemigos[i].life;
        }

        SaveSystem.Save(datosJuego);

        Application.Quit();
    }

    void ReiniciarJuego()
    {
        SaveSystem.DeleteSave();

        datosJuego.Reset();

        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
        );
    }

    void MoverJugador()
    {
        Ray rayo = Camera.main.ScreenPointToRay(posicionMouse);

        RaycastHit golpe;

        if (Physics.Raycast(rayo, out golpe, Mathf.Infinity, enemigoMask))
        {
            enemigoActual =
                golpe.collider.gameObject.GetComponent<EnemyBehaviour>();
        }
        else
        {
            enemigoActual = null;
            enemigoCerca = false;
        }

        if (Physics.Raycast(rayo, out golpe, Mathf.Infinity, sueloMask)
            && !enemigoCerca)
        {
            NavMeshHit puntoNav;

            NavMesh.SamplePosition(
                golpe.point,
                out puntoNav,
                5,
                NavMesh.AllAreas
            );

            navAgent.SetDestination(puntoNav.position);
        }
    }

    void Disparar()
    {
        armaActual.Shoot(transform, enemigoActual);
    }

    private void OnDrawGizmos()
    {
        if (enemigoCerca)
        {
            Gizmos.color = Color.red;
        }
        else
        {
            Gizmos.color = Color.yellow;
        }

        if (armaActual != null)
        {
            Gizmos.DrawWireSphere(
                transform.position,
                armaActual.GetRange()
            );
        }

        Gizmos.color = Color.blue;

        if (navAgent != null)
        {
            Gizmos.DrawSphere(navAgent.destination, 0.5f);
        }

        Gizmos.color = Color.red;

        if (enemigoActual != null &&
            enemigoActual.GetComponentInChildren<MeshFilter>() != null)
        {
            Gizmos.DrawWireMesh(
                enemigoActual.GetComponentInChildren<MeshFilter>().mesh,
                0,
                enemigoActual.transform.position + Vector3.up
            );
        }
    }
}
