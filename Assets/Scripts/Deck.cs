using UnityEngine;
using UnityEngine.UI;
using System;

public class Deck : MonoBehaviour
{
    public Sprite[] faces;
    public GameObject dealer;
    public GameObject player;
    public Button hitButton;
    public Button stickButton;
    public Button playAgainButton;
    public Text finalMessage;
    public Text probMessage;
    public Text TextPointsP;
    public Text TextPointsD;

    public int[] values = new int[52];
    int cardIndex = 0;
    
    private void Awake()
    {
        InitCardValues();
    }
    private void Start()
    {
        ShuffleCards();
        StartGame();
    }
    private void InitCardValues()
    {
        for (int i = 0; i < values.Length; i++)
        {
            int rango = i % 13; 
        
            if (rango == 0) 
            {
                values[i] = 1;
            }
            else if (rango >= 1 && rango <= 9) 
            {
                values[i] = rango + 1;
            }
            else 
            {
                values[i] = 10; 
            }
        }
    }
    private void ShuffleCards()
    {
        for (int i = 51; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            Sprite tempSprite = faces[i];
            faces[i] = faces[j];
            faces[j] = tempSprite;

            int tempValue = values[i];
            values[i] = values[j];
            values[j] = tempValue;
        }
    }
    void StartGame()
    {
        for (int i = 0; i < 2; i++)
        {
            PushPlayer();
            PushDealer();
        }
    }
    private void CalculateProbabilities()
    {
        int playerPoints = player.GetComponent<CardHand>().points;
        int[] restantes = new int[52 - cardIndex];
        Array.Copy(values, cardIndex, restantes, 0, 52 - cardIndex);

        int valorCartaOcultaDealer = values[cardIndex];
        float probabilidadDealerGanaConCartaOculta = 0f;

        for (int i = 0; i < restantes.Length; i++)
        {
            int puntosPosiblesDealer = valorCartaOcultaDealer + restantes[i];
            if (puntosPosiblesDealer > playerPoints && puntosPosiblesDealer <= 21)
            {
                probabilidadDealerGanaConCartaOculta += 1f;
            }
        }
        probabilidadDealerGanaConCartaOculta = (probabilidadDealerGanaConCartaOculta / restantes.Length) * 100f;

        float probabilidadJugadorEntre17Y21 = 0f;
        for (int i = 0; i < restantes.Length; i++)
        {
            int puntosPosiblesJugador = playerPoints + restantes[i];
            if (puntosPosiblesJugador >= 17 && puntosPosiblesJugador <= 21)
            {
                probabilidadJugadorEntre17Y21 += 1f;
            }
        }
        
        probabilidadJugadorEntre17Y21 = (probabilidadJugadorEntre17Y21 / restantes.Length) * 100f;

        float probabilidadJugadorSePasa = 0f;
        for (int i = 0; i < restantes.Length; i++)
        {
            int puntosPosiblesJugador = playerPoints + restantes[i];
            if (puntosPosiblesJugador > 21)
            {
                probabilidadJugadorSePasa += 1f;
            }
        }

        probabilidadJugadorSePasa = (probabilidadJugadorSePasa / restantes.Length) * 100f;
        probMessage.text =
            $"Dealer > Jugador: {probabilidadDealerGanaConCartaOculta}%\n" +
            $"17<=x<=21: {probabilidadJugadorEntre17Y21}%\n" +
            $"x>21: {probabilidadJugadorSePasa}%";
    }
    void PushDealer()
    {
        dealer.GetComponent<CardHand>().Push(faces[cardIndex], values[cardIndex]);
        cardIndex++;
    }

    void PushPlayer()
    {
        player.GetComponent<CardHand>().Push(faces[cardIndex], values[cardIndex]);
        cardIndex++;
        int playerPoints = player.GetComponent<CardHand>().points;
        TextPointsP.text = $" {playerPoints}";

        CalculateProbabilities();
    }

    public void Hit()
    {
        PushPlayer();
        CalculateProbabilities();
        int playerPoints = player.GetComponent<CardHand>().points;
        if (playerPoints > 21)
        {
            finalMessage.text = "El jugador pierde!";
            EndGame();
        }
    }

    public void Stand()
    {
        dealer.GetComponent<CardHand>().InitialToggle();
        int dealerPoints = dealer.GetComponent<CardHand>().points;
        while (dealerPoints < 17)
        {
            PushDealer();
            dealerPoints = dealer.GetComponent<CardHand>().points;
        }
        TextPointsD.text = $"{dealerPoints}";
        int playerPoints = player.GetComponent<CardHand>().points;
        dealerPoints = dealer.GetComponent<CardHand>().points;

        if (dealerPoints > 21)
        {
            finalMessage.text = "El dealer pierde!";
        }
        else if (playerPoints > dealerPoints)
        {
            finalMessage.text = "El jugador gana!";
        }
        else if (playerPoints < dealerPoints)
        {
            finalMessage.text = "El dealer gana!";
        }
        else
        {
            finalMessage.text = "Empate!";
        }
        EndGame();
    }
    
    public void PlayAgain()
    {
        hitButton.interactable = true;
        stickButton.interactable = true;
        finalMessage.text = "";
        player.GetComponent<CardHand>().Clear();
        dealer.GetComponent<CardHand>().Clear();
        cardIndex = 0;
        ShuffleCards();
        StartGame();
    }
    private void EndGame()
    {
        hitButton.interactable = false;
        stickButton.interactable = false;
    }
}