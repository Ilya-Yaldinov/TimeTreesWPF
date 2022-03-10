using System.Windows;
using System.Windows.Shapes;

namespace TimeTrees
{
    static class ConnectionFigures
    {
        public static bool connection;
        public static Point start;
        public static Point end;
        public static Rectangle? rectangleFirst;
        public static Rectangle? rectangleLast;

        public static void Clear()
        {
            ConnectionFigures.connection = false;
            ConnectionFigures.start = new Point();
            ConnectionFigures.end = new Point();
            ConnectionFigures.rectangleFirst = null;
            ConnectionFigures.rectangleLast = null;
        }

    }

}
