using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGeneratorOptimized : MonoBehaviour
{

    [SerializeField] private Tilemap grassTilemap;
    [SerializeField] private Tilemap stoneTilemap;

    [SerializeField] private RuleTile grassTile;
    [SerializeField] private RuleTile stoneTile;

    [SerializeField] private GameObject[] trees;
    [SerializeField] private GameObject[] rocks;
    [SerializeField] private GameObject[] bushes;

    [SerializeField] private Tilemap waterTilemap;
    [SerializeField] private Tilemap foamTilemap;

    [SerializeField] private RuleTile waterTile;
    [SerializeField] private AnimatedTile foamTile;

    
    private Vector2Int mapSize;
    private float[,] perlinNoiseMap;

    private float treeThreshold = 0.6f;
    private float rockThreshold = 0.4f;
    private float bushThreshold = 0.8f;

    private float waterThreshold = 0.3f;
    private float grassThreshold = 0.6f;
    private float stoneThreshold = 0.8f;

    private void Start()
    {
        // Taille de la carte
        mapSize = new Vector2Int(50, 50);

        // Générer bruit Perlin
        perlinNoiseMap = GeneratePerlinNoiseMap();

        // Générer la carte
        GenerateMap();
        AddFoam();
        AddGrassUnderStoneBorders();
        
        // Placer les décors
        //PlaceTrees();
        //PlaceRocks();
        //PlaceBushes();
    }


    private void GenerateMap()
    {

        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                float value = perlinNoiseMap[x, y];

                if (value < waterThreshold)
                {
                    waterTilemap.SetTile(new Vector3Int(x, y, 0), waterTile);
                }
                else if (value < grassThreshold)
                {
                    grassTilemap.SetTile(new Vector3Int(x, y, 0), grassTile);
                }
                else
                {
                    stoneTilemap.SetTile(new Vector3Int(x, y, 0), stoneTile);
                }
            }
        }
    }



    private void AddFoam()
    {

        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {

                if (IsShoreTile(x, y))
                {
                    foamTilemap.SetTile(new Vector3Int(x, y, 0), foamTile);
                }
            }
        }
    }

    private bool IsShoreTile(int x, int y)
    {
        // Directions à vérifier
        Vector3Int[] directions = {
            new Vector3Int(0, 1, 0), // haut
            new Vector3Int(0, -1, 0), // bas  
            new Vector3Int(-1, 0, 0), // gauche
            new Vector3Int(1, 0, 0) // droite
          };

        // Vérifier que la tuile actuelle est de l'herbe
        if (grassTilemap.GetTile(new Vector3Int(x, y, 0)) != grassTile)
        {
            return false;
        }

        // Vérifier les tuiles adjacentes
        foreach (Vector3Int dir in directions)
        {
            Vector3Int neighborPos = new Vector3Int(x + dir.x, y + dir.y, 0);

            // La tuile adjacente est de l'eau
            if (waterTilemap.HasTile(neighborPos))
            {

                return true;
            }
        }

        // Aucune tuile d'eau adjacente
        return false;
    }

    private void AddGrassUnderStoneBorders()
    {

        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {

                if (IsStoneNextToGrass(x, y))
                {

                    // Placer herbe à la position de la pierre
                    grassTilemap.SetTile(new Vector3Int(x, y, 0), grassTile);
                }
            }
        }
    }

    private bool IsStoneNextToGrass(int x, int y)
    {

        // Directions à vérifier
        Vector3Int[] directions = {
            new Vector3Int(0, 1, 0), // haut
            new Vector3Int(0, -1, 0), // bas  
            new Vector3Int(-1, 0, 0), // gauche
            new Vector3Int(1, 0, 0) // droite
          };

        // Si pierre à cette position
        if (stoneTilemap.GetTile(new Vector3Int(x, y, 0)) == stoneTile)
        {

            // Vérifier adjacents
            foreach (Vector3Int dir in directions)
            {

                Vector3Int neighborPos = new Vector3Int(x + dir.x, y + dir.y, 0);

                // Si herbe ici, retourner vrai
                if (grassTilemap.HasTile(neighborPos))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private float[,] GeneratePerlinNoiseMap()
    {
        float[,] noiseMap = new float[mapSize.x, mapSize.y];

        float offsetX = Random.Range(0, 9999f);
        float offsetY = Random.Range(0, 9999f);

        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                float sampleX = (float)x / mapSize.x * 10f + offsetX;
                float sampleY = (float)y / mapSize.y * 10f + offsetY;

                float perlinValue = Mathf.PerlinNoise(sampleX, sampleY);

                noiseMap[x, y] = perlinValue;
            }
        }

        return noiseMap;
    }

    private void PlaceTrees()
    {
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                if (perlinNoiseMap[x, y] > treeThreshold)
                {

                    if (IsGroundTile(x, y))
                    {
                        PlaceTree(x, y);
                    }
                }
            }
        }
    }

    private void PlaceRocks()
    {
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                if (perlinNoiseMap[x, y] > rockThreshold)
                {
                    if (IsGroundTile(x, y))
                    {
                        PlaceRock(x, y);
                    }
                }
            }
        }
    }

    private void PlaceBushes()
    {
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                if (perlinNoiseMap[x, y] > bushThreshold)
                {
                    if (IsGroundTile(x, y))
                    {
                        PlaceBush(x, y);
                    }
                }
            }
        }
    }

    private void PlaceTree(int x, int y)
    {
        // Choix aléatoire de l'arbre
        int treeIndex = Random.Range(0, trees.Length);

        // Instanciation
        GameObject tree = Instantiate(trees[treeIndex], new Vector3(x, y), Quaternion.identity);

        // Échelle aléatoire
        float scale = Random.Range(0.8f, 1.2f);
        tree.transform.localScale = Vector3.one * scale;

        // Rotation aléatoire
        float rotation = Random.Range(-10f, 10f);
        tree.transform.Rotate(Vector3.forward, rotation);

        // Couleur aléatoire
        Color color = new Color(
          Random.Range(0.8f, 1f),
          Random.Range(0.8f, 1f),
          Random.Range(0.8f, 1f)
        );
        tree.GetComponent<SpriteRenderer>().color = color;

        // Assigner comme enfant du tilemap
        tree.transform.SetParent(grassTilemap.transform);
    }

    private void PlaceRock(int x, int y)
    {
        // Choix aléatoire du rock
        int rockIndex = Random.Range(0, rocks.Length);

        // Instanciation
        GameObject rock = Instantiate(rocks[rockIndex], new Vector3(x, y), Quaternion.identity);

        // Échelle aléatoire
        float scale = Random.Range(0.5f, 1.5f);
        rock.transform.localScale = Vector3.one * scale;

        // Rotation aléatoire
        float rotation = Random.Range(-15f, 15f);
        rock.transform.Rotate(Vector3.forward, rotation);

        // Couleur aléatoire
        Color color = new Color(
          Random.Range(0.5f, 1f),
          Random.Range(0.5f, 1f),
          Random.Range(0.5f, 1f)
        );
        rock.GetComponent<SpriteRenderer>().color = color;

        // Assigner comme enfant du tilemap
        rock.transform.SetParent(stoneTilemap.transform);
    }

    private void PlaceBush(int x, int y)
    {
        // Choix aléatoire du bush
        int bushIndex = Random.Range(0, bushes.Length);

        // Instanciation 
        GameObject bush = Instantiate(bushes[bushIndex], new Vector3(x, y), Quaternion.identity);

        // Échelle aléatoire
        float scale = Random.Range(0.8f, 1.2f);
        bush.transform.localScale = Vector3.one * scale;

        // Rotation aléatoire
        float rotation = Random.Range(-5f, 5f);
        bush.transform.Rotate(Vector3.forward, rotation);

        // Couleur aléatoire
        Color color = new Color(
          Random.Range(0.8f, 1f),
          Random.Range(0.8f, 1f),
          Random.Range(0.8f, 1f)
        );
        bush.GetComponent<SpriteRenderer>().color = color;

        // Assigner comme enfant du tilemap 
        bush.transform.SetParent(grassTilemap.transform);
    }


    private bool IsGroundTile(int x, int y)
    {
        return grassTilemap.HasTile(new Vector3Int(x, y, 0));
    }




}
