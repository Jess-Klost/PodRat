using System;
using System.ComponentModel.DataAnnotations;

namespace RSSPod.Models;

public sealed class DirectoryNameAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        string? valueString = value as string; 
        if (valueString == null)
            return new("Name must be a string");    
        if (valueString.ContainsAny(System.IO.Path.GetInvalidFileNameChars()))
        {
            return new("Name cannot contain " + string.Join(" ", System.IO.Path.GetInvalidFileNameChars()) + ".");
        }
        return ValidationResult.Success;
    }
}