using UnityEngine;

// Repr�sente un n�ud dans la grille contenant un identifiant de tuile et une r�f�rence � un personnage.
public class Node
{
    public int tileId;
    public MapElement mapElement;
}

// Classe principale qui repr�sente la grid avec ce qu'elle contienne
public class GridMap
{
    //////////////////////////////////
    //          Variables           // 
    //////////////////////////////////

    // Hauteur de la grid
    private int _height;
    public int Height
    {
        get { return _height; }
        private set { _height = value; }
    }

    // Largeur de la grid
    private int _width;
    public int Width
    {
        get { return _width; }
        private set { _width = value; }
    }

    private Node[,] _grid;


    //////////////////////////////////
    //          Fonctions           // 
    //////////////////////////////////

    // Initialise la grille avec la largeur et la hauteur sp�cifi�es.
    public void Init(int width, int height)
    {
        _grid = new Node[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                _grid[x, y] = new Node();
            }
        }

        _height = height;
        _width = width;
    }

    // Efface l'�l�ment � la position sp�cifi�e dans la grille.
    public void ClearMapElement(int xPos, int yPos)
    {
        _grid[xPos, yPos].mapElement = null;
    }

    // D�finit l'�l�ment � la position sp�cifi�e dans la grille.
    public void SetMapElement(MapElement mapElement, int x_pos, int y_pos)
    {
        // On verifi que la position existe dans la grid
        if(x_pos >= 0 && x_pos < Width && y_pos >= 0 && y_pos < Height)
            _grid[x_pos, y_pos].mapElement = mapElement;
    }

    // D�finit l'identifiant de tuile � la position sp�cifi�e dans la grille.
    public void SetTile(int x, int y, int to)
    {
        if (!CheckPosition(x, y))
        {
            Debug.LogWarning("Trying to Set a cell outside the Grid boundaries x = "
                + x.ToString() + ", y = " + y.ToString());
            return;
        }

        _grid[x, y].tileId = to;
    }

    // R�cup�re l'identifiant de tuile � la position sp�cifi�e dans la grille.
    public int GetTile(int x, int y)
    {
        if (!CheckPosition(x, y))
        {
            Debug.LogWarning("Trying to Get a cell outside the Grid boundaries x = "
                + x.ToString() + ", y = " + y.ToString());
            return -1;
        }

        return _grid[x, y].tileId;
    }

    // V�rifie si la position sp�cifi�e dans la grille est valide.
    public bool CheckPosition(int x, int y)
    {
        if (x < 0 || x >= _width)
            return false;

        if (y < 0 || y >= _height)
            return false;

        return true;
    }

    // R�cup�re l'�l�ment � la position sp�cifi�e dans la grille.
    public MapElement GetMapElement(int x, int y)
    {
        return _grid[x, y].mapElement;
    }

    // V�rifie si la position sp�cifi�e dans la grille est marchable.
    public bool CheckWalkable(int xPos, int yPos)
    {
        return _grid[xPos, yPos].tileId == 0 && _grid[xPos, yPos].mapElement == null;
    }
}
