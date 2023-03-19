using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FileProcessorWpf.Infrastructure;

public class AsyncCommand : ICommand
{
    public event EventHandler? CanExecuteChanged;

    private readonly Func<object?, bool>?  _canExecute;
    private readonly IErrorHandler? _errorHandler;
    private readonly Func<Task> _execute;

    public AsyncCommand(
        Func<Task> execute, 
        Func<object?, bool>? canExecute = null,
        IErrorHandler? errorHandler = null)
    {
        _execute = execute;
        _canExecute = canExecute;
        _errorHandler = errorHandler;
    }
    public bool CanExecute(object? parameter)
    {
        return  _canExecute?.Invoke(parameter) ?? true;
    }

    public void Execute(object? parameter)
    {
        Task.Run(async () =>
        {
            try
            {
                await _execute();
            }
            catch (Exception ex)
            {
                _errorHandler?.Handle(ex);
            }
        });
    }

    public void RaiseCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this,EventArgs.Empty);
    }
}