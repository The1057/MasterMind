using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ArchivePlateUI : MonoBehaviour
{
    public TMP_Text titleText;
    public CanvasGroup leftArea;  // Навесь CanvasGroup на левую часть/кнопку
    public CanvasGroup rightArea; // Навесь CanvasGroup на правую часть/кнопку
    public CanvasGroup wholePlateGroup; // CanvasGroup на корне плашки

    public void SetState(bool leftDone, bool rightDone, bool isCurrent)
    {
        // Затемнение отдельных областей (0.23f если пройдено, 1.0f если нет)
        if (leftArea != null) leftArea.alpha = leftDone ? 0.23f : 1.0f;
        if (rightArea != null) rightArea.alpha = rightDone ? 0.23f : 1.0f;

        // Если обе стороны пройдены — вся плашка становится тусклой
        if (wholePlateGroup != null)
        {
            wholePlateGroup.alpha = (leftDone && rightDone) ? 0.23f : 1.0f;
            // Если плашка не текущая и не пройдена (будущая) — скрываем её совсем
            // Но мы будем управлять этим из менеджера черезSetActive
        }
    }
}