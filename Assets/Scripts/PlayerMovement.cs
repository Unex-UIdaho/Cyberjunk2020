using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script is given to the Player prefab////////////////////////////////////
public class PlayerMovement : MonoBehaviour
{
    //Properites
    public float Health = 50; //Total HP
    public float moveSpeed = 5f; //Determines player speed
    private SpriteRenderer selfsort; //Renders player sprite - used to read player's layer

    //Player Weapon
    public GameObject weapon; //This contains the weapon object
    private SpriteRenderer sort; //This will render the weapon sprite
    Vector3 gunFlip; //This will be used to determine if the gun should be flipped (if the player is looking left)

    //Conditions
    int hit = 0; //acts as a counter to turn red, then back to normal
    float movementx, movementy; //These variables determine the direction the player is looking
    bool invincible = false; //This determines of the player is invincible

    //Links player with rigibody 2D physics
    public Rigidbody2D rb; //This contains the player's rigidbody
    public Animator animator; //This contains the player's animator, which contains the players animations

    Vector2 movement; //This will be used to determine the players movement (x and y movement)
    Vector3 mouse_position; //This will be used to determine the mouse's coordinates

    // Start is called before the first frame update
    void Start()
    {
        //Create gun
        weapon = Instantiate(weapon, transform.position, Quaternion.identity) as GameObject;
    }

    // Update is called once per frame
    void Update()
    {
        //Check if dead
        if (Health <= 0)
        {
            if (weapon)
            {
                Destroy(weapon);
            }
            Destroy(gameObject);
        }

        //If hit, wait then turn back to normal
        if (hit > 0)
        {
            if (Mathf.FloorToInt(hit / 20) % 2 == 0)
            {
                GetComponent<Renderer>().material.color = new Color(0, 0, 0, 0);
                if (weapon)
                    weapon.GetComponent<Renderer>().material.color = new Color(0, 0, 0, 0);
            }
            else
            {
                GetComponent<Renderer>().material.color = Color.red;
                if (weapon)
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

        selfsort = gameObject.GetComponent<SpriteRenderer>(); //Used to find sorting order

        mouse_position = Camera.main.ScreenToWorldPoint(Input.mousePosition); //This is the mouse position

        //Input
        movement.x = Input.GetAxisRaw("Horizontal"); //Input for x movement
        movement.y = Input.GetAxisRaw("Vertical"); //Input for y movement

        movementx = mouse_position.x - transform.position.x; //Gets x direction from mouse
        movementy = mouse_position.y - transform.position.y; //Gets y direction from mouse

        //Setting Animation Values
        animator.SetFloat("Horizontal", movementx); //Moves based on mouse.x position, not movement
        animator.SetFloat("Vertical", movementy); //Moves based on mouse.y position, not movement
        animator.SetFloat("Speed", movement.sqrMagnitude); //Give Animator Speed, player speed

        animator.SetFloat("Latitude", movementx); //This determines last direction for idle position
        animator.SetFloat("Longitude", movementy); //This determines last direction for idle position

        //Weapon point system
        if (weapon)
        {
            Vector3 offset_right = new Vector3(0.5f, -0.5f, 0); //Offset for the weapon on the right hand
            Vector3 offset_left = new Vector3(-0.5f, -0.5f, 0); //Offset for the weapon on the left hand

            float rotation;

            //If player is moving right,
            if (Mathf.Sign(movementx) > 0)
            {
                //Rotate the gun towards the mouse
                rotation = Mathf.Atan2((mouse_position.y - 0.3f) - weapon.transform.position.y, mouse_position.x - weapon.transform.position.x) * Mathf.Rad2Deg;
                weapon.transform.rotation = Quaternion.Euler(0, 0, rotation); //Apply the rotation to the gun

                //Offset gun position
                weapon.transform.position = transform.position + offset_right;
            }
            else
            {
                //Rotate the gun towards the mouse
                rotation = Mathf.Atan2(weapon.transform.position.y - (mouse_position.y - 0.3f), weapon.transform.position.x - mouse_position.x) * Mathf.Rad2Deg;
                weapon.transform.rotation = Quaternion.Euler(0, 0, rotation); //Apply the rotation to the gun

                //Offset gun position
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
        //Bullet Collision
        if (col.gameObject.tag == "EnemyBullet" && invincible == false)
        {
            BulletScript bulletScript = col.GetComponent<BulletScript>();

            // Health -= bulletScript.bulletDamage;
            Destroy(col.gameObject);
            GetComponent<Renderer>().material.color = Color.red;

            hit = 100;
            invincible = true;
        }
    }
}
