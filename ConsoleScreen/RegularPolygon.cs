using System;
using Graphics2D;
using Utility;

namespace ConsoleScreen {
    class RegularPolygon : ActiveObject {

        public int sides = 4;
        public float sideLength;

        public int x;
        public int y;

        public float angle;

        public static double degToRad = 180 / Math.PI;

        public override void Tick (float deltaTime) {
            angle += deltaTime;
        }

        public override void RenderToBuffer () {

            Vector2D mousePos = new Vector2D (x, y);
            double locAngle = angle;
            Vector2D offset = new Vector2D (0, 0);

            offset.x -= sideLength / 2;
            offset.y -= sideLength / (2 * Math.Tan (Math.PI / sides));

            mousePos -= Vector2D.TransfromPosition (offset, locAngle - Math.PI);

            for (int i = 0; i < sides; i++) {
                Vector2D end = mousePos + Vector2D.TransfromPosition (new Vector2D (sideLength, 0), locAngle);
                Drawing.RenderLine (mousePos, end);
                locAngle += (Math.PI * 2) / sides;
                mousePos = end;
            }

        }

        public RegularPolygon (int centerX, int centerY, int _sides, float _length) {
            this.sides = _sides;
            this.x = centerX;
            this.y = centerY;
            this.sideLength = _length;
        }
    }
}
