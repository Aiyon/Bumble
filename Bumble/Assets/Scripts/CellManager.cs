using UnityEngine;
using System.Collections;

public class CellManager : MonoBehaviour {

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
    public GameObject[] typeIcons;
    public int[] typeHealths;
    // 0 = food
    // 1 = spawn
    int cellType;
    int cellHealth;
    int cellMaxHP;
    bool needRepair;
    bool dead = false;
    bool damaged = false;

    // Use this for initialization
    void Start ()
    {

        Debug.Log("test");
        adjCheck(true);
        cellType = -1;
        cellHealth = cellMaxHP = 100; //health of cell frame.
        needRepair = false;

        Debug.Log(scriptManager);
        if (scriptManager == null) findScriptManager();
        Debug.Log(scriptManager);
        
        setHiveSize();

    }

    // Update is called once per frame
    void Update()
    {
        if (dead) return;



        //SHOW CELL DAMAGE (Temporary?)
        if(cellHealth < cellMaxHP)
        {
            Color temp = cellTypes[cellType].GetComponent<SpriteRenderer>().color;
            temp.a = (float)cellHealth / cellMaxHP;

            cellTypes[cellType].GetComponent<SpriteRenderer>().color = temp;
        }

        if (needRepair)
        {
            if (cellHealth == cellMaxHP)
            {
                needRepair = false;
                typeIcons[cellType].SetActive(true);
                scriptManager.GetComponent<ResourceManager>().cellFixed(gameObject, true);
            }
            else if (cellHealth <= 0)
            {
                dead = true;
                scriptManager.GetComponent<PlayerInput>().killCell(gameObject);
                scriptManager.GetComponent<ResourceManager>().cellFixed(gameObject, true);
            }
        }
        else if (cellHealth < (cellMaxHP / 2))
        {
            needRepair = true;
            typeIcons[cellType].SetActive(false);
        }
        else if (!damaged && cellHealth < cellMaxHP)
        {
            scriptManager.GetComponent<ResourceManager>().cellFixed(gameObject, false);
            damaged = true;
        }
    }

    public void adjCheck(bool reCheck)
    {
        Vector3 dir = new Vector3(-0.5f, 0.75f, 1.5f);
        uLeft = cellCheck(dir, reCheck);

        dir = new Vector3(0.5f, 0.75f, 1.5f);
        uRight = cellCheck(dir, reCheck);

        dir = new Vector3(-1, 0, 1.5f);
        Debug.Log(left);
        left = cellCheck(dir, reCheck);
        Debug.Log(left);

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
            Debug.Log(hit.transform.parent.gameObject);
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
            else if(hit.transform.tag == "Empty")
            {
                return hit.transform.parent.gameObject;
            }
            else return null;
        }
        else
        {
            if (dead) return null; 
            {
                Vector3 temp = gameObject.transform.position + dir; temp.z = 0;
                adj = (GameObject)Instantiate(emptyCell, temp, Quaternion.identity);
                return adj;
            }

        }
    }

    public void setCellType(int i)
    {
        cellType = i;
        if(i >= 0) cellHealth = cellMaxHP = typeHealths[i];
        scriptManager.GetComponent<PlayerInput>().cellInfoBars(gameObject);
        for (int j = 0; j < cellTypes.Length; j++)
        {
            if (j == cellType)
            {
                cellTypes[j].SetActive(true);
                typeIcons[j].SetActive(true);
            }
            else
            {
                cellTypes[j].SetActive(false);
                typeIcons[j].SetActive(false);
            }

        }
    }

    public int getCellType()
    { return cellType; }

    public int numTypes()
    { return cellTypes.Length; }

    public void deltaHealth(int i)
    {
        cellHealth = Mathf.Clamp(cellHealth + i, -1, cellMaxHP);
    }

    public int getHealth()
    { return cellHealth; }

    public int getMaxHealth()
    { return cellMaxHP; }

    public void findScriptManager()
    {
        GameObject tempO = new GameObject();
        if (uLeft != null && uLeft.tag == "Cell" && uLeft.GetComponent<CellManager>().getCellType() >= 0)
            tempO = uLeft;
        else if (uRight != null && uRight.tag == "Cell" && uRight.GetComponent<CellManager>().getCellType() >= 0)
            tempO = uRight;
        else if (right != null && right.tag == "Cell" && right.GetComponent<CellManager>().getCellType() >= 0)
            tempO = right;
        else if (dRight != null && dRight.tag == "Cell" && dRight.GetComponent<CellManager>().getCellType() >= 0)
            tempO = dRight;
        else if (dLeft != null && dLeft.tag == "Cell" && dLeft.GetComponent<CellManager>().getCellType() >= 0) tempO = dLeft;
        else if (left != null) tempO = left;

        Debug.Log(tempO);
        scriptManager = tempO.GetComponent<CellManager>().getScriptManager();
    }

    public GameObject getScriptManager()
    {
        return scriptManager;
    }

    public void death()
    {
        //Debug.Log(uRight);
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

        if (!dead) dead = true;
    }

    public bool getBroken()
    {
        float hpP = cellHealth; hpP /= cellMaxHP;
        return (hpP < 0.5f);
    }

    void setHiveSize()
    {
        //update hive size.
        float l = scriptManager.GetComponent<ResourceManager>().getLeft();
        float r = scriptManager.GetComponent<ResourceManager>().getRight();
        float t = scriptManager.GetComponent<ResourceManager>().getTop();
        float b = scriptManager.GetComponent<ResourceManager>().getBot();

        Vector3 temp = gameObject.transform.position;
        if (temp.x < l) l = temp.x - 0.5f;
        if (temp.x > r) r = temp.x + 0.5f;
        if (temp.y > t) t = temp.y + 0.375f;
        if (temp.y < b) b = temp.y - 0.375f;
        scriptManager.GetComponent<ResourceManager>().setHiveSize(l, r, t, b);
        //end of size update.
    }
}
