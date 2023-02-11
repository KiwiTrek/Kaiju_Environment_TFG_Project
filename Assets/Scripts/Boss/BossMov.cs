using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMov : MonoBehaviour
{
    // Start is called before the first frame update
    public bool attacking;
    public Animator animator;

    int isAttackingHash;
    void Start()
    {
        isAttackingHash = Animator.StringToHash("isAttacking");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            attacking = !attacking;
            animator.SetBool(isAttackingHash, attacking);
        }
    }
}
