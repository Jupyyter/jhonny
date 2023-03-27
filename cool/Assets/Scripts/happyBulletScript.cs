using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
//(◣_◢)ノ-=≡≡卍
public class happyBulletScript : NetworkBehaviour
{
    private float speed = 37;
    private float number = 1;
    private Rigidbody2D rb;
    [SerializeField] private GameObject happyend;
    private DeadTile DT;
    private BoxCollider2D BX;
    private DoorScript DS;
    private GangsterScript GS;
    [SerializeField] Transform truTransform;
    [SerializeField] Transform d2;
    [SerializeField] Transform d3;
    private void Start()
    {
        BX = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = transform.right * speed;
    }
    private void Update()
    {
        if (!GetComponent<Renderer>().isVisible)//if the bullet is not on the screen....
        {
            number -= Time.deltaTime;
            if (number < 0)//and 1 second passes....
            {
                Destroy(gameObject);//IT DIES ☠️:):):):)
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag != "Jhonny")
        {
            if (col.gameObject.GetComponent<CompositeCollider2D>() != null && !col.isTrigger && col.gameObject.GetComponent<PlatformEffector2D>() == null)//walls
            {
                DT = col.GetComponent<DeadTile>();
                DT.giveTiles(truTransform.position);
                DT.giveTiles(d2.position);
                DT.giveTiles(d3.position);
                //yandere dev code
                DT.giveTiles(this.transform.position);
                DT.giveTiles(this.transform.position);
                DT.giveTiles(this.transform.position);
                DT.giveTiles(this.transform.position);
                CmdendBullet();
                //Destroy(gameObject);
            }
            else if (!col.isTrigger && col.gameObject.GetComponent<Animator>() == null && col.GetComponent<DoorScript>() != null)//door
            {
                DS = col.GetComponent<DoorScript>();
                DS.DoorDamage();
                CmdendBullet();
            }
            else if (col.gameObject.GetComponent<Animator>() != null)//gangster
            {
                GS = col.GetComponent<GangsterScript>();
                GS.takeDMG();
                CmdendBullet();
            }
        }
    }
    [Command(requiresAuthority = false)]
    private void CmdendBullet()
    {
        GameObject happyendPref = Instantiate(happyend, truTransform.position, transform.rotation);
        NetworkServer.Spawn(happyendPref);
        NetworkServer.Destroy(gameObject);
    }
}
/*
⠄⣾⣿⡇⢸⣿⣿⣿⠄⠈⣿⣿⣿⣿⠈⣿⡇⢹⣿⣿⣿⡇⡇⢸⣿⣿⡇⣿⣿⣿
⢠⣿⣿⡇⢸⣿⣿⣿⡇⠄⢹⣿⣿⣿⡀⣿⣧⢸⣿⣿⣿⠁⡇⢸⣿⣿⠁⣿⣿⣿
⢸⣿⣿⡇⠸⣿⣿⣿⣿⡄⠈⢿⣿⣿⡇⢸⣿⡀⣿⣿⡿⠸⡇⣸⣿⣿⠄⣿⣿⣿
⢸⣿⡿⠷⠄⠿⠿⠿⠟⠓⠰⠘⠿⣿⣿⡈⣿⡇⢹⡟⠰⠦⠁⠈⠉⠋⠄⠻⢿⣿
⢨⡑⠶⡏⠛⠐⠋⠓⠲⠶⣭⣤⣴⣦⣭⣥⣮⣾⣬⣴⡮⠝⠒⠂⠂⠘⠉⠿⠖⣬
⠈⠉⠄⡀⠄⣀⣀⣀⣀⠈⢛⣿⣿⣿⣿⣿⣿⣿⣿⣟⠁⣀⣤⣤⣠⡀⠄⡀⠈⠁
⠄⠠⣾⡀⣾⣿⣧⣼⣿⡿⢠⣿⣿⣿⣿⣿⣿⣿⣿⣧⣼⣿⣧⣼⣿⣿⢀⣿⡇⠄
⡀⠄⠻⣷⡘⢿⣿⣿⡿⢣⣾⣿⣿⣿⣿⣿⣿⣿⣿⣿⣜⢿⣿⣿⡿⢃⣾⠟⢁⠈
⢃⢻⣶⣬⣿⣶⣬⣥⣶⣿⣿⣿⣿⣿⣿⢿⣿⣿⣿⣿⣿⣷⣶⣶⣾⣿⣷⣾⣾⢣
⡄⠈⢿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣷⣼⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⡏⠘
⣿⡐⠘⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⠟⢠⢃
⣿⣷⡀⠈⠻⣿⣿⣿⣿⣿⣿⣿⣿⠿⠿⠿⠿⢿⣿⣿⣿⣿⣿⣿⣿⡿⠋⢀⠆⣼
⣿⣿⣷⡀⠄⠈⠛⢿⣿⣿⣿⣿⣷⣶⣶⣶⣶⣶⣿⣿⣿⣿⣿⠿⠋⠠⠂⢀⣾⣿
⣿⣿⣿⣧⠄⠄⢵⢠⣈⠛⠿⣿⣿⣿⣿⣿⣿⣿⣿⣿⠟⢋⡁⢰⠏⠄⠄⣼⣿⣿
⢻⣿⣿⣿⡄⢢⠨⠄⣯⠄⠄⣌⣉⠛⠻⠟⠛⢋⣉⣤⠄⢸⡇⣨⣤⠄⢸⣿⣿⣿
*/