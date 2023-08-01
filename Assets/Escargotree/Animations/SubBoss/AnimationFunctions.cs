using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationFunctions : MonoBehaviour
{
    [SerializeField] private BossSubBehaviour behaviour;
    public void ReturnToInitial()
    {
        behaviour.canReturn = true;
    }
}
