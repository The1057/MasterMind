using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public enum activeTree
{
    internet = 0
}
public enum adEfficiency
{    

}
public enum adReach
{

}

[Serializable]
public class AdUpgrade
{
    public  string name = "";
    public float globalAdEffect = 0.05f;
    public float cost = 0;
    public bool active = false;
    public List<int> prerequisiteIDs;
    public Image Image;
}
[Serializable]
public class AdUpgradeTree
{
    public List<AdUpgrade> upgrades;
}

[Serializable]
public class AdCampaign
{
    public string name = "";
    public adEfficiency efficiency;
    public adReach reach;
    public float maxEfficiency;
    public AnimationCurve efficiencyCurve;
    public float adModifier;
    public int intendedDuration;
    public int currentDuration;
    bool isStopped = false;
    public void start()
    {
        currentDuration = 0;
        adModifier = efficiencyCurve.Evaluate(0);
        isStopped = false;
    }

    public void stop()
    {
        currentDuration = 0;
        adModifier = 0;
        isStopped = true;
    }
    public void nextStep()
    {
        if (isStopped) return;
        currentDuration++;
        if(currentDuration > intendedDuration) 
        {
            stop();
        }
        adModifier = efficiencyCurve.Evaluate((float)currentDuration/(float)intendedDuration);
    }
}
public class AdSystemSctipt : MonoBehaviour, ITickable, ISaveLoadable
{
    public MoneyScript moneyScript;
    float campaignModifier;
    float upgradeModifier;
    public float globalAdModifier = 1;
    public AdUpgradeTree internetAd = new();
    public activeTree activeTree;
    public Sprite inactiveUpgrade;
    public List<AdCampaign> possibleCampaigns;
    public AdCampaign currentCampaign;
    public void load(saveData data)
    {
        this.activeTree = data.adSystemData.activeTree;
        this.globalAdModifier = data.adSystemData.globalAdModifier;
        this.internetAd = data.adSystemData.internetAd;
    }
    public void save(ref saveData data)
    {
        data.adSystemData.activeTree = this.activeTree;
        data.adSystemData.globalAdModifier = this.globalAdModifier;
        data.adSystemData.internetAd = this.internetAd;
    }
    public void nextTurn(int month, int year)
    {
        currentCampaign.nextStep();
        campaignModifier = currentCampaign.adModifier;
        reApplyModifierToStores();
    }
    public void setActiveCampaignById(int id)
    {
        currentCampaign = possibleCampaigns[id];
        currentCampaign.start();
    }
    public void setActiveTree(int id)
    {
        activeTree = (activeTree)id;
    }
    public void tryUpgradeActiveById(int id)//здесь проверяются необходимые условия для улучшения
    {
        switch (activeTree)
        {
            case activeTree.internet:
                foreach(var upgrade in internetAd.upgrades[id].prerequisiteIDs)
                {
                    if (!internetAd.upgrades[upgrade].active)
                    {
                        Debug.Log($"Upgrade №{id} prerequisit №{upgrade} is inactive");
                        return;
                    }
                }//prerequisit check

                if(internetAd.upgrades[id].cost > moneyScript.player_money)
                {
                    Debug.Log($"Недостаточно денег! Стоимость улучшения: {internetAd.upgrades[id].cost}, ваш баланс: {moneyScript.player_money}");
                    return;
                }//cost check

                setUpgradeActiveById(id);
            break;
        }
    }
    void setUpgradeActiveById(int id)//здесь выставляются реальные эффекты улучшений
    {
        switch (activeTree)//не забыть про другие деревья
        {
            case activeTree.internet:
                var thisUpgrade = internetAd.upgrades[id];
                thisUpgrade.active = true;
                thisUpgrade.Image.sprite = inactiveUpgrade;
                globalAdModifier += thisUpgrade.globalAdEffect;
                reApplyModifierToStores();
                moneyScript.addMoney(-thisUpgrade.cost);
            break;
        }
    }
    void reApplyModifierToStores()
    {
        globalAdModifier = 1 + campaignModifier + upgradeModifier;
        foreach (storeScript store in moneyScript.storeList)
        {
            store.gloabalAdModifier = globalAdModifier;
        }
    }
}
