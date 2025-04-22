using Unity.VisualScripting;
using UnityEngine;

public class flagTrackingScript : MonoBehaviour
{
    //public Camera cam;
    public GameObject facingObject;
    public GameObject target;
    public SpriteRenderer spriteRenderer;
    public bool shrinkFlag = false, enlargeFlag = false, blinkFlag = false;
    public float lowerMargin = 0.7f, higherMargin = 1.3f;
    public float blinkingSpeed = 0.5f;

    // Update is called once per frame
    private void Start()
    {
        target = Camera.main.GameObject();
    }
    void Update()
    {
        facingObject.transform.LookAt(target.transform.position);
        if (enlargeFlag)
        {
            spriteRenderer.transform.localScale += Vector3.one * Time.deltaTime * blinkingSpeed;
        }
        else if (shrinkFlag)
        {
            spriteRenderer.transform.localScale -= Vector3.one * Time.deltaTime * blinkingSpeed;
        }
        if (blinkFlag)
        {
            if (spriteRenderer.transform.localScale.y > higherMargin || spriteRenderer.transform.localScale.y < lowerMargin)
            {
                enlargeFlag = !enlargeFlag;
                shrinkFlag = !shrinkFlag;
            }
            if(spriteRenderer.transform.localScale.y > higherMargin)
            {
                spriteRenderer.transform.localScale = Vector3.one * higherMargin;
                enlargeFlag = false;
                shrinkFlag = true;
            }
            else if (spriteRenderer.transform.localScale.y < lowerMargin)
            {
                spriteRenderer.transform.localScale = Vector3.one * lowerMargin;
                enlargeFlag = true;
                shrinkFlag = false;
            }
        }
        shrinkFlag = !enlargeFlag;
    }
}
