﻿namespace TXM.Vm.Base
    {
    /// <summary>
    /// Commande simple pour les ViewModels (MVVM).
    /// </summary>
    public class RelayCommand : ICommand
        {
        private readonly Action<object?> _execute;
        private readonly Predicate<object?>? _canExecute;

        public event EventHandler? CanExecuteChanged;

        public RelayCommand(Action<object?> execute, Predicate<object?>? canExecute = null)
            {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
            }

        public bool CanExecute(object? parameter) =>
            _canExecute?.Invoke(parameter) ?? true;

        public void Execute(object? parameter) =>
            _execute(parameter);

        /// <summary>
        /// Déclenche manuellement la vérification du CanExecute.
        /// </summary>
        public void RaiseCanExecuteChanged() =>
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
