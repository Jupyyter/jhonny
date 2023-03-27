using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JSTriggerScript : MonoBehaviour
{
    [SerializeField] private JhonnyScript JS;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "ladder")
        {
            JS.ladder = true;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "ladder")
        {
            JS.ladder = false;
        }
    }
}
