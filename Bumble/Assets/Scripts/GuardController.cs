using UnityEngine;
using System.Collections;

public class GuardController : MonoBehaviour {

    //update test
    float nextActionTime = 0.0f;
    float period = 0.25f;
    float moveSpeed = 0.25f;
    float time;
    GameObject scriptManager;
    GameObject target;
    bool attacking = false;
    bool noStinger = false;

	// Update is called once per frame
	void Update ()
    {
        if (attacking)
        {
            //attack enemy.
            target.GetComponent<EnemyManager>().setHealth(-1);

            //does stinging kill the guard?
            if (target.tag == "Large") noStinger = true;
            else if (target.tag == "Small")
            {
                if (Random.Range(0, 3) == 0)
                    noStinger = true;
            }

        }
        else if (target == null)
        {
            //wander hive, implement later.
        }
        else
        {
            time += Time.deltaTime;
            if (time > nextActionTime)
            {
                nextActionTime += period;
                moveTowards(target.transform.position);
                Vector3 dist = target.transform.position - gameObject.transform.position;
                if (dist.sqrMagnitude <= 0.5)
                {
                    attacking = true;
                }
            }
        }
    }

    void moveTowards(Vector3 dir)
    {
        Vector3 newPos = gameObject.transform.position;
        Vector3 move = dir - gameObject.transform.position; move.Normalize();

        newPos.x += move.x * moveSpeed /* Time.deltaTime*/; newPos.y += move.y * moveSpeed /* Time.deltaTime*/;
        gameObject.transform.position = newPos;

        //set z rotation
        Vector3 rot = transform.rotation.eulerAngles;

        float ratio = Mathf.Abs(move.y);
        if (move.x == 0)
        {
            if (move.y > 0) rot.z = 180;
            else rot.z = 0;
        }
        else if (move.y == 0)
        {
            rot.z = 90;
        }
        else
        {
            ratio /= Mathf.Abs(move.x);
            if (ratio >= 1) rot.z = ratio * 45;
            else rot.z = 90 - (ratio * 45);
        }
        if (move.y > 0) rot.z = 180 - rot.z;
        if (move.x < 0) rot.z *= -1;

        gameObject.transform.rotation = Quaternion.Euler(rot);
    }

    public void setTarget(GameObject gObj)
    {
        target = gObj;
    }

    public bool stinger()
    {
        return noStinger;
    }
}
