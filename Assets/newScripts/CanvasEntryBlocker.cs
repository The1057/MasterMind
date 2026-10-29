using System.Collections.Generic;
using UnityEngine;

public enum SetupRequirement
{
    Niche = 0,
    LegalForm = 1,
    TaxSystem = 2,
    StorePic = 3,
    PlayerName = 4,
}

public class CanvasEntryBlocker : MonoBehaviour, ICanvasEntryValidator
{
    [Tooltip("—сылка на playerDataClass Ч можно оставить пустым, тогда найдЄтс€ через FindObjectOfType")]
    public playerDataClass playerData;

    [Header("“ребовать ¬џЅќ–ј (иначе нельз€ войти)")]
    public List<SetupRequirement> requireSelected = new();

    [Header("“ребовать Ќ≈выбора (если уже выбрано Ч нельз€ войти повторно)")]
    public List<SetupRequirement> requireNotSelected = new();

    public bool CanEnter()
    {
        if (playerData == null)
            playerData = FindObjectOfType<playerDataClass>();

        if (playerData == null || playerData.data == null)
        {
            Debug.LogError($"CanvasEntryBlocker на {name}: playerData не найден!");
            return true; // не блокируем, чтобы не сломать игру
        }

        var d = playerData.data;

        foreach (var req in requireSelected)
        {
            if (!IsSelected(d, req))
            {
                Debug.Log($"[Blocker] {name}: нужен выбор {req}, но он не сделан");
                return false;
            }
        }

        foreach (var req in requireNotSelected)
        {
            if (IsSelected(d, req))
            {
                Debug.Log($"[Blocker] {name}: {req} уже выбран Ч повторный вход запрещЄн");
                return false;
            }
        }

        return true;
    }

    private bool IsSelected(playerData d, SetupRequirement req) => req switch
    {
        SetupRequirement.Niche => d.first_niche != biz_niche.none,
        SetupRequirement.LegalForm => d.legal_form != legal_form.none,
        SetupRequirement.TaxSystem => d.tax_system != tax_system.none,
        SetupRequirement.StorePic => d.storePicIndex >= 0,
        SetupRequirement.PlayerName => !string.IsNullOrEmpty(d.player_name),
        _ => false
    };
}