using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
    public class Point
    {
        public int x, y;
        public bool isCollider;

        /// <summary>
        /// World position of the point.
        /// </summary>
        public Vector2 position;

        /// <summary>
        /// The previous point of the path.
        /// </summary>
        public Point parent;

        public int startDistance, goalDistance;

        /// <summary>
        /// Distance to the point that started the current path + distance to the goal point. 
        /// </summary>
        public int PointValue { get => startDistance + goalDistance; }

        public Point(bool collider, Vector2 pos, int _x, int _y)
        {
            isCollider = collider;
            position = pos;
            x = _x;
            y = _y;
        }
    }
}
