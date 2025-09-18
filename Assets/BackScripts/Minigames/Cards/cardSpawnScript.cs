using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class cardSpawnScript : MonoBehaviour
{
    public GameObject spawnCard;
    public cardContainerScript thisCardContainer;
    public GameObject lastCard;
    public void Start()
    {
        lastCard = Instantiate(spawnCard, transform);
        lastCard.GetComponent<Image>().color = Color.clear;
        lastCard.GetComponent<cardScript>().container = thisCardContainer;
        lastCard.GetComponent<cardScript>().parentAfterDrag = thisCardContainer.transform;
        thisCardContainer.containedCard = lastCard.GetComponent<cardScript>();
    }
    public void Update()
    {
        if (thisCardContainer.containedCard == null)
        {
            //lastCard.GetComponent<Image>().color = Color.white;
            lastCard = Instantiate(spawnCard, transform);
            lastCard.GetComponent<Image>().color = Color.clear;
            lastCard.GetComponent<cardScript>().container = thisCardContainer;
            lastCard.GetComponent<cardScript>().parentAfterDrag = thisCardContainer.transform;
            thisCardContainer.containedCard = lastCard.GetComponent<cardScript>();
        } else if(transform.childCount > 0) 
        {
            transform.GetChild(0).GetComponent<Image>().color = Color.clear;
        }
    }
}
