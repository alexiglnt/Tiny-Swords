using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour
{
    public Tilemap waterTilemap;
    public Tilemap foamTilemap;
    public Tilemap grassTilemap;
    public Tilemap mountailGrassTilemap;
    public Tilemap stoneTilemap;

    public Tile Grass;
    public RuleTile waterRuleTile;
    public RuleTile grassRuleTile;
    public RuleTile stoneRuleTile;
    public AnimatedTile foamRuleTile;
    public List<AnimatedTile> animatedRockTiles;

    public GameObject treePrefab; // Assign in the Unity Editor

    public float forestScale = 0.05f; // Adjusts the "zoom" level of the forest noise
    public float forestThreshold = 0.6f; // Controls the density of the forest
    public float maxTreeScale = 1.5f; // Maximum scale for a tree
    public float minTreeScale = 0.5f; // Minimum scale for a tree
    public float forestOffsetX = 100f; // Default value, adjust in Inspector
    public float forestOffsetY = 100f; // Default value, adjust in Inspector
    public GameObject treesParent;

    public float decorationScale = 0.1f; // Adjusts the "zoom" level of the decoration noise
    public GameObject[] bushPrefabs;
    public GameObject[] mushroomPrefabs;
    public GameObject[] pumpkinPrefabs;
    public GameObject[] stonePrefabs;
    public GameObject decorationsParent;

    // Prefabs for unique decorations
    public GameObject bonePrefab; // There might be only one bone prefab

    // Noise scale and threshold for each type of decoration
    public float bushNoiseScale = 0.1f;
    public float bushThreshold = 0.5f;

    public float mushroomNoiseScale = 0.1f;
    public float mushroomThreshold = 0.5f;

    public float pumpkinNoiseScale = 0.1f;
    public float pumpkinThreshold = 0.5f;

    public float stoneNoiseScale = 0.1f;
    public float stoneThreshold = 0.5f;

    // Chance variables for random spawning
    public float boneSpawnChance = 0.05f; // Adjust this value to control bone spawn rate

    // You might want to control the spawn chance of rocks or other items
    public float rockSpawnChance = 0.1f;

    // Variables to hold offset values for noise functions (if needed)
    public float bushOffsetX;
    public float bushOffsetY;
    public float mushroomOffsetX;
    public float mushroomOffsetY;
    public float pumpkinOffsetX;
    public float pumpkinOffsetY;
    public float stoneOffsetX;
    public float stoneOffsetY;


    public int mapWidth = 100;
    public int mapHeight = 100;
    public float noiseScale = 0.3f;

    public float offsetX = 100f; // Default value, change in Inspector for different maps
    public float offsetY = 100f; // Default value, change in Inspector for different maps

    public float baseRockSpawnChance = 0.1f;


    private void Start()
    {
        StartGeneration();

        // Generate random offsets for map
        offsetX = Random.Range(0f, 9999f);
        offsetY = Random.Range(0f, 9999f);

        // Initialize the trees parent if not set
        if (treesParent == null)
        {
            treesParent = new GameObject("TreesParent");
        }
        // Initialize the deco parent if not set
        if (decorationsParent == null)
        {
            decorationsParent = new GameObject("DecoParent");
        }
        // Generate random offsets for forest
        forestOffsetX = Random.Range(0f, 9999f);
        forestOffsetY = Random.Range(0f, 9999f);

        // Initialize offsets for decorations if needed
        bushOffsetX = Random.Range(0f, 9999f);
        bushOffsetY = Random.Range(0f, 9999f);
        mushroomOffsetX = Random.Range(0f, 9999f);
        mushroomOffsetY = Random.Range(0f, 9999f);
        pumpkinOffsetX = Random.Range(0f, 9999f);
        pumpkinOffsetY = Random.Range(0f, 9999f);
        stoneOffsetX = Random.Range(0f, 9999f);
        stoneOffsetY = Random.Range(0f, 9999f);

        GenerateMap();
    }

    void ClearTilemap(Tilemap tilemap)
    {
        tilemap.ClearAllTiles(); // This will clear all tiles in the tilemap
    }

    // Call this function before you start generating your new tile map
    void ResetTilemaps(Tilemap[] tilemaps)
    {
        foreach (Tilemap tilemap in tilemaps)
        {
            ClearTilemap(tilemap);
        }
    }

    void StartGeneration()
    {
        Tilemap[] tilemapsToReset = new Tilemap[] { stoneTilemap, grassTilemap, mountailGrassTilemap, waterTilemap, foamTilemap };
        ResetTilemaps(tilemapsToReset);

        // Your generation code here
    }




    void GenerateMap()
    {

        // Generate base tiles
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                GenerateTile(x, y);
            }
        }

        // Additional tile rules
        AddFoamToShore();
        EnforceStoneRule();
        AddGrassUnderStoneBorders();
        AddFoamAtBorders(waterTilemap, foamTilemap, foamRuleTile);

        // Place rocks and trees last
        PlaceAnimatedRocks();
        GenerateForest();

        // Last step: place decorative elements
        PlaceDecorations();
        
        // Refresh all tilemaps
        waterTilemap.RefreshAllTiles();
        foamTilemap.RefreshAllTiles();
        grassTilemap.RefreshAllTiles();
        stoneTilemap.RefreshAllTiles();


        GridManager.Instance.GetComponent<SaveLoadMap>().Save();
        GridManager.Instance.GetComponent<SaveLoadMap>().Load();


    }



    void GenerateTile(int x, int y)
    {
        float perlinValue = Mathf.PerlinNoise((x + offsetX) * noiseScale, (y + offsetY) * noiseScale);

        if (perlinValue < 0.3f)
        {
            waterTilemap.SetTile(new Vector3Int(x, y, 0), waterRuleTile);

        }
        else if (perlinValue < 0.6f)
        {
            grassTilemap.SetTile(new Vector3Int(x, y, 0), grassRuleTile);
        }
        else
        {
            stoneTilemap.SetTile(new Vector3Int(x, y, 0), stoneRuleTile);
        }
    }


    void AddFoamToShore()
    {
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                if (IsAdjacentToWater(x, y))
                {
                    foamTilemap.SetTile(new Vector3Int(x, y, 0), foamRuleTile);
                }
            }
        }
    }

    bool IsAdjacentToWater(int x, int y)
    {
        Vector3Int[] adjacentDirections = new Vector3Int[]
        {
            new Vector3Int(0, 1, 0),  // top
            new Vector3Int(0, -1, 0), // bottom
            new Vector3Int(-1, 0, 0), // left
            new Vector3Int(1, 0, 0)   // right
        };

        foreach (Vector3Int dir in adjacentDirections)
        {
            Vector3Int neighborPosition = new Vector3Int(x + dir.x, y + dir.y, 0);
            if (waterTilemap.HasTile(neighborPosition) && grassTilemap.HasTile(new Vector3Int(x, y, 0)))
            {
                return true;
            }
        }

        return false;
    }
    void EnforceStoneRule()
    {
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                // Check if the current tile is a stone tile
                if (stoneTilemap.GetTile(new Vector3Int(x, y, 0)) == stoneRuleTile)
                {
                    // Check for stone tiles above and below
                    bool hasStoneAbove = y + 1 < mapHeight && stoneTilemap.GetTile(new Vector3Int(x, y + 1, 0)) == stoneRuleTile;
                    bool hasStoneBelow = y - 1 >= 0 && stoneTilemap.GetTile(new Vector3Int(x, y - 1, 0)) == stoneRuleTile;

                    // If there's no stone tile above or below, add one
                    if (!hasStoneAbove && !hasStoneBelow)
                    {
                        if (y - 1 >= 0)
                        {
                            // If there's room below, add the stone tile below
                            stoneTilemap.SetTile(new Vector3Int(x, y - 1, 0), stoneRuleTile);
                        }
                        else if (y + 1 < mapHeight)
                        {
                            // If there's room above, add the stone tile above
                            stoneTilemap.SetTile(new Vector3Int(x, y + 1, 0), stoneRuleTile);
                        }
                    }
                }
            }
        }
    }

    void AddGrassUnderStoneBorders()
    {
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                if (IsStoneNextToGrass(x, y))
                {
                    // Set grass tile at the same position as the stone tile
                    mountailGrassTilemap.SetTile(new Vector3Int(x, y, 0), Grass);
                }
            }
        }
    }

    bool IsStoneNextToGrass(int x, int y)
    {
        TileBase currentTile = stoneTilemap.GetTile(new Vector3Int(x, y, 0));
        Vector3Int[] adjacentDirections = new Vector3Int[]
        {
        new Vector3Int(0, 1, 0),  // top
        new Vector3Int(0, -1, 0), // bottom
        new Vector3Int(-1, 0, 0), // left
        new Vector3Int(1, 0, 0)   // right
        };

        bool isNextToGrass = false;

        if (currentTile == stoneRuleTile)
        {
            foreach (Vector3Int dir in adjacentDirections)
            {
                Vector3Int grassPosition = new Vector3Int(x + dir.x, y + dir.y, 0);
                // Check if there is grass on this adjacent tile
                if (grassTilemap.HasTile(grassPosition))
                {
                    isNextToGrass = true;
                    // Check surrounding tiles of the grass tile for additional grass
                    foreach (Vector3Int adjacentDir in adjacentDirections)
                    {
                        Vector3Int adjacentGrassPosition = grassPosition + adjacentDir;
                        // Make sure we're not checking the original stone tile position
                        if (!stoneTilemap.HasTile(adjacentGrassPosition) && grassTilemap.HasTile(adjacentGrassPosition))
                        {
                            // Set mountain grass tile at the position of the initial adjacent grass tile
                            mountailGrassTilemap.SetTile(grassPosition, Grass);
                            break;
                        }
                    }
                }
            }
        }

        return isNextToGrass;
    }



    void PlaceAnimatedRocks()
    {
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                if (IsWaterTile(x, y))
                {
                    float proximityToShore = CalculateProximityToShore(x, y);
                    float spawnChance = baseRockSpawnChance * proximityToShore;
                    if (Random.value < spawnChance)
                    {
                        int rockIndex = Random.Range(0, animatedRockTiles.Count);
                        AnimatedTile rockTile = animatedRockTiles[rockIndex];

                        foamTilemap.SetTile(new Vector3Int(x, y, 0), rockTile);
                    }
                }
            }
        }
    }

    bool IsWaterTile(int x, int y)
    {
        TileBase currentTile = waterTilemap.GetTile(new Vector3Int(x, y, 0));
        return currentTile == waterRuleTile;
    }

    float CalculateProximityToShore(int x, int y)
    {
        // Calculate proximity to shore
        // This is a simple example where the proximity increases the closer the tile is to a non-water tile
        Vector3Int[] adjacentDirections = new Vector3Int[]
        {
        new Vector3Int(0, 1, 0),  // top
        new Vector3Int(0, -1, 0), // bottom
        new Vector3Int(-1, 0, 0), // left
        new Vector3Int(1, 0, 0)   // right
        };

        foreach (Vector3Int dir in adjacentDirections)
        {
            Vector3Int neighborPosition = new Vector3Int(x + dir.x, y + dir.y, 0);
            if (!waterTilemap.HasTile(neighborPosition)) // Check for non-water tiles
            {
                return 0.5f; // Increase chance near the shore
            }
        }

        return 0.1f; // Base chance away from the shore
    }


    void GenerateForest()
    {
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                float forestNoise = Mathf.PerlinNoise((x + forestOffsetX) * forestScale, (y + forestOffsetY) * forestScale);
                if (forestNoise > forestThreshold && CanPlaceTree(x, y))
                {
                    PlaceTree(x, y);
                }
            }
        }
        CleanupTrees();
    }



    bool CanPlaceTree(int x, int y)
    {
        TileBase grassTile = grassTilemap.GetTile(new Vector3Int(x, y, 0));
        TileBase stoneTile = stoneTilemap.GetTile(new Vector3Int(x, y, 0));
        TileBase waterTile = waterTilemap.GetTile(new Vector3Int(x, y, 0));

        bool canPlace = grassTile == grassRuleTile && stoneTile == null && waterTile == null;
        return canPlace;
    }



    void PlaceTree(int x, int y)
    {
        Vector3 treePosition = new Vector3(x + Random.Range(-0.125f, 0.125f), y + Random.Range(-0.125f, 0.125f), 0);
        GameObject tree = Instantiate(treePrefab, treePosition, Quaternion.identity);
        float scale = Random.Range(minTreeScale, maxTreeScale);

        // Randomly flip the tree horizontally
        float flipScaleX = Random.Range(0, 2) == 0 ? -scale : scale;
        tree.transform.localScale = new Vector3(flipScaleX, scale, scale);

        // Random rotation
        float rotationAngle = Random.Range(-4f, 4f); // Random rotation within -10 to 10 degrees
        tree.transform.rotation = Quaternion.Euler(0f, 0f, rotationAngle);

        SpriteRenderer treeSpriteRenderer = tree.GetComponent<SpriteRenderer>();
        if (treeSpriteRenderer != null)
        {
            // Random color tint
            Color tint = new Color(Random.Range(0.8f, 1f), Random.Range(0.8f, 1f), Random.Range(0.8f, 1f), 1f); // Random tint

            treeSpriteRenderer.color = tint;

            // Sorting order based on Y position
            treeSpriteRenderer.sortingOrder = Mathf.RoundToInt((mapHeight - treePosition.y) * 100f);
        }

        tree.transform.SetParent(treesParent.transform);
    }


    void CleanupTrees()
    {
        // Create a list to hold all trees to be removed
        List<GameObject> treesToRemove = new List<GameObject>();

        // Find all trees that are out of bounds or on invalid tiles
        foreach (Transform child in treesParent.transform)
        {
            Vector3Int treeCellPosition = grassTilemap.WorldToCell(child.position);
            if (!IsValidTreePosition(treeCellPosition))
            {
                treesToRemove.Add(child.gameObject);
            }
        }

        // Remove the invalid trees from the scene
        foreach (GameObject tree in treesToRemove)
        {
            Destroy(tree);
        }
    }

    bool IsValidTreePosition(Vector3Int cellPosition)
    {
        // Check if the cell position is out of bounds
        if (cellPosition.x < 0 || cellPosition.x >= mapWidth || cellPosition.y < 0 || cellPosition.y >= mapHeight)
        {
            return false;
        }

        // Check if the cell position has a stone or water tile
        if (stoneTilemap.HasTile(cellPosition) || waterTilemap.HasTile(cellPosition))
        {
            return false;
        }

        // The position is valid if it's in bounds and not occupied by stone or water
        return true;
    }

    void PlaceMushrooms(int x, int y)
    {
        GameObject mushroom;

        // Customize logic for mushroom placement
        float noiseValue = Mathf.PerlinNoise(x * mushroomNoiseScale, y * mushroomNoiseScale);
        if (noiseValue > mushroomThreshold && CanPlaceDecoration(grassTilemap, x, y))
        {
            mushroom = InstantiateRandomPrefab(mushroomPrefabs, x, y);
            mushroom.transform.SetParent(decorationsParent.transform);
        }

    }

    void PlacePumpkins(int x, int y)
    {
        GameObject pumpkin;

        // Customize logic for pumpkin placement
        float noiseValue = Mathf.PerlinNoise(x * pumpkinNoiseScale, y * pumpkinNoiseScale);
        if (noiseValue > pumpkinThreshold && CanPlaceDecoration(grassTilemap, x, y))
        {
            pumpkin = InstantiateRandomPrefab(pumpkinPrefabs, x, y);
            pumpkin.transform.SetParent(decorationsParent.transform);
        }
    }

    void PlaceStones(int x, int y)
    {
        GameObject stone;

        // Customize logic for stone placement
        float noiseValue = Mathf.PerlinNoise(x * stoneNoiseScale, y * stoneNoiseScale);
        if (noiseValue > stoneThreshold && CanPlaceDecoration(stoneTilemap, x, y))
        {
            stone = InstantiateRandomPrefab(stonePrefabs, x, y);
            stone.transform.SetParent(decorationsParent.transform);
        }
    }

    void PlaceBones(int x, int y)
    {
        GameObject bones;

        // Logic for placing bones (possibly random scatter)
        if (Random.value < boneSpawnChance && CanPlaceDecoration(grassTilemap, x, y))
        {
            bones = Instantiate(bonePrefab, CalculateWorldPosition(x, y), Quaternion.identity);
            bones.transform.SetParent(decorationsParent.transform);
        }
    }
 

    // Utility method to instantiate a random prefab from an array at a given position
    GameObject InstantiateRandomPrefab(GameObject[] prefabs, int x, int y)
    {
        GameObject prefab = prefabs[Random.Range(0, prefabs.Length)];
        return Instantiate(prefab, CalculateWorldPosition(x, y), Quaternion.identity);
    }

    // Utility method to check if a tile position can have a decoration placed on it
    bool CanPlaceDecoration(Tilemap tilemap, int x, int y)
    {
        TileBase grassTile = grassTilemap.GetTile(new Vector3Int(x, y, 0));
        TileBase stoneTile = stoneTilemap.GetTile(new Vector3Int(x, y, 0));
        TileBase waterTile = waterTilemap.GetTile(new Vector3Int(x, y, 0));

        bool canPlace = grassTile == grassRuleTile && stoneTile == null && waterTile == null;

        return canPlace;
    }

    // Utility method to convert tilemap coordinates to world position
    Vector3 CalculateWorldPosition(int x, int y)
    {
        // Add randomization if needed to avoid grid-like placement
        return new Vector3(x + Random.Range(-0.5f, 0.5f), y + Random.Range(-0.5f, 0.5f), 0);
    }


    // Place all types of decorations
    void PlaceDecorations()
    {
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                PlaceMushrooms(x, y);
                PlacePumpkins(x, y);
                PlaceStones(x, y);
                PlaceBones(x, y);
            }
        }
        // After placing all decorations
        CleanupInvalidDecorations();

    }

    // Cleanup function for decorations
    void CleanupInvalidDecorations()
    {
        // Create a list to hold all decorations to be removed
        List<GameObject> decorationsToRemove = new List<GameObject>();

        // Find all decorations that are out of bounds or on invalid tiles
        foreach (Transform child in decorationsParent.transform)
        {
            Vector3Int decorationCellPosition = grassTilemap.WorldToCell(child.position);
            if (!IsValidDecorationPosition(decorationCellPosition))
            {
                decorationsToRemove.Add(child.gameObject);
            }
        }

        // Remove the invalid decorations from the scene
        foreach (GameObject decoration in decorationsToRemove)
        {
            Destroy(decoration);
        }
    }

    // Utility method to check if a tile position can have a decoration placed on it
    bool IsValidDecorationPosition(Vector3Int cellPosition)
    {
        // Check if the cell position is out of bounds
        if (cellPosition.x < 0 || cellPosition.x >= mapWidth || cellPosition.y < 0 || cellPosition.y >= mapHeight)
        {
            return false;
        }

        // Check if the cell position has a stone or water tile
        if (stoneTilemap.HasTile(cellPosition) || waterTilemap.HasTile(cellPosition))
        {
            return false;
        }

        // The position is valid if it's in bounds and not occupied by stone or water
        return true;
    }

    void AddFoamAtBorders(Tilemap waterTilemap, Tilemap foamTilemap, TileBase foamTile)
    {
        BoundsInt bounds = waterTilemap.cellBounds;
        TileBase borderTile;

        // Parcours des bordures horizontales (haut et bas)
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            // Haut
            borderTile = waterTilemap.GetTile(new Vector3Int(x, bounds.yMax - 1, 0));
            if (borderTile != waterRuleTile)
            {
                foamTilemap.SetTile(new Vector3Int(x, bounds.yMax - 1, 0), foamTile);
            }

            // Bas
            borderTile = waterTilemap.GetTile(new Vector3Int(x, bounds.yMin, 0));
            if (borderTile != waterRuleTile)
            {
                foamTilemap.SetTile(new Vector3Int(x, bounds.yMin, 0), foamTile);
            }
        }

        // Parcours des bordures verticales (gauche et droite)
        for (int y = bounds.yMin; y < bounds.yMax; y++)
        {
            // Gauche
            borderTile = waterTilemap.GetTile(new Vector3Int(bounds.xMin, y, 0));
            if (borderTile != waterRuleTile)
            {
                foamTilemap.SetTile(new Vector3Int(bounds.xMin, y, 0), foamTile);
            }

            // Droite
            borderTile = waterTilemap.GetTile(new Vector3Int(bounds.xMax - 1, y, 0));
            if (borderTile != waterRuleTile)
            {
                foamTilemap.SetTile(new Vector3Int(bounds.xMax - 1, y, 0), foamTile);
            }
        }
    }


}
