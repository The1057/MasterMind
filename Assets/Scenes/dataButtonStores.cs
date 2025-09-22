using System.IO;
using UnityEngine;
using UnityEngine.UI;


public class dataButtonStores : MonoBehaviour
{
    private Image buttonImage;
    private string filePath = "playerData.json"; // Имя файла с гендером
    public void Bak() {
        Transform canvasTransform = transform.parent.Find("Canvas (7.3)");
        canvasTransform.gameObject.SetActive(true);
    }
    public void Bok() {
        Transform canvasTransform = transform.parent.Find("Canvas (7.1)");
        canvasTransform.gameObject.SetActive(true);
    }
    public void Clo() {
        Transform canvasTransform = transform.parent.Find("Canvas (7.2)");
        canvasTransform.gameObject.SetActive(true);
    }
    public void PicBook1() {
        Transform canvasTransform = transform.parent.Find("Book1");
        canvasTransform.gameObject.SetActive(true);
    }
    public void PicBook2() {
        Transform canvasTransform = transform.parent.Find("Book2");
        canvasTransform.gameObject.SetActive(true);
    }
    public void PicBook3() {
        Transform canvasTransform = transform.parent.Find("Book3");
        canvasTransform.gameObject.SetActive(true);
    }
    public void PicBake1() {
        Transform canvasTransform = transform.parent.Find("Bake1");
        canvasTransform.gameObject.SetActive(true);
    }
    public void PicBake2() {
        Transform canvasTransform = transform.parent.Find("Bake2");
        canvasTransform.gameObject.SetActive(true);
    }
    public void PicBake3() {
        Transform canvasTransform = transform.parent.Find("Bake3");
        canvasTransform.gameObject.SetActive(true);
    }
    public void PicClose1() {
        Transform canvasTransform = transform.parent.Find("Close1");
        canvasTransform.gameObject.SetActive(true);
    }
    public void PicClose2() {
        Transform canvasTransform = transform.parent.Find("Close2");
        canvasTransform.gameObject.SetActive(true);
    }
    public void PicClose3() {
        Transform canvasTransform = transform.parent.Find("Close3");
        canvasTransform.gameObject.SetActive(true);
    }
    public void ForMapAndTheory() {
        Transform canvasTransform = transform.parent.Find("фон");
        Transform canvasTransform2 = transform.parent.Find("CanvasBoss");
        canvasTransform.gameObject.SetActive(false);
        canvasTransform2.gameObject.SetActive(false);
    }
    public void ForMapAndTheoryReverse() {
        Transform canvasTransform = transform.parent.Find("фон");
        Transform canvasTransform2 = transform.parent.Find("CanvasBoss");
        canvasTransform.gameObject.SetActive(true);
        canvasTransform2.gameObject.SetActive(true);
    }
    public void Prof() {
        if (GetPlayerGender() == "F") {
            Transform canvasTransform = transform.parent.Find("ПрофильВумэн");
            canvasTransform.gameObject.SetActive(true);
        }
        else {
            Transform canvasTransform = transform.parent.Find("ПрофильМэн");
            canvasTransform.gameObject.SetActive(true);
        }
    }

    public void ActivateBeginTheory () {
        Transform canvasTransform = transform.parent.Find("Начало");
        canvasTransform.gameObject.SetActive(true);
    }
    private string GetPlayerGender() {
        playerData playerData;
        if (File.Exists(filePath))
        {
            try
            {
                string rawJSON;
                using (FileStream stream = new FileStream(filePath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        rawJSON = reader.ReadToEnd();//magic to read from file
                    }
                }
                playerData = JsonUtility.FromJson<playerData>(rawJSON);
                return playerData.player_gender;
            }
            catch (System.Exception e)
            {
                Debug.LogError("Ошибка при загрузке файла гендера: " + e.Message);
                return "F";
            }
        }
        else
        {
            return "F";
        }
    }
}