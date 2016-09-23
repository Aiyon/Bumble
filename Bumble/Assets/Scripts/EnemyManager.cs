using UnityEngine;
using System.Collections;

public class EnemyManager : MonoBehaviour
{

    public int health;
    public int damage;
    public int moveSpeed;

    bool attacking;
    bool dead;
    public GameObject target;

	// Use this for initialization
	void Start ()
    {
        attacking = false;
        dead = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0)
        {
            dead = true;
        }
        else
        {
            if (target == null)
            {
                attacking = false;
                RaycastHit hit;
                if (Physics.Raycast(gameObject.transform.position, gameObject.transform.position * -1, out hit, 20.0f) && hit.transform.tag != "Empty")
                {
                    target = hit.transform.parent.gameObject;
                }
                else
                {
                    moveTowards(Vector3.zero);
                }
            }
            else
            {
                if (!attacking)
                {
                    moveTowards(target.transform.position);
                }
                else
                {
                    attack(target);
                }
                Vector3 dist = target.transform.position - gameObject.transform.position;
                if (dist.sqrMagnitude <= 0.5f) attacking = true;
            }
        }

    }

    void moveTowards(Vector3 dir)
    {
        Vector3 newPos = gameObject.transform.position;
        Vector3 move = dir - gameObject.transform.position; move.Normalize();

        newPos.x += move.x * moveSpeed*Time.deltaTime; newPos.y += move.y * moveSpeed * Time.deltaTime;
        gameObject.transform.position = newPos;

        //set z rotation
        Vector3 rot = transform.rotation.eulerAngles;

        float ratio = Mathf.Abs(move.y);
        if (move.x == 0)
        {
            if (move.y > 0) rot.z = 180;
            else rot.z = 0;
        }
        else if(move.y == 0)
        {
            rot.z = 90;
        }
        else
        {
            ratio /= Mathf.Abs(move.x);
            if (ratio >= 1) rot.z = ratio * 45;
            else rot.z = 90 - (ratio * 45);
        }
        if(move.y > 0) rot.z = 180 - rot.z;
        if (move.x < 0) rot.z *= -1;

        gameObject.transform.rotation = Quaternion.Euler(rot);
    }

    void attack(GameObject gamObj)
    {
        if (target.tag == "Cell")
            target.GetComponent<CellManager>().deltaHealth(damage * -1);
        else if(target.tag == "Queen")
            target.GetComponent<QueenManager>().deltaHealth(damage * -1);
    }

    public int getHealth()
    { return health; }

    public void setHealth(int i)
    { health += i; }
}
