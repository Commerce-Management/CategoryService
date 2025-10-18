namespace CategoryService.Application.Validators;

public class RegexPatterns
{
    public const string categoryNamePattern = @"^[A-Za-z][A-Za-z0-9\s\-’']{1,98}[A-Za-z0-9]$";
}