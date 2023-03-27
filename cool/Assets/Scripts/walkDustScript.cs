using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class walkDustScript : NetworkBehaviour
{
    private Animator anim;
    private void Start()
    {
        anim = GetComponent<Animator>();
    }
    private void Update()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !anim.IsInTransition(0))//if animation over IT DIES 💀:)
        {
            CmddestroyThis();
            Destroy(gameObject);
        }
    }
    [Command(requiresAuthority = false)]
    private void CmddestroyThis()
    {
        NetworkServer.Destroy(gameObject);
    }
}
