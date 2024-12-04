using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.labirint
{
    public class BotController : MonoBehaviour
    {
        public Transform target;
        public float speed = 3.5f;
        public float turnSpeed = 3.0f;
        public float turnDst = 0.5f;
        public float stoppingDst = 1f;

        Pathfinding pathfinding;
        List<WallsGenerate.Node> path;
        int targetIndex;

        void Start()
        {
            pathfinding = GetComponent<Pathfinding>();
            StartCoroutine(UpdatePath());
        }

        void Update()
        {
            if (path != null)
            {
                int pathLength = path.Count;
                if (pathLength > 0)
                {
                    Vector3 targetPosition = path[0].worldPosition;

                    if (Vector3.Distance(transform.position, targetPosition) < stoppingDst)
                    {
                        targetIndex++;
                        if (targetIndex >= pathLength)
                        {
                            targetIndex = 0;
                            path = null;
                            StartCoroutine(UpdatePath());
                        }
                    }

                    if (path != null)
                    {
                        targetPosition = path[targetIndex].worldPosition;
                        Vector3 dir = (targetPosition - transform.position).normalized;
                        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
                        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), turnSpeed * Time.deltaTime);
                    }
                }
            }
        }

        IEnumerator UpdatePath()
        {
            if (pathfinding.FindPath(transform.position, target.position))
            {
                path = pathfinding.path;
                targetIndex = 0;
                StopCoroutine("UpdatePath");
            }
            yield return null;
        }

        public void OnPathFound(Vector3[] waypoints, bool pathSuccessful)
        {
            if (pathSuccessful)
            {
                path = new List<WallsGenerate.Node>();
                foreach (Vector3 waypoint in waypoints)
                {
                    path.Add(pathfinding.gridGenerator.NodeFromWorldPoint(waypoint));
                }
                targetIndex = 0;
            }
        }
    }
}
