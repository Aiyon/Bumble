using UnityEngine;
using System.Collections;

public class PlayerInput : MonoBehaviour {

    public GameObject cell;
    public Camera cam;
    public int zoomSpeed;
    public float panSpeed;

    Vector3 lastPosition;

    GameObject currentCell;
    public GameObject cellHighlight;
    public GameObject cellMenu;

    public GameObject emptyMenu;

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
                if(hit.transform.tag != "Empty") emptyMenu.SetActive(false); //if not empty clicked on, wipe highlight.

                if (hit.transform.tag == "Cell")
                {
                    currentCell = hit.transform.gameObject;
                    cellHighlight.SetActive(true);

                    Vector3 temp = currentCell.transform.position; temp.z -= 0.01f;
                    cellHighlight.transform.position = temp;
                    cellMenu.SetActive(true);
                }
                else
                {
                    cellMenu.SetActive(false);
                    cellHighlight.SetActive(false);

                    if (hit.transform.tag == "Empty")
                    {
                        currentCell = hit.transform.gameObject;
                            cellHighlight.SetActive(true);

                            Vector3 temp = currentCell.transform.position; temp.z -= 0.01f;
                            cellHighlight.transform.position = temp;
                            emptyMenu.SetActive(true);
                    }
                }

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
                        gameObject.GetComponent<ResourceManager>().newStorage();
                    }
                    break;

                case 1:
                    if (gameObject.GetComponent<ResourceManager>().getFood() < 250) type = -1;
                    else
                    {
                        gameObject.GetComponent<ResourceManager>().setFood(-250);
                        gameObject.GetComponent<ResourceManager>().newForager();
                    }
                    break;

                case 2:
                    if (gameObject.GetComponent<ResourceManager>().getFood() < 250) type = -1;
                    else
                    {
                        gameObject.GetComponent<ResourceManager>().setFood(-250);
                        gameObject.GetComponent<ResourceManager>().newBuilder();
                    }
                    break;
                case 3:
                    if (gameObject.GetComponent<ResourceManager>().getFood() < 400) type = -1;
                    else
                    {
                        gameObject.GetComponent<ResourceManager>().setFood(-400);
                        gameObject.GetComponent<ResourceManager>().newGuard();
                    }
                    break;
            }

            currentCell.GetComponent<CellManager>().setCellType(type);
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
            cellMenu.SetActive(true);
        }
    }

}
