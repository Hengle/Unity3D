using UnityEngine;
using System.Collections;

public class tile : MonoBehaviour {

    [Header("Attributes")]
    public Color m_hoverColor;

    private Renderer m_rend;
    private Color m_originalColor;

    // Use this for initialization

    void Start () {
        m_rend = GetComponent<Renderer>();
        m_originalColor = m_rend.material.color;
    }

    public void MouseHover()
    {
        m_rend.material.color = m_hoverColor;
    }

    public void MouseDownLeftClick()
    {
            PathFinder.node block = new PathFinder.node((int)transform.position.x, (int)transform.position.z);
            Debug.Log("set block");
            PathFinder.m_instance.SetNodeAsBusy(block.X, block.Y);
            m_originalColor = Color.black;
            m_rend.material.color = m_originalColor;

    }

    public void MouseDownRightClick()
    {
            PathFinder.node thisNode = new PathFinder.node((int)transform.position.x, (int)transform.position.z);
            GameTable.m_instance.SetClick(thisNode);
    }

    public void ResetColor()
    {
        m_rend.material.color = m_originalColor;
    }
}
