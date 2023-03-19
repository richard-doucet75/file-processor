namespace FileProcessorWpf.Model;

public record Datum(double? Value)
{
    private bool HasValue => Value.HasValue;

    public static Datum CreateEmpty()
    {
        return new Datum((double?)null);
    } 

    public static implicit operator double?(Datum datum)
    {
        return datum.HasValue ? datum.Value : null;
    }
    
    public static implicit operator Datum(double datum)
    {
        return new Datum((double?)datum);
    }

    public static implicit operator Datum(double? datum)
    {
        return new Datum(datum);
    }
}