namespace MyPlan;

using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

public static class DialogHelper
{
    public static async Task ShowMessageAsync(Window? parent, string message, string title = "Information")
    {
        var dialog = new Window
        {
            Title = title,
            Width = 440,
            Height = 220,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            CanResize = false,
            Background = Brush.Parse("#FFF9F5"),
            FontFamily = new FontFamily("Georgia"),
            FontSize = 14
        };

        var mainPanel = new Grid
        {
            RowDefinitions = new RowDefinitions("Auto,*,Auto"),
            Margin = new Thickness(15)
        };

        var header = new Border
        {
            Background = Brush.Parse("#FFA726"),
            CornerRadius = new CornerRadius(4),
            Padding = new Thickness(10),
            Margin = new Thickness(0, 0, 0, 15),
            Child = new TextBlock
            {
                Text = title,
                Foreground = Brushes.White,
                FontWeight = FontWeight.Bold,
                FontSize = 16,
                HorizontalAlignment = HorizontalAlignment.Center,
                FontFamily = new FontFamily("Verdana")
            }
        };
        Grid.SetRow(header, 0);

        var textBlock = new TextBlock
        {
            Text = message,
            TextWrapping = TextWrapping.Wrap,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
            Foreground = Brush.Parse("#333"),
            Margin = new Thickness(10, 0, 10, 15)
        };
        Grid.SetRow(textBlock, 1);

        var okButton = new Button
        {
            Content = "OK",
            Width = 90,
            Height = 35,
            HorizontalAlignment = HorizontalAlignment.Center,
            Background = Brush.Parse("#FFA726"),
            Foreground = Brushes.White,
            FontWeight = FontWeight.Bold,
            CornerRadius = new CornerRadius(4),
            HorizontalContentAlignment = HorizontalAlignment.Center,
            VerticalContentAlignment = VerticalAlignment.Center
        };
        okButton.Click += (s, e) => dialog.Close();
        Grid.SetRow(okButton, 2);

        mainPanel.Children.Add(header);
        mainPanel.Children.Add(textBlock);
        mainPanel.Children.Add(okButton);

        dialog.Content = mainPanel;
        if (parent != null)
        {
            await dialog.ShowDialog(parent);
        }
        else
        {
            dialog.Show();
        }
    }

    public static async Task<bool> ShowConfirmAsync(Window? parent, string message, string title = "Confirmation")
    {
        var result = false;
        var dialog = new Window
        {
            Title = title,
            Width = 440,
            Height = 220,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            CanResize = false,
            Background = Brush.Parse("#FFF9F5"),
            FontFamily = new FontFamily("Georgia"),
            FontSize = 14
        };

        var mainPanel = new Grid
        {
            RowDefinitions = new RowDefinitions("Auto,*,Auto"),
            Margin = new Thickness(15)
        };

        var header = new Border
        {
            Background = Brush.Parse("#FFA726"),
            CornerRadius = new CornerRadius(4),
            Padding = new Thickness(10),
            Margin = new Thickness(0, 0, 0, 15),
            Child = new TextBlock
            {
                Text = title,
                Foreground = Brushes.White,
                FontWeight = FontWeight.Bold,
                FontSize = 16,
                HorizontalAlignment = HorizontalAlignment.Center,
                FontFamily = new FontFamily("Verdana")
            }
        };
        Grid.SetRow(header, 0);

        var textBlock = new TextBlock
        {
            Text = message,
            TextWrapping = TextWrapping.Wrap,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
            Foreground = Brush.Parse("#333"),
            Margin = new Thickness(10, 0, 10, 15)
        };
        Grid.SetRow(textBlock, 1);

        var buttonPanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Center,
            Spacing = 15
        };

        var yesButton = new Button
        {
            Content = "Oui",
            Width = 90,
            Height = 35,
            Background = Brush.Parse("#FB8C00"),
            Foreground = Brushes.White,
            FontWeight = FontWeight.Bold,
            CornerRadius = new CornerRadius(4),
            HorizontalContentAlignment = HorizontalAlignment.Center,
            VerticalContentAlignment = VerticalAlignment.Center
        };
        yesButton.Click += (s, e) => { result = true; dialog.Close(); };

        var noButton = new Button
        {
            Content = "Non",
            Width = 90,
            Height = 35,
            Background = Brush.Parse("#BF360C"),
            Foreground = Brushes.White,
            FontWeight = FontWeight.Bold,
            CornerRadius = new CornerRadius(4),
            HorizontalContentAlignment = HorizontalAlignment.Center,
            VerticalContentAlignment = VerticalAlignment.Center
        };
        noButton.Click += (s, e) => { result = false; dialog.Close(); };

        buttonPanel.Children.Add(yesButton);
        buttonPanel.Children.Add(noButton);
        Grid.SetRow(buttonPanel, 2);

        mainPanel.Children.Add(header);
        mainPanel.Children.Add(textBlock);
        mainPanel.Children.Add(buttonPanel);

        dialog.Content = mainPanel;
        if (parent != null)
        {
            await dialog.ShowDialog(parent);
        }
        else
        {
            dialog.Show();
        }

        return result;
    }

    public static async Task<string?> ShowInputAsync(Window? parent, string prompt, string title = "Saisie", string defaultValue = "")
    {
        string? result = null;
        var dialog = new Window
        {
            Title = title,
            Width = 480,
            Height = 260,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            CanResize = false,
            Background = Brush.Parse("#FFF9F5"),
            FontFamily = new FontFamily("Georgia"),
            FontSize = 14
        };

        var mainPanel = new Grid
        {
            RowDefinitions = new RowDefinitions("Auto,*,Auto,Auto"),
            Margin = new Thickness(15)
        };

        var header = new Border
        {
            Background = Brush.Parse("#FFA726"),
            CornerRadius = new CornerRadius(4),
            Padding = new Thickness(10),
            Margin = new Thickness(0, 0, 0, 10),
            Child = new TextBlock
            {
                Text = title,
                Foreground = Brushes.White,
                FontWeight = FontWeight.Bold,
                FontSize = 16,
                HorizontalAlignment = HorizontalAlignment.Center,
                FontFamily = new FontFamily("Verdana")
            }
        };
        Grid.SetRow(header, 0);

        var textBlock = new TextBlock
        {
            Text = prompt,
            TextWrapping = TextWrapping.Wrap,
            Foreground = Brush.Parse("#333"),
            Margin = new Thickness(0, 0, 0, 10)
        };
        Grid.SetRow(textBlock, 1);

        var textBox = new TextBox
        {
            Text = defaultValue,
            Margin = new Thickness(0, 0, 0, 15),
            Background = Brushes.White,
            BorderBrush = Brush.Parse("#FFCC80"),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(4),
            Padding = new Thickness(8)
        };
        Grid.SetRow(textBox, 2);

        var buttonPanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Center,
            Spacing = 15
        };

        var okButton = new Button
        {
            Content = "Valider",
            Width = 100,
            Height = 35,
            Background = Brush.Parse("#FB8C00"),
            Foreground = Brushes.White,
            FontWeight = FontWeight.Bold,
            CornerRadius = new CornerRadius(4),
            HorizontalContentAlignment = HorizontalAlignment.Center,
            VerticalContentAlignment = VerticalAlignment.Center
        };
        okButton.Click += (s, e) => { result = textBox.Text; dialog.Close(); };

        var cancelButton = new Button
        {
            Content = "Annuler",
            Width = 100,
            Height = 35,
            Background = Brush.Parse("#BF360C"),
            Foreground = Brushes.White,
            FontWeight = FontWeight.Bold,
            CornerRadius = new CornerRadius(4),
            HorizontalContentAlignment = HorizontalAlignment.Center,
            VerticalContentAlignment = VerticalAlignment.Center
        };
        cancelButton.Click += (s, e) => { result = null; dialog.Close(); };

        buttonPanel.Children.Add(okButton);
        buttonPanel.Children.Add(cancelButton);
        Grid.SetRow(buttonPanel, 3);

        mainPanel.Children.Add(header);
        mainPanel.Children.Add(textBlock);
        mainPanel.Children.Add(textBox);
        mainPanel.Children.Add(buttonPanel);

        dialog.Content = mainPanel;
        if (parent != null)
        {
            await dialog.ShowDialog(parent);
        }
        else
        {
            dialog.Show();
        }

        return result;
    }
}
