using UnityEngine;
using Mirror;

public class walkDustScript : NetworkBehaviour
{
    private Animator anim;
    private bool hasTriggeredDestroy = false;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!hasTriggeredDestroy && 
            anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f && 
            !anim.IsInTransition(0))
        {
            hasTriggeredDestroy = true;
            // If we're on the server, destroy directly
            if (isServer)
            {
                NetworkServer.Destroy(gameObject);
            }
            // If we're on the client, request destruction from server
            else
            {
                CmddestroyThis();
            }
        }
    }

    [Command(requiresAuthority = false)]
    private void CmddestroyThis()
    {
        NetworkServer.Destroy(gameObject);
    }
}