using System.Diagnostics.CodeAnalysis;

namespace Kohlhaas.Interfaces;

public record Result
{
    protected Result(bool isSuccess, Error error)
    {
        
        switch(isSuccess)
        {
            case true when error != Error.None:
            case false when error == Error.None:
                throw new InvalidOperationException();
            default:
                this.Error = error;
                this.IsSuccess = isSuccess;
                break;
        }
        
    }
    public bool IsSuccess { get; }
    public Error Error { get; }
    public static Result Success() => new(true, Error.None); 
    public static Result Failure(Error error) => new(false, error);
    
    public static Result<T> Success<T>(T value) => new(value, true, Error.None);

    public static Result<T> Failure<T>(Error error) => new(default, false, error);

    protected static Result<T> Create<T>(T? value) =>
        value is not null ? Success(value) : Failure<T>(Error.NullValue);

}

public record Result<T> : Result
{
    private readonly T? _value;
    
    protected internal Result(T? value, bool isSuccess, Error error) : base(isSuccess, error) => _value = value;
    
    [NotNull]
    public T Value => _value! ?? throw new InvalidOperationException("Result has no value");

    public static implicit operator Result<T>(T? value) => Create(value);
}