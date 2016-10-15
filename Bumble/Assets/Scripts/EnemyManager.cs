using UnityEngine;
using System.Collections;

public class EnemyManager : MonoBehaviour
{

    public int health;
    public int damage;
    public int moveSpeed;
    Camera cam;

    bool attacking;
    bool dead;
    public GameObject target;
    public GameObject alert;
    public GameObject enemySprite;
    
    //update test
    float nextActionTime = 0.0f;
    float period = 1.0f;

    // Use this for initialization
    void Start ()
    {
        attacking = false;
        dead = false;
        nextActionTime = Time.time;
        cam = Camera.main;

    }

    // Update is called once per frame
    void Update()
    {
        if (!transform.GetChild(0).gameObject.GetComponent<Renderer>().isVisible)
        {
            alert.transform.localScale = new Vector3(1,1,1) * cam.orthographicSize / 5;

            if (!alert.activeSelf) alert.SetActive(true);
            Vector3 newAPos = gameObject.transform.position - cam.transform.position;

            //height = 2 * size, if resolution = x:y, width = 2x/y.
            float height = 2 * cam.orthographicSize;
            float width = height * cam.aspect;
            width = (width/2); height = (height / 2);
            if (Mathf.Abs(newAPos.x) / Mathf.Abs(newAPos.y+1) > cam.aspect)
            {
                float modifier = newAPos.x / Mathf.Clamp(newAPos.x, 0.5f-width, width-0.5f);
                newAPos /= modifier;
                newAPos.y = Mathf.Clamp(newAPos.y, (cam.orthographicSize / 2.5f) - height, height);
            }
            else
            {
                float modifier = newAPos.y / Mathf.Clamp(newAPos.y, (cam.orthographicSize / 2.5f) - height, height);
                newAPos /= modifier;
                newAPos.x = Mathf.Clamp(newAPos.x, 0.5f - width, width - 0.5f);
            }

            if (newAPos.y < 0) newAPos.y += (alert.transform.localScale.y / 2);
            else newAPos.y -= (alert.transform.localScale.y / 2);

            newAPos += cam.transform.position;
            newAPos.z = -1;
            alert.transform.position = newAPos;

        }
        else if (alert.activeSelf) alert.SetActive(false);

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
                    if (Time.time > nextActionTime)
                    {
                        attack(target);
                        nextActionTime += period;
                    }
                }
                Vector3 dist = target.transform.position - gameObject.transform.position;
                if (dist.sqrMagnitude <= 0.5f) attacking = true;
            }
        }
        if (Time.time > nextActionTime)
        {
            nextActionTime += period;
        }
    }

    void moveTowards(Vector3 dir)
    {
        Vector3 newPos = gameObject.transform.position;
        Vector3 move = dir - gameObject.transform.position; move.Normalize();

        float dist = moveSpeed / period;

        newPos.x += move.x * dist*Time.deltaTime; newPos.y += move.y * dist * Time.deltaTime;
        gameObject.transform.position = newPos;

        //set z rotation
        Vector3 rot = enemySprite.transform.rotation.eulerAngles;

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

        enemySprite.transform.rotation = Quaternion.Euler(rot);

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

    public void setDilation(float amount)
    {
        period = amount;
    }
}
