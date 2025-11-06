using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public enum pullDirection
{
    none = -1,
    right = 0,
    up = 1,
    left = 2,
    down = 3,
}

[Serializable]
public class productionItem
{
    public bool empty = true;
    public int ID;
    public int quality;
    public string Name;
    public Sprite sprite;

    public productionItem() { }
    public productionItem(productionItem item)
    {
        this.empty = item.empty;
        this.ID = item.ID;
        this.quality = item.quality;
        this.Name = item.Name;
        this.sprite = item.sprite;
    }
}
public class ProducerCard : MonoBehaviour, IProduction
{
    public float productionTime;
    public productionItem producedItem;
    public float productionStatus;
    public bool canInput { get; set; }
    public bool canOutput { get; set; } = true;
    public bool isProducing;
    public pullDirection pullDirection;
    public productionItem inputItem { get; set; } = new();
    public productionItem outputItem { get; set; } = new();
    [HideInInspector] public productionItem debugItem;
    public Image image;
    public cardManagerScript cardManager;
    public Image itemImage;
    public Vector2Int position;

    public Sprite cardSprite;

    cardScript thisCard;
    public IProduction pullNeighbour; 
    void Start()
    {
        cardManager = GetComponentInParent<cardManagerScript>();
        image.sprite = cardSprite;
        image.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90 * (int)pullDirection));
        thisCard = gameObject.GetComponent<cardScript>();
        productionStatus = productionTime;
    }

    // Update is called once per frame
    void Update()
    {
        canInput = outputItem.empty && inputItem.empty;
        position = thisCard.container.position;
        if (productionStatus < productionTime)
        {
            productionStatus += Time.deltaTime;
        }

        itemImage.gameObject.SetActive(isProducing || !canOutput);

        switch (pullDirection)
        {
            case pullDirection.up:
                if(position.x > 0 && cardManager.slotSpace[position.x - 1, position.y].GetComponentInChildren<IProduction>()!=null)
                    pullNeighbour = cardManager.slotSpace[position.x-1,position.y].GetComponentInChildren<IProduction>();
                itemImage.rectTransform.localPosition = new Vector3(Mathf.Lerp(
                    itemImage.rectTransform.sizeDelta.x,
                    -itemImage.rectTransform.sizeDelta.x,
                    productionStatus / productionTime
                    ), 0, 0);

                break;
            case pullDirection.down:
                if(position.x < cardManager.slotSpaceWidth-1 && cardManager.slotSpace[position.x + 1, position.y].GetComponentInChildren<IProduction>()!=null)
                    pullNeighbour = cardManager.slotSpace[position.x + 1, position.y].GetComponentInChildren<IProduction>();
                itemImage.rectTransform.localPosition = new Vector3(Mathf.Lerp(
                    itemImage.rectTransform.sizeDelta.x,
                    -itemImage.rectTransform.sizeDelta.x,
                    productionStatus / productionTime
                    ), 0, 0);
                break;
            case pullDirection.left:
                if(position.y > 0 && cardManager.slotSpace[position.x, position.y - 1].GetComponentInChildren<IProduction>()!=null)
                    pullNeighbour = cardManager.slotSpace[position.x, position.y-1].GetComponentInChildren<IProduction>();
                itemImage.rectTransform.localPosition = new Vector3(Mathf.Lerp(
                    itemImage.rectTransform.sizeDelta.x,
                    -itemImage.rectTransform.sizeDelta.x,
                    productionStatus / productionTime
                    ), 0, 0);
                break;
            case pullDirection.right:
                if(position.y < cardManager.slotSpaceHeight-1 && cardManager.slotSpace[position.x, position.y + 1].GetComponentInChildren<IProduction>()!=null)
                    pullNeighbour = cardManager.slotSpace[position.x, position.y+1].GetComponentInChildren<IProduction>();
                itemImage.rectTransform.localPosition = new Vector3(Mathf.Lerp(
                    itemImage.rectTransform.sizeDelta.x,
                    -itemImage.rectTransform.sizeDelta.x,
                    productionStatus / productionTime
                    ), 0, 0);
                break;
        }

        if(pullNeighbour != null && !pullNeighbour.outputItem.empty && canInput)
        {
            inputItem = pullNeighbour.outputItem;
            pullNeighbour.outputItem = new();
            pullNeighbour.canOutput = true;
        }

        if (!inputItem.empty)
        {
            StartCoroutine(produce());
        }
    }

    public IEnumerator produce()
    {
        if (canOutput)
        {
            productionStatus = 0;
            canOutput = false;
            isProducing = true;
            itemImage.sprite = inputItem.sprite;
            yield return new WaitForSeconds(productionTime);
            isProducing = false;
            outputItem = new productionItem(producedItem);
            canInput = false;
            inputItem = new productionItem();
        }
    }

    [ContextMenu("Add item")]
    public void putItemAtInput()
    {
        if (canInput)
        {
            inputItem = new productionItem() { ID = 0, Name = "Ffff", empty = false };
            canOutput = true;
            canInput = false;
        }
    }

    [ContextMenu("Force production")]
    public void forceProduction()
    {
        inputItem = new productionItem(debugItem);
        canOutput = true;
        canInput = false;
        StartCoroutine(produce());
    }
    [ContextMenu("Print pull neighbour")]
    public void printPullNeighbour()
    {
        print(pullNeighbour);
    }
}
