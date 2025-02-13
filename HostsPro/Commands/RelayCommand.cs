using System.Windows.Input;

namespace HostsPro.Commands
{
    public class RelayCommand : ICommand
    {
        private readonly Action _executeNoParam;
        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;

        // Constructor for parameterless methods
        public RelayCommand(Action execute, Predicate<object> canExecute = null)
        {
            _executeNoParam = execute;
            _canExecute = canExecute;
        }

        // Original constructor for methods with parameters
        public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => _canExecute == null || _canExecute(parameter);

        public void Execute(object parameter)
        {
            if (_execute != null)
                _execute(parameter);
            else
                _executeNoParam();
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}
//using System;
//using System.Windows.Input;

//namespace HostsPro.Commands
//{
//    public class RelayCommand<T> : ICommand
//    {
//        private readonly Action<T> _execute;
//        private readonly Predicate<T> _canExecute;

//        public RelayCommand(Action<T> execute, Predicate<T> canExecute = null)
//        {
//            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
//            _canExecute = canExecute;
//        }

//        public bool CanExecute(object parameter)
//        {
//            return _canExecute == null || (parameter is T param && _canExecute(param));
//        }

//        public void Execute(object parameter)
//        {
//            if (parameter is T param)
//                _execute(param);
//        }

//        public event EventHandler CanExecuteChanged
//        {
//            add { CommandManager.RequerySuggested += value; }
//            remove { CommandManager.RequerySuggested -= value; }
//        }
//    }
//}
