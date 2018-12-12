using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathFinding {
    class GridSquare {
        public Vector3Int pos;
        public float G; // Movement cost from start to this square.
        public float H; // Estimated movement cost from this square to finish.
        public float F; // G + H
        public GridSquare parent;

        public GridSquare(Vector3Int pos, Vector3Int destPos, GridSquare parent) {
            if (parent != null) {
                Debug.Assert(Helper.ManhattanDistance(pos, parent.pos) == 1 ||
                             Helper.ManhattanDistance(pos, parent.pos) == 0);
            }

            this.G = parent != null ? parent.G + 1 : 0; // 0 If starting position.
            this.pos = pos;
            //this.H = Helper.ManhattanDistance(pos, destPos);
            this.H = 0;
            this.F = G + H;
            this.parent = parent;
        }

        public override bool Equals(object obj) {
            if (!(obj is GridSquare)) {
                return false;
            }

            GridSquare o = (GridSquare)obj;
            return o.pos == this.pos;
        }

        public override int GetHashCode() {
            int hashcode = 23;
            hashcode = (hashcode * 37) + pos.x;
            hashcode = (hashcode * 37) + pos.y;
            return hashcode;
        }

        public static bool operator ==(GridSquare c1, GridSquare c2) {
            return EqualityComparer<GridSquare>.Default.Equals(c1, c2);
        }

        public static bool operator !=(GridSquare c1, GridSquare c2) {
            return !EqualityComparer<GridSquare>.Default.Equals(c1, c2);
        }
    }

    static void AddToSortedList(SortedList<float, List<GridSquare>> sl, GridSquare square) {
        if (sl.ContainsKey(square.F)) {
            sl[square.F].Add(square);
        }
        else {
            sl[square.F] = new List<GridSquare>() {square};
        }
    }

    static void RemoveFromSortedList(SortedList<float, List<GridSquare>> sl, GridSquare square) {
        var list = sl[square.F];
        list.Remove(square);

        if (list.Count == 0) {
            sl.Remove(square.F);
        }
    }

    /*
     * Simple A* Implementation
     * AdditionalNonWalkableSquares is an optional array of square positions that are not walkable and will be inluded in the path finding.
     */
    public static List<Vector3Int> FindPath(Vector3Int startPos, Vector3Int destPos, List<Vector3Int> AdditionalNonWalkableSquares = null) {
        int its = 0;
        var firstSquare = new GridSquare(startPos, destPos, null);

        // Probably could use OrderedDict somehow but I was unsuccessful because the key would be compared using a BST using the comparator instead of the hash value thus OrderecDict.ContainsValue(gridSquare) would return True and OrderedDict.ContainsKey(gridSquare) would return false even though they are completely the same because the comparator needs to eliminate collisions based on the F value but return equality based on the hash, maybe some other implementation of the comparator can fix this.
        Dictionary<GridSquare, GridSquare> openListHashSet = new Dictionary<GridSquare, GridSquare>();  // Dict because TryGetELement is only implemented above  .NET 4.7.2 
        SortedList<float, List<GridSquare>> openList = new SortedList<float, List<GridSquare>>(); // Sorted by F
        openListHashSet.Add(firstSquare, firstSquare);
        AddToSortedList(openList, firstSquare);
        
        
        HashSet<GridSquare> closedList = new HashSet<GridSquare>();
        GridSquare dest = new GridSquare(destPos, destPos, null);
        bool pathFound = false;


        do {
            its++;
            GridSquare currentSquare = openList.Values.First()[0]; // Take the square with minimum F
            closedList.Add(currentSquare);
            RemoveFromSortedList(openList, currentSquare);
            openListHashSet.Remove(currentSquare);

            if (closedList.Contains(dest)) {
                pathFound = true;
                break; // Path found.
            }

            List<Vector3Int> adjacentSquares = Terrain.Instance.GetAdjacentSquares(currentSquare.pos);
            if (AdditionalNonWalkableSquares != null) { // Filter out any additional non walkable squares from the walkable adjacent squares.
                adjacentSquares = adjacentSquares.Where(x => !AdditionalNonWalkableSquares.Contains(x)).ToList();
            }

            
            foreach (var square in adjacentSquares) {
                if (closedList.Contains(new GridSquare(square, destPos, null))) {
                    continue;
                }

                if (!openListHashSet.ContainsKey(new GridSquare(square, destPos, null))) {
                    // Not in open list.
                    GridSquare newGridSquare = new GridSquare(square, destPos, currentSquare);
                    AddToSortedList(openList, newGridSquare);
                    openListHashSet.Add(newGridSquare, newGridSquare);
                } else {
                    // In open list, see if current G score makes the F score better.
                    GridSquare existing = openListHashSet[new GridSquare(square, destPos, null)];

                    //GridSquare existing = openList[new GridSquare(square, destPos, null)];
                    if (currentSquare.G < existing.G) {
                        existing.G = currentSquare.G + 1;
                        existing.parent = currentSquare;
                    }
                }
            }

        } while (openList.Count != 0);

        Debug.Log(its);

        if (pathFound) {
            // Reconstruct path.
            List<Vector3Int> ret = new List<Vector3Int>();
            GridSquare currentSquare = closedList.Where(x => x == dest).First();

            while (currentSquare.parent != null) {
                ret.Add(currentSquare.pos);
                currentSquare = currentSquare.parent;
            }

            return ret;
        } else {
            Debug.LogWarning("Path not found.");
            return null;
        }
    }

    public static List<Vector3Int> FindPath(Vector2Int startPos, Vector2Int destPos,
        List<Vector3Int> AdditionalNonWalkableSquares = null) {
        return FindPath(new Vector3Int(startPos.x, startPos.y, 0), new Vector3Int(destPos.x, destPos.y, 0), AdditionalNonWalkableSquares);
    }
}
