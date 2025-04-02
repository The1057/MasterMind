using UnityEngine;
using TMPro;

public class InputFieldFix : MonoBehaviour
{
    public TMP_InputField inputField;

    void Start()
    {
        if (inputField != null)
        {
            inputField.onValueChanged.AddListener(UpdateText);
        }
    }

    void UpdateText(string text)
    {
        inputField.textComponent.SetText(text); // Принудительное обновление
        inputField.textComponent.ForceMeshUpdate(); // Перерисовка текста
    }
}
