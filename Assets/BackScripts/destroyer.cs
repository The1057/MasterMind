using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class destroyer : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject Destroyer;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Destroyer.CompareTag("Finish"))
        {
            Destroy(gameObject);
        }
    }
}
