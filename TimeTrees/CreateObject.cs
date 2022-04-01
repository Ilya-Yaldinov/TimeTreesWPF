using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace TimeTrees
{
    public class CreateObject
    {
        public StackPanel CreateIcon()
        {
            StackPanel stackPanel = new StackPanel();
            TextBox name = CreateNameTextBox();
            TextBox birth = CreateDateTextBox();
            TextBox death = CreateDateTextBox();

            stackPanel.Children.Add(name);
            stackPanel.Children.Add(birth);
            stackPanel.Children.Add(death);

            return stackPanel;
        }

        public Line CreateLine()
        {
            Line line = new Line();
            line.Stroke = Brushes.LightBlue;
            line.StrokeThickness = 3;
            line.StrokeStartLineCap = PenLineCap.Round;
            line.StrokeEndLineCap = PenLineCap.Round;

            return line;
        }

        private TextBox CreateNameTextBox()
        {
            TextBox textBox = new TextBox();

            textBox.Background = Brushes.Orange;
            textBox.BorderBrush = Brushes.Orange;
            textBox.Width = 120;
            textBox.Height = 20;
            textBox.TextAlignment = TextAlignment.Center;
            textBox.ContextMenu = null;
            textBox.IsEnabled = false;

            textBox.KeyDown += (sender, e) => 
            {
                TextBox text = (TextBox)sender;
                if(e.Key == System.Windows.Input.Key.Enter)
                {
                    if(text.Text == "" || text.Text == "Can't be empty") 
                    { 
                        text.Text = "Can't be empty"; 
                    }
                    else { text.IsEnabled = false; }
                }
            };

            return textBox;
        }

        private TextBox CreateDateTextBox()
        {
            TextBox textBox = new TextBox();

            textBox.Background = Brushes.Orange;
            textBox.BorderBrush = Brushes.Orange;
            textBox.Width = 120;
            textBox.Height = 20;
            textBox.TextAlignment = TextAlignment.Center;
            textBox.ContextMenu = null;
            textBox.IsEnabled = false;

            textBox.KeyDown += (sender, e) =>
            {
                TextBox text = (TextBox)sender;
                if (e.Key == System.Windows.Input.Key.Enter)
                {
                    DateTime date = ParseDate(textBox.Text);
                    if (date == default || text.Text == "Format yyyy-mm-dd") 
                    { 
                        text.Text = "Format yyyy-mm-dd"; 
                    }
                    else { text.IsEnabled = false; }
                }
            };

            return textBox;
        }

        public DateTime ParseDate(string value)
        {
            DateTime date;
            if (!DateTime.TryParseExact(value, "yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                if (!DateTime.TryParseExact(value, "yyyy-MM", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                    if (!DateTime.TryParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                        date = default;

            return date;
        }
    }
}
