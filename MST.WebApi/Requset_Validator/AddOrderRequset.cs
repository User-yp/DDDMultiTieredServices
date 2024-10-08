﻿using ASPNETCore;
using FluentValidation;
using Validation;
using Validation.Attributes;

namespace MTS.WebApi.Requset_Validator;

[Validator(typeof(AddOrderRequsetValidator))]
public record AddOrderRequset(string OrderName, string ProductName, string ProductDescription): RequsetBase;

public class AddOrderRequsetValidator : AbstractValidator<AddOrderRequset>
{
    public AddOrderRequsetValidator()
    {
        RuleFor(e => e.OrderName).NotNull().NotEmpty().MaximumLength(11);
        RuleFor(e => e.ProductName).NotEmpty().NotEmpty().MaximumLength(20).MinimumLength(2);
        RuleFor(e => e.ProductDescription).NotEmpty().NotEmpty().MaximumLength(20).MinimumLength(2);
    }
}