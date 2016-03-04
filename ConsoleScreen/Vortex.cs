using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleScreen {

    class Circle : ActiveObject {

        public int x;
        public int y;
        public float radius;
        private float modifiedRadius;
        private float time;

        public char character;

        public override void Tick (float deltaTime) {
            modifiedRadius = radius + (float)Math.Sin (time) * radius;
            time += deltaTime;
        }

        public override void RenderToBuffer () {
            int surface = (int)(radius * 2 * (float)Math.PI);

            for (int i = 0; i < surface; i++) {
                int cursorX = x + (int)(Math.Cos ((float)i / (float)surface * (float)Math.PI * 2) * modifiedRadius);
                int cursorY = y + (int)(Math.Sin ((float)i / (float)surface * (float)Math.PI * 2) * modifiedRadius);

                if (Program.IsInsideScreen (cursorX, (int)((float)cursorY * Program.aspect)))
                    Program.pixels[cursorX, (int)((float)cursorY * Program.aspect)] = character;
            }
        }

        public Circle (int _x, int _y, float _radius, char _character) {
            x = _x;
            y = _y;
            radius = _radius;
            character = _character;
        }

    }
}
