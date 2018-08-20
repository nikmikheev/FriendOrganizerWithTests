using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace FriendOrganizer.UI.Wrapper
{
    public class ModelWrapper<T> : NotifyDataErrorBase
    {
        public ModelWrapper(T model)
        {
            Model = model;
        }

        public T Model { get; }

        protected virtual TValue GetValue<TValue>([CallerMemberName] string propertyName = null)
        {
            return (TValue)typeof(T).GetProperty(propertyName).GetValue(Model);
        }

        protected virtual void SetValue<TValue>(TValue value, [CallerMemberName] string propertyName = null)
        {
            typeof(T).GetProperty(propertyName).SetValue(Model, value);
            OnPropertyChanged(propertyName);
            ValidatePropertyInternal(propertyName, value);

        }

        protected void ValidatePropertyInternal(string propertyName, object currValue)
        {
            ClearErrors(propertyName);

            ValidateDateAnnotation(propertyName, currValue);

            ValidateCustomError(propertyName);
        }

        /// <summary>
        /// Validate Customs errors
        /// </summary>
        /// <param name="propertyName"></param>
        private void ValidateCustomError(string propertyName)
        {
            
            var errors = ValidateProperty(propertyName);
            if (errors != null)
            {
                foreach (var error in errors)
                {
                    AddError(propertyName, error);
                }
            }
        }

        /// <summary>
        /// Validate data annotation
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="currValue"></param>
        private void ValidateDateAnnotation(string propertyName, object currValue)
        {
            var results = new List<ValidationResult>();
            var cnx = new ValidationContext(Model) {MemberName = propertyName};

            Validator.TryValidateProperty(currValue, cnx, results);
            foreach (var result in results)
            {
                AddError(propertyName, result.ErrorMessage);
            }
        }

        protected virtual IEnumerable<string> ValidateProperty(string propertyName)
        {
            return null;
        }
    }
}
