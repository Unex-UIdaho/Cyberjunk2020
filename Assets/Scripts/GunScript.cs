using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// worked on by
// Ezequiel
// Merry

//This script is given to ALL Gun prefabs////////////////////////////////////
public class GunScript : MonoBehaviour
{
    public GameObject Bullet;
    private ScreenShake Shaker;
    public float duration = 10;

    public float intensity;
    public bool automaticAssault;
    public float assaultDelay;
    public float bulletDamage;
    public float bulletSpeed;
    public float bulletRange;
    public float bulletSpread = 1;
	public float bulletSpreadAngle = 0; // default 6 degrees
	public float bulletAccel = 0;
	public float bulletAngVel = 0;
    public bool bulletSpreadRandom;

    float tempAssault = 0;

    float width;
    float height;
    float angle_width;
    float angle_height;
    float offsetx_width = 0;
    float offsety_width = 0;
    float offsetx_height = 0;
    float offsety_height = 0;
    float sign;
    bool shoot = false;

    float offset_angle = 0;
    float offset_angle_range = 10.0f;

    private SpriteRenderer sprite;
    //public Sprite bulletImage;

    public GameObject enemyParent;
    float semi_delay = 100;
    float semi_temp = 100;

    // Start is called before the first frame update
    void Start()
    {
        //Size of weapon
        sprite = GetComponent<SpriteRenderer>(); //Set the reference to our SpriteRenderer component
        width = sprite.bounds.extents.x * 2; //Distance to the right side * 2, from your center point
        height = sprite.bounds.extents.x * 2; //Distance to the top * 2, from your center point

        Shaker = Object.FindObjectOfType<ScreenShake>();
    }

    void Update()
    {
        if (automaticAssault == true)
        {
            tempAssault--;
        }

        if (tag == "EnemyWeapon")
        {
            if (enemyParent)
            {
                semi_temp--;
                EnemyScript enemyScript = enemyParent.gameObject.GetComponent<EnemyScript>();
                shoot = enemyScript.shoot;
            }
        }

        //Shoot Action
        if ((((Input.GetMouseButtonDown(0) && automaticAssault == false) ||
            (Input.GetMouseButtonDown(0) && automaticAssault == true && tempAssault <= 0) ||
            (Input.GetMouseButton(0) && automaticAssault == true && tempAssault <= 0)) && tag == "Weapon") ||
            (shoot == true && semi_temp <= 0 && (automaticAssault == false) ||
			(shoot == true && automaticAssault == true && tempAssault <= 0) && tag == "EnemyWeapon")) {
            if (tag == "Weapon")
            {
                Shaker.Shake(duration, intensity);
            }

            if (automaticAssault == true)
            {
                tempAssault = assaultDelay;
            }

            if (tag == "EnemyWeapon")
            {
                semi_temp = semi_delay;
            }

            float pos_x = transform.position.x;
            float pos_y = transform.position.y;

            if (Mathf.Sign(transform.localScale.x) > 0)
            {
                sign = 1;

                angle_width = transform.rotation.eulerAngles.z * (Mathf.PI / 180);
                angle_height = (90 + transform.rotation.eulerAngles.z) * (Mathf.PI / 180);

                offsetx_width = Mathf.Cos(angle_width);
                offsety_width = Mathf.Sin(angle_width);

                offsetx_height = Mathf.Cos(angle_height);
                offsety_height = Mathf.Sin(angle_height);
            }
            else
            {
                sign = -1;

                angle_width = (transform.rotation.eulerAngles.z + 180) * (Mathf.PI / 180);
                angle_height = (90 + (transform.rotation.eulerAngles.z + 180)) * (Mathf.PI / 180);

                offsetx_width = Mathf.Cos(angle_width);
                offsety_width = Mathf.Sin(angle_width);

                offsetx_height = Mathf.Cos(angle_height);
                offsety_height = Mathf.Sin(angle_height);
            }

            for (int i = 0; i < bulletSpread; i++)
            {
				float tempAngle = 0;
                // Instantiate at position (0, 0, 0) and zero rotation.
                GameObject bullet = Instantiate(Bullet, new Vector3(pos_x + (offsetx_width * (width / 2)) + (sign * offsetx_height * (height / 6)), pos_y + (offsety_width * (width / 2)) + (sign * offsety_height * (height / 6)), 0), transform.rotation) as GameObject;

                if (gameObject.tag == "EnemyWeapon")
                {
                    bullet.gameObject.tag = "EnemyBullet";
                }

                SpriteRenderer bulletRender = bullet.GetComponent<SpriteRenderer>();
                BulletScript bulletScript = bullet.GetComponent<BulletScript>();

                //Bullet Image
                //bulletRender.sprite = bulletImage;

                //Bullet Angle
                float bulletAngle;

                if (Mathf.Sign(transform.localScale.x) > 0)
                {
                    bulletAngle = transform.rotation.eulerAngles.z;
                }
                else
                {
                    bulletAngle = transform.rotation.eulerAngles.z + 180;
                }

                //Bullet Spread
				if (bulletSpreadAngle <= 0) {
					bulletSpreadAngle = 6;
				}
				
				if (bulletSpread > 1) {
					tempAngle = (i / (bulletSpread - 1) * bulletSpread * bulletSpreadAngle) - bulletSpread * bulletSpreadAngle / 2;
				}

                //Offset Bullet Angle
                if (bulletSpreadRandom == true)
                {
                    offset_angle = Random.Range(-offset_angle_range, offset_angle_range);
                }
                else
                {
                    offset_angle = 0;
                }

                //Data given to bullet

                bulletScript.bulletAngle = (bulletAngle + tempAngle + offset_angle) * (Mathf.PI / 180);
                bulletScript.transform.localScale = transform.localScale;

                //bullet.transform.Rotate(45, 0, 0);

                bullet.transform.eulerAngles = new Vector3(
                    bullet.transform.eulerAngles.x,
                    bullet.transform.eulerAngles.y,
                    bullet.transform.eulerAngles.z + tempAngle + offset_angle
                );

                bulletScript.bulletDamage = bulletDamage;
                bulletScript.bulletSpeed = bulletSpeed;
                bulletScript.bulletRange = bulletRange;
                bulletScript.bulletAccel = bulletAccel;
                bulletScript.bulletAngVel = bulletAngVel;

                //bulletScript.sortingLayer = sprite.sortingLayerName;
            }
        }
    }
}
