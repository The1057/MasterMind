using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using TMPro;

public class DocumentPacketMinigame : MonoBehaviour
{
    public GameObject TipPrefab;
    public sequentialCardSpawnScript spawner;
    public List<cardContainerScript> correctContainers;
    public List<cardContainerScript> incorrectContainers;
    public bool fullyCorrect = true;
    public Canvas uiCanvas;
    public GameObject gameplayCanvas;
    public GameObject successCanvas;

    int correctCardAmount = 0;
    public List<cardContainerScript> incorrectlyPlaced = new();
    //public List<cardContainerScript> incorrectInGarbage = new();

    void Start()
    {
        foreach (var card in spawner.spawnCards)//counting correct cards
        {
            if (card.GetComponent<cardScript>().isCorrect)
            {
                correctCardAmount++;
            }
        }
    }

    void Update()
    {
        
    }

    public void submit()
    {
        if(spawner.spawnCards.Count == 0)
        {
            fullyCorrect = true;
            incorrectlyPlaced.Clear();
            foreach (var container in correctContainers)
            {
                if (container.containedCard != null)
                {
                    if (!container.containedCard.isCorrect)
                    {
                        fullyCorrect = false;
                        incorrectlyPlaced.Add(container);
                    }
                }
            }

            foreach (var container in incorrectContainers)
            {
                if (container.containedCard != null)
                {
                    if (container.containedCard.isCorrect)
                    {
                        fullyCorrect = false;
                        incorrectlyPlaced.Add(container);
                    }
                }
            }

            foreach(var container in incorrectlyPlaced)
            {
                container.MarkAsIncorrect(true);
            }

            if (fullyCorrect)
            {
                gameplayCanvas.SetActive(false);
                successCanvas.SetActive(true);
            }
        }
    }
}
