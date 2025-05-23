﻿using System.ComponentModel.DataAnnotations;

namespace JuanApp.Areas.Manage.Attributes
{
    public class AllowedLength:ValidationAttribute
    {

        private readonly int _length;

        public AllowedLength(int lenght)
        {
            _length = lenght;
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            List<IFormFile> _files = new List<IFormFile>();
            if (value is List<IFormFile> files)
                _files = files;
            if (value is IFormFile file)
                _files.Add(file);
            foreach (var _file in _files)
            {
                if (_file.Length > _length)
                    return new ValidationResult("File Lenght is so big");
            }

            return ValidationResult.Success;
        }
    }
}
