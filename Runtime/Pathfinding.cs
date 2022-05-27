using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

namespace Pathfinder
{
    [Serializable]
    public class Pathfinding
    {
        #region private variables
        private List<Point> chosenPath;
        private bool choosingPath;
        private int pathDeviationFactor, currentTarget;
        #endregion

        #region public variables
        public Vector2 startingPoint;
        #endregion

        #region serialized fields
        [SerializeField]
        private MovementAxis defaultMovement;
        [SerializeField]
        private Map map;
        [SerializeField]
        private bool multiplePoints;
        [SerializeField]
        private Vector2 targetPoint;
        [SerializeField]
        private List<Vector2> targetPoints;

        public Vector2 TargetPoint { get { return targetPoint; } }
        public List<Vector2> TargetPoints { get { return targetPoints; } }
        public bool MultiplePoints { get { return multiplePoints; } }


        #endregion

        #region thread class
        private static class Thread
        {
            static bool isRunning = false;
            static bool isSleeping = false;
            static List<Pathfinding> queue;
            static System.Threading.Thread workingThread;

            public static void AddToQueue(Pathfinding reference)
            {
                if (!isRunning)
                    InitializeThread();
                if (!queue.Contains(reference))
                    queue.Add(reference);
                if (isSleeping)
                    workingThread.Interrupt();
            }

            private static void InitializeThread()
            {
                isRunning = true;
                queue = new List<Pathfinding>();
                workingThread = new System.Threading.Thread(ExecuteQueue);
                workingThread.Start();
            }

            static void ExecuteQueue()
            {
                if (queue.Count == 0)
                {
                    System.Threading.Thread.Sleep(Timeout.Infinite);
                    isSleeping = true;
                }
                else
                    queue[0].FindPath();
            }
        }

        #endregion

        #region constructors
        public Pathfinding(int deviationFactor = 0)
        {
            chosenPath = new List<Point>();
            pathDeviationFactor = deviationFactor;
            Debug.Log(targetPoint);
            //Debug.Log(targetPoints.Length);
        }
        #endregion

        #region public methods
        /// <summary>
        /// Get a random position with a max distance.
        /// </summary>
        /// <param name="totalDistance">The max distance it can move from it's starting position for both X and Y.</param>
        /// <param name="startingPosition">The starting point of the pathfinding Pathfinding.</param>
        /// <returns>A Vector2 with the position of the point.</returns>
        public Vector2 GetRandomPosition(float totalDistance, Vector2 startingPosition)
        {
            float x = Mathf.Max(startingPosition.x - totalDistance, startingPosition.x + totalDistance);
            float y = Mathf.Max(startingPosition.y - totalDistance, startingPosition.y + totalDistance);
            Vector2 chosenPoint = new Vector2(x, y);
            return chosenPoint;
        }

        /// <summary>
        /// Get a random position with a max distance that could be different on both axis.
        /// </summary>
        /// <param name="xDistance">The max distance it can move from it's starting position on the X axis.</param>
        /// <param name="yDistance">The max distance it can move from it's starting position on the Y axis.</param>
        /// <param name="startingPosition">The starting point of the pathfinding Pathfinding.</param>
        /// <returns>A Vector2 with the position of the point.</returns>
        public Vector2 GetRandomPosition(float xDistance, float yDistance, Vector2 startingPosition)
        {
            float x = Mathf.Max(startingPosition.x - xDistance, startingPosition.x + xDistance);
            float y = Mathf.Max(startingPosition.y - yDistance, startingPosition.y + yDistance);
            Vector2 chosenPoint = new Vector2(x, y);
            return chosenPoint;
        }

        /// <summary>
        /// Execute the pathfinding calculation, can be queued on an asyncronous thread.
        /// <param name="async">Set it to true to queue the calculation on an asyncronous thread.</param>
        /// </summary>
        public void FindPath(bool async = false)
        {
            if (async)
                Thread.AddToQueue(this);
            else
            {
                choosingPath = true;
                Point origin = map.GetPoint(startingPoint);
                Point end;
                if (multiplePoints)
                    end = map.GetPoint(targetPoints[0]);
                else
                    end = map.GetPoint(targetPoint);

                List<Point> pointsToCheck = new List<Point>();
                HashSet<Point> checkedPoints = new HashSet<Point>();

                pointsToCheck.Add(origin);

                while (pointsToCheck.Count > 0)
                {
                    Point currentPoint = pointsToCheck[0];

                    for (int index = 1; index < pointsToCheck.Count; index++)
                    {
                        if (pointsToCheck[index].PointValue < currentPoint.PointValue || pointsToCheck[index].PointValue == currentPoint.PointValue && pointsToCheck[index].goalDistance < currentPoint.goalDistance)
                            currentPoint = pointsToCheck[index];
                    }

                    pointsToCheck.Remove(currentPoint);
                    checkedPoints.Add(currentPoint);

                    if (currentPoint == end)
                    {
                        FinalPath(origin, currentPoint);
                        break;
                    }
                    List<Point> sidePoints = map.GetSidePoints(currentPoint, defaultMovement);

                    if (sidePoints.Count > 0)
                        foreach (Point sidePoint in sidePoints)
                        {
                            if (sidePoint.isCollider || checkedPoints.Contains(sidePoint))
                                continue;

                            int movementValue = currentPoint.startDistance + GetPointDistance(currentPoint, sidePoint) + 1 + UnityEngine.Random.Range(-pathDeviationFactor, pathDeviationFactor + 1);

                            if (movementValue < sidePoint.startDistance || !pointsToCheck.Contains(sidePoint))
                            {
                                sidePoint.startDistance = movementValue;
                                sidePoint.goalDistance = GetPointDistance(sidePoint, end);
                                sidePoint.parent = currentPoint;

                                if (!pointsToCheck.Contains(sidePoint))
                                    pointsToCheck.Add(sidePoint);
                            }
                        }
                }
            }
        }
        #endregion

        #region private methods
        /// <summary>
        /// Starts moving around the chosen path backwards to ensure it gets the shortest path.
        /// </summary>
        /// <param name="startPoint">Point where the path started</param>
        /// <param name="targetPoint">Point where the path finished.</param>
        void FinalPath(Point startPoint, Point targetPoint)
        {
            List<Point> path = new List<Point>();
            Point currentPoint = targetPoint;

            while (currentPoint != startPoint)
            {
                path.Add(currentPoint);
                currentPoint = currentPoint.parent;
            }

            path.Reverse();

            if (multiplePoints)
            {
                chosenPath.AddRange(path);
                targetPoints.RemoveAt(0);
                if (targetPoints.Count > 0)
                {
                    startingPoint = targetPoints[0];
                    FindPath();
                }
            }
            else
                chosenPath = path;

            choosingPath = false;
        }

        /// <summary>
        /// Calculates the distance from a given point to another (x distance + y distance).
        /// </summary>
        /// <param name="firstPoint">Point you want to check</param>
        /// <param name="secondPoint">Objective point</param>
        /// <returns>int with given distance (in units).</returns>
        int GetPointDistance(Point firstPoint, Point secondPoint)
        {
            int _x = Mathf.Abs((int)(firstPoint.position.x - secondPoint.position.x));
            int _y = Mathf.Abs((int)(firstPoint.position.y - secondPoint.position.y));

            return _x + _y;
        }
        #endregion
    }

    [Serializable]
    public enum MovementAxis { HORIZONTAL, VERTICAL, FOUR_SIDES, EIGHT_SIDES }
}



