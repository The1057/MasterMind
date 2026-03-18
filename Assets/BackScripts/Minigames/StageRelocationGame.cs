using UnityEngine;
using System.Collections.Generic;
using System.Linq;
public class StageRelocationGame : MonoBehaviour
{
    [Tooltip("Карты в правильном порядке")]
    public List<GameObject> cards;
    public GameObject container;

    public List<GameObject> _cards;
    public GameObject cardContainerPrefab;
    void Start()
    {
        foreach (GameObject card in cards)
        {            
            _cards.Add(Instantiate(card, container.transform));
            _cards.Last().GetComponent<StageCard>().container = _cards.Last().transform.parent.GetComponent<StageContainer>();
        }
        shuffleCards();
    }

    void Update()
    {
        
    }

    [ContextMenu("Shuffle Cards")]
    public void shuffleCards()
    {
        Shuffle(_cards);
        for(int i=0;i<_cards.Count;i++)
        {
            _cards[i].transform.SetSiblingIndex(i);
        }
    }
    private void Shuffle<T>(IList<T> list)
    {
        int n = list.Count;
        System.Random rng = new System.Random();
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
