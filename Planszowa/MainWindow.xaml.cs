using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Planszowa
{
    public partial class MainWindow : Window
    {
        private readonly DispatcherTimer gameTimer = new DispatcherTimer();
        private readonly DispatcherTimer flipBackTimer = new DispatcherTimer();
        private readonly Random rng = new Random();

        private Button firstButton;
        private Button secondButton;
        private bool allowClicks = true;

        private int moves;
        private int secondsElapsed;
        private int pairsCount;
        private int matchedPairs;

        private readonly List<string> imagePaths = new()
        {
            "images/1.jpg", "images/1.jpeg", "images/2.jpeg", "images/3.jpeg",
            "images/sss.jpeg", "images/dadew.jpeg", "images/michal.jpeg", "images/d.png",
            "images/a.jpeg", "images/b.jpeg", "images/c.jpeg", "images/e.jpeg"
        };

        public MainWindow()
        {
            InitializeComponent();

            // Reaguj na zmianę rozmiaru panelu kart i okna — dzięki temu obrazy skalują się dynamicznie
            CardPanel.SizeChanged += CardPanel_SizeChanged;
            this.SizeChanged += Window_SizeChanged;

            gameTimer.Interval = TimeSpan.FromSeconds(1);
            gameTimer.Tick += GameTimer_Tick;

            flipBackTimer.Interval = TimeSpan.FromSeconds(0.8);
            flipBackTimer.Tick += FlipBackTimer_Tick;

            PairCountCombo.SelectedIndex = 0;
            StartNewGame();
        }

        private void StartNewGame()
        {
            moves = 0;
            secondsElapsed = 0;
            matchedPairs = 0;
            firstButton = null;
            secondButton = null;
            allowClicks = true;

            MovesText.Text = "0";
            TimerText.Text = "00:00";
            gameTimer.Stop();

            if (!int.TryParse(((ComboBoxItem)PairCountCombo.SelectedItem).Content.ToString(), out pairsCount))
                pairsCount = 8;

            if (pairsCount > imagePaths.Count)
            {
                MessageBox.Show("Brak wystarczającej liczby obrazów do utworzenia tylu par.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var selectedImages = imagePaths.Take(pairsCount).ToList();
            var values = selectedImages.Concat(selectedImages).OrderBy(_ => rng.Next()).ToList();

            int totalCards = values.Count;
            int columns = (int)Math.Ceiling(Math.Sqrt(totalCards));
            int rows = (int)Math.Ceiling((double)totalCards / columns);
            CardPanel.Columns = columns;
            CardPanel.Rows = rows;

            CardPanel.Children.Clear();

            MessageBox.Show($"Katalog uruchomieniowy:\n{Environment.CurrentDirectory}", "Debug ścieżki");

            foreach (var path in values)
            {
                BitmapImage bitmap = null;
                try
                {
                    bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(path, UriKind.RelativeOrAbsolute);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Błąd ładowania obrazu:\n{path}\n{ex.Message}", "Błąd obrazu", MessageBoxButton.OK, MessageBoxImage.Error);
                    continue;
                }

                var img = new Image
                {
                    Source = bitmap,
                    Visibility = Visibility.Hidden,
                    Stretch = System.Windows.Media.Stretch.UniformToFill
                    // Width/Height będą ustawione centralnie w UpdateCardSizes()
                };

                var btn = new Button
                {
                    Tag = path,
                    Content = img,
                    Margin = new Thickness(6),
                    Padding = new Thickness(6),
                    Background = SystemColors.ControlLightBrush
                };
                btn.Click += Card_Click;
                CardPanel.Children.Add(btn);
            }

            // Zaktualizuj rozmiary kart po zbudowaniu siatki — użyj dispatcher, bo ActualWidth/ActualHeight może nie być jeszcze poprawne
            Dispatcher.BeginInvoke(new Action(UpdateCardSizes), DispatcherPriority.Loaded);
        }

        private void Card_Click(object sender, RoutedEventArgs e)
        {
            if (!allowClicks) return;

            var btn = (Button)sender;
            if (btn.IsEnabled == false) return;

            if (!gameTimer.IsEnabled && firstButton == null && secondButton == null)
                gameTimer.Start();

            if (firstButton == btn) return;

            RevealButton(btn);

            if (firstButton == null)
            {
                firstButton = btn;
                return;
            }

            if (secondButton == null)
            {
                secondButton = btn;
                allowClicks = false;
                moves++;
                MovesText.Text = moves.ToString();

                var val1 = (string)firstButton.Tag;
                var val2 = (string)secondButton.Tag;
                if (val1 == val2)
                {
                    firstButton.IsEnabled = false;
                    secondButton.IsEnabled = false;
                    matchedPairs++;
                    firstButton = null;
                    secondButton = null;
                    allowClicks = true;

                    if (matchedPairs == pairsCount)
                    {
                        gameTimer.Stop();
                        MessageBox.Show($"Gratulacje! Znalazłeś wszystkie pary.\nRuchy: {moves}\nCzas: {FormatTime(secondsElapsed)}", "Koniec gry", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                {
                    flipBackTimer.Start();
                }
            }
        }

        private void RevealButton(Button btn)
        {
            if (btn.Content is Image img)
                img.Visibility = Visibility.Visible;

            btn.Background = SystemColors.ControlLightLightBrush;
        }

        private void HideButton(Button btn)
        {
            if (btn.Content is Image img)
                img.Visibility = Visibility.Hidden;

            btn.Background = SystemColors.ControlLightBrush;
        }

        private void FlipBackTimer_Tick(object sender, EventArgs e)
        {
            flipBackTimer.Stop();

            if (firstButton != null && secondButton != null)
            {
                HideButton(firstButton);
                HideButton(secondButton);
            }

            firstButton = null;
            secondButton = null;
            allowClicks = true;
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            secondsElapsed++;
            TimerText.Text = FormatTime(secondsElapsed);
        }

        private static string FormatTime(int totalSeconds)
        {
            var ts = TimeSpan.FromSeconds(totalSeconds);
            return ts.ToString(@"mm\:ss");
        }

        private void RestartButton_Click(object sender, RoutedEventArgs e)
        {
            StartNewGame();
        }

        private void PairCountCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsLoaded)
                StartNewGame();
        }

        // ------------------ DODATKOWE METODY DLA SKALOWANIA ------------------

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Przy zmianie rozmiaru okna zaktualizuj rozmiary kart
            UpdateCardSizes();
        }

        private void CardPanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateCardSizes();
        }

        // Oblicza rozmiar "komórki" siatki i ustawia Width/Height przycisków (a przez to obrazów)
        private void UpdateCardSizes()
        {
            // Jeśli brak dzieci lub wymiary nieokreślone — nic nie rób
            if (CardPanel.Children.Count == 0) return;
            if (CardPanel.ActualWidth <= 0 || CardPanel.ActualHeight <= 0) return;

            int cols = Math.Max(1, CardPanel.Columns);
            int rows = Math.Max(1, CardPanel.Rows);

            // Margines boczny każdego przycisku: używamy Margin + Padding + mały zapas
            // Margines występuje po obu stronach komórki, więc odejmujemy je przy obliczeniu
            double horizontalMargin = 12; // suma odstępów (przybliżona: left+right)
            double verticalMargin = 12;   // suma odstępów (top+bottom)

            double availableWidth = Math.Max(0.0, CardPanel.ActualWidth - cols * horizontalMargin);
            double availableHeight = Math.Max(0.0, CardPanel.ActualHeight - rows * verticalMargin);

            double cellWidth = availableWidth / cols;
            double cellHeight = availableHeight / rows;

            // Ustawiamy kwadratowe komórki — wielkość zależna od mniejszego z wymiarów
            double targetSize = Math.Max(48, Math.Floor(Math.Min(cellWidth, cellHeight)));

            foreach (var child in CardPanel.Children.OfType<Button>())
            {
                child.Width = targetSize;
                child.Height = targetSize;

                // Upewnij się, że obraz w środku dostosowuje się do kontenera
                if (child.Content is Image img)
                {
                    img.Width = double.NaN;  // pozwól layoutowi dopasować obraz do przycisku
                    img.Height = double.NaN;
                    img.Stretch = System.Windows.Media.Stretch.UniformToFill;
                }
            }
        }
    }
}