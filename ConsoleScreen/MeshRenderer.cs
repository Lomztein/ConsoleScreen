using ConsoleScreen;
using Graphics2D;
using System;

namespace Graphics3D {
    class MeshRenderer : ActiveObject {

        public Vector2D screenPoint;
        public Vector3D rotation;
        public double scale;

        public Vector3D rotationSpeed;

        public Mesh mesh;

        public MeshRenderer (Vector2D sp, Vector3D rs, Mesh m, double s) {
            rotation = new Vector3D (0, 0, 0);
            screenPoint = sp;
            rotationSpeed = rs;
            mesh = m;
            scale = s;
        }

        public override void Tick (float deltaTime) {
            rotation += rotationSpeed * deltaTime;
        }

        public override void RenderToBuffer () {
            // Rotate verts
            Vector3D[] local = new Vector3D[mesh.verticies.Length];
            Vector3D[] localNorms = new Vector3D[mesh.faces.Length];
            for (int i = 0; i < local.Length; i++) {
                local[i] = Vector3D.TransformPosition (mesh.verticies[i], rotation);
            }
            for (int i = 0; i < localNorms.Length; i++) {
                localNorms[i] = Vector3D.TransformPosition (mesh.faces[i].normal, rotation);
            }

            // Draw verts
            for (int i = 0; i < mesh.faces.Length; i++) {
                if (localNorms[i].z > 0) {
                    for (int j = 0; j < mesh.faces[i].edges.Length; j++) {

                        Vector3D startOrtho = (Vector3D)screenPoint + new Vector3D (
                           local[mesh.faces[i].edges[j].start].x,
                           local[mesh.faces[i].edges[j].start].y,
                           local[mesh.faces[i].edges[j].start].z) * scale;

                        Vector3D endOrtho = (Vector3D)screenPoint + new Vector3D (
                            local[mesh.faces[i].edges[j].end].x,
                            local[mesh.faces[i].edges[j].end].y,
                            local[mesh.faces[i].edges[j].end].z) * scale;

                        Drawing.RenderLine (startOrtho, endOrtho);
                    }
                }
            }
        }
    }
}
