using System;
namespace UltraUnitTesterSpace.ResultTypesSpace;
public abstract record ResultType(String Message);
public record SuccessType(String SuccessMessage="Test Succeeded"):ResultType(SuccessMessage)
{
}
public record FailureType(String FailureMessage="Test Failed"):ResultType(FailureMessage)
{
}
public record ExceptionType(String ExceptionStringRepr="Test threw an exception"):ResultType(ExceptionStringRepr)
{
}
