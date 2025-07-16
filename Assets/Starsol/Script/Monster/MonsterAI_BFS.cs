using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAI_BFS : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float pathUpdateInterval = 1f;

    private Transform player;
    private MazeGenerator maze;
    private bool[,] grid;
    private Queue<Vector2> currentPath = new Queue<Vector2>();

    private bool isMoving = false;

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        maze = FindFirstObjectByType<MazeGenerator>();
        grid = maze != null ? maze.GetGrid() : null;

        StartCoroutine(UpdatePathRoutine());
    }

    IEnumerator UpdatePathRoutine()
    {
        while (true)
        {
            CalculatePath();
            yield return new WaitForSeconds(pathUpdateInterval);
        }
    }

    void CalculatePath()
    {
        if (player == null || grid == null) return;

        Vector2Int start = maze.WorldToGrid(transform.position);
        Vector2Int end = maze.WorldToGrid(player.position);

        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();

        queue.Enqueue(start);
        visited.Add(start);

        Vector2Int[] directions = {
            Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left
        };

        bool pathFound = false;

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();

            if (current == end)
            {
                pathFound = true;
                break;
            }

            foreach (var dir in directions)
            {
                Vector2Int next = current + dir;

                if (IsWalkable(next) && !visited.Contains(next))
                {
                    queue.Enqueue(next);
                    visited.Add(next);
                    cameFrom[next] = current;
                }
            }
        }

        currentPath.Clear();

        if (!pathFound)
            return;

        // 경로 재구성 (end부터 start까지)
        Vector2Int cur = end;
        Stack<Vector2> pathStack = new Stack<Vector2>();

        while (cur != start)
        {
            pathStack.Push(maze.GridToWorld(cur));
            cur = cameFrom[cur];
        }

        while (pathStack.Count > 0)
        {
            currentPath.Enqueue(pathStack.Pop());
        }
    }

    void Update()
    {
        if (!isMoving && currentPath.Count > 0)
        {
            StartCoroutine(MoveToNext());
        }
    }

    IEnumerator MoveToNext()
    {
        isMoving = true;

        Vector2 target = currentPath.Dequeue();

        while (Vector2.Distance(transform.position, target) > 0.01f)
        {
            transform.position = Vector2.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = target;
        isMoving = false;
    }

    bool IsWalkable(Vector2Int pos)
    {
        if (pos.x < 0 || pos.y < 0 || pos.x >= grid.GetLength(0) || pos.y >= grid.GetLength(1))
            return false;

        return grid[pos.x, pos.y];
    }
}
