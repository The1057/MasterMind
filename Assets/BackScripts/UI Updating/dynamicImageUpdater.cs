using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ProfilePicUpload : MonoBehaviour
{
    public Image targetImage;
    public List<Sprite> imageVariants = new List<Sprite>();
    public int imageIndex = 0;
    void Start()
    {
        if(imageVariants.Count > imageIndex)
        {
            targetImage.sprite = imageVariants[imageIndex];
        }
    }
    void Update()
    {
        if (imageVariants.Count  > imageIndex)
        {
            targetImage.sprite = imageVariants[imageIndex];
        }
    }
}
