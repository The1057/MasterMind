using System.Collections.Generic;
using UnityEngine;

public class AdvertisingManager : MonoBehaviour, ISaveLoadable
{
    [System.Serializable]
    public class UpgradeEffect
    {
        public float reachChange;
        public float conversionChange;
        public float viralityChange;
        public float stabilityChange;
        public float fatigueChange;
    }
    [Header("“екущие показатели всех видов рекламы")]
    public AdTypeData[] adTypes = new AdTypeData[6];

    private void Awake()
    {
        EnsureInitialized();
    }

    private void EnsureInitialized()
    {
        if (adTypes == null || adTypes.Length != 6)
        {
            adTypes = new AdTypeData[6];
        }

        for (int i = 0; i < 6; i++)
        {
            if (adTypes[i] == null)
            {
                adTypes[i] = new AdTypeData();
            }
        }
    }

    public void save(ref saveData data)
    {
        EnsureInitialized();

        for (int i = 0; i < 6; i++)
        {
            data.AdvertisingData.adTypes[i].reach = adTypes[i].reach;
            data.AdvertisingData.adTypes[i].conversion = adTypes[i].conversion;
            data.AdvertisingData.adTypes[i].virality = adTypes[i].virality;
            data.AdvertisingData.adTypes[i].stability = adTypes[i].stability;
            data.AdvertisingData.adTypes[i].fatigue = adTypes[i].fatigue;

            data.AdvertisingData.adTypes[i].purchasedNodes = new List<bool>(adTypes[i].purchasedNodes);
        }
    }

    public void load(saveData data)
    {
        EnsureInitialized();

        for (int i = 0; i < 6; i++)
        {
            adTypes[i].reach = data.AdvertisingData.adTypes[i].reach;
            adTypes[i].conversion = data.AdvertisingData.adTypes[i].conversion;
            adTypes[i].virality = data.AdvertisingData.adTypes[i].virality;
            adTypes[i].stability = data.AdvertisingData.adTypes[i].stability;
            adTypes[i].fatigue = data.AdvertisingData.adTypes[i].fatigue;

            adTypes[i].purchasedNodes = new List<bool>(data.AdvertisingData.adTypes[i].purchasedNodes);
        }
    }

    public void ApplyEffect(int adTypeIndex, UpgradeEffect effect)
    {
        EnsureInitialized();

        adTypes[adTypeIndex].reach += effect.reachChange;
        adTypes[adTypeIndex].conversion += effect.conversionChange;
        adTypes[adTypeIndex].virality += effect.viralityChange;
        adTypes[adTypeIndex].stability += effect.stabilityChange;
        adTypes[adTypeIndex].fatigue += effect.fatigueChange;
    }
    public void SetPurchasedNodes(int adTypeIndex, List<bool> nodes)
    {
        EnsureInitialized();
        if (adTypeIndex < 0 || adTypeIndex >= 6) return;

        adTypes[adTypeIndex].purchasedNodes = new List<bool>(nodes);
    }

    public List<bool> GetPurchasedNodes(int adTypeIndex)
    {
        EnsureInitialized();
        if (adTypeIndex < 0 || adTypeIndex >= 6) return new List<bool>();

        return adTypes[adTypeIndex].purchasedNodes;
    }
}