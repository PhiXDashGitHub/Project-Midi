using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CounterGraphic : MonoBehaviour
{
    public GameObject Counter;

    void Update()
    {
        transform.position = new Vector2(Counter.transform.position.x, transform.position.y);
    }
}
