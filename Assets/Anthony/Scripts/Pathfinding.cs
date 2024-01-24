using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

// Représente un nœud dans le chemin avec des informations telles que la position, les valeurs g, h et le nœud parent.
public class PathNode
{
    public int xPos;
    public int yPos;
    public int gValue;
    public int hValue;

    public PathNode parentNode;

    public int fValue
    {
        get
        {
            return gValue + hValue;
        }
    }

    public PathNode(int xPos, int yPos)
    {
        this.xPos = xPos;
        this.yPos = yPos;
    }

    // Réinitialise les valeurs du nœud.
    public void Clear()
    {
        gValue = 0;
        hValue = 0;
        parentNode = null;
    }
}

// Classe responsable du calcul des chemins sur la grille.
public class Pathfinding
{
    //////////////////////////////////
    //          Variables           // 
    //////////////////////////////////

    private GridMap _gridMap;
    private PathNode[,] _pathNodes;


    public Pathfinding()
    {
        Init();
    }

    //////////////////////////////////////////
    //          Fonctions private           // 
    //////////////////////////////////////////


    // Initialise la classe avec une référence à la grille.
    private void Init()
    {
        if (_gridMap == null)
        {
            _gridMap = GridManager.Instance.GroundGridMap;
        }

        UpdateGrid();
    }

    // Retrace le chemin à partir du nœud final vers le nœud de départ.
    private List<PathNode> RetracePath(PathNode startNode, PathNode endNode)
    {
        List<PathNode> path = new List<PathNode>();

        PathNode currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parentNode;
        }
        path.Reverse();

        return path;
    }

    // Calcule la distance entre deux nœuds.
    private int CalculateDistance(PathNode current, PathNode target)
    {
        int distX = Mathf.Abs(current.xPos - target.xPos);
        int distY = Mathf.Abs(current.yPos - target.yPos);

        return 10 * distY + 10 * distX;
    }


    /////////////////////////////////////////
    //          Fonctions public           // 
    /////////////////////////////////////////

    // Calcule les cellules marchables dans la portée spécifiée autour du point de départ.
    public void CalculateWalkableTerrain(int startX, int startY, int range, ref List<PathNode> toHighlight)
    {
        range *= 10;

        PathNode startNode = _pathNodes[startX, startY];

        List<PathNode> openList = new List<PathNode>();
        List<PathNode> closeList = new List<PathNode>();

        openList.Add(startNode);

        while (openList.Count > 0)
        {
            PathNode currentNode = openList[0];

            openList.Remove(currentNode);
            closeList.Add(currentNode);

            List<PathNode> neighbourNodes = new List<PathNode>();

            for (int x = -1; x < 2; x++)
            {
                for (int y = -1; y < 2; y++)
                {
                    if (x == 0 && y == 0)
                        continue;

                    if (_gridMap.CheckPosition(currentNode.xPos + x, currentNode.yPos + y) == false)
                        continue;

                    // Test pour éviter les voisins dans les angles
                    if (x != 0 && y != 0)
                        continue;

                    neighbourNodes.Add(_pathNodes[currentNode.xPos + x, currentNode.yPos + y]);
                }
            }
            
            for (int i = 0; i < neighbourNodes.Count; i++)
            {
                if (closeList.Contains(neighbourNodes[i]))
                    continue;

                if (!_gridMap.CheckWalkable(neighbourNodes[i].xPos, neighbourNodes[i].yPos))
                    continue;

                int moveCost = currentNode.gValue + CalculateDistance(currentNode, neighbourNodes[i]);

                if (moveCost > range)
                    continue;

                if (!openList.Contains(neighbourNodes[i]) || moveCost < neighbourNodes[i].gValue)
                {
                    neighbourNodes[i].gValue = moveCost;
                    neighbourNodes[i].parentNode = currentNode;

                    if (!openList.Contains(neighbourNodes[i]))
                        openList.Add(neighbourNodes[i]); 
                }
                
            }
        }

        if (toHighlight != null)
        {
            toHighlight.AddRange(closeList);
        }
    }

    // Réinitialise les nœuds de chemin.
    public void Clear()
    {
        for (int x = 0; x < _gridMap.Width; x++)
        {
            for (int y = 0; y < _gridMap.Height; y++)
            {
                _pathNodes[x, y].Clear();
            }
        }
    }

    // Retrace le chemin à partir du nœud final vers le nœud de départ.
    public List<PathNode> TrackBackPath(Character selectedCharacter, int x, int y)
    {
        List<PathNode> path = new List<PathNode>();

        if (!_gridMap.CheckPosition(x, y))
            return null;

        PathNode currentNode = _pathNodes[x, y];

        while (currentNode.parentNode != null)
        {
            path.Add(currentNode);
            currentNode = currentNode.parentNode;
        }

        return path;
    }

    // Trouve un chemin entre deux points sur la grille.
    public List<PathNode> FindPath(int startX, int startY, int endX, int endY)
    {
        // La cellule cible est en dehors de la grille
        if (!_gridMap.CheckPosition(endX, endY))
        {
            return null;
        }

        PathNode startNode = _pathNodes[startX, startY];
        PathNode endNode = _pathNodes[endX, endY];

        List<PathNode> openList = new List<PathNode>();
        List<PathNode> closeList = new List<PathNode>();

        openList.Add(startNode);

        while (openList.Count > 0)
        {
            PathNode currentNode = openList[0];

            for (int i = 0; i < openList.Count; i++)
            {
                if (currentNode.fValue > openList[i].fValue)
                {
                    currentNode = openList[i];
                }

                if (currentNode.fValue == openList[i].fValue
                    && currentNode.hValue > openList[i].hValue)
                {
                    currentNode = openList[i];
                }
            }

            openList.Remove(currentNode);
            closeList.Add(currentNode);

            if (currentNode == endNode)
            {
                // La recherche du chemin est terminée
                return RetracePath(startNode, endNode);
            }

            List<PathNode> neighbourNodes = new List<PathNode>();

            for (int x = -1; x < 2; x++)
            {
                for (int y = -1; y < 2; y++)
                {
                    if (x == 0 && y == 0)
                        continue;

                    if (_gridMap.CheckPosition(currentNode.xPos + x, currentNode.yPos + y) == false)
                        continue;

                    // Test pour éviter les voisins dans les angles
                    if (x != 0 && y != 0)
                        continue;

                    neighbourNodes.Add(_pathNodes[currentNode.xPos + x, currentNode.yPos + y]);
                }
            }

            for (int i = 0; i < neighbourNodes.Count; i++)
            {
                if (closeList.Contains(neighbourNodes[i]))
                    continue;

                if (!_gridMap.CheckWalkable(neighbourNodes[i].xPos, neighbourNodes[i].yPos))
                    continue;

                int movementCost = currentNode.gValue + CalculateDistance(currentNode, neighbourNodes[i]);

                if (!openList.Contains(neighbourNodes[i]) || movementCost < neighbourNodes[i].gValue)
                {
                    neighbourNodes[i].gValue = movementCost;
                    neighbourNodes[i].hValue = CalculateDistance(neighbourNodes[i], endNode);
                    neighbourNodes[i].parentNode = currentNode;

                    if (!openList.Contains(neighbourNodes[i]))
                        openList.Add(neighbourNodes[i]);
                }
            }
        }

        return null;
    }

    public void UpdateGrid()
    {
        _pathNodes = new PathNode[_gridMap.Width, _gridMap.Height];

        for (int x = 0; x < _gridMap.Width; x++)
        {
            for (int y = 0; y < _gridMap.Height; y++)
            {
                _pathNodes[x, y] = new PathNode(x, y);
            }
        }
    }
}
