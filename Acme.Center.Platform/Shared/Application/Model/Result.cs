using Acme.Center.Platform.Shared.Domain.Model;

namespace Acme.Center.Platform.Shared.Application.Model;

/// <summary>
///     Generic Result class for Command Handlers in the Application Layer.
/// </summary>
/// <typeparam name="T">The type of the result value.</typeparam>
public class Result<T>
{
    protected Result(bool isSuccess, T value, Error error)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public T Value { get; }
    public Error Error { get; }

    public static Result<T> Success(T value)
    {
        return new Result<T>(true, value, Error.None);
    }

    public static Result<T> Failure(Error error)
    {
        return new Result<T>(false, default!, error);
    }

    public static Result<T> Failure(string code, string message)
    {
        return new Result<T>(false, default!, new Error(code, message));
    }
}

/// <summary>
///     Non-generic Result class for Command Handlers.
/// </summary>
public class Result : Result<object>
{
    private Result(bool isSuccess, Error error) : base(isSuccess, null!, error)
    {
    }

    public static Result Success()
    {
        return new Result(true, Error.None);
    }

    public new static Result Failure(Error error)
    {
        return new Result(false, error);
    }

    public new static Result Failure(string code, string message)
    {
        return new Result(false, new Error(code, message));
    }
}