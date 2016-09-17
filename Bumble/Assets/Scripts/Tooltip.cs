using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Tooltip : MonoBehaviour {


    public Text TTObject;
    public GameObject TooltipBox;
    public GameObject TTInnerBox;

    Vector2 tooltipSize;

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (TooltipBox.activeSelf)
        {
            Vector2 sizeDelta = TooltipBox.GetComponent<Image>().rectTransform.sizeDelta;
            float offset = sizeDelta.y;
            Vector3 newPos = Input.mousePosition + new Vector3(0, offset + 30, 0);

            if (Input.mousePosition.x < sizeDelta.x * 1.225f)
            {
                newPos.x = sizeDelta.x * 1.225f;
            }
            TooltipBox.transform.position = newPos;

        }
	}

    public void onTooltip(string text)
    {
        TTObject.text = text;

        tooltipSize.x = 110*text.Length/19;
        if (tooltipSize.x > 165)
        {
            tooltipSize.y = 10 + (tooltipSize.x*15/165);
            tooltipSize.x = 165;
        }
        else tooltipSize.y = 25;        

        TTObject.rectTransform.sizeDelta = tooltipSize;
        tooltipSize.x += 10;
        TooltipBox.GetComponent<Image>().rectTransform.sizeDelta = tooltipSize;
        tooltipSize -= new Vector2(5, 5);
        TTInnerBox.GetComponent<Image>().rectTransform.sizeDelta = tooltipSize;

        TooltipBox.SetActive(true);
    }

    public void offTooltip()
    {
        TooltipBox.SetActive(false);
    }
}
