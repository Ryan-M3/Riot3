//using System.Linq;
//using System.Collections.Generic;
//using UnityEngine;
//using NUnit.Framework;


//public class Node<T> {
//    public T data;
//    public Point pt;
//    public bool vert;
//    public Node<T> left;
//    public Node<T> right;

//    public Node(Point pt, T data, bool vert) {
//        this.pt   = pt;
//        this.data = data;
//        this.vert = vert;
//    }

//    public static bool IsLeft(Point queryPt, Point anchor, bool vert) {
//        return vert ? queryPt.x < anchor.x : queryPt.y < anchor.y;
//    }

//    public Node<T> GetNode(Point query) {
//        if (this.pt == query) {
//            return this;
//        }

//        else if (vert ? pt.x > query.x : pt.y > query.y) {
//            if (right == null) {
//                throw new System.IndexOutOfRangeException();
//            }
//            return right.GetNode(query);
//        }

//        else {
//            if (left == null) {
//                throw new System.IndexOutOfRangeException();
//            }
//            return left.GetNode(query);
//        }
//    }

//    public T GetData(Point query) {
//        return GetNode(query).data;
//    }

//    public List<Node<T>> ListNodes() {
//        List<Node<T>> nodes = new List<Node<T>>();

//        nodes.Add(this);

//        if (left != null)
//            nodes.AddRange(left.ListNodes());

//        if (right != null)
//            nodes.AddRange(right.ListNodes());

//        return nodes;
//    }

//    public List<Point> ListPoints() {
//        return ListNodes().Select(node => node.pt).ToList();
//    }

//    public static bool Intersects(Rect rect, Point pt, bool vert) {
//        float min  = vert ? rect.xMin : rect.yMin;
//        float max  = vert ? rect.xMax : rect.yMax;
//        float line = vert ? pt.x      :      pt.y;
//        return line <= max && line >= min;
//    }

//    public List<Node<T>> Neighborhood(Rect area) {
//        List<Node<T>> inArea = new List<Node<T>>();
//        if (area.Contains(pt.ToV2()))
//            inArea.Add(this);

//        if (left == null && right == null)
//            return inArea;

//        // Check if the rectangle intesects a line one of our child nodes is split on.
//        if (Intersects(area, pt, !vert)) {
//            if (right != null)
//                inArea.AddRange(right.Neighborhood(area));

//            if (left != null)
//                inArea.AddRange(left.Neighborhood(area));
//        }

//        else if (IsLeft(new Point(area.center), pt, vert) && left != null) {
//            inArea.AddRange(left.Neighborhood(area));
//        }

//        else if (right != null) {
//            inArea.AddRange(right.Neighborhood(area));
//        }

//        return inArea;
//    }

//    public void Add(Point newPt, T data) {
//        if (vert ? pt.x > newPt.x : pt.y > newPt.y) {
//            if (right == null)
//                right = new Node<T>(newPt, data, !vert);
//            else
//                right.Add(newPt, data);
//        }

//        else {
//            if (left == null)
//                left = new Node<T>(newPt, data, !vert);
//            else
//                left.Add(newPt, data);
//        }
//    }

//    /// <summary>
//    /// Find the node with the smallest value on dimension, d.
//    /// </summary>
//    public Node<T> FindMinimum(bool d) {
//        // Our goal is to compare our best candidate to the
//        // current node and return the smaller.
//        Node<T> candidate;
//        Node<T> maybeLeft  = left  != null ? left .FindMinimum(d) : null;
//        Node<T> maybeRight = right != null ? right.FindMinimum(d) : null;

//        // If either left or right are null, our best candidate
//        // the one that exists. If both are null, our candidate
//        // is null.
//        if (maybeLeft == null) {
//            candidate = maybeRight;
//        }

//        else if (maybeRight == null) {
//            candidate = maybeLeft;
//        }

//        // If we have both a left and right subtree, compare them
//        // on dimension, d, and the smaller as our best candidate.
//        else {
//            if (d) {
//                candidate = maybeRight.pt.x < maybeLeft.pt.x ? maybeRight : maybeLeft;
//            }
//            else {
//                candidate = maybeRight.pt.y < maybeLeft.pt.y ? maybeRight : maybeLeft;
//            }
//        }

//        // If no non-null candidates have been found, this node
//        // is the smallest in this subtree.
//        if (candidate == null)
//            return this;

//        // Return the smaller of the this and the candidate node
//        // on the queried dimension, d.
//        if (d) {
//            return candidate.pt.x < pt.x ? candidate : this;
//        }

//        else {
//            return candidate.pt.y < pt.y ? candidate : this;
//        }
//    }

//    public void Remove(Point query) {
//        //  GeeksForGeeks.Org summarized (and I followed) how to delete a node from a KDTree succinctly:
//        /*  1) If current node contains the point to be deleted
//         *      a) If node to be deleted is a leaf node, simply delete it (Same as BST Delete)
//         *      b) If node to be deleted has right child as not NULL(Different from BST)
//         *          i.  Find minimum of current node�s dimension in right subtree.
//         *          ii. Replace the node with above found minimum and recursively delete minimum in right subtree.
//         *      c) Else If node to be deleted has left child as not NULL(Different from BST)
//         *          i.   Find minimum of current node�s dimension in left subtree.
//         *          ii.  Replace the node with above found minimum and recursively delete minimum in left subtree.
//         *          iii. Make new left subtree as right child of current node.
//         *  2) If current doesn�t contain the point to be deleted
//         *      a) If node to be deleted is smaller than current node on current dimension, recur for left subtree.
//         *      b) Else recur for right subtree.
//         */

//        Node<T> maybeFound = null;
//        if (right != null && right.pt == query)
//            maybeFound = right;

//        if (left != null && left.pt == query)
//            maybeFound = left;


//        if (maybeFound == null)
//        {
//            try {
//                if (vert ? query.x < pt.x : query.y < pt.y)
//                    left.Remove(query);
//                else
//                    right.Remove(query);
//            }

//            catch (System.NullReferenceException) {
//                throw new System.IndexOutOfRangeException("Cannot delete because query point not found.");
//            }
//        }

//        else
//        {
//            if (maybeFound.right != null) {
//                Node<T> rmin = maybeFound.FindMinimum(vert);
//                left.Remove(rmin.pt);
//                left = rmin;
//            }

//            else if (maybeFound.left != null) {
//                Node<T> lmin = maybeFound.FindMinimum(vert);
//                right.Remove(lmin.pt);
//                right = lmin;
//            }

//            // Leaf node found.
//            else {
//                if (left == maybeFound)
//                    left = null;
//                else
//                    right = null;
//            }
//        }
//    }
//}

///// <summary>
///// Technically, this is only a 2D Tree as it isn't generalized
///// into k dimensions, but you can't name a class 2DTree.
///// </summary>
///// <typeparam name="T"></typeparam>
//public class KDTree<T> {
//    public Node<T> root;

//    #region API_Functions
//    public void Add(Point pt, T data) {
//        if (root == null)
//            root = new Node<T>(pt, data, true);
//        else
//            root.Add(pt, data);
//    }

//    public void Remove(Point query) {
//        if (root.pt == query) {
//            if (root.right == null && root.left == null) {
//                root = null;
//            }

//            else if (root.right != null && root.left == null) {
//                root = root.right;
//            }

//            else if (root.right == null && root.left != null) {
//                root = root.left;
//            }

//            else {
//                // This probably isn't the most efficient way to do this, but the
//                // fancy way was throwing errors, it doesn't really matter for my
//                // purposes, and frankly I'd rather just move on from implement-
//                // ing something that I probably should have just grabbed off of
//                // github to begin with (I always underestimate how much time it
//                // takes to implement seemingly simple algorithms like KDTrees).
//                List<Node<T>> leftNodes = root.left.ListNodes();
//                root = root.right;
//                foreach (Node<T> node in leftNodes) {
//                    root.Add(node.pt, node.data);
//                }
//            }
//        }

//        else {
//            root.Remove(query);
//        }
//    }

//    public Node<T> GetNode(Point query) {
//        return root.GetNode(query);
//    }

//    public T GetData(Point query) {
//        return root.GetData(query);
//    }

//    public List<Node<T>> ListNodes() {
//        return root.ListNodes();
//    }

//    public List<Point> ListPoints() {
//        return root.ListPoints();
//    }

//    public List<Node<T>> Neighborhood(Rect area) {
//        return root.Neighborhood(area);
//    }
//    #endregion
//}

//public class TestNodes {
//    KDTree<int> tree;
//    List<Point> pts;

//    [SetUp]
//    public void SetUp() {
//        // based on the data and results from https://www.youtube.com/watch?v=W94M9D_yXKk
//        tree = new KDTree<int>();
//        pts = new List<Point>() {
//            new Point(23, 18),
//            new Point(32, 14),
//            new Point(16, 22),
//            new Point(12,  3),
//            new Point( 9, 19),
//            new Point(21, 28),
//            new Point(25, 11),
//            new Point(47, 44),
//            new Point(43, 29),
//            new Point(29,  4)
//        };

//        for (int i = 0; i < 10; i++) {
//            tree.Add(pts[i], i + 1);
//        }
//    }

//    [Test]
//    public void TestAdd() {
//        Debug.Assert(pts[0] == tree.root.pt                   );
//        Debug.Assert(pts[1] == tree.root.left.pt              );
//        Debug.Assert(pts[2] == tree.root.right.pt             );
//        Debug.Assert(pts[3] == tree.root.right.right.pt       );
//        Debug.Assert(pts[4] == tree.root.right.right.right.pt );
//        Debug.Assert(pts[5] == tree.root.right.left.pt        );
//        Debug.Assert(pts[6] == tree.root.left.right.pt        );
//        Debug.Assert(pts[7] == tree.root.left.left.pt         );
//        Debug.Assert(pts[8] == tree.root.left.left.right.pt   );
//        Debug.Assert(pts[9] == tree.root.left.right.left.pt   );
//    }

//    [Test]
//    public void TestGet() {
//        for (int i = 0; i < 10; i++) {
//            Debug.Assert(tree.GetData(pts[i]) == i + 1);
//        }
//    }

//    [Test]
//    public void TestGetChildren() {
//        List<Point> kids = tree.ListPoints();
//        foreach (Point pt in pts) {
//            Debug.Assert(kids.Contains(pt));
//        }
//    }

//    [Test]
//    public void TestGetInArea() {
//        // Remember that Rects are measured from the lower left
//        // point, rather than the center for whatever reason.
//        List<Point> inArea = tree
//                            .Neighborhood(new Rect(4, 18, 14, 13))
//                            .Select(node => node.pt)
//                            .ToList();
//        Debug.Assert(inArea.Count == 2);
//        Debug.Assert(inArea.Contains(pts[2]));
//        Debug.Assert(inArea.Contains(pts[4]));

//        inArea = tree
//                .Neighborhood(new Rect(16, 18, 12, 10))
//                .Select(node => node.pt)
//                .ToList();
//        Debug.Assert(inArea.Count == 2);
//        Debug.Assert(inArea.Contains(pts[0]));
//        Debug.Assert(inArea.Contains(pts[2]));
//        Debug.Assert(inArea.Contains(pts[5]));
//        Rect r = new Rect(4, 18, 14, 13);
//        Debug.Assert(r.Contains(pts[2].ToV2()));

//        inArea.Clear();
//        inArea = tree
//                .Neighborhood(new Rect(0f, 0f, pts[0].x, 99f))
//                .Select(node => node.pt)
//                .ToList();
//        Debug.Assert(inArea.Count == 4);
//        Debug.Assert(inArea.Contains(pts[2]));
//        Debug.Assert(inArea.Contains(pts[3]));
//        Debug.Assert(inArea.Contains(pts[4]));
//        Debug.Assert(inArea.Contains(pts[5]));
//    }

//    [Test]
//    public void TestRemove() {
//        tree.Remove(pts[5]);
//        List<Point> kids = tree.ListPoints();
//        Debug.Assert(kids.Count == 9);
//    }

//    [Test]
//    public void TestGetNode() {
//        foreach (Point pt in pts) {
//            Debug.Assert(tree.GetNode(pt).pt == pt);
//        }
//    }

//    [Test]
//    public void TestRemoveRoot() {
//        Point rootPt = tree.root.pt;
//        tree.Remove(rootPt);
//        foreach (Point pt in pts) {
//            if (pt != rootPt) {
//                if (pt.x == 12 && pt.y == 3) {
//                    tree.GetNode(pt);
//                }
//                Debug.Assert(tree.GetNode(pt).pt == pt);
//            }
//        }
//    }

//    [Test]
//    public void TestIsLeft() {
//        Node<int> center = new Node<int>(new Point(0f, 0f), 0, true);
//        Point up    = new Point( 0f,  1f);
//        Point down  = new Point( 0f, -1f);
//        Point left  = new Point(-1f,  0f);
//        Point right = new Point( 1f,  0f);
//        Debug.Assert(!Node<int>.IsLeft(up   , center.pt, false));
//        Debug.Assert( Node<int>.IsLeft(down , center.pt, false));
//        Debug.Assert( Node<int>.IsLeft(left , center.pt, true ));
//        Debug.Assert(!Node<int>.IsLeft(right, center.pt, true ));

//        center = new Node<int>(new Point(11.5f, 29f), 0, false);
//        Point right2 = new Point(32, 14);
//        Debug.Assert(Node<int>.IsLeft(right2, center.pt, false));
//    }

//    [Test]
//    public void TestIntersects() {
//        //Node<int> center = new Node<int>(new Point(0f, 0f), 0, true);
//        Rect rect = new Rect(-2f, -2f, 1f, 1f);
//        Debug.Assert(!Node<int>.Intersects(rect, new Point(0f, 0f), true ));
//        Debug.Assert(!Node<int>.Intersects(rect, new Point(0f, 0f), false));

//        rect = new Rect(-2f, -2f, 1f, 10f);
//        Debug.Assert(!Node<int>.Intersects(rect, new Point(0f, 0f), true ));
//        Debug.Assert( Node<int>.Intersects(rect, new Point(0f, 0f), false));

//        rect = new Rect(-2f, -2f, 10f, 1f);
//        Debug.Assert( Node<int>.Intersects(rect, new Point(0f, 0f), true ));
//        Debug.Assert(!Node<int>.Intersects(rect, new Point(0f, 0f), false));
//    }
//}
