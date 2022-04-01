using System.Windows;
using System.Windows.Controls;

namespace TimeTrees
{
    class ConnectionFigures
    {
        public bool connection;
        public Point start;
        public Point end;
        public StackPanel? rectangleFirst;
        public StackPanel? rectangleLast;
        private static ConnectionFigures? instance;

        private ConnectionFigures() { }

        public static ConnectionFigures GetInstance()
        {
            if(instance == null) instance = new ConnectionFigures();
            return instance;
        }

        public void Clear()
        {
            connection = false;
            start = new Point();
            end = new Point();
            rectangleFirst = null;
            rectangleLast = null;
        }
    }
}
