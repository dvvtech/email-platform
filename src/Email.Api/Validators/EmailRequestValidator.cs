using Email.Models.MppTests;
using FluentValidation;

namespace Email.Api.Validators
{
    public class EmailRequestValidator : AbstractValidator<EmailRequest>
    {
        private readonly string[] _allowedImageExtensions = { ".jpg", ".jpeg", ".png", ".gif" };
        private readonly int _maxFileSize = 5 * 1024 * 1024; // 5 MB

        public EmailRequestValidator()
        {
            // Валидация Email
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format")
                .MaximumLength(100).WithMessage("Email cannot exceed 100 characters")
                .Must(email => email.Contains("@")).WithMessage("Email must contain @ symbol")
                .Must(email => !string.IsNullOrWhiteSpace(email)).WithMessage("Email cannot be empty or whitespace");

            // Валидация UserData
            RuleFor(x => x.UserData)
                .MaximumLength(5000).WithMessage("UserData cannot exceed 5000 characters")
                .When(x => !string.IsNullOrEmpty(x.UserData));

            // Валидация Results
            RuleFor(x => x.Results)
                .MaximumLength(5000).WithMessage("Results cannot exceed 5000 characters")
                .When(x => !string.IsNullOrEmpty(x.Results));

            // Валидация Image
            RuleFor(x => x.Image)
                .NotNull().WithMessage("Image is required")
                .Must(BeValidImage).WithMessage("Invalid image format. Allowed formats: JPG, JPEG, PNG, GIF")
                .Must(HaveValidSize).WithMessage($"Image size cannot exceed {_maxFileSize / (1024 * 1024)} MB");

            // Валидация Stats
            RuleFor(x => x.Stats)
                .MaximumLength(5000).WithMessage("Stats cannot exceed 5000 characters")
                .When(x => !string.IsNullOrEmpty(x.Stats));
        }

        private bool BeValidImage(IFormFile file)
        {
            if (file == null) return false;

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            return _allowedImageExtensions.Contains(extension);
        }

        private bool HaveValidSize(IFormFile file)
        {
            if (file == null) return false;
            return file.Length <= _maxFileSize;
        }
    }
}
