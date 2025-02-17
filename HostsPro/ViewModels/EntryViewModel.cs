//using CommunityToolkit.Mvvm.Input;
using HostsPro.BussinessServices;
using HostsPro.Commands;
using HostsPro.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace HostsPro.ViewModels
{
    public class EntryViewModel : INotifyPropertyChanged
    {
        //Observable collection is tied to the view
        public ObservableCollection<HostEntryModel> Entries { get; set; }
        //Command to bound to the "Save" button in the view
        public ICommand SaveCommand { get; set; }
        //Command to bound to the "Delete" button in the view
        public ICommand DeleteEntryCommand { get; set; }
        //Command to bound to the "Add" button in the view
        public ICommand AddEntryCommand { get; set; }

        //Implementation if Property changed to bind chanegs to the view
        public event PropertyChangedEventHandler PropertyChanged;

        //Utility class definitions 
        private readonly FileManager _hostsFileService;
        private readonly IpLookupManager _dnsLookupService;

        /// <summary>
        /// Constructor to insitialize all variables
        /// </summary>
        public EntryViewModel()
        {
            _hostsFileService = new FileManager();
            _dnsLookupService = new IpLookupManager();
            Entries = new ObservableCollection<HostEntryModel>(_hostsFileService.ReadFile());
            AddEntryCommand = new RelayCommand(obj => AddEntry(obj as string));
            SaveCommand = new RelayCommand(SaveEntries);
            DeleteEntryCommand = new RelayCommand(obj => DeleteEntry(obj as HostEntryModel));
        }


        /// <summary>
        /// Interface implementation to invoke the event
        /// </summary>
        /// <param name="propertyName"></param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Method called by view to save the current collection of entries to the database
        /// </summary>
        private void SaveEntries()
        {
            //Validate all entries
            bool isValid = true;
            foreach (var entry in Entries)
            {
                entry.Validate();
                if (entry.HasErrors)
                {
                    isValid = false;
                }
            }
            //Only execute if no validation errors
            if (isValid)
            {
                bool success = _hostsFileService.SaveEntries(Entries);
                if (success)
                {
                    MessageBox.Show("Save successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("There was an error saving to the file. Please contact support if issue persists.", "Error", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                //Show error message if errors
                //Red outlines text blocks appear from errors
                MessageBox.Show("Save Unsuccessful! Hover over red markings to see error message", "Error", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        /// <summary>
        /// Method to remove an entry from the grid
        /// </summary>
        /// <param name="entry"></param>
        private void DeleteEntry(HostEntryModel entry)
        {
            Entries.Remove(entry);
        }
        /// <summary>
        /// Method to add a new entry
        /// </summary>
        /// <param name="entryType"></param>
        private void AddEntry(string entryType)
        {
            //Check what type of entry to addd based on param
            //Add to Entries object
            if (entryType == "IP")
            {
                Entries.Add(new HostEntryModel
                {
                    IpEntry = new IPEntryModel(),
                    IsCommentBlock = false
                });
            }
            else if (entryType == "Comment")
            {
                Entries.Add(new HostEntryModel
                {
                    CommentBlock = string.Empty,
                    IsCommentBlock = true
                });
            }
        }

        /// <summary>
        /// Async Method called by lost focus event in the view
        /// </summary>
        /// <param name="model"></param>
        public async void RoutesTo_LostFocus(HostEntryModel model)
        {
            if (model != null)
            {
                //Lookup the IPaddress if not null model
                await LookupIpAddressAsync(model);
            }
        }

        /// <summary>
        /// Async Method to get the IP address and make the changes visible using OnPropertyChanged
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public async Task LookupIpAddressAsync(HostEntryModel entry)
        {
            //Check if valid data
            if (entry.IpEntry != null && !string.IsNullOrEmpty(entry.IpEntry.RoutesTo))
            {
                //use the utility class to get IP with a 5 second timout
                string resolvedIp = await _dnsLookupService.ResolveDnsWithTimeoutAsync(entry.IpEntry.RoutesTo, TimeSpan.FromSeconds(5));
                if (!string.IsNullOrEmpty(resolvedIp) && resolvedIp != null)
                {
                   entry.IpEntry.IpAddress = resolvedIp;
                }
                else
                {
                    //Error message if no ip returned
                    entry.IpEntry.AddError("IpAddress", "No Ip Found");
                    entry.IpEntry.IpAddress = "Error: No IP found";
                }
                //call OnChange to update the view
                OnPropertyChanged(nameof(entry.IpEntry.IpAddress));
            }
        }
    }
}
