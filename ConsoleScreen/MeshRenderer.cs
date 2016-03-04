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
            for (int i = 0; i < local.Length; i++) {
                local[i] = Vector3D.TransformPosition (mesh.verticies[i], rotation);
            }

            // Draw verts
            for (int i = 0; i < mesh.edges.Length; i++) {

                Vector3D startOrtho = (Vector3D)screenPoint + new Vector3D (
                    local[mesh.edges[i].start].x,
                    local[mesh.edges[i].start].z,
                    local[mesh.edges[i].start].y) * scale;

                Vector3D endOrtho = (Vector3D)screenPoint + new Vector3D (
                    local[mesh.edges[i].end].x,
                    local[mesh.edges[i].end].z,
                    local[mesh.edges[i].end].y) * scale;

                Drawing.RenderLine (startOrtho, endOrtho);

            }

        }

    }
}
