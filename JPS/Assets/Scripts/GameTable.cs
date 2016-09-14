using UnityEngine;
using System.Collections.Generic;

public class GameTable : MonoBehaviour {


    static public GameTable m_instance;

    public GameObject m_tile;
    public int width;
    public int height;

    private GameObject[,] m_map;
    private int numClick = 0;
    private PathFinder.node start;
    private PathFinder.node goal;

    void Awake()
    {
        if (m_instance == null)
        {
            m_instance = this; ;
        }
    }
    // Create map and instantiate tiles
    void Start () {
        m_map = new GameObject[width, height];
        for (int i = 0; i < width; ++i)
        {
            for(int j = 0; j < height; ++j)
            {
                GameObject tile = (GameObject)Instantiate(m_tile,
                    new Vector3(i,transform.position.y, j), transform.rotation);
                tile.transform.parent = this.transform;
                m_map[i,j] = tile;
            }
        }

        PathFinder.m_instance.CreateTable(width, height);
    }
	
    // set start tile, goal tile and find a path
    public void SetClick(PathFinder.node n)
    {
        for (int i = 0; i < width; ++i)
            for (int j = 0; j < height; ++j)
                m_map[i, j].GetComponent<tile>().ResetColor();
        if (numClick % 2 == 0)
        {
            start = n;
        } else
        {
            goal = n;
            FindPath(start, goal);
        }
        numClick++;
    }

    // find a path a change the color of the jump points( for debug purposses)
	private void FindPath(PathFinder.node s, PathFinder.node g)
    {
        PathFinder.m_instance.ResetTable();
        List<PathFinder.node> list = PathFinder.m_instance.FindPath(s, g);
        if(list!=null)
        foreach(PathFinder.node i in list)
        {
            m_map[i.X, i.Y].GetComponent<Renderer>().material.color = Color.red;
        }

    }
}
