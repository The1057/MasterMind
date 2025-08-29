using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class ShopItemRedactScript : MonoBehaviour
{
    public int storeIndex = 0; //to do: change this based on smth

    public saveData saveData;
    public saveLoadManager saveLoadManager;
    public biz_niche Get_Niche()
    {
        saveData = saveLoadManager.loadData();

        return saveData.PlayerData.first_niche;
    }
    public List<Item> getPossibleItems()
    {
        biz_niche playerNiche = Get_Niche();

        switch (playerNiche)
        {
            case biz_niche.breadShop:

                return breadItemList.possibleItems;

                break;
            case biz_niche.bookShop:

                return bookItemList.possibleItems;

                break;
            case biz_niche.clothShop:

                return clothItemList.possibleItems;

                break;
            default:
                return null;
        }
    }
    public void addItem(Item item)
    {
        saveData = saveLoadManager.loadData();
        saveData.StoreDatas[storeIndex].items.Add(item);
        saveLoadManager.saveData(saveData);
    }
    public void addItemByButtonName()
    {
        var thisButton = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        var buttonName = thisButton.name;
        //print($"Button name: '{buttonName}'");
        if (thisButton != null)
        {
            var itemList = getPossibleItems();
            foreach (var item in itemList)
            {
                //print($"Currently on item: '{item.name}'");
                if (item.name == buttonName)
                {
                    addItem(item);
                    //print($"Matching item:{item.name}");
                }
            }
        }
    }
}
