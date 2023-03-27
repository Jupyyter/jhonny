using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ladderScript : MonoBehaviour
{
    private float direction = 0;
    PlatformEffector2D pf;
    JhonnyScript JS;
    private void Start()
    {
        pf = GetComponent<PlatformEffector2D>();
        //JS = FindObjectOfType<JhonnyScript>();
    }
    /*private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "JSTrigger")
        {
            JS.ladder = true;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        pf.rotationalOffset = 0;
        if (other.tag == "JSTrigger")
        {
            JS.ladder = false;
        }
    }*/
    private void Update()
    {
        direction = Input.GetAxisRaw("Vertical");
        if (direction < 0&&pf.colliderMask!= 1 << LayerMask.NameToLayer("JSTrigger"))
        {
            StartCoroutine(deactivate());
        }
    }
    IEnumerator deactivate(){
        //pf.surfaceArc = 0;
        pf.colliderMask= 1 << LayerMask.NameToLayer("JSTrigger");
        yield return new WaitForSeconds(0.3f);
        pf.colliderMask= 1 << LayerMask.NameToLayer("Jhonny")| (1 << LayerMask.NameToLayer("JSTrigger"));
        //pf.surfaceArc = 179;
    }
}
