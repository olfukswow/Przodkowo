using System.Collections.ObjectModel;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ToDo
{
    public partial class MainWindow : Window
    {
        private readonly ObservableCollection<TaskItem> _tasks = new();
        private readonly string _dataFilePath;

        public MainWindow()
        {
            InitializeComponent();

            TasksListBox.ItemsSource = _tasks;

            var appData = System.IO.Path.Combine(
                System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData),
                "ToDoApp");
            System.IO.Directory.CreateDirectory(appData);
            _dataFilePath = System.IO.Path.Combine(appData, "tasks.json");

            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;
        }

        private void MainWindow_Loaded(object? sender, RoutedEventArgs e)
        {
            LoadTasks();
        }

        private void MainWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveTasks();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            AddNewTask();
        }

        private void NewTaskTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AddNewTask();
                e.Handled = true;
            }
        }

        private void AddNewTask()
        {
            var text = NewTaskTextBox.Text?.Trim();
            if (string.IsNullOrEmpty(text)) return;

            _tasks.Add(new TaskItem { Text = text });
            NewTaskTextBox.Clear();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is TaskItem item)
            {
                _tasks.Remove(item);
            }
        }

        private void LoadTasks()
        {
            try
            {
                if (!System.IO.File.Exists(_dataFilePath)) return;

                var json = System.IO.File.ReadAllText(_dataFilePath);
                var loaded = JsonSerializer.Deserialize<TaskItem[]>(
                    json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (loaded is not null)
                {
                    _tasks.Clear();
                    foreach (var t in loaded) _tasks.Add(t);
                }
            }
            catch
            {
                // jeśli błąd odczytu, ignorujemy — aplikacja zaczyna z pustą listą
            }
        }

        private void SaveTasks()
        {
            try
            {
                var arr = _tasks.ToArray();
                var json = JsonSerializer.Serialize(arr, new JsonSerializerOptions { WriteIndented = true });
                System.IO.File.WriteAllText(_dataFilePath, json);
            }
            catch
            {
                // w razie problemu z zapisem pomijamy — nie chcemy przerywać zamykania aplikacji
            }
        }
    }
}