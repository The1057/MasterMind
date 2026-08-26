using UnityEngine;

public class PlayerDataUpdate : MonoBehaviour
{
    public playerDataClass playerData;
    public void updateStorePic(int index)
    {
        playerData.data.storePicIndex = index;
    }
    public void updateNiche(int index)
    {
        playerData.data.first_niche = (biz_niche) index;
    }
    public void updateTaxSystem(int index)
    {
        playerData.data.tax_system = (tax_system)index;
    }
    public void updateLegalForm(int index)
    {
        playerData.data.legal_form = (legal_form)index;
    }
}
