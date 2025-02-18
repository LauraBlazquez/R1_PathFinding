using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public int Size;
    public BoxCollider2D Panel;
    public GameObject token;
    private Node[,] NodeMatrix;
    private int startPosx, startPosy;
    private int endPosx, endPosy;
    void Awake()
    {
        Instance = this;
        Calculs.CalculateDistances(Panel, Size);
    }
    private void Start()
    {
        startPosx = Random.Range(0, Size);
        startPosy = Random.Range(0, Size);
        do
        {
            endPosx = Random.Range(0, Size);
            endPosy = Random.Range(0, Size);
        } while(endPosx== startPosx || endPosy== startPosy);
        NodeMatrix = new Node[Size, Size];
        CreateNodes();
        aAlgorithm();
    }

    public void CreateNodes()
    {
        for(int i=0; i<Size; i++)
        {
            for(int j=0; j<Size; j++)
            {
                NodeMatrix[i, j] = new Node(i, j, Calculs.CalculatePoint(i,j));
                NodeMatrix[i,j].Heuristic = Calculs.CalculateHeuristic(NodeMatrix[i,j],endPosx,endPosy);
            }
        }
        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size; j++)
            {
                SetWays(NodeMatrix[i, j], i, j);
            }
        }
        DebugMatrix();
    }
    public void DebugMatrix()
    {
        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size; j++)
            {
                GameObject newNode = Instantiate(token, NodeMatrix[i, j].RealPosition, Quaternion.identity);
                if (startPosx == i && startPosy == j)
                {
                    SpriteRenderer sprite = newNode.GetComponent<SpriteRenderer>();
                    sprite.color = Color.yellow;
                }
                if (endPosx == i && endPosy == j)
                {
                    SpriteRenderer sprite = newNode.GetComponent<SpriteRenderer>();
                    sprite.color = Color.blue;
                }
            }
        }
    }
    public void SetWays(Node node, int x, int y)
    {
        node.WayList = new List<Way>();
        if (x>0)
        {
            node.WayList.Add(new Way(NodeMatrix[x - 1, y], Calculs.LinearDistance));
            if (y > 0)
            {
                node.WayList.Add(new Way(NodeMatrix[x - 1, y - 1], Calculs.DiagonalDistance));
            }
        }
        if(x<Size-1)
        {
            node.WayList.Add(new Way(NodeMatrix[x + 1, y], Calculs.LinearDistance));
            if (y > 0)
            {
                node.WayList.Add(new Way(NodeMatrix[x + 1, y - 1], Calculs.DiagonalDistance));
            }
        }
        if(y>0)
        {
            node.WayList.Add(new Way(NodeMatrix[x, y - 1], Calculs.LinearDistance));
        }
        if (y<Size-1)
        {
            node.WayList.Add(new Way(NodeMatrix[x, y + 1], Calculs.LinearDistance));
            if (x>0)
            {
                node.WayList.Add(new Way(NodeMatrix[x - 1, y + 1], Calculs.DiagonalDistance));
            }
            if (x<Size-1)
            {
                node.WayList.Add(new Way(NodeMatrix[x + 1, y + 1], Calculs.DiagonalDistance));
            }
        }
    }
    public void aAlgorithm()
    {
        Node startCell = NodeMatrix[startPosx, startPosy];
        Node endCell = NodeMatrix[endPosx, endPosy];
        Node currentCell = startCell;

        float aCUMulatedCost = 0; //Coste acumulado Way.ACUMulatedCost

        Dictionary<Node,float> openList = new Dictionary<Node,float>();
        List<Node> closedList = new List<Node>();
        openList.Add(startCell, 0);

        while (currentCell != endCell)
        {
            aCUMulatedCost = openList[currentCell];
            openList.Remove(currentCell);
            closedList.Add(currentCell);
            foreach (Way way in currentCell.WayList)
            {
                if (!closedList.Contains(way.NodeDestiny))
                {
                    bool cellVisited = false;
                    foreach (var node in openList)
                    {
                        if (node.Key == way.NodeDestiny)
                        {
                            cellVisited = true;
                        }
                    }
                    if (!cellVisited)
                    {
                        way.NodeDestiny.NodeParent = currentCell;
                        openList.Add(way.NodeDestiny,aCUMulatedCost + way.Cost);
                    }
                }
            }
            currentCell = openList.OrderBy(x => x.Value + x.Key.Heuristic).First().Key;
            StartCoroutine(ShowWays(closedList,startCell,currentCell));
        }
    }
    private IEnumerator ShowWays(List<Node> nodes, Node startCell, Node currentCell)
    {
        foreach (var node in nodes)
        {
            if (node != startCell)
            {
                GameObject pin = Instantiate(token, node.RealPosition, Quaternion.identity);
                pin.GetComponent<SpriteRenderer>().color = Color.magenta;
            }
            yield return new WaitForSeconds(0.5f);
        }
        StartCoroutine(ShowPath(currentCell, startCell));
    }
    private IEnumerator ShowPath(Node endCell, Node startCell)
    {
        Node currentCell = endCell.NodeParent;
        while (currentCell != startCell)
        {
            GameObject pin = Instantiate(token, currentCell.RealPosition, Quaternion.identity);
            pin.GetComponent<SpriteRenderer>().color = Color.black;
            currentCell = currentCell.NodeParent;
            yield return null;
        }
    }
}
