﻿using System;
using System.Collections.Generic;
using System.Threading;
using Graphics2D;
using Graphics3D;
using System.IO;

namespace ConsoleScreen {
    class Program {

        public static int width = 80, height = 40;
        public static float aspect;

        public static char[,] pixels;
        public static double[,] depth;

        public static bool vsync = true;
        public static int desiredFPS = 20;

        private static float lastTickTime;
        private static int lastTime;
        private static bool isRunning;

        public static List<ActiveObject> activeObjects = new List<ActiveObject>();

        static void Main () {

            InitializeScreen();
            isRunning = true;

            // Human interaction is kind of a flustercuck, so beware of chaotic code.
            string path = "";
            Console.WriteLine("Input path to desired model in .obj format, please.");
            bool moveOn = false;
            while (moveOn == false) {
                path = Console.ReadLine ();
                if (File.Exists (path)) {
                    if (path.Substring (path.LastIndexOf (".")) != ".obj")
                        Console.WriteLine ("Wrong file format, it needs to be in .obj format.");
                    else
                        moveOn = true;
                } else
                    Console.WriteLine ("That file doesn't exist, try again!");
            }

            Console.WriteLine("Input model scale.");
            double scale = 0;
            moveOn = false;
            while (moveOn == false) {
                if (double.TryParse (Console.ReadLine (), out scale))
                    moveOn = true;
                else
                    Console.WriteLine ("Could not parse scale, please try again.");
            }
            // Human interaction code finished. Read on without worries.

            Mesh mesh = Mesh.LoadMeshFromOBJFile(path);
            activeObjects.Add(new MeshRenderer(new Vector2D(width / 2, height / 2), new Vector3D(0.1, 0.2, 0.1), mesh, -scale));

            while (isRunning) {
                int time = DateTime.Now.Millisecond;
                float deltaTime = 1.0f / desiredFPS;

                GlobalTick (deltaTime);
                GlobalRender (deltaTime);

                lastTickTime = time / 1000f;
                lastTime = time;
               
                Thread.Sleep (Math.Max (1000 / desiredFPS, 0));

            }
        }

        static void InitializeScreen () {
            pixels = new char[width, height];
            depth = new double[width, height];
            aspect = (float)height / (float)width;
            ClearScreenBuffer ();
        }

        static void ClearScreenBuffer () {
            Console.Clear ();
            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    pixels[x, y] = ' ';
                    depth[x, y] = double.MinValue;
                }
            }
        }

        static void GlobalTick (float deltaTime) {
            // Tick every object in the activeObjects list.
            for (int i = 0; i < activeObjects.Count; i++) {
                activeObjects[i].Tick (deltaTime);
            }
        }

        public static bool IsInsideScreen (int x, int y) {
            if (x < 0 || x > width - 1)
                return false;
            if (y < 0 || y > height - 1)
                return false;
            return true;
        }

        static void GlobalRender (float deltaTime) {
            // First clear the buffer from previous frame.
            ClearScreenBuffer ();

            // Render all objects to buffer.
            for (int i = 0; i < activeObjects.Count; i++) {
                activeObjects[i].RenderToBuffer ();
            }

            // Construct scanlines. One additional scanline for displaying tick time.
            string[] scanlines = new string[height];
            string render = "";

            //scanlines[0] = "Tick time: " + deltaTime;
            render += scanlines[0] + "\n";

            for (int y = 0; y < scanlines.Length; y++) {
                scanlines[y] = "";
                for (int x = 0; x < width; x++) {
                    scanlines[y] += pixels[x, y];
                }

                render += scanlines[y] + "\n";
            }

            Console.WriteLine (render);
        }
    }
}
