using System;
using System.Windows.Input;

namespace FileProcessorWpf.Infrastructure;

public class Command : ICommand
{
    public event EventHandler? CanExecuteChanged;

    private readonly Func<object?, bool>?  _canExecute;
    private readonly Action<object?> _execute;

    public Command(Action<object?> execute, Func<object?, bool>? canExecute = null)
    {
        _execute = execute;
        _canExecute = canExecute;
    }
    public bool CanExecute(object? parameter)
    {
        return  _canExecute?.Invoke(parameter) ?? true;
    }

    public void Execute(object? parameter)
    {
        _execute(parameter);
    }

    public void RaiseCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this,EventArgs.Empty);
    }
}

public interface IErrorHandler
{
    void Handle(Exception exception);
}