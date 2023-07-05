using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuitTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(Card.ParseCard(101, 7));
        Debug.Log(Card.ParseCard(102, 7));
        Debug.Log(Card.ParseCard(103, 7));
        Debug.Log(Card.ParseCard(104, 7));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
