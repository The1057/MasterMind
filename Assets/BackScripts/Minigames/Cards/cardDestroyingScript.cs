using UnityEngine;

public class cardDestroyingScript : MonoBehaviour
{
    public cardContainerScript cardContainer;
    public bool funDestruction = true;
    public float shrinkSpeed = 0.95f;
    public float destructionThreshhold = 0.1f;
    void Update()
    {
        if (cardContainer.containedCard != null)
        {
            if (funDestruction)
            {
                cardContainer.containedCard.transform.localScale *= shrinkSpeed;
                if (cardContainer.containedCard.transform.localScale.x < destructionThreshhold)
                {
                    Destroy(cardContainer.containedCard.gameObject);
                    cardContainer.containedCard = null;
                }
            }
            else
            {
                Destroy(cardContainer.containedCard.gameObject);
                cardContainer.containedCard = null;

            }
        }
    }
}
