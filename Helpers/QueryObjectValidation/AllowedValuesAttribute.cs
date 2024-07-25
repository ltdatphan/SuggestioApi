using System.ComponentModel.DataAnnotations;

public class AllowedValuesAttribute : ValidationAttribute
{
    private readonly List<string> _allowedValues;

    public AllowedValuesAttribute(string[] allowedValues)
    {
        _allowedValues = allowedValues.ToList();
    }

    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            return ValidationResult.Success!; // Null or empty values are considered valid

        if (_allowedValues.Contains(value.ToString(), StringComparer.OrdinalIgnoreCase))
            return ValidationResult.Success!; // Value is in the allowed list

        // Value is not in the allowed list
        return new ValidationResult(GetErrorMessage(validationContext.DisplayName));
    }

    private string GetErrorMessage(string fieldName)
    {
        return $"Invalid value for {fieldName}. Valid values are: {string.Join(", ", _allowedValues)}";
    }
}