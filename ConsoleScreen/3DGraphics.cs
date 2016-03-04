using System.Collections.Generic;
using System;
using Graphics2D;
using System.IO;
using ConsoleScreen;
using Graphics3D;

namespace Utility {

    class Utility {

        public static char[,] directionalChars = new char[,] {
            {'\\', '-', '/'},
            {'|', '*', '|'},
            {'/', '-', '\\'}
        };

        public static char GetDirectionalChar (Vector2D vec) {
            vec = vec.GetNormalized () + new Vector2D (1, 1);
            vec.x = Clamp ((int)Math.Round (vec.x), 0, 2);
            vec.y = Clamp ((int)Math.Round (vec.y), 0, 2);
            return directionalChars[(int)vec.x, (int)vec.y];
        }

        private static double Clamp (double n, double min, double max) {
            if (n < min)
                return min;
            if (n > max)
                return max;
            return n;
        }

        public static string[] GetContents (string file) {
            StreamReader reader = File.OpenText (file);
            List<string> con = new List<string> ();
            int maxTries = short.MaxValue;

            while (true && maxTries > 0) {
                maxTries--;
                string loc = reader.ReadLine ();
                if (loc == "") {
                    break;
                } else {
                    con.Add (loc);
                }
            }

            return con.ToArray ();
        }
    }
}

namespace Graphics3D {

    class Vector3D {

        public double x;
        public double y;
        public double z;

        public Vector3D (double _x, double _y, double _z) {
            x = _x;
            y = _y;
            z = _z;
        }

        public override string ToString () {
            return "(" + x + ", " + y + ", " + z + ")";
        }

        public static Vector3D TransformPosition (Vector3D point, Vector3D rot) {
            Vector3D copy = new Vector3D (point.x, point.y, point.z);
            copy = TransformAroundAxis (copy, rot.x, Axis.X);
            copy = TransformAroundAxis (copy, rot.y, Axis.Y);
            copy = TransformAroundAxis (copy, rot.z, Axis.Z);
            return copy;
        }

        private enum Axis { X, Y, Z };
        private static Vector3D TransformAroundAxis (Vector3D point, double angle, Axis axis) {
            switch (axis) {
                case Axis.X:
                    return new Vector3D (
                        (point.x * 1),
                        (point.y * Math.Cos (angle) - point.z * Math.Sin (angle)),
                        (point.y * Math.Sin (angle) + point.z * Math.Cos (angle))
                        );

                case Axis.Y:
                    return new Vector3D (
                        (point.x * Math.Cos (angle) + point.z * Math.Sin (angle)),
                        (point.y * 1),
                        (point.x * -Math.Sin (angle) + point.z * Math.Cos (angle))
                        );


                case Axis.Z:
                    return new Vector3D (
                        (point.x * Math.Cos (angle) - point.y * Math.Sin (angle)),
                        (point.x * Math.Sin (angle) + point.y * Math.Cos (angle)),
                        (point.z * 1)
                        );

                default:
                    return point;
            }
        }

        public static implicit operator Vector3D (Vector2D x) {
            return new Vector3D (x.x, x.y, 0);
        }

        public static Vector3D operator / (Vector3D vec, double n) {
            return new Vector3D (vec.x / n, vec.y / n, vec.z / n);
        }

        public static Vector3D operator * (Vector3D vec, double n) {
            return new Vector3D (vec.x * n, vec.y * n, vec.z * n);
        }

        public static Vector3D operator + (Vector3D vec1, Vector3D vec2) {
            return new Vector3D (vec1.x + vec2.x, vec1.y + vec2.y, vec1.z + vec2.z);
        }

        public static Vector3D operator - (Vector3D vec1, Vector3D vec2) {
            return new Vector3D (vec1.x - vec2.x, vec1.y - vec2.y, vec1.z - vec2.z);
        }
    }

    class Mesh {

        public Vector3D[] verticies;
        public Edge[] edges;
        
        public class Edge {

            public int start;
            public int end;

            public Edge (int s, int e) {

                start = s;
                end = e;

            }
        }

        public static Mesh LoadMeshFromOBJFile (string path) {
            string[] contents = Utility.Utility.GetContents (path);

            List<Vector3D> verts = new List<Vector3D> ();
            List<Edge> edges = new List<Edge> ();
        
            for (int i = 0; i < contents.Length; i++) {
                if (contents[i] != null) {
                    switch (contents[i].Substring (0, 2)) {
                        case "v ":

                            string[] locv = contents[i].Split (' ');
                            verts.Add (new Vector3D (
                                double.Parse (locv[1]),
                                double.Parse (locv[2]),
                                double.Parse (locv[3])
                                ));

                            break;

                        case "f ":

                            string[] loce = contents[i].Split (' ');
                            string[] copy = new string[loce.Length - 1];

                            for (int a = 1; a < loce.Length; a++) {
                                copy[a - 1] = loce[a];
                            }
                            
                            for (int j = 0; j < copy.Length; j++) {
                                edges.Add (new Edge (
                                    int.Parse (copy[j].Substring (0, 1)) - 1,
                                    int.Parse (copy[(j + 1) % (copy.Length)].Substring (0, 1)) - 1
                                    ));
                            }

                            break;

                        default:
                            break;
                    }
                }
            }

            Console.WriteLine ("Vertices: " + verts.Count.ToString ());
            Console.WriteLine ("Edges: " + edges.Count.ToString ());

            return new Mesh (verts.ToArray (), edges.ToArray ());
        }

        public Mesh (Vector3D[] verts, Edge[] edgelords) {
            verticies = verts;
            edges = edgelords;
        }
    }
}

namespace Graphics2D {

    class Drawing {

        public static void RenderLine (Vector3D start, Vector3D end) {
            char character = Utility.Utility.GetDirectionalChar (end - start);

            int steps = Math.Max ((int)((Vector2D)start).Distance (end), 2);

            for (int i = 0; i < steps; i++) {
                double progress = (i / (steps - 1));
                Vector3D pos = start + ((end - start) / steps) * i;
                if (Program.IsInsideScreen ((int)pos.x, (int)(pos.y * Program.aspect) + Program.height / 4)
                    && Program.depth[(int)pos.x, (int)(pos.y * Program.aspect) + Program.height / 4] < pos.z) {
                    Program.pixels[(int)pos.x, (int)(pos.y * Program.aspect) + Program.height / 4] = character;
                    Program.depth[(int)pos.x, (int)(pos.y * Program.aspect) + Program.height / 4] = pos.z;
                }
            }
        }
    }

    class Vector2D {

        public static Vector2D zero = new Vector2D (0, 0);

        public double x;
        public double y;

        public Vector2D (double _x, double _y) {
            x = _x;
            y = _y;
        }

        public override string ToString () {
            return "(" + x + ", " + y + ")";
        }

        public static Vector2D TransfromPosition (Vector2D point, double angle) {
            return new Vector2D ((point.x * Math.Cos (angle) - point.y * Math.Sin (angle)),
                                 (point.x * Math.Sin (angle) + point.y * Math.Cos (angle)));
        }

        public double Distance (Vector2D toVector) {
            return Math.Sqrt (Math.Pow (x - toVector.x, 2) + Math.Pow (y - toVector.y, 2));
        }

        public static Vector2D operator / (Vector2D vec, double n) {
            return new Vector2D (vec.x / n, vec.y / n);
        }

        public static Vector2D operator * (Vector2D vec, double n) {
            return new Vector2D (vec.x * n, vec.y * n);
        }

        public static Vector2D operator + (Vector2D vec1, Vector2D vec2) {
            return new Vector2D (vec1.x + vec2.x, vec1.y + vec2.y);
        }

        public static Vector2D operator - (Vector2D vec1, Vector2D vec2) {
            return new Vector2D (vec1.x - vec2.x, vec1.y - vec2.y);
        }

        public static implicit operator Vector2D (Vector3D x) {
            return new Vector2D (x.x, x.y);
        }

        public Vector2D GetNormalized () {
            double d = zero.Distance (this);
            return new Vector2D (x / d, y / d);
        }
    }
}