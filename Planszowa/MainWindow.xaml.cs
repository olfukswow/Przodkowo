using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
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

        private readonly List<string> emojiPool = new()
        {
            "🍎","🍌","🍇","🍓","🍑","🍍","🍒","🥝","🍉","🍋","🥑","🍐","🍊","🍈"
        };

        public MainWindow()
        {
            InitializeComponent();

            gameTimer.Interval = TimeSpan.FromSeconds(1);
            gameTimer.Tick += GameTimer_Tick;

            flipBackTimer.Interval = TimeSpan.FromSeconds(0.8);
            flipBackTimer.Tick += FlipBackTimer_Tick;

            // Domyślnie 8 par
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

            MovesText.Text = moves.ToString();
            TimerText.Text = "00:00";
            gameTimer.Stop();

            if (!int.TryParse(((ComboBoxItem)PairCountCombo.SelectedItem).Content.ToString(), out pairsCount))
                pairsCount = 8;

            var values = new List<string>();
            for (int i = 0; i < pairsCount; i++)
            {
                string symbol = i < emojiPool.Count ? emojiPool[i] : $"#{i + 1}";
                values.Add(symbol);
                values.Add(symbol);
            }
            values = values.OrderBy(_ => rng.Next()).ToList();
            int totalCards = values.Count;
            int columns = (int)Math.Ceiling(Math.Sqrt(totalCards));
            int rows = (int)Math.Ceiling((double)totalCards / columns);
            CardPanel.Columns = columns;
            CardPanel.Rows = rows;

            CardPanel.Children.Clear();

            for (int i = 0; i < totalCards; i++)
            {
                var btn = new Button
                {
                    Tag = values[i], 
                    Content = "?", 
                    FontSize = 28,
                    Margin = new Thickness(6),
                    Padding = new Thickness(6),
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    Background = SystemColors.ControlLightBrush,
                };
                btn.Click += Card_Click;
                CardPanel.Children.Add(btn);
            }
        }

        private void Card_Click(object sender, RoutedEventArgs e)
        {
            if (!allowClicks) return;

            var btn = (Button)sender;

            if (btn.IsEnabled == false) return;

            if (!gameTimer.IsEnabled && firstButton == null && secondButton == null)
                gameTimer.Start();

            if (firstButton == btn) return;

            // Odsłoń kartę
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
            btn.Content = btn.Tag; 
            btn.Background = SystemColors.ControlLightLightBrush;
        }

        private void HideButton(Button btn)
        {
            btn.Content = "?";
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
    }
}