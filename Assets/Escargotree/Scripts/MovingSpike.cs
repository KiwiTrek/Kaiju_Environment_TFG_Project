using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingSpike : MonoBehaviour
{
    // Start is called before the first frame update

    public float speed = 1.0f;
    public Vector3 lowDecrease = Vector3.zero;
    public Vector3 highIncrease = Vector3.zero;

    bool reverse = true;
    Vector3 lowPos = Vector3.zero;
    Vector3 highPos = Vector3.zero;
    void Start()
    {
        lowPos = transform.localPosition + lowDecrease;
        highPos = transform.localPosition + highIncrease;

        transform.localPosition = lowPos;
        reverse = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (reverse)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, highPos, Time.deltaTime * speed);
            if (Vector3.Distance(transform.localPosition, highPos) <= 0.001f)
            {
                reverse = false;
            }
        }
        else
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, lowPos, Time.deltaTime * speed);
            if (Vector3.Distance(transform.localPosition, lowPos) <= 0.001f)
            {
                reverse = true;
            }
        }
    }
}
