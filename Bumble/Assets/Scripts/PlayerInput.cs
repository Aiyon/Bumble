using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PlayerInput : MonoBehaviour {

    public GameObject cell;
    public Camera cam;
    public int zoomSpeed;
    public float panSpeed;

    Vector3 lastPosition;

    GameObject currentCell;
    public GameObject cellHighlight;
    public GameObject[] cellMenus;
    public GameObject queenMenu;
    public Text cellHPText;

    public GameObject emptyMenu;

    

    public int numTypes;

    // Use this for initialization
    void Start ()
    {
        lastPosition = Vector3.zero;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetMouseButtonDown(0) && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100.0f))
            {
                int temp1 = 0; int temp2 = 0;

                if (hit.transform.tag != "Empty")
                {
                    emptyMenu.SetActive(false); //if not empty clicked on, wipe highlight.
                    cellHPText.gameObject.SetActive(false);
                }
                if (hit.transform.tag == "Queen")
                {
                    currentCell = hit.transform.parent.gameObject;
                    Vector3 temp = currentCell.transform.position; temp.z -= 0.02f;
                    cellHighlight.transform.position = temp;
                    queenMenu.SetActive(true);
                    cellHighlight.SetActive(true);

                    cellHPText.gameObject.SetActive(true);
                    temp1 = currentCell.GetComponent<QueenManager>().getHealth();
                    temp2 = currentCell.GetComponent<QueenManager>().getMaxHealth();
                }
                else
                {
                    queenMenu.SetActive(false);

                    cellHighlight.SetActive(false);


                    if (hit.transform.tag == "Cell")
                    {
                        currentCell = hit.transform.parent.gameObject;

                        cellHighlight.SetActive(true);

                        Vector3 temp = currentCell.transform.position; temp.z -= 0.02f;
                        cellHighlight.transform.position = temp;

                        cellInfoBars(currentCell);

                        cellHPText.gameObject.SetActive(true);
                        temp1 = currentCell.GetComponent<CellManager>().getHealth();
                        temp2 = currentCell.GetComponent<CellManager>().getMaxHealth();
                    }
                    else
                    {
                        cellHPText.gameObject.SetActive(false);
                        for (int i = 0; i <= numTypes; i++)
                        {
                            cellMenus[i].SetActive(false);
                        }
                    
                        currentCell = hit.transform.parent.gameObject;
                        cellHighlight.SetActive(false);

                        if (hit.transform.tag == "Empty")
                        {
                            cellHPText.gameObject.SetActive(false);
                            cellHighlight.SetActive(true);

                            Vector3 temp = currentCell.transform.position; temp.z -= 0.02f;
                            cellHighlight.transform.position = temp;
                            emptyMenu.SetActive(true);
                        }
                    }
                }

                cellHPText.text = "Health: " + temp1 + "/" + temp2;
            }
            else
            {
                for (int i = 0; i <= numTypes; i++)
                {
                    cellMenus[i].SetActive(false);
                    cellHighlight.SetActive(false);
                }
                cellHPText.gameObject.SetActive(false);
                emptyMenu.SetActive(false);
            }

        }
        if (Input.GetMouseButtonDown(2))
        {
            lastPosition = Input.mousePosition;
        }
        if (Input.GetMouseButton(2))
        {
            Vector3 delta = Input.mousePosition - lastPosition;
            float temp = panSpeed * cam.orthographicSize / 5;
            cam.transform.Translate(delta.x * temp, delta.y * temp, 0);
            lastPosition = Input.mousePosition;
            Debug.Log(delta);
        }

        float scrollSpeed = Input.GetAxis("Mouse ScrollWheel");
        if (scrollSpeed != 0)
        {
            float temp = cam.orthographicSize -= zoomSpeed * scrollSpeed * Time.deltaTime;
            cam.orthographicSize = Mathf.Clamp(temp, 1, 20);
        }
    }

    public void setCellType(int type)
    {
        if (currentCell.GetComponent<CellManager>().getCellType() == -1)
        {
            switch (type)
            {
                case 0:
                    if (gameObject.GetComponent<ResourceManager>().getFood() < 100) type = -1;
                    else
                    {
                        gameObject.GetComponent<ResourceManager>().setFood(-100);
                        gameObject.GetComponent<ResourceManager>().newStorage(currentCell);
                    }
                    break;

                case 1:
                    if (gameObject.GetComponent<ResourceManager>().getFood() < 250) type = -1;
                    else
                    {
                        gameObject.GetComponent<ResourceManager>().setFood(-250);
                        gameObject.GetComponent<ResourceManager>().newForager(currentCell);
                    }
                    break;

                case 2:
                    if (gameObject.GetComponent<ResourceManager>().getFood() < 250) type = -1;
                    else
                    {
                        gameObject.GetComponent<ResourceManager>().setFood(-250);
                        gameObject.GetComponent<ResourceManager>().newBuilder(currentCell);
                    }
                    break;
                case 3:
                    if (gameObject.GetComponent<ResourceManager>().getFood() < 400) type = -1;
                    else
                    {
                        gameObject.GetComponent<ResourceManager>().setFood(-400);
                        gameObject.GetComponent<ResourceManager>().newGuard(currentCell);
                    }
                    break;
                case 4:
                    if(gameObject.GetComponent<ResourceManager>().getFood() < 200 && gameObject.GetComponent<ResourceManager>().getWax() < 50) type = -1;
                    else
                    {
                        gameObject.GetComponent<ResourceManager>().setFood(-200);
                        gameObject.GetComponent<ResourceManager>().setWax(-50);
                        gameObject.GetComponent<ResourceManager>().newNurse(currentCell);
                    }
                    break;
            }

            Debug.Log("sCT TEST 2");
            cellInfoBars(currentCell);

            currentCell.GetComponent<CellManager>().setCellType(type);
            Debug.Log(currentCell.GetComponent<CellManager>().getCellType());
            int temp1 = currentCell.GetComponent<CellManager>().getHealth();
            int temp2 = currentCell.GetComponent<CellManager>().getMaxHealth();
            cellHPText.text = "Health: " + temp1 + "/" + temp2;

        }
    }

    public void newCell()
    {
        if (gameObject.GetComponent<ResourceManager>().getWax() >= 200)
        {
            gameObject.GetComponent<ResourceManager>().setWax(-200);
            Vector3 newPos = currentCell.transform.position;
            Destroy(currentCell);
            currentCell = (GameObject)Instantiate(cell, newPos, Quaternion.identity);
            emptyMenu.SetActive(false);
            cellMenus[0].SetActive(true);
        }
    }

    public void cellInfoBars(GameObject GamObj)
    {
        for (int i = 0; i <= GamObj.GetComponent<CellManager>().numTypes(); i++)
        {
            //Debug.Log(GamObj.GetComponent<CellManager>().getCellType());

            if (i - 1 == GamObj.GetComponent<CellManager>().getCellType()) cellMenus[i].SetActive(true);
            else cellMenus[i].SetActive(false);
        }
    }

    public void killCell(GameObject cell)
    {
        int temp = cell.transform.childCount;
        for (int i = 0; i < temp; i++)
        {
            DestroyImmediate(cell.transform.GetChild(0).gameObject);
        }

        cell.layer = 2;
        if (cell.tag == "Cell")
        {
            cell.GetComponent<CellManager>().death();
            cell.GetComponent<CellManager>().adjCheck(true);
            if(cell.GetComponent<CellManager>().getCellType() >= 0)
            {
                gameObject.GetComponent<ResourceManager>().deadCell(cell);
            }
        }
        if (cell.tag == "Queen")
        {
            cell.GetComponent<QueenManager>().death();
            cell.GetComponent<QueenManager>().adjCheck(true);
            //GAME OVER
        }

        Destroy(cell);
        //not replacing destroyed empties.
    }
}
