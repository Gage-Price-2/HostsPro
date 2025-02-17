using System.Collections;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HostsPro.Models
{
    
    public class IPEntryModel : INotifyDataErrorInfo, INotifyPropertyChanged
    {
        private readonly Dictionary<string, List<string>> _errors = new();

        private string _ipAddress;

        //IpAddress property definition to auto-update value in view when changed by DNSLookup
        public string IpAddress
        {
            get => _ipAddress;
            set
            {
                if (_ipAddress != value)
                {
                    _ipAddress = value;
                    OnPropertyChanged(nameof(IpAddress));
                }
            }
        }
        //Other properties don't need to be updated from model class
        public string DNS { get; set; }
        public string RoutesTo { get; set; }
        public bool IsActive { get; set; }
        public string Comment { get; set; } 


        public event PropertyChangedEventHandler PropertyChanged;

        //Method to auto update view with model value
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        //Checks if there are any erros
        public bool HasErrors => _errors.Any();

        //Event to auto update view with errors
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        /// <summary>
        /// Method to add error
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="error"></param>
        public void AddError(string propertyName, string error)
        {
            if (!_errors.ContainsKey(propertyName))
            {
                _errors[propertyName] = new List<string>();
            }
            _errors[propertyName].Add(error);

            // Notify that the errors for the property have changed
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Clears errors before validating again
        /// </summary>
        /// <param name="propertyName"></param>
        public void ClearErrors(string propertyName)
        {
            if (_errors.ContainsKey(propertyName))
            {
                _errors.Remove(propertyName);
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Method to validate model(IPAdress and DNS)
        /// </summary>
        public void Validate()
        {
            // Clear errors and then add new errors if no value is present
            ClearErrors(nameof(IpAddress));
            ClearErrors(nameof(DNS));
            if (string.IsNullOrWhiteSpace(IpAddress))
            {
                AddError(nameof(IpAddress), "IP Address is required.");
            }
            if (string.IsNullOrWhiteSpace(DNS))
            {
                AddError(nameof(DNS), "DNS Address is required.");
            }
        }

        /// <summary>
        /// Method to get all the model errors
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public IEnumerable GetErrors(string? propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                // Return all errors
                return _errors.Values.SelectMany(e => e);
            }

            return _errors.ContainsKey(propertyName) ? _errors[propertyName] : Enumerable.Empty<string>();
        }
    }

}

