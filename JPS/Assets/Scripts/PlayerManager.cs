using UnityEngine;
using System.Collections;

public class PlayerManager : MonoBehaviour {

    public GameObject hoveredGO;

    void Start()
    {
        hoveredGO = this.gameObject;
    }

    void Update()
    {
        RaycastHit hitInfo = new RaycastHit();
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hitInfo))
        {
            //reset color of Tile if the last raycast was over a builder
            if(hoveredGO.tag == "Tile")
            {
                hoveredGO.GetComponent<tile>().ResetColor();
            } 

            hoveredGO = hitInfo.collider.gameObject;

            if (hoveredGO.tag == "Tile")
            {
                hoveredGO.GetComponent<tile>().MouseHover();

                if (Input.GetMouseButton(0) || Input.GetMouseButtonDown(0))
                {
                    hoveredGO.GetComponent<tile>().MouseDownLeftClick();
                }
                else if (Input.GetMouseButtonDown(1))
                {
                    hoveredGO.GetComponent<tile>().MouseDownRightClick();
                }

            }

        }
    }
}
