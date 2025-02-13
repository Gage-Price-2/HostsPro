using System.Collections;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HostsPro.Models
{
    //public class IPEntryModel
    //{

    //    public string IpAddress { get; set; } //= string.Empty;

    //    public string DNS { get; set; } //= string.Empty;

    //    public string RoutesTo { get; set; } //= string.Empty;
    //    public bool IsActive { get; set; } //= false;
    //    public string Comment { get; set; } //= string.Empty;
    //    
    //}
    public class IPEntryModel : INotifyDataErrorInfo, INotifyPropertyChanged
    {
        private readonly Dictionary<string, List<string>> _errors = new();

        private string _ipAddress;

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
        //public string IpAddress { get; set; }
        public string DNS { get; set; }
        public string RoutesTo { get; set; }
        public bool IsActive { get; set; } //= false;
        public string Comment { get; set; } //= string.Empty;


        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public bool HasErrors => _errors.Any();

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

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

        public void ClearErrors(string propertyName)
        {
            if (_errors.ContainsKey(propertyName))
            {
                _errors.Remove(propertyName);
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            }
        }

        public void Validate()
        {
            // Example validation logic
            ClearErrors(nameof(IpAddress)); // Clear previous errors
            if (string.IsNullOrWhiteSpace(IpAddress))
            {
                AddError(nameof(IpAddress), "IP Address is required.");
            }
            // Repeat for other fields...
        }

        public IEnumerable GetErrors(string? propertyName)
        {
            return _errors.ContainsKey(propertyName) ? _errors[propertyName] : Enumerable.Empty<string>();
        }
    }

}

