using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;

namespace TimeTrees
{
    public partial class MainWindow : Window
    {
        private Dictionary<StackPanel, List<Line>> connections = new Dictionary<StackPanel, List<Line>>();
        private Point? _movePoint;
        private CreateObject createObject = new CreateObject();

        public MainWindow()
        {
            InitializeComponent();
        } 

        private void CreateFigure_Click(object sender, RoutedEventArgs e)
        {
            StackPanel stackPanel = createObject.CreateIcon();
            connections.Add(stackPanel, new List<Line>());
            MainRoot.Children.Add(stackPanel);

            Canvas.SetLeft(stackPanel, 350);
            Canvas.SetTop(stackPanel, 180);

            stackPanel.MouseLeftButtonDown += FigureMouseDown;
            stackPanel.MouseMove += FigureMouseMove;
            stackPanel.MouseLeftButtonUp += FigureMouseUp;
            stackPanel.MouseRightButtonDown += WriteText;
            stackPanel.MouseRightButtonDown += Connection;
        }

        private void FigureConnect_Click(object sender, RoutedEventArgs e)
        {
            ConnectionFigures connectionFigures = ConnectionFigures.GetInstance();
            connectionFigures.connection = true;
        }

        private void File_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {

        }

        private void WriteText(object sender, MouseEventArgs e)
        {
            ConnectionFigures connectionFigures = ConnectionFigures.GetInstance();
            if (connectionFigures.connection) return;

            StackPanel stackPanel = (StackPanel)sender;
            foreach (var args in stackPanel.Children)
            {
                if (args.GetType() == typeof(TextBox))
                {
                    TextBox textBox = (TextBox)args;
                    textBox.IsEnabled = true;
                }
            }
        }

        private void Connection(object sender, MouseEventArgs e)
        {
            ConnectionFigures connectionFigures = ConnectionFigures.GetInstance();
            if (connectionFigures.connection == false) return;

            Point point = e.GetPosition(MainRoot);

            if (connectionFigures.start.X == 0 && connectionFigures.start.Y == 0)
            {
                connectionFigures.start = point;
                connectionFigures.rectangleFirst = (StackPanel)sender;
            }

            else if(connectionFigures.end.X == 0 && connectionFigures.end.Y == 0)
            {
                connectionFigures.rectangleLast = (StackPanel)sender;
                connectionFigures.end = point;

                Line line = createObject.CreateLine();

                line.X1 = connectionFigures.start.X;
                line.Y1 = connectionFigures.start.Y;
                line.X2 = connectionFigures.end.X;
                line.Y2 = connectionFigures.end.Y;

                if (connectionFigures.rectangleFirst == connectionFigures.rectangleLast)
                {
                    connectionFigures.Clear();
                    return;
                }

                foreach(Line lineStart in connections[connectionFigures.rectangleFirst])
                    foreach (Line lineEnd in connections[connectionFigures.rectangleLast])
                        if (lineStart == lineEnd) return;

                connections[connectionFigures.rectangleFirst].Add(line);
                connections[connectionFigures.rectangleLast].Add(line);
                MainRoot.Children.Add(line);
                RedrawCanvas();
                connectionFigures.Clear();
            }

        }

        private void RedrawCanvas()
        {
            MainRoot.Children.Clear();

            foreach (var keyValuePair in connections)
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

        private void FigureMouseDown(object sender, MouseButtonEventArgs e)
        {
            StackPanel stackPanel = (StackPanel)sender;
            _movePoint = e.GetPosition(stackPanel);
            stackPanel.CaptureMouse();
        }

        private void FigureMouseMove(object sender, MouseEventArgs e)
        {
            StackPanel stackPanel = (StackPanel)sender;

            if (_movePoint == null) { return; }
            
            Point point = e.GetPosition(MainRoot) - (Vector)_movePoint.Value;

            Canvas.SetLeft(stackPanel, point.X);
            Canvas.SetTop(stackPanel, point.Y);

            foreach (Line line in connections[stackPanel])
            {
                double line1 = Math.Sqrt(Math.Pow(point.X + stackPanel.ActualWidth / 2 - line.X1, 2) + Math.Pow(point.Y + stackPanel.ActualHeight / 2 - line.Y1, 2));
                double line2 = Math.Sqrt(Math.Pow(point.X + stackPanel.ActualWidth / 2 - line.X2, 2) + Math.Pow(point.Y + stackPanel.ActualHeight / 2 - line.Y2, 2));

                if(line1 < line2)
                {
                    line.X1 = point.X + stackPanel.ActualWidth / 2;
                    line.Y1 = point.Y + stackPanel.ActualHeight / 2;
                }
                else
                {
                    line.X2 = point.X + stackPanel.ActualWidth / 2;
                    line.Y2 = point.Y + stackPanel.ActualHeight / 2;
                }
            }
        }

        private void FigureMouseUp(object sender, MouseButtonEventArgs e)
        {
            StackPanel stackPanel = (StackPanel)sender;
            _movePoint = null;
            stackPanel.ReleaseMouseCapture();
        }
    }
}
