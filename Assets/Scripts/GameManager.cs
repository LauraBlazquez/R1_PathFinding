using System.Collections.Generic;
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
        int actualNodeX = startPosx;
        int actualNodeY = startPosy;
        while (actualNodeX != endPosx && actualNodeY != endPosy)
        {
            Way bestChoice = new Way(NodeMatrix[actualNodeX, actualNodeY], 8000f);
            foreach (Way way in NodeMatrix[actualNodeX, actualNodeY].WayList)
            {
                float realCost = way.Cost + way.NodeDestiny.Heuristic;
                //Debug.Log(realCost);
                if (realCost < (bestChoice.Cost + bestChoice.NodeDestiny.Heuristic))
                {
                    bestChoice = way;
                }
            }
            Debug.Log(bestChoice.Cost + bestChoice.NodeDestiny.Heuristic);
            actualNodeX = bestChoice.NodeDestiny.PositionX;
            actualNodeY = bestChoice.NodeDestiny.PositionY;
            if (actualNodeX != endPosx && actualNodeY != endPosy)
            {
                GameObject newOrigin = Instantiate(token, NodeMatrix[actualNodeX, actualNodeY].RealPosition, Quaternion.identity);
                SpriteRenderer sprite = newOrigin.GetComponent<SpriteRenderer>();
                sprite.color = Color.magenta;
            }
        }
    }
}
