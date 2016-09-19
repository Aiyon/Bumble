using UnityEngine;
using System.Collections;

public class QueenManager : MonoBehaviour {

    public GameObject uLeft;   //-0.5,0.75
    public GameObject uRight;  //0.5,0.75
    public GameObject left;    //-1.0,0
    public GameObject right;   //1.0,0
    public GameObject dLeft;   //-0.5,-0.75
    public GameObject dRight;  //0.5,-0.75

    public GameObject cell;
    public GameObject emptyCell;
    public GameObject scriptManager;

    public GameObject[] cellTypes;
    // 0 = food
    // 1 = spawn
    int cellType;
    int cellHealth;
    int cellMaxHP;

    // Use this for initialization
    void Start()
    {
        adjCheck(true);
        cellHealth = cellMaxHP = 1000;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void adjCheck(bool reCheck)
    {
        Vector3 dir = new Vector3(-0.5f, 0.75f, 1.25f);
        uLeft = cellCheck(dir, reCheck);

        dir = new Vector3(0.5f, 0.75f, 1.25f);
        uRight = cellCheck(dir, reCheck);

        dir = new Vector3(-1, 0, 1);
        left = cellCheck(dir, reCheck);

        dir = new Vector3(1, 0, 1);
        right = cellCheck(dir, reCheck);

        dir = new Vector3(-0.5f, -0.75f, 1.25f);
        dLeft = cellCheck(dir, reCheck);

        dir = new Vector3(0.5f, -0.75f, 1.25f);
        dRight = cellCheck(dir, reCheck);
    }

    public GameObject cellCheck(Vector3 dir, bool reCheck)
    {

        RaycastHit hit;
        float maxRay = dir.z; dir.z = -1;
        if (Physics.Raycast(gameObject.transform.position + dir, new Vector3(0, 0, 1), out hit, 10.0f))
        {
            if (hit.transform.tag == "Cell")
            {
                GameObject adj = hit.transform.parent.gameObject;
                if (reCheck) adj.GetComponent<CellManager>().adjCheck(false);
                return adj;
            }
            else
            {
                return emptyCell;
            }
        }
        else
        {
            Vector3 temp = gameObject.transform.position + dir; temp.z = 0;
            left = (GameObject)Instantiate(emptyCell, temp, Quaternion.identity);
            return emptyCell;
        }
    }
    public void deltaHealth(int i)
    {
        cellHealth += i;
    }

    public int getHealth()
    { return cellHealth; }

    public int getMaxHealth()
    { return cellMaxHP; }
}
