using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
public class sequentialCardSpawnScript : MonoBehaviour
{
    public List<GameObject> spawnCards;
    public cardContainerScript thisCardContainer;
    public cardContainerScript table;
    public GameObject lastCard;
    public void Start()
    {
        //if (spawnCards.Count > 0)
        //{
        //    lastCard = Instantiate(spawnCards.First(), transform);
        //    spawnCards.RemoveAt(0);

        //    lastCard.GetComponent<Image>().color = Color.clear;
        //    lastCard.GetComponent<cardScript>().container = thisCardContainer;
        //    lastCard.GetComponent<cardScript>().parentAfterDrag = thisCardContainer.transform;
        //    thisCardContainer.containedCard = lastCard.GetComponent<cardScript>();
        //}
    }
    public void Update()
    {
        //if (thisCardContainer.containedCard == null && spawnCards.Count > 0)
        //{
        //    //lastCard.GetComponent<Image>().color = Color.white;
        //    lastCard = Instantiate(spawnCards.First(), transform);
        //    spawnCards.RemoveAt(0);

        //    lastCard.GetComponent<Image>().color = Color.clear;
        //    lastCard.GetComponent<cardScript>().container = thisCardContainer;
        //    lastCard.GetComponent<cardScript>().parentAfterDrag = thisCardContainer.transform;
        //    thisCardContainer.containedCard = lastCard.GetComponent<cardScript>();
        //} else if(transform.childCount > 0) 
        //{
        //    transform.GetChild(0).GetComponent<Image>().color = Color.clear;
        //}
    }

    public void spawnAtTable()
    {
        if (table.transform.childCount == 0)
        {
            lastCard = Instantiate(spawnCards.First());
            lastCard.transform.parent = table.transform;
            spawnCards.RemoveAt(0);
        }
    }
}
