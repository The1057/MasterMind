using UnityEngine;
using System.Collections.Generic;
public class AutoResSetter : MonoBehaviour
{
    public List<Texture> texturesForAutoRes;
    private void Start()
    {
        foreach (var texture in texturesForAutoRes)
        {
            texture.height = Screen.height;
            texture.width = Screen.width;
        }
    }
}
