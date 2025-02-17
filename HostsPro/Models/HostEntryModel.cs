using System.Collections;
using System.ComponentModel;
using System.Net;

namespace HostsPro.Models
{
    //Implements interface to validate the model and show errors in the view
    public class HostEntryModel : INotifyDataErrorInfo
    {
        public IPEntryModel? IpEntry { get; set; }
        public string? CommentBlock { get; set; }
        public bool IsCommentBlock { get; set; }

        private readonly Dictionary<string, List<string>> _errors = new();
        //Used to check errors
        public bool HasErrors => _errors.Any();

        //Event bound to the view
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        /// <summary>
        /// Method to add an error to a property
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="error"></param>
        public void AddError(string propertyName, string error)
        {
            if (!_errors.ContainsKey(propertyName))
            {
                _errors[propertyName] = new List<string>();
            }
            //Add error to _erros variable
            _errors[propertyName].Add(error);

            //Shows error in view
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Method to clear errors before re-validating
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
        /// Method to validate the model adn adds errors if fails
        /// Checks both comment block or IP entry fields
        /// </summary>
        public void Validate()
        {
            if (IsCommentBlock)
            {
                // Clear previous errors
                ClearErrors(nameof(CommentBlock));
                //Validate comment block has data
                if (string.IsNullOrWhiteSpace(CommentBlock))
                {
                    AddError(nameof(CommentBlock), "Comment Block is required.");
                }
            }
            else
            {
                // Validate IPEntryModel
                IpEntry?.Validate(); 
                if (IpEntry?.HasErrors == true)
                {
                    foreach (var error in IpEntry.GetErrors(null))
                    {
                        //Adds the error for IPEntry - will use its own unique validation message from other validatie method
                        AddError(nameof(IpEntry), error?.ToString() ?? "Unknown error");
                    }
                }
            }
        }

        /// <summary>
        /// Method to get errors.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public IEnumerable GetErrors(string? propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
            {
                return _errors.SelectMany(e => e.Value);
            }

            return _errors.ContainsKey(propertyName) ? _errors[propertyName] : Enumerable.Empty<string>();
        }
    }


}
