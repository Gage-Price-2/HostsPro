using System.Collections;
using System.ComponentModel;
using System.Net;

namespace HostsPro.Models
{
    //public class HostEntryModel
    //{
    //    public IPEntryModel? IpEntry {  get; set; }
    //    public string? CommentBlock { get; set; }
    //    public bool IsCommentBlock { get; set; }
    //}
    public class HostEntryModel : INotifyDataErrorInfo
    {
        public IPEntryModel? IpEntry { get; set; }
        public string? CommentBlock { get; set; }
        public bool IsCommentBlock { get; set; }

        private readonly Dictionary<string, List<string>> _errors = new();

        public bool HasErrors => _errors.Any();

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public void AddError(string propertyName, string error)
        {
            if (!_errors.ContainsKey(propertyName))
            {
                _errors[propertyName] = new List<string>();
            }
            _errors[propertyName].Add(error);

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
            // Clear previous errors
            if (IsCommentBlock)
            {
                ClearErrors(nameof(CommentBlock));

                if (string.IsNullOrWhiteSpace(CommentBlock))
                {
                    AddError(nameof(CommentBlock), "Comment Block is required.");
                }
            }
            else
            {
                IpEntry?.Validate(); // Validate IPEntryModel
                if (IpEntry?.HasErrors == true)
                {
                    foreach (var error in IpEntry.GetErrors(null))
                    {
                        AddError(nameof(IpEntry), error?.ToString() ?? "Unknown error");
                    }
                }
            }
        }

        public IEnumerable GetErrors(string? propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
            {
                return _errors.SelectMany(e => e.Value);
            }

            return _errors.ContainsKey(propertyName) ? _errors[propertyName] : Enumerable.Empty<string>();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }


}
