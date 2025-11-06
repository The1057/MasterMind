using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ProductionItemVoider : MonoBehaviour, IProduction
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
    public cardManagerScript cardManager;
    public Vector2Int position;
    cardScript thisCard;
    public IProduction pullNeighbour;

    void Start()
    {
        cardManager = GetComponentInParent<cardManagerScript>();
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
        switch (pullDirection)
        {
            case pullDirection.up:
                if (position.x > 0 && cardManager.slotSpace[position.x - 1, position.y].GetComponentInChildren<IProduction>() != null)
                    pullNeighbour = cardManager.slotSpace[position.x - 1, position.y].GetComponentInChildren<IProduction>();

                break;
            case pullDirection.down:
                if (position.x < cardManager.slotSpaceWidth - 1 && cardManager.slotSpace[position.x + 1, position.y].GetComponentInChildren<IProduction>() != null)
                    pullNeighbour = cardManager.slotSpace[position.x + 1, position.y].GetComponentInChildren<IProduction>();
                break;
            case pullDirection.left:
                if (position.y > 0 && cardManager.slotSpace[position.x, position.y - 1].GetComponentInChildren<IProduction>() != null)
                    pullNeighbour = cardManager.slotSpace[position.x, position.y - 1].GetComponentInChildren<IProduction>();
                break;
            case pullDirection.right:
                if (position.y < cardManager.slotSpaceHeight - 1 && cardManager.slotSpace[position.x, position.y + 1].GetComponentInChildren<IProduction>() != null)
                    pullNeighbour = cardManager.slotSpace[position.x, position.y + 1].GetComponentInChildren<IProduction>();
                break;
        }

        if (pullNeighbour != null && !pullNeighbour.outputItem.empty && canInput)
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
            yield return new WaitForSeconds(productionTime);
            isProducing = false;
            outputItem = new productionItem();
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
}
