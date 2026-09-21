using TMPro;
using UnityEngine;

public class AdTypeStatsDisplay : MonoBehaviour
{
    [Header("Вид рекламы")]
    public int adTypeIndex;

    public AdvertisingManager advertisingManager;

    public TextMeshProUGUI textFeatures;
    public TextMeshProUGUI textcount;

    [SerializeField] 
    private GameObject info;

    private bool yn = false;

    void OnEnable()
    {
        ShowStats();
    }
    public void ShowStats()
    {
        var stats = advertisingManager.adTypes[adTypeIndex];

        textFeatures.text = $"Охват: {stats.reach:P1}\nКонверсия: {stats.conversion:P1}\n" +
                            $"Вирусность: {stats.virality:P1}\nУстойчивость: {stats.stability:P1}\nУсталость: {stats.fatigue:P1}";
        int upgradedCount = 0;
        if (stats.purchasedNodes != null)
        {
            for (int i = 0; i < stats.purchasedNodes.Count; i++)
            {
                if (stats.purchasedNodes[i]) { upgradedCount++; }
            }
        }
        textcount.text = $"{upgradedCount}/14";
    }
    public void openOrClose()
    {
        yn = !yn;
        info.SetActive(yn);
    }

}