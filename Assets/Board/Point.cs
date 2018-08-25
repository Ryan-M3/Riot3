using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;

namespace UnityEngine
{
    /// The primary purpose of this class is to get Unity
    /// to throw a type error when a Vector2 is used where
    /// it should be a Vector3 or vice versa.
    [TestFixture(0f, 0f)]
    public class Point {
        public float x;
        public float y;
        public float magnitude {
            get {
                return Mathf.Sqrt(x * x + y * y);
            }
        }
        public Point normalized {
            get {
                return this / this.magnitude;
            }
        }
        public Point North {
            get { return new Point(x, y + 1); }
        }
        public Point East {
            get { return new Point(x + 1, y); }
        }
        public Point South {
            get { return new Point(x, y - 1); }
        }
        public Point West {
            get { return new Point(x - 1, y); }
        }

        public Point(float x, float y) {
            this.x = x;
            this.y = y;
        }

        [Test] public void TestNormalized() {
            Point a = new Point(12f, -34f);
            Vector2 v = new Vector2(12, -34f);
            Debug.Assert(a.normalized.x == v.normalized.x);
            Debug.Assert(a.normalized.y == v.normalized.y);
        }

        [Test] public void TestFloatInit() {
            Point pt = new Point(12f, 34f);
            Debug.Assert(pt.x == 12f && pt.y == 34f);
        }

        // This constructor is a convenience for converting
        // a transform.position to a grid coord. That's why
        // the values are rounded.
        public Point(Vector3 v) {
            this.x = Mathf.Round(v.x);
            this.y = Mathf.Round(v.z);
        }

        [Test] public void TestV3Init() {
            Point pt = new Point(new Vector3(1, 2, 3));
            Debug.Assert(pt.x == 1f && pt.y == 3f);
        }

        public List<Point> Adjacent() {
            return new List<Point> {
                North, East, South, West
            };
        }

        [Test] public void TestAdjacent() {
            Point pt = new Point(0f, 0f);
            List<Point> pts = pt.Adjacent();
            Debug.Assert(pts[0].x == 0 && pts[0].y == 1);
            Debug.Assert(pts[1].x == 1 && pts[1].y == 0);
            Debug.Assert(pts[2].x == 0 && pts[2].y == -1);
            Debug.Assert(pts[3].x == -1 && pts[3].y == 0);
        }

        public static Point operator +(Point a, Point b) {
            return new Point(a.x + b.x, a.y + b.y);
        }

        [Test] public void TestAdd() {
            Point a = new Point(10, -8);
            Point b = new Point(99, 0);
            Point c = a + b;
            Debug.Assert(c.x == 109);
            Debug.Assert(c.y == -8);
        }

        public static Point operator -(Point a, Point b) {
            return new Point(a.x - b.x, a.y - b.y);
        }

        [Test] public void TestSubtract() {
            Point a = new Point(10, -8);
            Point b = new Point(99, 0);
            Point c = a - b;
            Debug.Assert(c.x == -89);
            Debug.Assert(c.y == -8);

            Point d = b - a;
            Debug.Assert(d.x == 89);
            Debug.Assert(d.y == 8);
        }

        public static bool operator ==(Point a, Point b) {
            if (ReferenceEquals(a, null) && ReferenceEquals(b, null))
                return true;
            else if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
                return false;
            else
                return a.x == b.x && a.y == b.y;
        }

        [Test] public void TestEqualsSign() {
            Point a = new Point(10, -8);
            Point b = new Point(99, 0);
            Debug.Assert(!(a == b));

            Point c = new Point(10, -8);
            Debug.Assert(a == c);
        }

        public static bool operator !=(Point a, Point b) {
            return !(a == b);
        }

        [Test] public void TestNotEqualsSign() {
            Point a = new Point(10, -8);
            Point b = new Point(99, 0);
            Debug.Assert(a != b);

            Point c = new Point(10, -8);
            Debug.Assert(!(a != c));

        }

        public override bool Equals(object obj) {
            if (obj == null || GetType() != obj.GetType())
                return false;
            Point c = (Point)obj;
            return (c.x == x) && (c.y == y);
        }

        [Test] public void TestEqualsFn() {
            Point a = new Point(10, -8);
            Point b = new Point(99, 0);
            Debug.Assert(!a.Equals(b));
            Debug.Assert(!b.Equals(a));

            Point c = new Point(10, -8);
            Debug.Assert(a.Equals(c));
            Debug.Assert(c.Equals(a));
        }

        public override int GetHashCode() {
            return Mathf.RoundToInt(x) ^ Mathf.RoundToInt(y);
        }

        [Test] public void TestGetHashCode() {
            Point a = new Point(10, -8);
            Point b = new Point(99, 0);
            Debug.Assert(a.GetHashCode() != b.GetHashCode());

            Point c = new Point(10, -8);
            Debug.Assert(a.GetHashCode() == c.GetHashCode());
        }

        public static Point operator *(Point a, float factor) {
            return new Point(a.x * factor, a.y * factor);
        }

        public static Point operator *(float factor, Point a) {
            return a * factor;
        }

        [Test] public void TestMultiplication() {
            Point a = new Point(1f, 2f) * 10f;
            Debug.Assert(a.x == 10f);
            Debug.Assert(a.y == 20f);

            Point b = 10f * new Point(1f, 2f);
            Debug.Assert(b.x == 10f);
            Debug.Assert(b.y == 20f);
        }

        public static Point operator /(Point a, float divisor) {
            return new Point(a.x / divisor, a.y / divisor);
        }

        public static Point operator /(float divisor, Point a) {
            throw new System.Exception("You can't divide a scalar by a vector.");
        }

        [Test] public void TestDivision() {
            Point a = new Point(10f, 2f) / 10f;
            Debug.Assert(a.x == 1.0f);
            Debug.Assert(a.y == 0.2f);
        }

        public static float Distance(Point a, Point b) {
            return (a - b).magnitude;
        }

        [Test] public void TestDistance() {
            Point a = new Point(3f, 4f);
            Debug.Assert(a.magnitude == 5f);
        }

        public Vector3 ToV3() {
            return new Vector3(x, 0f, y);
        }

        [Test] public void TestToV3() {
            Point a = new Point(1f, 22f);
            Vector3 v = a.ToV3();
            Debug.Assert(v.x == 1f);
            Debug.Assert(v.y == 0f);
            Debug.Assert(v.z == 22);
        }

        public Vector2 ToV2() {
            return new Vector2(x, y);
        }

        [Test] public void TestToV2() {
            Point a = new Point(1f, 2f);
            Vector2 v = a.ToV2();
            Debug.Assert(v.x == 1f);
            Debug.Assert(v.y == 2f);
        }

        public static Point Lerp(Point beg, Point end, float t) {
            return (1 - t) * beg + t * end;
        }

        [Test] public void TestLerp() {
            Point a = new Point(0f, 0f);
            Point b = new Point(10f, 10f);
            Point c = Point.Lerp(a, b, 0.5f);
            Debug.Assert(c.x == 5f);
            Debug.Assert(c.y == 5f);

            b.y *= 2f;
            Point d = Point.Lerp(a, b, 0.5f);
            Debug.Assert(d.x == 5f);
            Debug.Assert(d.y == 10f);
        }

        /// <summary>
        /// Distance using only 90 degree turns. As
        /// opposed to distance "as the crow flies."
        /// </summary>
        public static float ManhattanDistance(Point a, Point b) {
            if (a == b) {
                Debug.LogWarning("Why are you asking for the Manhattan Distance to yourself?");
                return 0f;
            }
            float dist = Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
            Debug.Assert(dist != 0f);
            Debug.Assert(!float.IsNaN(dist));
            Debug.Assert(!float.IsInfinity(dist));
            return dist;
        }

        [Test] public void TestManhattanDistance() {
            Point a = new Point(0f, 0f);
            Point b = new Point(5f, 7f);
            Debug.Assert(Point.ManhattanDistance(a, b) == 12);
        }

        public override string ToString() {
            return string.Format("Point({0}, {1})", x, y);
        }

        [Test] public void TestToString() {
            Point a = new Point(1f, 2f);
            string s = "Point(1, 2)";
            Debug.Assert(a.ToString() == s);
        }

        public Point Rounded() {
            return new Point(Mathf.Round(x), Mathf.Round(y));
        }

        [Test] public void TestRounded() {
            Point a = new Point(0.1f, 2.6f).Rounded();
            Debug.Assert(a.x == 0f);
            Debug.Assert(a.y == 3f);
        }

        public static float Dot(Point a, Point b) {
            return a.x * b.x + a.y * b.y;
        }

        [Test] public void TestDotProduct() {
            Point a = new Point(1f, 2f);
            Point b = new Point(3f, 4f);
            float product = Point.Dot(a, b);
            Debug.Assert(product == 11f);
        }

        public static float SignedAngle(Point a, Point b) {
            return Vector2.SignedAngle(a.ToV2(), b.ToV2());
        }
    }
}
