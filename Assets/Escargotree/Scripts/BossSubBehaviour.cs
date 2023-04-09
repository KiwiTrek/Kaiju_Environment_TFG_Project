using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSubBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject playerTarget;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       gameObject.transform.LookAt(playerTarget.transform.position);
    }
}
