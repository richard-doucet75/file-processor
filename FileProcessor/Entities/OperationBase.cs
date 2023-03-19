namespace FileProcessor.Entities;

public abstract class OperationBase
{
    public abstract string OperationName
    {
        get;
    }
    public abstract double Execute(IList<double> dataSet);

    public static implicit operator OperationBase(string operation)
    {
        return operation switch
        {
            SumOperation.Name => Sum,
            AverageOperation.Name => Average,
            MinOperation.Name => Min,
            MaxOperation.Name => Max,
            _ => throw new NotImplementedException(operation)
        };
    }

    private static readonly OperationBase Sum = new SumOperation();
    private class SumOperation : OperationBase
    {
        public const string Name = "sum";
        public override string OperationName => Name;

        public override double Execute(IList<double> dataSet)
        {
            return dataSet.Sum();
        }
    }
    
    private static readonly OperationBase Average = new AverageOperation();
    private class AverageOperation : OperationBase
    {
        public const string Name = "average";
        public override string OperationName => Name;
        
        public override double Execute(IList<double> dataSet)
        {
            return dataSet.Average();
        }
    }
        
    private static readonly OperationBase Min = new MinOperation();
    private class MinOperation : OperationBase
    {
        public const string Name = "min";
        public override string OperationName => Name;
        
        public override double Execute(IList<double> dataSet)
        {
            return dataSet.Min();
        }
    }
            
    private static readonly OperationBase Max = new MaxOperation();
    private class MaxOperation : OperationBase
    {
        public const string Name = "max";
        public override string OperationName => Name;
        
        public override double Execute(IList<double> dataSet)
        {
            return dataSet.Max();
        }
    }
}