namespace ProjectFileConverter
{
    using System;
    using System.Windows.Input;

    public class RelayCommand : ICommand
    {
        private readonly Action execute;
        private readonly Func<bool> canExecute;

        public RelayCommand(Action execute)
            : this(execute, null)
        {
        }

        public RelayCommand(Action execute, Func<bool> canExecute)
        {
            this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
            this.canExecute = canExecute ?? (() => true);
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object parameter)
        {
            if (parameter != null)
            {
                throw new InvalidOperationException("Expected parameter to be null.");
            }

            return this.canExecute();
        }

        public void Execute(object parameter)
        {
            if (parameter != null)
            {
                throw new InvalidOperationException("Expected parameter to be null.");
            }

            this.execute();
        }
    }
}