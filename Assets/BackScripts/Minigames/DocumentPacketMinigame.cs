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
        if (spawner.spawnCards.Count == 0)
        {
            fullyCorrect = true;
            incorrectlyPlaced.Clear();

            // Проверяем ВСЕ контейнеры, включая правильные и неправильные
            var allContainers = new List<cardContainerScript>();
            allContainers.AddRange(correctContainers);      // теперь это "потенциальные правильные"
            allContainers.AddRange(incorrectContainers);    // неправильные остаются

            foreach (var container in allContainers)
            {
                if (container.containedCard != null)
                {
                    bool isCorrectPlacement = false;

                    // Если контейнер — из "правильных", проверяем совпадение зоны
                    if (correctContainers.Contains(container))
                    {
                        // Сравниваем зону карточки и контейнера
                        if (container.containedCard.correctZoneId == container.zoneId)
                        {
                            isCorrectPlacement = true;
                        }
                    }
                    // Если контейнер — из "неправильных", то размещение всегда неверно
                    // (или можно разрешить туда только неправильные карточки — зависит от логики)

                    if (!isCorrectPlacement)
                    {
                        fullyCorrect = false;
                        incorrectlyPlaced.Add(container);
                    }
                }
            }

            foreach (var container in incorrectlyPlaced)
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
