using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Ce script centralise les actions qui sont déclenchées par l'appui sur une touche
 */
public class KeyListener : MonoBehaviour
{

    public Animator animator;
    public DialogManager dialogManager;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetKeyDown("a"))
        {
            animator.SetTrigger("HandsForward");
        }

        if (Input.GetKeyDown("z"))
        {
            animator.SetTrigger("Acknowledging");
        }

        if (Input.GetKeyDown("e"))
        {
            animator.SetTrigger("Talking");
        }

        if (Input.GetKeyDown("s"))
        {
            dialogManager.PlayAudio(0);
        }

        if (Input.GetKeyDown("q"))
        {
            animator.SetTrigger("LookAway");
        }

    }
}
