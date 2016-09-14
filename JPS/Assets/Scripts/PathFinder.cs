using UnityEngine;
using System.Collections.Generic;
using System.Linq;
public class PathFinder : MonoBehaviour {

    static public PathFinder m_instance;

    public class node
    {
        public node parent;
        public int X;
        public int Y;
        public int G;
        public int F;
        public bool busy;

        public node(int x, int y)
        {
            X = x;
            Y = y;
            G = 0;
            F = 0;
            busy = false;
            parent = null;
        }
        public void reset()
        {
            G = 0;
            F = 0;
            parent = null;
        }
        public void XY(int x, int y)
        {
            X = x;
            Y = y;
        }
        public int DistanceTo(node _other)
        {
            float distanceX = X - _other.X;
            float distanceY = Y - _other.Y;

            return (int)Mathf.Sqrt((distanceX) * (distanceX) +
            (distanceY) * (distanceY));
        }
    };

    private struct Vec2
    {
        public int x;
        public int y;
        public Vec2(int _x, int _y)
        {
            x = _x;
            y = _y;
        }
    };

    private node[,] m_map;
    private int m_width = 0;
    private int m_height = 0;
    private List<node> m_poolAvailable = null;
    private List<node> m_pool = new List<node>();
    private List<node> m_closedList = null;
    private List<node> m_openList = null;
    private List<Vec2> m_vSides = null;

    void Awake()
    {
        if(m_instance == null)
        {
            m_instance = this; ;
        }
    }

    
    public void Start()
    {
        m_vSides = new List<Vec2>();
        m_vSides.Add(new Vec2(1, 1));
        m_vSides.Add(new Vec2(1, -1));
        m_vSides.Add(new Vec2(-1, 1));
        m_vSides.Add(new Vec2(-1, -1));
        m_vSides.Add(new Vec2(0, 1));
        m_vSides.Add(new Vec2(0, -1));
        m_vSides.Add(new Vec2(1, 0));
        m_vSides.Add(new Vec2(-1, 0));

        DumpPool();

        m_openList = new List<node>();
        m_closedList = new List<node>();
    }

    //create a copy of the table but using nodes instead of X gameobject
    public void CreateTable(int _width, int _height)
    {
        if(m_map==null)
            m_map = new node[_width, _height];

        for(int i = 0; i< _width; ++i)
        {
            for (int j = 0; j < _height; ++j)
            {
                node newnode = new node(i,j);
                m_map[i, j] = newnode;
            }
        }
        m_width = _width;
        m_height = _height;
    }

    public void ResetTable()
    {
        for (int i = 0; i < m_width; ++i)
        {
            for (int j = 0; j < m_height; ++j)
            {
                m_map[i, j].reset();
            }
        }
    }
    // set not walkable nodes
    public void SetNodeAsBusy(int _x,int _y)
    {
        m_map[_x, _y].busy = true;
    }

    int GetNodeHeuristic(node _start, node _goal)
    {
        return Mathf.Abs(_goal.X - _start.X) + Mathf.Abs(_goal.Y - _start.Y);
    }

    // return if a node is blocked
    bool IsBlocked(int _x, int _y)
    {
        if (_x < 0 || _y < 0 || _x > m_width - 1 || _y > m_height - 1)
        {
            return true;
        }

        if (m_map[_x, _y] == null)
        {
            return true;
        }

        return m_map[_x, _y].busy;
    }

    // set neighbours using the list of possible sides
    List<node> SetNeighbours(node _current)
    {
        List<node> neighbours = new List<node>();

        for (int i = 0; i < m_vSides.Count; ++i)
        {
            int potentialX = _current.X + m_vSides[i].x;
            int potentialY = _current.Y + m_vSides[i].y;
            if (!IsBlocked(potentialX, potentialY))
            {
                neighbours = PushBackNeighbour(potentialX, potentialY, neighbours);
            }
        }
        return neighbours;
    }

    List<node> PushBackNeighbour(int _x, int _y, List<node> _neighbours)
    {
        List<node> neighbours = _neighbours;
        neighbours.Add(PooledNode(_x, _y));
        return neighbours;
    }

    // pool to performance improvement
    // if exist nodes without using then recycle it
    node PooledNode(int _x, int _y)
    {
        if (m_poolAvailable.Count == 0)
        {
            node newnode = new node(_x, _y);
            m_pool.Add(newnode);
            return newnode;
        }
        else {
            node newnode = m_poolAvailable[0];
            m_poolAvailable.RemoveAt(0);
            newnode.reset();
            newnode.XY(_x, _y);
            return newnode;
        }
    }

    // return a jump node
    node Jump(int _x, int _y, int _dirX, int _dirY, node _start, node _goal)
    {
        int nextX = _x + _dirX;
        int nextY = _y + _dirY;

        if (IsBlocked(nextX, nextY))
            return null;

        if (nextX == _goal.X && nextY == _goal.Y)
            return PooledNode(nextX, nextY);

        int offsetX = nextX;
        int offsetY = nextY;

        if (_dirX !=0 && _dirY != 0)
        {
            while (true)
            {
                if(!IsBlocked(offsetX - _dirX, offsetY + _dirY) &&
                    IsBlocked(offsetX - _dirX, offsetY) ||
                    !IsBlocked(offsetX + _dirX, offsetY - _dirY) &&
                    IsBlocked(offsetX, offsetY - _dirY))
                {
                    return PooledNode(offsetX, offsetY);
                }

                if (Jump(offsetX, offsetY, _dirX, 0, _start, _goal) != null ||
                    Jump(offsetX, offsetY, 0, _dirY, _start, _goal) != null)
                    return PooledNode(offsetX, offsetY);

                offsetX += _dirX;
                offsetY += _dirY;

                if (IsBlocked(offsetX, offsetY))
                    return null;

                if (offsetX == _goal.X && offsetY == _goal.Y)
                    return PooledNode(offsetX, offsetY);
            }
        } else
        {
            if (_dirX != 0)
            {
                while (true)
                {
                    if (!IsBlocked(offsetX + _dirX, nextY + 1) &&
                        IsBlocked(offsetX, nextY + 1) ||
                        !IsBlocked(offsetX + _dirX, nextY - 1) &&
                        IsBlocked(offsetX, nextY - 1))
                    {
                        return PooledNode(offsetX, nextY);
                    }

                    offsetX += _dirX;

                    if (IsBlocked(offsetX, nextY))
                        return null;

                    if (offsetX == _goal.X && nextY == _goal.Y)
                        return PooledNode(offsetX, nextY);
                }
            } else // dirY
            {
                while (true)
                {
                    if (!IsBlocked(nextX + 1, offsetY + _dirY) &&
                        IsBlocked(nextX + 1, offsetY) ||
                        !IsBlocked(nextX - 1, offsetY + _dirY) &&
                        IsBlocked(nextX - 1, offsetY))
                    {
                        return PooledNode(nextX, offsetY);
                    }

                    offsetY += _dirY;

                    if (IsBlocked(nextX, offsetY))
                        return null;

                    if (nextX == _goal.X && offsetY == _goal.Y)
                        return PooledNode(nextX, offsetY);
                }
            }
        }
    }

    // identify possible successors as jump points
    List<node> IdentifySuccessors(node _current, node _start, node _goal)
    {
        List<node> successors = new List<node>();
        List<node> neighbours;
        neighbours = SetNeighbours(_current);

        for (int i = 0; i < neighbours.Count; ++i)
        {
            int dirX = Mathf.Min(Mathf.Max(-1, neighbours[i].X - _current.X), 1);
            int dirY = Mathf.Min(Mathf.Max(-1, neighbours[i].Y - _current.Y), 1);

            node jumpPoint = Jump(_current.X, _current.Y, dirX, dirY, _start, _goal);
            if (jumpPoint != null)
            {
                successors.Add(jumpPoint);
            }
        }
        return successors;
    }

    //findpath
    public List<node> FindPath(node _start, node _goal)
    {
        List<node> finalList = null;

        node startNode = new node(_start.X, _start.Y);
        node goalNode = new node(_goal.X, _goal.Y);

        DumpPool();
        // clear lists
        if (m_closedList.Count > 0)
            m_closedList.Clear();
        if (m_openList.Count > 0)
            m_openList.Clear();

        //check if goal is null or busy
        if (goalNode == null || m_map[_goal.X, _goal.Y].busy)
            return null;

        // get heuristic
        startNode.F = GetNodeHeuristic(startNode, goalNode);
        //add node to the open list
        m_openList.Add(startNode);
        // while open list is not empty ...
        while(m_openList.Count != 0)
        {
            // order openlist by F and get the first node
            m_openList.Sort((a, b) => a.F.CompareTo(b.F));
            node current = m_openList[0];
            m_openList.RemoveAt(0);
            // if is goal, return final list
            if (current.X == goalNode.X && current.Y == goalNode.Y)
                return GetFinalList(current);
            // add to the closed list
            m_closedList.Add(current);
            // identify successors
            List<node> successors = IdentifySuccessors(current, _start, _goal);
            // for each successor:
            // find if the successor exist in closed list
            // find if the successor exist in open list
            for (int i = 0; i < successors.Count; ++i)
            {
                node jumpNode = successors[i];

                if (m_closedList.Find(x => (x.X == jumpNode.X) && (x.Y == jumpNode.Y)) != null)
                {
                    continue;
                }

                int score = current.G + current.DistanceTo(jumpNode);
                
                node findIfExistNode = m_openList.Find(x => (x.X == jumpNode.X) && (x.Y == jumpNode.Y));

                if (findIfExistNode == null || score < jumpNode.G)
                {
                    node newJumpNode = new node(jumpNode.X, jumpNode.Y);
                    newJumpNode.parent = current;
                    newJumpNode.G = score;
                    newJumpNode.F = newJumpNode.G + GetNodeHeuristic(newJumpNode, goalNode);

                    if (findIfExistNode == null)
                    {// add to the open list
                        m_openList.Add(newJumpNode);
                    } else
                    {// replace from the open list if exist
                        m_openList = ReplaceNodeFromList(m_openList, jumpNode, newJumpNode);
                    }
                }

            }
        }

        return finalList;
    }
    // return final list
    List<node> GetFinalList(node _node)
    {
        List<node> finalList = new List<node>();
        node current = _node;
        while (current != null)
        {
            finalList.Add(current);
            current = current.parent;
        }
        return finalList;
    }

    // reset pool
    void DumpPool()
    {
        m_poolAvailable = m_pool;
        m_pool = new List<node>();
    }
    // replace 
    private List<node> ReplaceNodeFromList(List<node> _list, node _old, node _new)
    {
        List<node> finalList = _list;
        int i = 0;
        foreach (node it in _list)
        {
            if (it == _old)
            {
                finalList[i] = _new;
                return finalList;
            }
            i++;
        }
        return _list;
    }
}
