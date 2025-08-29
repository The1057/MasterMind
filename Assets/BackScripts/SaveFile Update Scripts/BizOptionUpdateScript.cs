using UnityEngine;

public class BizOptionUpdater : MonoBehaviour
{
    public saveLoadManager saveLoadManager;
    saveData saveData;

    [ContextMenu("Change Business niche")]
    public void changeBizNiche(biz_niche niche)
    {
        saveData = saveLoadManager.loadData();
        saveData.PlayerData.first_niche = niche;
        saveLoadManager.saveData(saveData);
    }

    [ContextMenu("Change Business Tax System")]
    public void changeBizTaxSystem(tax_system tax_system)
    {
        saveData = saveLoadManager.loadData();
        saveData.PlayerData.tax_system = tax_system;
        saveLoadManager.saveData(saveData);
    }

    [ContextMenu("Change Business Legal Form")]
    public void changeBizLegalForm(legal_form legal_form)
    {
        saveData = saveLoadManager.loadData();
        saveData.PlayerData.legal_form = legal_form;
        saveLoadManager.saveData(saveData);
    }

    //legal Form
    public void setLegalForm2IP()
    {
        changeBizLegalForm(legal_form.IP);
    }
    public void setLegalForm2OOO()
    {
        changeBizLegalForm(legal_form.OOO);
    }

    //Tax system
    public void setTaxSystem2PSN()
    {
        changeBizTaxSystem(tax_system.PSN);
    }
    public void setTaxSystem2USN()
    {
        changeBizTaxSystem(tax_system.USN);
    }

    //business niche
    public void setNiche2Bread()
    {
        changeBizNiche(biz_niche.breadShop);
    }
    public void setNiche2Cloth()
    {
        changeBizNiche(biz_niche.clothShop);
    }
    public void setNiche2Book()
    {
        changeBizNiche(biz_niche.bookShop);
    }
}
