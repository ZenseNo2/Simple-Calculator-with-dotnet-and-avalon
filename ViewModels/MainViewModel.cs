using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Avalonia.Controls;

namespace CalculatorApp.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private string _display = "0";
        private double _firstNumber = 0;
        private string _operation = "";
        private bool _shouldResetDisplay = false;

        public string Display
        {
            get => _display;
            set
            {
                if (_display != value)
                {
                    _display = value;
                    OnPropertyChanged();
                }
            }
        }

        public RelayCommand<string> NumberCommand { get; }
        public RelayCommand<string> OperatorCommand { get; }
        public RelayCommand EqualsCommand { get; }
        public RelayCommand ClearCommand { get; }
        public RelayCommand BackspaceCommand { get; }

        public MainViewModel()
        {
            NumberCommand = new RelayCommand<string>(OnNumberClick);
            OperatorCommand = new RelayCommand<string>(OnOperatorClick);
            EqualsCommand = new RelayCommand(OnEquals);
            ClearCommand = new RelayCommand(OnClear);
            BackspaceCommand = new RelayCommand(OnBackspace);
        }

        private void OnNumberClick(string number)
        {
            if (_shouldResetDisplay)
            {
                Display = number;
                _shouldResetDisplay = false;
            }
            else
            {
                if (Display == "0" && number != ".")
                    Display = number;
                else if (number == "." && Display.Contains("."))
                    return;
                else
                    Display += number;
            }
        }

        private void OnOperatorClick(string op)
        {
            if (!double.TryParse(Display, out double num))
                return;

            if (!string.IsNullOrEmpty(_operation) && !_shouldResetDisplay)
            {
                OnEquals();
            }

            _firstNumber = num;
            _operation = op;
            _shouldResetDisplay = true;
        }

        private void OnEquals()
        {
            if (string.IsNullOrEmpty(_operation) || !double.TryParse(Display, out double secondNumber))
                return;

            double result = _operation switch
            {
                "+" => _firstNumber + secondNumber,
                "-" => _firstNumber - secondNumber,
                "*" => _firstNumber * secondNumber,
                "/" => secondNumber != 0 ? _firstNumber / secondNumber : 0,
                _ => 0
            };

            Display = result.ToString();
            _operation = "";
            _shouldResetDisplay = true;
        }

        private void OnClear()
        {
            Display = "0";
            _firstNumber = 0;
            _operation = "";
            _shouldResetDisplay = false;
        }

        private void OnBackspace()
        {
            if (Display.Length > 1)
                Display = Display[..^1];
            else
                Display = "0";
        }
    }

    // Simple command implementation
    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _execute;

        public RelayCommand(Action<T> execute)
        {
            _execute = execute;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => true;

        public void Execute(object? parameter)
        {
            _execute((T)parameter!);
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action _execute;

        public RelayCommand(Action execute)
        {
            _execute = execute;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => true;

        public void Execute(object? parameter)
        {
            _execute();
        }
    }
}