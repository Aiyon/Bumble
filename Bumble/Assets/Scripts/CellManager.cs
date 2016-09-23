using UnityEngine;
using System.Collections;

public class CellManager : MonoBehaviour {

    GameObject uLeft;   //-0.5,0.75
    GameObject uRight;  //0.5,0.75
    GameObject left;    //-1.0,0
    GameObject right;   //1.0,0
    GameObject dLeft;   //-0.5,-0.75
    GameObject dRight;  //0.5,-0.75

    public GameObject cell;
    public GameObject emptyCell;
    public GameObject scriptManager;

    public GameObject[] cellTypes;
    public int[] typeHealths;
    // 0 = food
    // 1 = spawn
    int cellType;
    int cellHealth;
    int cellMaxHP;
    bool needRepair;
    bool dead;

    // Use this for initialization
    void Start ()
    {
        adjCheck(true);
        cellType = -1;
        cellHealth = cellMaxHP = 100; //health of cell frame.
        needRepair = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (dead) return;

        for (int i = 0; i < cellTypes.Length; i++)
        {
            if (i == cellType) cellTypes[i].SetActive(true);
            else cellTypes[i].SetActive(false);
        }
        if (needRepair)
        {
            if (cellHealth == cellMaxHP)
            {
                needRepair = false;
            }
            else if(cellHealth <= 0)
            {
                dead = true;
                scriptManager.GetComponent<PlayerInput>().killCell(gameObject);
            }
        }
        else if (cellHealth < cellMaxHP / 2)
        {   
            needRepair = true;

        }

        if (scriptManager == null)
        {
            findScriptManager();
        }
    }

    public void adjCheck(bool reCheck)
    {
            Vector3 dir = new Vector3(-0.5f, 0.75f, 1.5f);
            uLeft = cellCheck(dir, reCheck);

            dir = new Vector3(0.5f, 0.75f, 1.5f);
            uRight = cellCheck(dir, reCheck);

            dir = new Vector3(-1, 0, 1.5f);
            left = cellCheck(dir, reCheck);

            dir = new Vector3(1, 0, 1.5f);
            right = cellCheck(dir, reCheck);

            dir = new Vector3(-0.5f, -0.75f, 1.5f);
            dLeft = cellCheck(dir, reCheck);

            dir = new Vector3(0.5f, -0.75f, 1.5f);
            dRight = cellCheck(dir, reCheck);
    }

    public GameObject cellCheck(Vector3 dir, bool reCheck)
    {

        RaycastHit hit;
        float maxRay = dir.z; dir.z = -1;
        GameObject adj;

        if (Physics.Raycast(gameObject.transform.position + dir, new Vector3(0, 0, 1), out hit, maxRay))
        {
            if (hit.transform.tag == "Cell")
            {
                adj = hit.transform.parent.gameObject;
                if (reCheck) adj.GetComponent<CellManager>().adjCheck(false);
                return adj;
            }
            else if (hit.transform.tag == "Queen")
            {
                adj = hit.transform.parent.gameObject;
                if (reCheck) adj.GetComponent<QueenManager>().adjCheck(false);
                return adj;
            }
            else return hit.transform.parent.gameObject;
        }
        else
        {
            if (dead) return null;
            Vector3 temp = gameObject.transform.position + dir; temp.z = 0;
            adj = (GameObject)Instantiate(emptyCell, temp, Quaternion.identity);
            return adj;
        }
    }

    public void setCellType(int i)
    {
        cellType = i;
        if(i >= 0) cellHealth = cellMaxHP = typeHealths[i];
        scriptManager.GetComponent<PlayerInput>().cellInfoBars(gameObject);
    }
    public int getCellType()
    { return cellType; }

    public int numTypes()
    { return cellTypes.Length; }

    public void deltaHealth(int i)
    {
        cellHealth += i;
    }

    public int getHealth()
    { return cellHealth; }

    public int getMaxHealth()
    { return cellMaxHP; }

    public void findScriptManager()
    {
        
            GameObject tempO = new GameObject();
            if (uLeft!= null && uLeft.tag == "Cell") tempO = uLeft;
            else if(uRight != null && uRight.tag == "Cell") tempO = uRight;
            else if (left != null && left.tag == "Cell") tempO = left;
            else if (right!= null && right.tag == "Cell") tempO = right;
            else if (dLeft != null && dLeft.tag == "Cell") tempO = dLeft;
            else if (dRight != null )tempO = dRight;

            scriptManager = tempO.GetComponent<CellManager>().getScriptManager();
    }

    public GameObject getScriptManager()
    {
        return scriptManager;
    }

    public void death()
    {
        if (uLeft != null && uLeft.tag == "Empty")
            DestroyImmediate(uLeft);
        if (uRight != null && uRight.tag == "Empty")
            DestroyImmediate(uRight);
        if (left != null && left.tag == "Empty")
            DestroyImmediate(left);
        if (right != null && right.tag == "Empty")
            DestroyImmediate(right);
        if (dLeft != null && dLeft.tag == "Empty")
            DestroyImmediate(dLeft);
        if (dRight != null && dRight.tag == "Empty")
            DestroyImmediate(dRight);
    }
}
