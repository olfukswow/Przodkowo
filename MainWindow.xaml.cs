using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace bmi
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void CalculateButton_Click(object sender, RoutedEventArgs e)
        {
            // Pobierz i waliduj wejście zgodnie z ustawieniami kultury (obsługa przecinka/kropki)
            if (!double.TryParse(WeightTextBox.Text.Trim(), NumberStyles.Number, CultureInfo.CurrentCulture, out double weight) ||
                !double.TryParse(HeightTextBox.Text.Trim(), NumberStyles.Number, CultureInfo.CurrentCulture, out double heightCm))
            {
                ShowError("Nieprawidłowy format danych. Użyj liczb (np. 70 lub 70,5).");
                return;
            }

            if (weight <= 0 || heightCm <= 0)
            {
                ShowError("Waga i wzrost muszą być większe od zera.");
                return;
            }

            double heightM = heightCm / 100.0;
            double bmi = weight / (heightM * heightM);
            string category;
            Brush color;

            // Klasyfikacja wg WHO (przykładowo)
            if (bmi < 18.5)
            {
                category = "Niedowaga";
                color = Brushes.LightSkyBlue;
            }
            else if (bmi < 25.0)
            {
                category = "Waga prawidłowa";
                color = Brushes.LightGreen;
            }
            else if (bmi < 30.0)
            {
                category = "Nadwaga";
                color = Brushes.Orange;
            }
            else
            {
                category = "Otyłość";
                color = Brushes.IndianRed;
            }

            ResultTextBlock.Text = $"BMI: {bmi:F1}\nKategoria: {category}";
            ResultBorder.Background = color;
            ResultBorder.BorderBrush = Brushes.Gray;
        }

        private void ShowError(string message)
        {
            MessageBox.Show(this, message, "Błąd walidacji", MessageBoxButton.OK, MessageBoxImage.Warning);
            ResultTextBlock.Text = "Podaj wagę i wzrost, a następnie kliknij Oblicz BMI.";
            ResultBorder.Background = Brushes.Transparent;
        }
    }
}