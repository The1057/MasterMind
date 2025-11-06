using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ProductionItemSpawner : MonoBehaviour, IProduction
{
    public float productionTime;
    public productionItem producedItem;
    public float productionStatus;
    public bool canInput { get; set; }
    public bool canOutput { get; set; } = true;
    public bool isProducing;
    public productionItem inputItem { get; set; } = new();
    public productionItem outputItem { get; set; } = new();
    [HideInInspector] public productionItem debugItem;
    public cardManagerScript cardManager;
    public Vector2Int position;
    cardScript thisCard;
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
            outputItem = new productionItem(producedItem);
            canInput = false;
            inputItem = new productionItem(producedItem);
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
