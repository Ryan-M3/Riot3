using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// I use this often enough and it's a simple enough concept
// that I figured I'd break out into it's own file.
namespace Edges
{
    public struct EdgeV3 {
        public Vector3 beg;
        public Vector3 end;
    }

    public struct Edge {
        public Point beg;
        public Point end;
    }
}
