//using CommunityToolkit.Mvvm.Input;
using HostsPro.BussinessServices;
using HostsPro.Commands;
using HostsPro.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace HostsPro.ViewModels
{
    public class EntryViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<HostEntryModel> Entries { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand DeleteEntryCommand { get; set; }
        public ICommand AddEntryCommand { get; set; }

        private readonly FileManager _hostsFileService;
        private readonly IpLookupManager _dnsLookupService;
        private string _statusMessage;
        public string StatusMessage
        {
            get => _statusMessage;
            set { _statusMessage = value; OnPropertyChanged(); }
        }
        public EntryViewModel()
        {
            _hostsFileService = new FileManager();
            _dnsLookupService = new IpLookupManager();
            Entries = new ObservableCollection<HostEntryModel>(_hostsFileService.ReadFile());
            AddEntryCommand = new RelayCommand(obj => AddEntry(obj as string));
            SaveCommand = new RelayCommand(SaveEntries);
            DeleteEntryCommand = new RelayCommand(obj => DeleteEntry(obj as HostEntryModel));


        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void SaveEntries()
        {
            bool isValid = true;
            foreach (var entry in Entries)
            {
                entry.Validate();
                if (entry.HasErrors)
                {
                    isValid = false;
                }
            }

            if (isValid)
            {
                _hostsFileService.SaveEntries(Entries);
                //MessageBox.Show("Save successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                StatusMessage = "Save successful!";
            }
            else
            {

            }
        }

        private void DeleteEntry(HostEntryModel entry)
        {
            Entries.Remove(entry);
        }
        private void AddEntry(string entryType)
        {
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

        public async void RoutesTo_LostFocus(HostEntryModel model)
        {
            if (model != null)
            {
                await LookupIpAddressAsync(model);
            }
        }
        public async Task LookupIpAddressAsync(HostEntryModel entry)
        {
            if (entry.IpEntry != null && !string.IsNullOrEmpty(entry.IpEntry.RoutesTo))
            {
                
                string resolvedIp = await _dnsLookupService.ResolveDnsWithTimeoutAsync(entry.IpEntry.RoutesTo, TimeSpan.FromSeconds(20));
                if (!string.IsNullOrEmpty(resolvedIp))
                {
                   entry.IpEntry.IpAddress = resolvedIp;
                }
                else
                {
                    entry.IpEntry.IpAddress = "Error: No IP found";
                }
                //OnPropertyChanged(nameof(Entries));
                OnPropertyChanged(nameof(entry.IpEntry.IpAddress));
            }
        }
    }
}
