using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using FileProcessor.UseCase;
using FileProcessor.UseCase.PresentationModels;
using FileProcessorWpf.Infrastructure;
using FileProcessorWpf.Model;
using Microsoft.Win32;

namespace FileProcessorWpf.ViewModel;

public sealed class ProcessorViewModel : 
    INotifyPropertyChanged, 
    GetOperationDefinition.IPresenter,
    ProcessDataSet.IPresenter
{
    private readonly TaskScheduler _scheduler;
    private readonly ProcessDataSet _processDataSet;
    private readonly GetOperationDefinition _getOperationDefinition;
    
    public Command OpenFileCommand { get; }
    public Command AddDataSetColumnCommand { get; }
    public Command AddDataSetRowCommand { get; }
    public AsyncCommand RunProcessCommand { get; }
    public ObservableCollection<ObservableCollection<Datum>> DataSets { get; }
    public ObservableCollection<GeneratorPresentation> Generators { get; }
    
    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    
    private bool _isExecuting;
    private bool IsExecuting
    {
        get => _isExecuting;
        set
        {
            _isExecuting = value;
            OnPropertyChanged();
        }
    }

    private string? _selectedFile;
    public string? SelectedFile
    {
        get => _selectedFile;
        private set
        {
            if (value == _selectedFile) return;
            _selectedFile = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(StatusText));
        }
    }
    
    public string StatusText =>
        string.IsNullOrWhiteSpace(SelectedFile) 
            ? string.Empty 
            : $@"Current File: {SelectedFile}";

    private string _processResults = string.Empty;
    public string ProcessResults
    {
        get => _processResults;
        set
        {
            if (value == _processResults) return;
            _processResults = value;
            OnPropertyChanged();
        }
    }

    public ProcessorViewModel(
        GetOperationDefinition getOperationDefinition,
        ProcessDataSet processDataSet)
    {
        _scheduler = TaskScheduler.FromCurrentSynchronizationContext();
        
        OpenFileCommand = new Command(_ => OpenFile());
        AddDataSetRowCommand = new Command(_ => AddRow());
        AddDataSetColumnCommand = new Command(
            _ => AddDataSetColumn(), 
            _ => (DataSets?.Count ?? 0) > 0);
        

        RunProcessCommand = new AsyncCommand(
              async () => await Generate(),
            _ => (Generators?.Count ?? 0) > 0
                 && (DataSets?.Count ?? 0) > 0
                 && !IsExecuting);
        
        DataSets = new ObservableCollection<ObservableCollection<Datum>>();
        Generators = new ObservableCollection<GeneratorPresentation>();
        Generators.CollectionChanged += (_, _) => RunProcessCommand.RaiseCanExecuteChanged();
        
        _getOperationDefinition = getOperationDefinition;
        _processDataSet = processDataSet;
    }


    private async void OpenFile()
    {
        var dialog = new OpenFileDialog
        {
            Filter = "Json (*.json) |*.json"
        };
        
        if (dialog.ShowDialog() ?? false)
        {
            SelectedFile = dialog.FileName;
            DataSets.Clear();
            Generators.Clear();
            await LoadOperationDefinition();
        }
    }

    private async Task LoadOperationDefinition()
    {
        if (SelectedFile != null)
        {
            await _getOperationDefinition.Execute(this, SelectedFile);
            NormalizeDataSet();
        }
    }
    
    private void AddRow()
    {
        DataSets.Add(new ObservableCollection<Datum>());
        NormalizeDataSet();
    }
    
    private void NormalizeDataSet()
    {
        var maxDataSetLength = Math.Max(DataSets.Max(c => c.Count), 1);
        foreach (var dataSet in DataSets)
        {
            for (var i = dataSet.Count; i < maxDataSetLength; i++)
            {
                dataSet.Add(Datum.CreateEmpty());
            }
        }
        AddDataSetColumnCommand.RaiseCanExecuteChanged();
        RunProcessCommand.RaiseCanExecuteChanged();
    }
        
    private void AddDataSetColumn()
    {
        foreach (var dataSet in DataSets)
            dataSet.Add(Datum.CreateEmpty());
    }

    private async Task Generate()
    {
        await GeneratorsBegin();
        await _processDataSet.Execute(this, Generators.ToList(), TrimToList(DataSets))
            .ContinueWith(async _ =>  await GeneratorsCompleted());
    }
    
    private async Task GeneratorsBegin()
    {
        await Task.Factory.StartNew(
            () =>
            {
                ProcessResults = $"Process Starting: {DateTime.Now}\r\n";
                IsExecuting = true;
                RunProcessCommand.RaiseCanExecuteChanged();
            },
            new CancellationToken(),
            TaskCreationOptions.None,
            _scheduler);
    }
    
    private static IList<IList<double>> TrimToList(ObservableCollection<ObservableCollection<Datum>> dataSets)
    {
        var result = new List<IList<double>>();
        foreach (var dataSet in dataSets)
        {
            var record = new List<double>();
            result.Add(record);
            foreach(double? datum in dataSet)
            {
                if(datum.HasValue)
                    record.Add(datum.Value);
            }
        }

        return result;
    }

    private async Task GeneratorsCompleted()
    {
        await Task.Factory.StartNew(
            () =>
            {
                IsExecuting = false;
                ProcessResults += ($"Process Ended: {DateTime.Now}");
                RunProcessCommand.RaiseCanExecuteChanged();
            },
            new CancellationToken(),
            TaskCreationOptions.None,
            _scheduler);
    }

    public void Present(IList<double> dataSet)
    {
        DataSets.Add(new ObservableCollection<Datum>(dataSet.Select(d=>(Datum)d)));
    }

    public void Present(GeneratorPresentation generator)
    {
        Generators.Add(generator);
    }

    public void Present(PresentableResult result)
    {
        Task.Factory.StartNew(
            () => ProcessResults += $"{result.Timestamp:HH:mm:ss}\t\t{result.OperationName}\t\t{result.Value:0.##}\r\n",
            new CancellationToken(),
            TaskCreationOptions.None,
            _scheduler);
    }
}