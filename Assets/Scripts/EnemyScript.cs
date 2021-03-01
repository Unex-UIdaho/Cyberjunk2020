using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script is given to the Enemy prefab////////////////////////////////////
public class EnemyScript : MonoBehaviour
{
    private SpriteRenderer sort;
    float deltaTime;

    int hit = 0;

    bool invincible = false;

    public float Health;

    //Enemy Weapon
    public GameObject weapon;
    private SpriteRenderer selfsort;
    Vector3 gunFlip;

    //Determines enemy speed
    public float moveSpeed = 5f;

    //Links player with rigibody 2D physics
    public Rigidbody2D rb;
    public Animator animator;

    float movementx, movementy;

    Vector2 movement;
    Vector3 player_position;

    public bool shoot = false;
    GunScript weaponScript;

    //Follow player
    GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        //Create gun
        weapon = Instantiate(weapon, transform.position, Quaternion.identity) as GameObject;
        weapon.gameObject.tag = "EnemyWeapon";
        weaponScript = weapon.gameObject.GetComponent<GunScript>();
        weaponScript.enemyParent = gameObject;
        player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (Health <= 0)
        {
            if (weapon)
            {
                Destroy(weapon);
            }
            Destroy(gameObject);
        }

        if (hit > 0)
        {
            if (Mathf.FloorToInt(hit/20) % 2 == 0)
            {
                GetComponent<Renderer>().material.color = new Color(0,0,0,0);
                weapon.GetComponent<Renderer>().material.color = new Color(0, 0, 0, 0);
            }
            else
            {
                GetComponent<Renderer>().material.color = Color.red;
                weapon.GetComponent<Renderer>().material.color = Color.red;
            }

            hit--;
            if (hit <= 0)
            {
                GetComponent<Renderer>().material.color = Color.white;
                if (weapon)
                    weapon.GetComponent<Renderer>().material.color = Color.white;
                invincible = false;
            }
        }

        selfsort = gameObject.GetComponent<SpriteRenderer>();
		  
		if (!player) {
			player = GameObject.FindWithTag("Player");
		}

        if (player)
        {
            player_position = player.gameObject.GetComponent<PlayerMovement>().transform.position;
        }

        if (player)
        {
            shoot = true;

            Vector2 enemyPoint = new Vector2(transform.position.x, transform.position.y);
            Vector2 playerPoint = new Vector2(player.transform.position.x, player.transform.position.y);

            RaycastHit2D[] hit = Physics2D.LinecastAll(enemyPoint, playerPoint);
            int arrayLength = hit.Length;

            //If something was hit, the RaycastHit2D.collider will not be null
            for (int i = 0; i < arrayLength; i++)
            {
                if (hit[i].collider != null)
                {
                    if (hit[i].collider.tag == "Collision")
                    {
                        Debug.Log(hit[i].collider.name);
                        shoot = false;
                    }
                }
            }

            if (invincible == true)
            {
                shoot = false;
            }
        }
        else
        {
            shoot = false;
        }

        //Movement AI Input
        movement.x = 0;
        movement.y = 0;

        movementx = player_position.x - transform.position.x;
        movementy = player_position.y - transform.position.y - 0.5f;

        animator.SetFloat("Horizontal", movementx);
        animator.SetFloat("Vertical", movementy);
        animator.SetFloat("Speed", movement.sqrMagnitude);

        animator.SetFloat("Latitude", movementx);
        animator.SetFloat("Longitude", movementy);

        //Weapon point system
        if (weapon)
        {
            Vector3 offset_right = new Vector3(0.5f, -0.5f, 0);
            Vector3 offset_left = new Vector3(-0.5f, -0.5f, 0);

            float rotation;

            if (Mathf.Sign(movementx) > 0)
            {
                //rotation = Quaternion.LookRotation(player_position - weapon.transform.position, weapon.transform.TransformDirection(Vector3.up));
                rotation = Mathf.Atan2((player_position.y - 0.75f) - weapon.transform.position.y, player_position.x - weapon.transform.position.x) * Mathf.Rad2Deg;
                weapon.transform.rotation = Quaternion.Euler(0, 0, rotation);
                weapon.transform.position = transform.position + offset_right;
            }
            else
            {
                //rotation = Quaternion.LookRotation(weapon.transform.position - player_position, weapon.transform.TransformDirection(Vector3.right));
                rotation = Mathf.Atan2(weapon.transform.position.y - (player_position.y - 0.75f), weapon.transform.position.x - player_position.x) * Mathf.Rad2Deg;
                weapon.transform.rotation = Quaternion.Euler(0, 0, rotation);
                weapon.transform.position = transform.position + offset_left;
            }

            //Weapon sorting layer
            sort = weapon.GetComponent<SpriteRenderer>();

            if (movementy > Mathf.Abs(movementx))
            {
                //sort.sortingLayerName = "Weapons_Behind";
                sort.sortingOrder = selfsort.sortingOrder - 1;
            }
            else
            {
                //sort.sortingLayerName = "Weapons_Front";
                sort.sortingOrder = selfsort.sortingOrder + 1;
            }

            gunFlip = weapon.transform.localScale;
            gunFlip.x = Mathf.Sign(movementx);

            weapon.transform.localScale = gunFlip;

            //Debug.Log(transform.position.y);
        }
    }

    void FixedUpdate()
    {
        //Movement
        rb.MovePosition(rb.position + (movement * moveSpeed * Time.fixedDeltaTime));
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Bullet" && invincible == false)
        {
            //col.gameObject.GetComponent<BulletScript>();
            BulletScript bulletScript = col.GetComponent<BulletScript>();

            Health -= bulletScript.bulletDamage;
            Destroy(col.gameObject);
            GetComponent<Renderer>().material.color = Color.red;

            deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
            float second = 1.0f / deltaTime;

            hit = 50;//second/10;
            invincible = true;
        }
    }
}