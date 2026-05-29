using UnityEngine;
using TMPro;

public class barraVida : MonoBehaviour
{
    public TextMeshProUGUI textoVida;
    public PlayerController jugador;

    public GameObject gameOverPanel;

    bool gameOver;

    void Start()
    {
        gameOverPanel.SetActive(false);
    }

    void Update()
    {
        if (gameOver || jugador == null) return;

        textoVida.text = jugador.vidaActual + " / " + jugador.vidaMax;

        if (jugador.vidaActual <= 0)
        {
            GameOver();
        }
    }

    void GameOver()
    {
        gameOver = true;

        gameOverPanel.SetActive(true);

        Destroy(jugador.gameObject);
    }
}