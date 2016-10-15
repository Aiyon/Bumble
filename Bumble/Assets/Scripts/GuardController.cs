using UnityEngine;
using System.Collections;

public class GuardController : MonoBehaviour {

    //update test
    float nextActionTime = 0.0f;
    float period = 1.0f;
    float moveSpeed = 1.2f;
    float time;
    GameObject scriptManager;
    GameObject target;
    bool attacking = false;
    bool noStinger = false;
    Vector3 wibble = Vector3.zero; //current move destination if no target.
    public GameObject sprite;

	// Update is called once per frame
	void Update ()
    {
        if (attacking)
        {
            time += Time.deltaTime;
            if (time > nextActionTime)
            {
                nextActionTime += (period*0.75f);
                //attack enemy.
                target.GetComponent<EnemyManager>().setHealth(-1);

                //does stinging kill the guard?
                if (target.tag == "Large") noStinger = true;
                else if (target.tag == "Small")
                {
                    if (Random.Range(0, 3) == 0)
                        noStinger = true;
                }

                attacking = false;
            }

        }
        else if (target == null)
        {
            time += Time.deltaTime;
            if (time > nextActionTime)
            {
                nextActionTime += period;
                wibble = Vector3.zero;
                float l = scriptManager.GetComponent<ResourceManager>().getLeft();
                float r = scriptManager.GetComponent<ResourceManager>().getRight();
                wibble.x = Random.Range(l, r);
                float t = scriptManager.GetComponent<ResourceManager>().getTop();
                float b = scriptManager.GetComponent<ResourceManager>().getBot();
                wibble.y = Random.Range(b, t);
            }
            moveTowards(wibble);
        }
        else
        {
            moveTowards(target.transform.position);
            Vector3 dist = target.transform.position - gameObject.transform.position;
            if (dist.sqrMagnitude <= 0.5)
            {
                attacking = true;
            }
        }
    }

    void moveTowards(Vector3 dir)
    {
        Vector3 newPos = gameObject.transform.position;
        Vector3 move = dir - gameObject.transform.position; move.Normalize();

        newPos.x += (move.x * moveSpeed * Time.deltaTime) / period; newPos.y += (move.y * moveSpeed *Time.deltaTime) / period;
        gameObject.transform.position = newPos;

        //set z rotation
        Vector3 rot = sprite.transform.rotation.eulerAngles;

        rot.z = Mathf.Atan(move.y / move.x); rot.z *= 180 / Mathf.PI;
        Debug.Log(rot.z);
        if (move.y == 0)
        {
            if (move.y > 0) rot.z = 180;
            else rot.z = 0;
        }
        else if (move.x == 0)
        {
            rot.z = 90;
        }
        if (move.x < 0) rot.z += 180;
        //if (move. y < 0) rot.z *= -1;
        rot.z += 90;
        sprite.transform.rotation = Quaternion.Euler(rot);
    }

    public void setTarget(GameObject gObj)
    {
        target = gObj;
    }

    public bool stinger()
    {
        return noStinger;
    }

    public void setDilation(float p)
    {
        period = p;
    }

    public void setSM(GameObject SM)
    { scriptManager = SM; }
}
