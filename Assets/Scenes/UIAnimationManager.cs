using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class UIAnimationTarget
{
    public GameObject target;
    public Vector2 direction = Vector2.up;
    public float distance = 50f;
    public bool fadeOut = true; // true = исчезает, false = появляется
}

[System.Serializable]
public class UIAnimationPreset
{
    public string id; // Уникальное имя, например: "StartToLevelSelect"
    public List<UIAnimationTarget> targets = new List<UIAnimationTarget>();
    public float duration = 0.8f;
}

public class UIAnimationManager : MonoBehaviour
{
    public List<UIAnimationPreset> presets = new List<UIAnimationPreset>();

    private Dictionary<string, UIAnimationPreset> presetMap = new Dictionary<string, UIAnimationPreset>();
    public UIAnimationManager animationManager;

    void Awake()
    {
        // Собираем словарь для быстрого доступа
        foreach (var preset in presets)
        {
            if (!string.IsNullOrEmpty(preset.id))
            {
                presetMap[preset.id] = preset;
            }
        }
    }

    // Этот метод можно вызывать из OnClick кнопки!
    public void PlayAnimation(string animationId)
    {
        if (presetMap.TryGetValue(animationId, out UIAnimationPreset preset))
        {
            StartCoroutine(RunPreset(preset));
        }
        else
        {
            Debug.LogError($"Animation preset '{animationId}' not found!");
        }
    }

    // Метод с колбэком — для интеграции с CanvasSwitcher1
    public void PlayAnimation(string animationId, System.Action onComplete)
    {
        if (presetMap.TryGetValue(animationId, out UIAnimationPreset preset))
        {
            StartCoroutine(RunPreset(preset, onComplete));
        }
        else
        {
            Debug.LogError($"Animation preset '{animationId}' not found!");
            onComplete?.Invoke();
        }
    }

    private IEnumerator RunPreset(UIAnimationPreset preset, System.Action onComplete = null)
    {
        List<CanvasRenderer> renderers = new List<CanvasRenderer>();
        List<Vector3> startPos = new List<Vector3>();
        List<Color> startColors = new List<Color>();

        // Подготовка
        foreach (var target in preset.targets)
        {
            if (target.target == null) continue;

            CanvasRenderer cr = target.target.GetComponent<CanvasRenderer>();
            if (cr == null)
            {
                Debug.LogWarning($"CanvasRenderer not found on {target.target.name}");
                renderers.Add(null);
                startPos.Add(Vector3.zero);
                startColors.Add(Color.white);
                continue;
            }

            renderers.Add(cr);
            startPos.Add(target.target.transform.position);
            startColors.Add(cr.GetColor());
            target.target.SetActive(true); // должен быть активен для анимации
        }

        float elapsed = 0f;
        while (elapsed < preset.duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / preset.duration;

            for (int i = 0; i < preset.targets.Count; i++)
            {
                var anim = preset.targets[i];
                if (anim.target == null || renderers[i] == null) continue;

                Vector3 moveDir = new Vector3(anim.direction.x, anim.direction.y, 0f).normalized * anim.distance;
                anim.target.transform.position = Vector3.Lerp(startPos[i], startPos[i] + moveDir, t);

                float startAlpha = anim.fadeOut ? 1f : 0f;
                float endAlpha = anim.fadeOut ? 0f : 1f;
                Color newColor = startColors[i];
                newColor.a = Mathf.Lerp(startAlpha, endAlpha, t);
                renderers[i].SetColor(newColor);
            }

            yield return null;
        }

        // Финал
        for (int i = 0; i < preset.targets.Count; i++)
        {
            var anim = preset.targets[i];
            if (anim.target == null || renderers[i] == null) continue;

            Vector3 moveDir = new Vector3(anim.direction.x, anim.direction.y, 0f).normalized * anim.distance;
            anim.target.transform.position = startPos[i] + moveDir;

            Color finalColor = startColors[i];
            finalColor.a = anim.fadeOut ? 0f : 1f;
            renderers[i].SetColor(finalColor);
        }

        onComplete?.Invoke();
    }
}