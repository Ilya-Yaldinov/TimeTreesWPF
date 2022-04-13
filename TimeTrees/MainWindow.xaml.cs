using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace TimeTrees
{
    public partial class MainWindow : Window
    {
        private Dictionary<StackPanel, List<Line>> _connections = new Dictionary<StackPanel, List<Line>>();
        private string _path;
        private Point? _movePoint;
        private TimeTreesLogic _logic = new TimeTreesLogic();
        private CreateObject _createObject = new CreateObject();
        private bool _isFileOpen = false;

        public MainWindow()
        {
            InitializeComponent();
        } 

        private void CreateFigure_Click(object sender, RoutedEventArgs e)
        {
            StackPanel stackPanel = _createObject.CreateIcon();
            AddIconToCanvas(stackPanel, 350, 180);
        }

        private void AddIconToCanvas(StackPanel stackPanel, double x, double y)
        {
            _connections.Add(stackPanel, new List<Line>());
            MainRoot.Children.Add(stackPanel);

            Canvas.SetLeft(stackPanel, x);
            Canvas.SetTop(stackPanel, y);

            stackPanel.MouseLeftButtonDown += (sender, e) => FigureMouseDown(sender, e);
            stackPanel.MouseMove += (sender, e) => FigureMouseMove(sender, e);
            stackPanel.MouseLeftButtonUp += (sender, e) => FigureMouseUp(sender, e);
            stackPanel.MouseRightButtonDown += (sender, e) => WriteText(sender, e);
            stackPanel.MouseRightButtonDown += (sender, e) => Connection(sender, e);
        }

        private void FigureConnect_Click(object sender, RoutedEventArgs e)
        {
            ConnectionFigures connectionFigures = ConnectionFigures.GetInstance();
            connectionFigures.connection = true;
        }

        private void File_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "Text documents (.csv)|*.csv";
            Nullable<bool> result = openFile.ShowDialog();
            if (result == true)
            {
                _isFileOpen = true;
                _path = openFile.FileName;
                Person[] people = _logic.ReadPerson(_path);
                foreach(Person person in people)
                {
                    StackPanel stackPanel = _createObject.CreateIcon(person);
                    AddIconToCanvas(stackPanel, person.PositionX, person.PositionY);
                }
                foreach(Person person in people)
                {
                    AddKinship(person);
                }
                RedrawCanvas();
            }
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (!_isFileOpen)
            {
                AddToNewFile();
                return;
            }
            AddToThisFile();
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            ConnectionFigures connectionFigures = ConnectionFigures.GetInstance();
            connectionFigures.Clear();
            _connections.Clear();
            MainRoot.Children.Clear();
            _path = null;
            _isFileOpen = false;
        }

        private void AddKinship(Person people)
        {
            int id = people.Id;
            double startPosX = Canvas.GetLeft(_connections.ElementAt(id - 1).Key);
            double startPosY = Canvas.GetTop(_connections.ElementAt(id - 1).Key);
            string[] parrents = people.Parrents;
            foreach(string parrent in parrents)
            {
                if (!string.IsNullOrEmpty(parrent) && !string.IsNullOrWhiteSpace(parrent))
                {
                    double endPosX = Canvas.GetLeft(_connections.ElementAt(Convert.ToInt32(parrent) - 1).Key);
                    double endPosY = Canvas.GetTop(_connections.ElementAt(Convert.ToInt32(parrent) - 1).Key);
                    Line line = _createObject.CreateLine();
                    line.X1 = startPosX;
                    line.Y1 = startPosY;
                    line.X2 = endPosX;
                    line.Y2 = endPosY;
                    _connections.ElementAt(Convert.ToInt32(parrent) - 1).Value.Add(line);
                    _connections.ElementAt(id - 1).Value.Add(line);
                    MainRoot.Children.Add(line);
                    line.MouseRightButtonDown += Kinship;
                }
            }
            if (people.Spouse.HasValue)
            {
                AddSpouse(people, startPosX, startPosY);
            }
        }

        private void AddSpouse(Person people, double startPosX, double startPosY)
        {
            double endPosX = Canvas.GetLeft(_connections.ElementAt(people.Spouse.Value - 1).Key);
            double endPosY = Canvas.GetTop(_connections.ElementAt(people.Spouse.Value - 1).Key);
            Line line = _createObject.CreateLine();
            line.X1 = startPosX;
            line.Y1 = startPosY;
            line.X2 = endPosX;
            line.Y2 = endPosY;
            line.Stroke = Brushes.Red;
            _connections.ElementAt(people.Spouse.Value - 1).Value.Add(line);
            _connections.ElementAt(people.Id - 1).Value.Add(line);
            MainRoot.Children.Add(line);
            line.MouseRightButtonDown += Kinship;
        }

        public void AddToThisFile()
        {
            List<string> peoples = StackPanelToString();
            int count = 0;
            StreamWriter streamWriter = new StreamWriter(_path, false);
            foreach (string people in peoples)
            {
                count++;
                streamWriter.WriteLine(people);
                if (count == people.Length - 1)
                {
                    streamWriter.Write(people);
                }
            }
            streamWriter.Close();
        }

        public void AddToNewFile()
        {
            var peoples = StackPanelToString();
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.FileName = "FamilyTree";
            saveFile.DefaultExt = ".csv";
            saveFile.Filter = "Text documents (.csv)|*.csv";

            Nullable<bool> result = saveFile.ShowDialog();

            if (result == true)
            {
                string filename = saveFile.FileName;
                StreamWriter streamWriter = new StreamWriter(filename);
                foreach (string people in peoples)
                {
                    streamWriter.WriteLine(people);
                }
                streamWriter.Close();
            }
        }

        private List<string> StackPanelToString()
        {
            List<string> result = new List<string>();
            int id = 0;
            foreach (var keyValue in _connections)
            {
                StringBuilder sb = new StringBuilder();
                id++;
                sb.Append($"{id};");
                StackPanel? key = keyValue.Key;
                foreach (var child in key.Children)
                {
                    TextBox textBox = (TextBox)child;
                    if (String.IsNullOrEmpty(textBox.Text))
                    {
                        sb.Append($" ;");
                    }
                    else
                    {
                        sb.Append($"{textBox.Text};");
                    }
                }
                StringBuilder stringBuilder = ConnectionToString(sb, keyValue);
                double posX = Canvas.GetLeft(keyValue.Key);
                double posY = Canvas.GetTop(keyValue.Key);
                stringBuilder.Append($"{posX},{posY};");
                string line = stringBuilder.ToString();
                result.Add(stringBuilder.ToString());
            }
            return result;
        }

        private StringBuilder ConnectionToString(StringBuilder sb, KeyValuePair<StackPanel, List<Line>> keyValue)
        {
            int parrentCount = 0;
            int childrenCount = 0;
            int? spouse = null;
            List<int> parrents = new List<int>();
            List<int> children = new List<int>();
            double startPos = Canvas.GetTop(keyValue.Key);
            for (int i = 0; i < _connections.Count; i++)
            {
                if (_connections.ElementAt(i).Key != keyValue.Key)
                {
                    foreach (var start in keyValue.Value)
                    {
                        foreach (var end in _connections.ElementAt(i).Value)
                        {
                            if (start == end)
                            {
                                if (start.Stroke == Brushes.Red)
                                {
                                    spouse = i + 1;
                                }
                                if (start.Stroke == Brushes.LightBlue)
                                {
                                    double endPos = Canvas.GetTop(_connections.ElementAt(i).Key);
                                    if (endPos < startPos)
                                    {
                                        parrents.Add(i+1);
                                    }
                                    else
                                    {
                                        children.Add(i+1);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            foreach(var parrent in parrents)
            {
                parrentCount++;
                sb.Append($"{Convert.ToInt32(parrent)},");
            }
            if(parrentCount > 2)
            {
                throw new Exception("Родителей не может быть больше двух.");
            }
            else if(parrentCount > 0)
            {
                sb.Remove(sb.Length - 1, 1);
                sb.Append(";");
            }
            else
            {
                sb.Append(" ;");
            }

            if(spouse == null)
            {
                sb.Append(" ;");
            }
            else
            {
                sb.Append($"{spouse.Value};");
            }
            
            foreach(var child in children)
            {
                childrenCount++;
                sb.Append($"{Convert.ToInt32(child)},");
            }

            if(childrenCount > 0)
            {
                sb.Remove(sb.Length - 1, 1);
                sb.Append(";");
            }
            else
            {
                sb.Append(" ;");
            }

            return sb;
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

                Line line = _createObject.CreateLine();

                line.X1 = connectionFigures.start.X;
                line.Y1 = connectionFigures.start.Y;
                line.X2 = connectionFigures.end.X;
                line.Y2 = connectionFigures.end.Y;

                if (connectionFigures.rectangleFirst == connectionFigures.rectangleLast)
                {
                    connectionFigures.Clear();
                    return;
                }

                foreach(Line lineStart in _connections[connectionFigures.rectangleFirst])
                    foreach (Line lineEnd in _connections[connectionFigures.rectangleLast])
                        if (lineStart == lineEnd) return;

                _connections[connectionFigures.rectangleFirst].Add(line);
                _connections[connectionFigures.rectangleLast].Add(line);
                MainRoot.Children.Add(line);
                line.MouseRightButtonDown += Kinship;
                RedrawCanvas();
                connectionFigures.Clear();
            }

        }

        private void Kinship(object sender, MouseButtonEventArgs e)
        {
            Line line = (Line)sender;
            if(line.Stroke == Brushes.LightBlue) line.Stroke = Brushes.Red;
            else line.Stroke = Brushes.LightBlue;
            RedrawCanvas();
        }

        private void RedrawCanvas()
        {
            MainRoot.Children.Clear();

            foreach (var keyValuePair in _connections)
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

            foreach (Line line in _connections[stackPanel])
            {
                double line1 = Math.Sqrt(Math.Pow(point.X + stackPanel.ActualWidth / 2 - line.X1, 2) + Math.Pow(point.Y + stackPanel.ActualHeight / 2 - line.Y1, 2));
                double line2 = Math.Sqrt(Math.Pow(point.X + stackPanel.ActualWidth / 2 - line.X2, 2) + Math.Pow(point.Y + stackPanel.ActualHeight / 2 - line.Y2, 2));

                if(line.Stroke == Brushes.LightBlue)
                {
                    if(line.Y1 >= line.Y2)
                    {
                        if (line1 >= line2)
                        {
                            line.X1 = point.X + stackPanel.ActualWidth / 2;
                            line.Y1 = point.Y + stackPanel.ActualHeight;
                        }
                        else
                        {
                            line.X2 = point.X + stackPanel.ActualWidth / 2;
                            line.Y2 = point.Y;
                        }
                    }
                    else
                    {
                        if (line1 <= line2)
                        {
                            line.X1 = point.X + stackPanel.ActualWidth / 2;
                            line.Y1 = point.Y + stackPanel.ActualHeight;
                        }
                        else
                        {
                            line.X2 = point.X + stackPanel.ActualWidth / 2;
                            line.Y2 = point.Y;
                        }
                    }
                }
                if (line.Stroke == Brushes.Red)
                {
                    if(line.X1 > line.X2)
                    {
                        if (line1 >= line2)
                        {
                            line.X1 = point.X + stackPanel.ActualWidth;
                            line.Y1 = point.Y + stackPanel.ActualHeight / 2;
                        }
                        else
                        {
                            line.X2 = point.X;
                            line.Y2 = point.Y + stackPanel.ActualHeight / 2;
                        }
                    }
                    else
                    {
                        if (line1 <= line2)
                        {
                            line.X1 = point.X + stackPanel.ActualWidth;
                            line.Y1 = point.Y + stackPanel.ActualHeight / 2;
                        }
                        else
                        {
                            line.X2 = point.X;
                            line.Y2 = point.Y + stackPanel.ActualHeight / 2;
                        }
                    }
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
