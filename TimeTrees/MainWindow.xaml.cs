using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace TimeTrees
{
    public partial class MainWindow : Window
    {
        private Dictionary<Rectangle, List<Line>> connections = new Dictionary<Rectangle, List<Line>>();
        private Point? _movePoint;

        public MainWindow()
        {
            InitializeComponent();
        } 

        private Rectangle CreateRectangle()
        {
            Rectangle rectangle = new Rectangle();
            rectangle.Width = 60;
            rectangle.Height = 60;
            rectangle.Fill = Brushes.LightBlue;
            connections.Add(rectangle, new List<Line>());

            return rectangle;
        }

        private Line CreateLine()
        {
            Line line = new Line();
            line.Stroke = Brushes.HotPink;
            line.StrokeThickness = 3;
            line.StrokeStartLineCap = PenLineCap.Round;
            line.StrokeEndLineCap = PenLineCap.Round;
            
            return line;
        }

        private void CreateFigure_Click(object sender, RoutedEventArgs e)
        {
            Rectangle rectangle = CreateRectangle();
            MainRoot.Children.Add(rectangle);

            Canvas.SetLeft(rectangle, 350);
            Canvas.SetTop(rectangle, 180);

            rectangle.MouseLeftButtonDown += FigureMouseDown;
            rectangle.MouseMove += FigureMouseMove;
            rectangle.MouseLeftButtonUp += FigureMouseUp;
            rectangle.MouseRightButtonDown += Connection;
        }

        private void FigureConnect_Click(object sender, RoutedEventArgs e)
        {
            ConnectionFigures.connection = true;
        }

        private void Connection(object sender, MouseEventArgs args)
        {
            if(ConnectionFigures.connection == false) return;

            Point point = args.GetPosition(MainRoot);

            if(ConnectionFigures.start.X == 0 && ConnectionFigures.start.Y == 0)
            {
                ConnectionFigures.start = point;
                ConnectionFigures.rectangleFirst = (Rectangle)sender;
            }

            else if(ConnectionFigures.end.X == 0 && ConnectionFigures.end.Y == 0)
            {
                ConnectionFigures.rectangleLast = (Rectangle)sender;
                ConnectionFigures.end = point;

                Line line = CreateLine();

                line.X1 = ConnectionFigures.start.X;
                line.Y1 = ConnectionFigures.start.Y;
                line.X2 = ConnectionFigures.end.X;
                line.Y2 = ConnectionFigures.end.Y;

                if (ConnectionFigures.rectangleFirst == ConnectionFigures.rectangleLast)
                {
                    ConnectionFigures.Clear();
                    return;
                }

                foreach(Line lineStart in connections[ConnectionFigures.rectangleFirst])
                    foreach (Line lineEnd in connections[ConnectionFigures.rectangleLast])
                        if (lineStart == lineEnd) return;

                connections[ConnectionFigures.rectangleFirst].Add(line);
                connections[ConnectionFigures.rectangleLast].Add(line);
                MainRoot.Children.Add(line);
                RedrawCanvas();
                ConnectionFigures.Clear();
            }

        }

        private void RedrawCanvas()
        {
            MainRoot.Children.Clear();

            foreach(var keyValuePair in connections)
            {
                foreach (Line line in keyValuePair.Value)
                {
                    if (!MainRoot.Children.Contains(line))
                    {
                        MainRoot.Children.Add(line);
                    }
                }
                MainRoot.Children.Add(keyValuePair.Key);
            }
        }

        private void FigureMouseUp(object sender, MouseButtonEventArgs args)
        {
            Rectangle rectangle = (Rectangle)sender;
            _movePoint = null;
            rectangle.ReleaseMouseCapture();
        }

        private void FigureMouseMove(object sender, MouseEventArgs args)
        {
            Rectangle rectangle = (Rectangle)sender;

            if (_movePoint == null) { return; }
            
            Point point = args.GetPosition(MainRoot) - (Vector)_movePoint.Value;

            Canvas.SetLeft(rectangle, point.X);
            Canvas.SetTop(rectangle, point.Y);

            foreach (Line line in connections[rectangle])
            {
                double line1 = Math.Sqrt(Math.Pow(point.X + rectangle.Width / 2 - line.X1, 2) + Math.Pow(point.Y + rectangle.Height / 2 - line.Y1, 2));
                double line2 = Math.Sqrt(Math.Pow(point.X + rectangle.Width / 2 - line.X2, 2) + Math.Pow(point.Y + rectangle.Height / 2 - line.Y2, 2));

                if(line1 < line2)
                {
                    line.X1 = point.X + rectangle.Width / 2;
                    line.Y1 = point.Y + rectangle.Height / 2;
                }
                else
                {
                    line.X2 = point.X + rectangle.Width / 2;
                    line.Y2 = point.Y + rectangle.Height / 2;
                }

            }

        }

        private void FigureMouseDown(object sender, MouseButtonEventArgs args)
        {
            Rectangle rectangle = (Rectangle)sender;
            _movePoint = args.GetPosition(rectangle);
            rectangle.CaptureMouse();
        }
    }
}
