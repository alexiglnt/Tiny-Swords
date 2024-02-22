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

    public GameObject EnemyVillage;
    public GameObject PlayerVillage;

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

        EcosystemScoreManager.Instance.ResetScore(); // To ensure score starts at 0
        EcosystemScoreManager.Instance.EnableScoring();
    }

    void ClearTilemap(Tilemap tilemap) => tilemap.ClearAllTiles(); // This will clear all tiles in the tilemap

    // Call this function before you start generating your new tile map
    private void ResetTilemaps(Tilemap[] tilemaps)
    {
        foreach (Tilemap tilemap in tilemaps)
        {
            ClearTilemap(tilemap);
        }
    }

    private void StartGeneration()
    {
        Tilemap[] tilemapsToReset = new Tilemap[] { stoneTilemap, grassTilemap, mountailGrassTilemap, waterTilemap, foamTilemap };
        ResetTilemaps(tilemapsToReset);

        // Your generation code here
    }

    private void GenerateMap()
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
        ConnectClosestRegions(FindIsolatedRegions());
        AddFoamToShore();
        EnforceStoneRule();
        AddGrassUnderStoneBorders();
        AddFoamAtBorders(waterTilemap, foamTilemap, foamRuleTile);

        // Refresh all tilemaps
        waterTilemap.RefreshAllTiles();
        foamTilemap.RefreshAllTiles();
        grassTilemap.RefreshAllTiles();
        stoneTilemap.RefreshAllTiles();


        GridManager.Instance.GetComponent<SaveLoadMap>().Save();
        GridManager.Instance.GetComponent<SaveLoadMap>().Load();

        // Setup villages
        List<(Vector3 Position, float Radius)> listOfVillages = SetupVillages();
        
        // Place rocks and trees last
        PlaceAnimatedRocks();
        GenerateForest(listOfVillages);

        // place decorative elements
        PlaceDecorations();





    }

    private List<List<Vector3Int>> FindIsolatedRegions()
    {
        List<List<Vector3Int>> isolatedRegions = new List<List<Vector3Int>>();
        HashSet<Vector3Int> visited = new HashSet<Vector3Int>();

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                if (!visited.Contains(pos) && grassTilemap.HasTile(pos))
                {
                    List<Vector3Int> newRegion = BFSGrassRegion(pos, visited);
                    isolatedRegions.Add(newRegion);
                }
            }
        }

        return isolatedRegions;
    }

    private List<Vector3Int> BFSGrassRegion(Vector3Int start, HashSet<Vector3Int> visited)
    {
        List<Vector3Int> region = new List<Vector3Int>();
        Queue<Vector3Int> queue = new Queue<Vector3Int>();

        queue.Enqueue(start);
        visited.Add(start);

        while (queue.Count > 0)
        {
            Vector3Int current = queue.Dequeue();
            region.Add(current);

            foreach (var dir in new Vector3Int[] { Vector3Int.up, Vector3Int.down, Vector3Int.left, Vector3Int.right })
            {
                Vector3Int neighbor = current + dir;
                if (IsInBounds(neighbor) && !visited.Contains(neighbor) && grassTilemap.HasTile(neighbor))
                {
                    visited.Add(neighbor);
                    queue.Enqueue(neighbor);
                }
            }
        }

        return region;
    }

    private bool IsInBounds(Vector3Int pos)
    {
        return pos.x >= 0 && pos.x < mapWidth && pos.y >= 0 && pos.y < mapHeight;
    }

    private void ConnectClosestRegions(List<List<Vector3Int>> regions)
    {
        // Conceptual outline: Further details needed for actual obstacle avoidance and pathfinding.
        for (int i = 0; i < regions.Count; i++)
        {
            List<Vector3Int> regionA = regions[i];
            List<Vector3Int> closestRegion = null;
            float closestDistance = float.MaxValue;
            Vector3Int closestPointA = Vector3Int.zero, closestPointB = Vector3Int.zero;

            for (int j = 0; j < regions.Count; j++)
            {
                if (i == j) continue;
                List<Vector3Int> regionB = regions[j];

                foreach (var pointA in regionA)
                {
                    foreach (var pointB in regionB)
                    {
                        float distance = Vector3Int.Distance(pointA, pointB);
                        if (distance < closestDistance)
                        {
                            closestDistance = distance;
                            closestRegion = regionB;
                            closestPointA = pointA;
                            closestPointB = pointB;
                        }
                    }
                }
            }

            if (closestRegion != null)
            {
                ConnectTwoPointsSimple(closestPointA, closestPointB);
            }
        }
    }

    private void ConnectTwoPointsSimple(Vector3Int start, Vector3Int end)
    {
        // Bresenham's line algorithm for a straight line between two points
        int x0 = start.x, y0 = start.y;
        int x1 = end.x, y1 = end.y;

        int dx = Mathf.Abs(x1 - x0);
        int dy = -Mathf.Abs(y1 - y0);

        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;
        int err = dx + dy, e2; /* error value e_xy */

        int perpX = dy == 0 ? 0 : (dy < 0 ? -1 : 1);
        int perpY = dx == 0 ? 0 : (dx < 0 ? -1 : 1);

        for (; ; )
        {
            ApplyGrassTile(x0, y0);

            // Apply grass to additional tiles on each side of the primary line.
            // Adjust the range (-1, 1) to widen or narrow the path.
            for (int i = -1; i <= 1; i++)
            {
                if (dx > dy)
                {
                    // Wider path in y direction
                    ApplyGrassTile(x0, y0 + i);
                }
                else
                {
                    // Wider path in x direction
                    ApplyGrassTile(x0 + i, y0);
                }
            }

            if (x0 == x1 && y0 == y1) break;
            e2 = 2 * err;
            if (e2 >= dy) { err += dy; x0 += sx; } /* e_xy+e_x > 0 */
            if (e2 <= dx) { err += dx; y0 += sy; } /* e_xy+e_y < 0 */
        }
    }

    private void ApplyGrassTile(int x, int y)
    {
        if (x >= 0 && x < mapWidth && y >= 0 && y < mapHeight)
        {
            Vector3Int tilePosition = new Vector3Int(x, y, 0);
            stoneTilemap.SetTile(tilePosition, null); // Remove stone tile, if any
            grassTilemap.SetTile(tilePosition, grassRuleTile); // Place grass tile
        }
    }

    private void GenerateTile(int x, int y)
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

    private void AddFoamToShore()
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

    private bool IsAdjacentToWater(int x, int y)
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

    private void EnforceStoneRule()
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

    private void AddGrassUnderStoneBorders()
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

    private bool IsStoneNextToGrass(int x, int y)
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

    private void PlaceAnimatedRocks()
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

    private bool IsWaterTile(int x, int y)
    {
        TileBase currentTile = waterTilemap.GetTile(new Vector3Int(x, y, 0));
        return currentTile == waterRuleTile;
    }

    private float CalculateProximityToShore(int x, int y)
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

    private void GenerateForest(List<(Vector3 Position, float Radius)> listOfVillages)
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
        CleanupTrees(listOfVillages);
    }

    private bool CanPlaceTree(int x, int y)
    {
        TileBase grassTile = grassTilemap.GetTile(new Vector3Int(x, y, 0));
        TileBase stoneTile = stoneTilemap.GetTile(new Vector3Int(x, y, 0));
        TileBase waterTile = waterTilemap.GetTile(new Vector3Int(x, y, 0));

        bool canPlace = grassTile == grassRuleTile && stoneTile == null && waterTile == null;
        return canPlace;
    }

    private void PlaceTree(int x, int y)
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

    private void CleanupTrees(List<(Vector3 Position, float Radius)> listOfVillages)
    {
        // Create a list to hold all trees to be removed
        List<GameObject> treesToRemove = new List<GameObject>();

        // Find all tree GameObjects within the cleanup radius of any village center
        foreach (Transform child in treesParent.transform)
        {
            foreach (var village in listOfVillages)
            {
                Vector3 center = village.Position;
                float radius = village.Radius;
                float squaredRadius = radius * radius;

                if ((child.position - center).sqrMagnitude <= squaredRadius)
                {
                    treesToRemove.Add(child.gameObject);
                }
            }
        }

        // Remove all trees in the list
        foreach (GameObject tree in treesToRemove)
        {
            Destroy(tree);
        }
    }

    private void PlaceMushrooms(int x, int y)
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

    private void PlacePumpkins(int x, int y)
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

    private void PlaceStones(int x, int y)
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

    private void PlaceBones(int x, int y)
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
    private GameObject InstantiateRandomPrefab(GameObject[] prefabs, int x, int y)
    {
        GameObject prefab = prefabs[Random.Range(0, prefabs.Length)];
        return Instantiate(prefab, CalculateWorldPosition(x, y), Quaternion.identity);
    }

    // Utility method to check if a tile position can have a decoration placed on it
    private bool CanPlaceDecoration(Tilemap tilemap, int x, int y)
    {
        TileBase grassTile = grassTilemap.GetTile(new Vector3Int(x, y, 0));
        TileBase stoneTile = stoneTilemap.GetTile(new Vector3Int(x, y, 0));
        TileBase waterTile = waterTilemap.GetTile(new Vector3Int(x, y, 0));

        bool canPlace = grassTile == grassRuleTile && stoneTile == null && waterTile == null;

        return canPlace;
    }

    // Utility method to convert tilemap coordinates to world position
    private Vector3 CalculateWorldPosition(int x, int y) => new Vector3(x + Random.Range(-0.5f, 0.5f), y + Random.Range(-0.5f, 0.5f), 0);


    // Place all types of decorations
    private void PlaceDecorations()
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
    private void CleanupInvalidDecorations()
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
    private bool IsValidDecorationPosition(Vector3Int cellPosition)
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

    private void AddFoamAtBorders(Tilemap waterTilemap, Tilemap foamTilemap, TileBase foamTile)
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

    private List<Vector3Int> FindSuitableLocations(Tilemap tilemap, int requiredSpace)
    {
        List<Vector3Int> suitableLocations = new List<Vector3Int>();

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                Vector3Int tilePosition = new Vector3Int(x, y, 0);
                if (IsLocationSuitable(tilemap, tilePosition, requiredSpace))
                {
                    suitableLocations.Add(tilePosition);
                }
            }
        }

        return suitableLocations;
    }

    private bool IsLocationSuitable(Tilemap tilemap, Vector3Int location, int requiredSpace)
    {
        // Check if the central tile itself is grass and not a tree
        if (!IsGrassTile(tilemap, location)) return false;

        int grassTileCount = 0;
        int radius = Mathf.CeilToInt(Mathf.Sqrt(requiredSpace / Mathf.PI)); // Circular approximation for required space

        // Expand the iteration area for checking stone tiles around the potential location
        int checkRadius = radius + 1; // +2 to check for stone tiles just outside the village area

        for (int x = -checkRadius; x <= checkRadius; x++)
        {
            for (int y = -checkRadius; y <= checkRadius; y++)
            {
                Vector3Int currentPos = new Vector3Int(location.x + x, location.y + y, location.z);

                if (x >= -radius && x <= radius && y >= -radius && y <= radius)
                {
                    // Core village area checks (within radius)
                    if (IsGrassTile(tilemap, currentPos))
                    {
                        grassTileCount++;
                    }
                    else
                    {
                        // Early return if a tree is found within the intended village area
                        return false;
                    }
                }
                else
                {
                    // Check surrounding area only for stone tiles (outside core village area but within checkRadius)
                    if (IsStoneTile(tilemap, currentPos))
                    {
                        return false; // Avoid locations adjacent to stone tiles
                    }
                }
            }
        }

        return grassTileCount >= requiredSpace;
    }


    // Helper method to check if a tile at a given position is a grass tile
    private bool IsGrassTile(Tilemap tilemap, Vector3Int position)
    {
        TileBase tile = tilemap.GetTile(position);
        return tile != null && tile == grassRuleTile; // Assuming 'grassRuleTile' is the reference to your grass tile
    }

    // Helper method to check if a tile at a given position is a tree tile
    private bool IsStoneTile(Tilemap tilemap, Vector3Int position)
    {
        TileBase tile = tilemap.GetTile(position);
        return tile != null && tile == stoneRuleTile; // Assuming 'treeRuleTile' is the reference to your tree tile
    }

    private (Vector3Int playerVillage, Vector3Int enemyVillage) ChooseVillageLocations(List<Vector3Int> locations)
    {
        Vector3Int playerVillage = new Vector3Int(-1, -1, -1);
        Vector3Int enemyVillage = new Vector3Int(-1, -1, -1);
        float maxDistance = 0f; // Initialize with 0 to ensure any positive distance will be larger

        for (int i = 0; i < locations.Count; i++)
        {
            for (int j = i + 1; j < locations.Count; j++)
            {
                float distance = Vector3Int.Distance(locations[i], locations[j]);
                if (distance > maxDistance) // Use '>' to ensure the farthest pair is selected
                {
                    maxDistance = distance;
                    playerVillage = locations[i];
                    enemyVillage = locations[j];
                }
            }
        }

        return (playerVillage, enemyVillage);
    }

    private void SpawnVillage(Vector3Int location, GameObject villagePrefab)
    {
        // Convert tilemap location to world space, adjust as necessary for your project
        Vector3 worldPosition = grassTilemap.CellToWorld(location) + new Vector3(0.5f, 0.5f, 0); // Centering the prefab
        Instantiate(villagePrefab, worldPosition, Quaternion.identity);
    }

    // Example usage
    List<(Vector3 Position, float Radius)> SetupVillages()
    {
        int requiredSpace = 15;
        var suitableLocations = FindSuitableLocations(grassTilemap, requiredSpace); // Example requiredSpace
        var (playerVillagePos, enemyVillagePos) = ChooseVillageLocations(suitableLocations); // Example minDistance

        float playerVillageRadius = CalculateVillageRadius(requiredSpace,2);
        float enemyVillageRadius = CalculateVillageRadius(requiredSpace,2);

        if (playerVillagePos != new Vector3Int (-1,-1,-1) && enemyVillagePos != new Vector3Int(-1, -1, -1))
        {
            GameObject playerVillagePrefab = PlayerVillage; // Assign your prefab
            GameObject enemyVillagePrefab = EnemyVillage; // Assign your prefab

            SpawnVillage(playerVillagePos, playerVillagePrefab);
            SpawnVillage(enemyVillagePos, enemyVillagePrefab);
        }
        else
        {
            Debug.LogError("Failed to find suitable locations for villages.");
        }
        // Initialize the list with positions
        List<(Vector3 Position, float Radius)> listOfVillages = new List<(Vector3 Position, float Radius)>
        {
            (playerVillagePos, playerVillageRadius),
            (enemyVillagePos, enemyVillageRadius)
        };

        return listOfVillages;
    }

    private float CalculateVillageRadius(int requiredSpace, float buffer = 0)
    {
        // Calculate the basic radius from required space
        float radius = Mathf.Sqrt(requiredSpace / Mathf.PI);

        // Add buffer to the radius to account for additional space around the village
        return radius + buffer;
    }


}
