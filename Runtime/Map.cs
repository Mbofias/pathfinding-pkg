using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
    [CreateAssetMenu(menuName = "Pathfinding/Map")]
    public class Map : ScriptableObject
    {
        Point[,] map;

        int sizeX, sizeY;
        public int highestX = -1000, highestY = -1000, lowestX = 1000, lowestY = 1000;

        /// <summary>
        /// Get the point of the map that is on a given vector;
        /// </summary>
        /// <param name="worldPosition">The given vector.</param>
        /// <returns></returns>
        public Point GetPoint(Vector2 worldPosition)
        {
            Vector2Int tmpVector = new Vector2Int();
            tmpVector = Vector2Int.RoundToInt(worldPosition - map[0, 0].position);
            tmpVector.x = tmpVector.x < sizeX ? tmpVector.x : sizeX;
            tmpVector.y = tmpVector.y < sizeY ? tmpVector.y : sizeY;
            return map[tmpVector.x, tmpVector.y];
        }

        /// <summary>
        /// Get every surounding point of a given central point.
        /// </summary>
        /// <param name="centralPoint">The central point</param>
        /// <returns>An array that contains every available point.</returns>
        public List<Point> GetSidePoints(Point centralPoint, MovementAxis movement)
        {
            List<Point> returnList = new List<Point>();

            bool right = true, left = true, top = true, bot = true;

            if (centralPoint == null)
                return returnList;

            switch (movement)
            {
                case MovementAxis.HORIZONTAL:
                    if (centralPoint.x + 1 < sizeX)
                    {
                        returnList.Add(map[centralPoint.y, centralPoint.x + 1]);
                        if (map[centralPoint.y, centralPoint.x + 1].isCollider)
                            right = false;
                    }
                    if (centralPoint.x - 1 >= 0)
                    {
                        returnList.Add(map[centralPoint.y, centralPoint.x - 1]);
                        if (map[centralPoint.y, centralPoint.x - 1].isCollider)
                            left = false;
                    }
                    break;
                case MovementAxis.VERTICAL:
                    if (centralPoint.y + 1 < sizeY)
                    {
                        returnList.Add(map[centralPoint.y + 1, centralPoint.x]);
                        if (map[centralPoint.y + 1, centralPoint.x].isCollider)
                            top = false;
                    }
                    if (centralPoint.y - 1 >= 0)
                    {
                        returnList.Add(map[centralPoint.y - 1, centralPoint.x]);
                        if (map[centralPoint.y - 1, centralPoint.x].isCollider)
                            bot = false;
                    }
                    break;
                case MovementAxis.FOUR_SIDES:
                    if (centralPoint.x + 1 < sizeX)
                    {
                        returnList.Add(map[centralPoint.y, centralPoint.x + 1]);
                        if (map[centralPoint.y, centralPoint.x + 1].isCollider)
                            right = false;
                    }
                    if (centralPoint.x - 1 >= 0)
                    {
                        returnList.Add(map[centralPoint.y, centralPoint.x - 1]);
                        if (map[centralPoint.y, centralPoint.x - 1].isCollider)
                            left = false;
                    }
                    if (centralPoint.y + 1 < sizeY)
                    {
                        returnList.Add(map[centralPoint.y + 1, centralPoint.x]);
                        if (map[centralPoint.y + 1, centralPoint.x].isCollider)
                            top = false;
                    }
                    if (centralPoint.y - 1 >= 0)
                    {
                        returnList.Add(map[centralPoint.y - 1, centralPoint.x]);
                        if (map[centralPoint.y - 1, centralPoint.x].isCollider)
                            bot = false;
                    }
                    break;
                case MovementAxis.EIGHT_SIDES:
                    if (centralPoint.x + 1 < sizeX)
                    {
                        returnList.Add(map[centralPoint.y, centralPoint.x + 1]);
                        if (map[centralPoint.y, centralPoint.x + 1].isCollider)
                            right = false;
                    }
                    if (centralPoint.x - 1 >= 0)
                    {
                        returnList.Add(map[centralPoint.y, centralPoint.x - 1]);
                        if (map[centralPoint.y, centralPoint.x - 1].isCollider)
                            left = false;
                    }
                    if (centralPoint.y + 1 < sizeY)
                    {
                        returnList.Add(map[centralPoint.y + 1, centralPoint.x]);
                        if (map[centralPoint.y + 1, centralPoint.x].isCollider)
                            top = false;
                    }
                    if (centralPoint.y - 1 >= 0)
                    {
                        returnList.Add(map[centralPoint.y - 1, centralPoint.x]);
                        if (map[centralPoint.y - 1, centralPoint.x].isCollider)
                            bot = false;
                    }
                    if (centralPoint.x + 1 < sizeX && centralPoint.y + 1 < sizeY && right && top)
                        returnList.Add(map[centralPoint.y + 1, centralPoint.x + 1]);
                    if (centralPoint.x - 1 >= 0 && centralPoint.y + 1 < sizeY && left && top)
                        returnList.Add(map[centralPoint.y + 1, centralPoint.x - 1]);
                    if (centralPoint.x + 1 < sizeX && centralPoint.y - 1 >= 0 && right && bot)
                        returnList.Add(map[centralPoint.y - 1, centralPoint.x + 1]);
                    if (centralPoint.x - 1 >= 0 && centralPoint.y - 1 >= 0 && bot && left)
                        returnList.Add(map[centralPoint.y - 1, centralPoint.x - 1]);
                    break;
            }

            return returnList;
        }
    }
}