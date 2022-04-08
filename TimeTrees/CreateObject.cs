using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace TimeTrees
{
    public class CreateObject
    {
        TimeTreesLogic logic = new TimeTreesLogic();
        public StackPanel CreateIcon()
        {
            StackPanel stackPanel = new StackPanel();

            TextBox name = CreateNameTextBox();
            TextBox birth = CreateBirthTextBox();
            TextBox death = CreateDeathTextBox();

            stackPanel.Children.Add(name);
            stackPanel.Children.Add(birth);
            stackPanel.Children.Add(death);

            return stackPanel;
        }

        public StackPanel CreateIcon(Person people)
        {
            StackPanel stackPanel = new StackPanel();

            TextBox name = CreateNameTextBox();
            TextBox birth = CreateBirthTextBox();
            TextBox death = CreateDeathTextBox();

            name.Text = people.Name;
            birth.Text = people.Birth.ToString(logic.OutputFormat(people.Birth.ToString()));
            death.Text = people.Death.HasValue ? people.Death.Value.ToString(logic.OutputFormat(people.Death.Value.ToString())) : "";
            if (death.Text == "0001-01-01") death.Text = "";

            stackPanel.Children.Add(name);
            stackPanel.Children.Add(birth);
            stackPanel.Children.Add(death);

            return stackPanel;
        }

        public Line CreateLine()
        {
            Line line = new Line();
            line.Stroke = Brushes.LightBlue;
            line.StrokeThickness = 5;
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
                if (e.Key == System.Windows.Input.Key.Enter)
                {
                    if (text.Text == "" || text.Text == "Can't be empty")
                    {
                        text.Text = "Can't be empty";
                    }
                    else { text.IsEnabled = false; }
                }
            };

            return textBox;
        }

        private TextBox CreateBirthTextBox()
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
                    DateTime date = logic.ParseDate(textBox.Text);
                    if (date != default) { text.IsEnabled = false; }
                    else { text.Text = "Format yyyy-mm-dd"; }
                }
            };

            return textBox;
        }

        private TextBox CreateDeathTextBox()
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
                    DateTime date = logic.ParseDate(textBox.Text);
                    if (text.Text == "" || date != default) { text.IsEnabled = false; }
                    else { text.Text = "Format yyyy-mm-dd"; }
                }
            };

            return textBox;
        }
    }
}
