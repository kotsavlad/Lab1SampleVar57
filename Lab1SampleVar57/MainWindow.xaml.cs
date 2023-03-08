using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Lab1SampleVar57
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly double[] _testNumbers = { 2, 100, -3, 4, 5, 1, 3, 0, -1, 4, 2, 1, 1, 2 };

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Handler(object? sender, NotifyCollectionChangedEventArgs e)
        {
            var items = sender as ItemCollection;
            RunButton.IsEnabled = items?.Count > 0;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (!double.TryParse(NumberTextBox.Text, out var number))
            {
                MessageBox.Show("Invalid value. Enter a valid number, please!", "Error", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                NumberTextBox.SelectAll();
                NumberTextBox.Focus();
            }
            else
            {
                //NumbersListBox.Items.Add(number);
                NumbersListBox.Items.Add(new ListBoxItem { Content = number });
            }
        }

        private void FillButton_Click(object sender, RoutedEventArgs e)
        {
            NumbersListBox.Items.Clear();
            foreach (var number in _testNumbers)
            {
                NumbersListBox.Items.Add(new ListBoxItem { Content = number });
            }

            //RunButton.IsEnabled = NumbersListBox.Items.Count > 0;
        }

        private void RunButton_Click(object sender, RoutedEventArgs e)
        {
            var numbers = new double[NumbersListBox.Items.Count];
            for (int i = 0; i < numbers.Length; i++)
            {
                //numbers[i] = double.Parse(NumbersListBox.Items[i].ToString());
                numbers[i] = Convert.ToDouble(((ListBoxItem)NumbersListBox.Items[i]).Content);
            }

            var areNumbersRequired = !SelectResultCheckBox?.IsChecked ?? true;
            var (indices, bigNumbers) = SelectBigNumbers(numbers, areNumbersRequired);
            var resultMessage = areNumbersRequired
                ? "Array items that are greater than all next items:\n" + string.Join(", ", bigNumbers!)
                : "Array items that are greater than all next items are selected in the list.";
            MessageBox.Show(resultMessage, "Result", MessageBoxButton.OK, MessageBoxImage.Information);
            if (!areNumbersRequired)
            {
                for (int i = 0; i < NumbersListBox.Items.Count; i++)
                {
                    ((ListBoxItem)NumbersListBox.Items[i]).IsSelected = indices.Contains(i);
                }

                NumbersListBox.Focus();
            }
        }

        /// <summary>
        /// Find the collection of items of numeric array that are greater than all next items.
        /// </summary>
        /// <param name="numbers">Input array of numbers</param>
        /// <param name="areNumbersRequired">If true, then result contains the list of selected numbers, otherwise---only their indices</param>
        /// <returns>Pair, which contains: 1) the list of found numbers; 2) </returns>
        private static (SortedSet<int> Indices, List<double>? Numbers) SelectBigNumbers(IReadOnlyList<double> numbers,
            bool areNumbersRequired = true)
        {
            var indices = new SortedSet<int>();
            var max = double.NegativeInfinity;
            for (int i = numbers.Count - 1; i >= 0; i--)
            {
                if (numbers[i] <= max) continue;
                //list.Insert(0, numbers[i]);
                //list.Add(numbers[i]);
                max = numbers[i];
                indices.Add(i);
            }

            indices.Reverse();
            List<double>? bigNumbers = null;
            if (areNumbersRequired)
            {
                bigNumbers = new();
                foreach (var index in indices)
                {
                    bigNumbers.Add(numbers[index]);
                }
            }

            return (indices, bigNumbers);
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            NumbersListBox.Items.Clear();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            for (int i = NumbersListBox.Items.Count - 1; i >= 0; i--)
            {
                if (NumbersListBox.Items[i] is ListBoxItem listBoxItem && listBoxItem.IsSelected)
                {
                    NumbersListBox.Items.RemoveAt(i);
                }
            }
            //if (NumbersListBox.SelectedIndex >= 0)
            //    NumbersListBox.Items.RemoveAt(NumbersListBox.SelectedIndex);
        }

        private void NumbersListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            DeleteButton.IsEnabled = NumbersListBox.SelectedIndex >= 0;
        }

        private void window_Loaded(object? sender, RoutedEventArgs e)
        {
            ((INotifyCollectionChanged)NumbersListBox.Items).CollectionChanged += Handler;
        }
    }
}