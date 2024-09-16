using System;
using System.Numerics;
using System.Windows;
using System.Windows.Input; // For key inputs
using System.Windows.Media;
using System.Windows.Shapes;

namespace _3D_Shapes
{
    public partial class MainWindow : Window
    {
        private Vector3[] allPoints; // Store cube points as a field
        private double rotationAngle = Math.PI / 36; // 5 degrees in radians per key press

        public MainWindow()
        {
            InitializeComponent();

            // Initialize 3D points of the cube
            allPoints = new Vector3[8];
            int min = 100;
            int max = 200;

            allPoints[0] = new Vector3(min, min, min);
            allPoints[1] = new Vector3(min, max, min);
            allPoints[2] = new Vector3(max, min, min);
            allPoints[3] = new Vector3(max, max, min);
            allPoints[4] = new Vector3(min, min, max);
            allPoints[5] = new Vector3(min, max, max);
            allPoints[6] = new Vector3(max, min, max);
            allPoints[7] = new Vector3(max, max, max);

            // Draw the initial cube
            DrawCube(allPoints);

            // Hook up the KeyDown event to listen for keyboard inputs
            this.KeyDown += new KeyEventHandler(OnKeyDown);
        }

        // Event handler for KeyDown
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            // Listen for Q/A for X-axis rotation, W/S for Y-axis, E/D for Z-axis
            switch (e.Key)
            {
                case Key.Q: // Rotate around X-axis clockwise
                    allPoints = RotateAroundCenter(allPoints, rotationAngle, 'X');
                    allPoints = RotateAroundCenter(allPoints, rotationAngle, 'Y');
                    allPoints = RotateAroundCenter(allPoints, rotationAngle, 'Z');
                    break;
                case Key.A: // Rotate around X-axis counter-clockwise
                    allPoints = RotateAroundCenter(allPoints, -rotationAngle, 'X');
                    break;
                case Key.W: // Rotate around Y-axis clockwise
                    allPoints = RotateAroundCenter(allPoints, rotationAngle, 'Y');
                    break;
                case Key.S: // Rotate around Y-axis counter-clockwise
                    allPoints = RotateAroundCenter(allPoints, -rotationAngle, 'Y');
                    break;
                case Key.E: // Rotate around Z-axis clockwise
                    allPoints = RotateAroundCenter(allPoints, rotationAngle, 'Z');
                    break;
                case Key.D: // Rotate around Z-axis counter-clockwise
                    allPoints = RotateAroundCenter(allPoints, -rotationAngle, 'Z');
                    break;
            }

            // Clear the canvas and redraw the rotated cube
            MyCanvas.Children.Clear();
            DrawCube(allPoints);
        }

        // Function to rotate around the center of the cube
        private Vector3[] RotateAroundCenter(Vector3[] points, double angle, char axis)
        {
            // Step 1: Calculate the center of the cube
            Vector3 center = CalculateCenter(points);

            // Step 2: Translate all points so that the center of the cube is at the origin
            for (int i = 0; i < points.Length; i++)
            {
                points[i] -= center;
            }

            // Step 3: Apply the rotation
            switch (axis)
            {
                case 'X':
                    points = RotateAllAroundX(points, angle);
                    break;
                case 'Y':
                    points = RotateAllAroundY(points, angle);
                    break;
                case 'Z':
                    points = RotateAllAroundZ(points, angle);
                    break;
            }

            // Step 4: Translate all points back to their original position
            for (int i = 0; i < points.Length; i++)
            {
                points[i] += center;
            }

            return points;
        }

        // Function to calculate the center of the cube
        private Vector3 CalculateCenter(Vector3[] points)
        {
            Vector3 sum = new Vector3(0, 0, 0);

            for (int i = 0; i < points.Length; i++)
            {
                sum += points[i];
            }

            return sum / points.Length; // Average of all points gives the center
        }

        // Function to project 3D points to 2D for canvas rendering
        private Point ProjectTo2D(Vector3 point)
        {
            double scale = 0.8; // scaling factor to shrink the cube for better view
            return new Point(point.X * scale, point.Y * scale);
        }

        // Function to draw the cube edges
        private void DrawCube(Vector3[] allPoints)
        {
            int[,] edges = new int[,]
            {
                { 0, 1 }, { 1, 3 }, { 3, 2 }, { 2, 0 }, // bottom face
                { 4, 5 }, { 5, 7 }, { 7, 6 }, { 6, 4 }, // top face
                { 0, 4 }, { 1, 5 }, { 2, 6 }, { 3, 7 }  // side edges
            };

            // Draw the lines for the cube
            for (int i = 0; i < edges.GetLength(0); i++)
            {
                int startIdx = edges[i, 0];
                int endIdx = edges[i, 1];

                Vector3 startPoint = allPoints[startIdx];
                Vector3 endPoint = allPoints[endIdx];

                Line line = new Line
                {
                    Stroke = Brushes.Black,
                    StrokeThickness = 2
                };

                Point p1 = ProjectTo2D(startPoint);
                Point p2 = ProjectTo2D(endPoint);

                line.X1 = p1.X;
                line.Y1 = p1.Y;
                line.X2 = p2.X;
                line.Y2 = p2.Y;

                MyCanvas.Children.Add(line);
            }
        }

        // Rotate all points in the array around the X-axis
        private Vector3[] RotateAllAroundX(Vector3[] points, double angle)
        {
            double cosTheta = Math.Cos(angle);
            double sinTheta = Math.Sin(angle);

            for (int i = 0; i < points.Length; i++)
            {
                float y = (float)(points[i].Y * cosTheta - points[i].Z * sinTheta);
                float z = (float)(points[i].Y * sinTheta + points[i].Z * cosTheta);
                points[i] = new Vector3(points[i].X, y, z);
            }

            return points;
        }

        // Rotate all points in the array around the Y-axis
        private Vector3[] RotateAllAroundY(Vector3[] points, double angle)
        {
            double cosTheta = Math.Cos(angle);
            double sinTheta = Math.Sin(angle);

            for (int i = 0; i < points.Length; i++)
            {
                float x = (float)(points[i].X * cosTheta + points[i].Z * sinTheta);
                float z = (float)(-points[i].X * sinTheta + points[i].Z * cosTheta);
                points[i] = new Vector3(x, points[i].Y, z);
            }

            return points;
        }

        // Rotate all points in the array around the Z-axis
        private Vector3[] RotateAllAroundZ(Vector3[] points, double angle)
        {
            double cosTheta = Math.Cos(angle);
            double sinTheta = Math.Sin(angle);

            for (int i = 0; i < points.Length; i++)
            {
                float x = (float)(points[i].X * cosTheta - points[i].Y * sinTheta);
                float y = (float)(points[i].X * sinTheta + points[i].Y * cosTheta);
                points[i] = new Vector3(x, y, points[i].Z);
            }

            return points;
        }
    }
}
