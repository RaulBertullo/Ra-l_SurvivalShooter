using UnityEngine;
using UnityEngine.UI;

public class barraVida : MonoBehaviour
{
    public Image barra;
    public PlayerController jugador;

    void Update()
    {
        if (jugador == null || barra == null) return;

        float porcentaje = (float)jugador.vidaActual / jugador.vidaMax;

        porcentaje = Mathf.Clamp01(porcentaje);

        barra.fillAmount = porcentaje;
    }
}