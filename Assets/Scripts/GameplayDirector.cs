using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayDirector : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {


        //SOURCE: https://answers.unity.com/questions/1197217/can-a-mesh-collider-work-with-an-animated-skinned.html
        SkinnedCollisionHelper[] items = FindObjectsOfType<SkinnedCollisionHelper>();
        foreach (SkinnedCollisionHelper item in items)
        {
            item.UpdateCollisionMesh();
        }
    }
}
