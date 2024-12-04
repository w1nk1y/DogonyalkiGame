using System.Collections.Generic;
using UnityEngine;

namespace Assets.labirint
{
    public class WallsGenerate : MonoBehaviour
    {
        private enum Grid { empty, floor, wall };
        private Grid[,] grid;
        private int width = 30;
        private int height = 30;

        private struct Indicator
        {
            public Vector2 pos;
            public Vector2 dir;
        }

        private List<Indicator> inds;
        private float chanceToTurn = 0.5f;
        private float chanceToSpawn = 0.02f;
        private float chanceToDestroy = 0.08f;
        private int maxAmount = 99999;
        private float fillRatio = 0.5f;

        public List<Node> path;

        void Init()
        {
            grid = new Grid[width, height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    grid[i, j] = Grid.empty;
                }
            }

            inds = new List<Indicator>();
            Indicator ind = new Indicator();
            ind.dir = RandomDirection();
            ind.pos = new Vector2(Mathf.RoundToInt(width / 2.0f), Mathf.RoundToInt(height / 2.0f));
            inds.Add(ind);
        }

        void FloorGeneration()
        {
            int counter = 0;
            while (counter < 123456)
            {
                foreach (Indicator floor in inds)
                {
                    grid[(int)floor.pos.x, (int)floor.pos.y] = Grid.floor;
                }

                int numbers = inds.Count;
                for (int i = 0; i < numbers; i++)
                {
                    if (Random.value < chanceToDestroy && numbers > 1)
                    {
                        inds.RemoveAt(i);
                        break;
                    }
                }

                for (int i = 0; i < inds.Count; i++)
                {
                    if (Random.value < chanceToTurn)
                    {
                        Indicator tempInd = inds[i];
                        tempInd.dir = RandomDirection();
                        inds[i] = tempInd;
                    }
                }

                int num = inds.Count;
                for (int i = 0; i < num; i++)
                {
                    if (Random.value < chanceToSpawn && num < maxAmount)
                    {
                        Indicator newInd = new Indicator();
                        newInd.dir = RandomDirection();
                        newInd.pos = inds[i].pos;
                        inds.Add(newInd);
                    }
                }

                for (int i = 0; i < inds.Count; i++)
                {
                    Indicator tempInd = inds[i];
                    tempInd.pos += tempInd.dir;
                    inds[i] = tempInd;
                }

                for (int i = 0; i < inds.Count; i++)
                {
                    Indicator tempInd = inds[i];
                    tempInd.pos.x = Mathf.Clamp(tempInd.pos.x, 1, width - 2);
                    tempInd.pos.y = Mathf.Clamp(tempInd.pos.y, 1, height - 2);
                    inds[i] = tempInd;
                }

                if (NumberOfFloors() / (float)grid.Length > fillRatio)
                {
                    break;
                }
                counter++;
            }
        }

        private int NumberOfFloors()
        {
            int count = 0;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (grid[i, j] == Grid.floor)
                    {
                        count++;
                    }
                }
            }
            return count;
        }

        private int CountAdjacentWalls(int x, int y)
        {
            int count = 0;
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0)
                        continue;

                    int newX = x + i;
                    int newY = y + j;

                    if (newX >= 0 && newX < width && newY >= 0 && newY < height)
                    {
                        if (grid[newX, newY] == Grid.wall)
                            count++;
                    }
                }
            }
            return count;
        }

        void WallGeneration()
        {
            for (int i = 1; i < width - 1; i++)
            {
                for (int j = 1; j < height - 1; j++)
                {
                    if (grid[i, j] == Grid.floor)
                    {
                        int adjacentWalls = CountAdjacentWalls(i, j);
                        if (adjacentWalls < 3)
                        {
                            continue;
                        }

                        if (grid[i - 1, j] == Grid.empty)
                            grid[i - 1, j] = Grid.wall;
                        if (grid[i + 1, j] == Grid.empty)
                            grid[i + 1, j] = Grid.wall;
                        if (grid[i, j - 1] == Grid.empty)
                            grid[i, j - 1] = Grid.wall;
                        if (grid[i, j + 1] == Grid.wall)
                            grid[i, j + 1] = Grid.wall;
                    }
                }
            }

            for (int i = 1; i < width - 1; i++)
            {
                for (int j = 1; j < height - 1; j++)
                {
                    if (grid[i, j] == Grid.floor)
                    {
                        if (grid[i - 1, j] == Grid.empty)
                            grid[i - 1, j] = Grid.wall;
                        if (grid[i + 1, j] == Grid.empty)
                            grid[i + 1, j] = Grid.wall;
                        if (grid[i, j - 1] == Grid.empty)
                            grid[i, j - 1] = Grid.wall;
                        if (grid[i, j + 1] == Grid.wall)
                            grid[i, j + 1] = Grid.wall;
                    }
                }
            }

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (i == 0 || i == width - 1 || j == 0 || j == height - 1)
                    {
                        grid[i, j] = Grid.wall;
                    }
                }
            }

            for (int i = 1; i < width - 1; i++)
            {
                for (int j = 1; j < height - 1; j++)
                {
                    if (grid[i, j] == Grid.empty)
                    {
                        grid[i, j] = Grid.floor;
                    }
                }
            }
        }

        void RemoveSingle()
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (grid[i, j] == Grid.wall)
                    {
                        bool all = true;
                        for (int x = -1; x <= 1; x++)
                        {
                            for (int y = -1; y <= 1; y++)
                            {
                                if (i + x < 0 || i + x > width - 1 || j + y < 0 || j + y > height - 1)
                                    continue;

                                if (x != 0 && y != 0 || x == 0 && y == 0)
                                    continue;

                                if (grid[i + x, j + y] != Grid.floor)
                                    all = false;
                            }
                        }
                        if (all)
                        {
                            grid[i, j] = Grid.floor;
                        }
                    }
                }
            }
        }

        Vector2 RandomDirection()
        {
            int choice = Mathf.FloorToInt(Random.value * 3.99f);

            switch (choice)
            {
                case 0:
                    return Vector2.down;
                case 1:
                    return Vector2.up;
                case 2:
                    return Vector2.left;
                default:
                    return Vector2.right;
            }
        }

        public GameObject[] floors;
        public GameObject[] walls;

        void Spawn(float x, float y, GameObject go)
        {
            int length = 1;
            Vector2 spawnPos = new Vector2(x, y) * length;
            Quaternion orientation = Quaternion.identity;
            orientation.eulerAngles = ObjOrientation();

            GameObject toSpawn = Instantiate(go, new Vector3(spawnPos.x, 0, spawnPos.y), orientation) as GameObject;
        }

        void SpawnMap()
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    switch (grid[x, y])
                    {
                        case Grid.empty:
                            break;
                        case Grid.floor:
                            Spawn(x, y, floors[Random.Range(0, floors.Length)]);
                            break;
                        case Grid.wall:
                            Spawn(x, y, walls[Random.Range(0, walls.Length)]);
                            break;
                    }
                }
            }
        }

        Vector3 ObjOrientation()
        {
            int choice = Mathf.FloorToInt(Random.value * 3.99f);

            switch (choice)
            {
                case 0:
                    return new Vector3(0, 0, 0);
                case 1:
                    return new Vector3(0, 90, 0);
                case 2:
                    return new Vector3(0, -90, 0);
                default:
                    return new Vector3(0, 180, 0);
            }
        }

        void Start()
        {
            Init();
            FloorGeneration();
            WallGeneration();
            RemoveSingle();
            SpawnMap();
        }

        void Update()
        {
        }

        public Node NodeFromWorldPoint(Vector3 worldPosition)
        {
            float percentX = (worldPosition.x + width / 2) / width;
            float percentY = (worldPosition.z + height / 2) / height;
            percentX = Mathf.Clamp01(percentX);
            percentY = Mathf.Clamp01(percentY);

            int x = Mathf.RoundToInt((width - 1) * percentX);
            int y = Mathf.RoundToInt((height - 1) * percentY);
            return new Node(grid[x, y] == Grid.floor, new Vector3(x, 0, y), x, y);
        }

        public List<Node> GetNeighbours(Node node)
        {
            List<Node> neighbours = new List<Node>();

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0)
                        continue;

                    int checkX = node.gridX + x;
                    int checkY = node.gridY + y;

                    if (checkX >= 0 && checkX < width && checkY >= 0 && checkY < height)
                    {
                        neighbours.Add(new Node(grid[checkX, checkY] == Grid.floor, new Vector3(checkX, 0, checkY), checkX, checkY));
                    }
                }
            }

            return neighbours;
        }

        public class Node
        {
            public bool walkable;
            public Vector3 worldPosition;
            public int gridX;
            public int gridY;

            public int gCost;
            public int hCost;
            public Node parent;

            public Node(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY)
            {
                walkable = _walkable;
                worldPosition = _worldPos;
                gridX = _gridX;
                gridY = _gridY;
            }

            public int fCost
            {
                get
                {
                    return gCost + hCost;
                }
            }
        }
    }
}
