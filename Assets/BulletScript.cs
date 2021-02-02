using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script is given to the ALL Bullet prefabs////////////////////////////////////
public class BulletScript : MonoBehaviour
{
    public float bulletAngle;
    public float angleTemp;
    public float bulletDamage;
    public float bulletSpeed;
    public float bulletRange;

    public float deltaTime;

    public string sortingLayer;

    Vector3 lastPosition;
    Vector3 currentPosition;

    public Rigidbody2D rb;

    private SpriteRenderer sort;

    Vector2 movement;


    // Start is called before the first frame update
    void Start()
    {
        lastPosition = transform.position;
        
        //Sorting Layer
        sort = GetComponent<SpriteRenderer>();
        //sort.sortingLayerName = sortingLayer;
    }

    // Update is called once per frame
    void Update()
    {
        //Sorting Order
        sort.sortingOrder = 100000 - Mathf.RoundToInt(transform.position.y);
        float height = sort.bounds.extents.x; //Distance to the top, from your center point
        sort.sortingOrder = 100000 - Mathf.RoundToInt(transform.position.y) - Mathf.RoundToInt(height)*2;

        //Current position
        currentPosition = transform.position;

        //bulletAngle = angleTemp * (Mathf.PI/180);
        movement.x = Mathf.Sin(-bulletAngle - (Mathf.PI / 2));
        movement.y = Mathf.Cos(-bulletAngle - (Mathf.PI / 2));
        //Debug.Log(bulletAngle);

        if (Vector2.Distance(lastPosition, currentPosition) > bulletRange)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Collision")
        {
            Destroy(gameObject);
        }
    }

    void FixedUpdate()
    {
        //Movement
        rb.MovePosition(rb.position + (movement * -bulletSpeed * Time.fixedDeltaTime));
    }
}
