using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartController : MonoBehaviour
{
    public GameObject heartPrefab;
    List<GameObject> hearts = new List<GameObject>();

    public void CreateHeart(int number)
    {
        for (int i = 0; i != number; i++)
        {
            GameObject newHeart = Instantiate(heartPrefab, transform);
            hearts.Add(newHeart);
        }
    }
    public void DestroyHeart(int number)
    {
        for (int i = 0; i != number; i++)
        {
            Destroy(hearts[hearts.Count - 1]);
        }
    }
    public void ClearHearts()
    {
        foreach (GameObject heart in hearts)
        {
            Destroy(heart);
        }
        hearts = new List<GameObject>();
    }
}
