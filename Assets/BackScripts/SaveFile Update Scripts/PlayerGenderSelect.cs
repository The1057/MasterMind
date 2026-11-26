using UnityEngine;
using UnityEngine.UI;

public static class GenderColorApplier
{
    // ÷вета подставьте свои
    static readonly Color maleOutline = new Color32(0, 205, 255, 255);
    static readonly Color maleMain = new Color32(23, 23, 46, 255);
    static readonly Color femaleOutline = new Color32(255, 123, 255, 255);
    static readonly Color femaleMain = new Color32(55, 0, 46, 255);

    public static void Apply(bool isMale)
    {
        Color outlineColor = isMale ? maleOutline : femaleOutline;
        Color mainColor = isMale ? maleMain : femaleMain;

        // true / FindObjectsInactive.Include Ч чтобы найти и выключенные объекты
        var outlines = Object.FindObjectsByType<Outline>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (var outline in outlines)
        {
            // только белые outline
            if (outline.effectColor != Color.white) continue;

            outline.effectColor = outlineColor;

            // основной цвет: Image, Text и т.д. на этом же объекте
            var graphic = outline.GetComponent<Graphic>();
            if (graphic != null) graphic.color = mainColor;
        }
    }
}
public class PlayerGenderSelect : MonoBehaviour
{
    public playerDataClass playerData;

    public Button maleButton;
    public Button femaleButton;


    public void selectMale()
    {
        playerData.data.player_gender = "M";
        GenderColorApplier.Apply(true);
    }
    public void selectFemale()
    {
        playerData.data.player_gender = "F";
        GenderColorApplier.Apply(false);
    }
}