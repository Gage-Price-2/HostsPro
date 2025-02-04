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

        public EntryViewModel()
        {
            _hostsFileService = new FileManager();
            _dnsLookupService = new IpLookupManager();
            Entries = new ObservableCollection<HostEntryModel>(_hostsFileService.ReadFile());

            //SaveCommand = new RelayCommand(SaveEntries);
            //DeleteEntryCommand = new RelayCommand(obj => DeleteEntry(obj as HostEntryModel));
            //AddEntryCommand = new RelayCommand(obj => AddEntry(obj as string)); 
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
            _hostsFileService.SaveEntries(Entries);
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

        public async void LookupIPAddress(HostEntryModel entry)
        {
            if (entry.IpEntry != null && !string.IsNullOrEmpty(entry.IpEntry.DNS))
            {
                string ipAddress = await _dnsLookupService.GetIPAddressAsync(entry.IpEntry.DNS);
                if (!string.IsNullOrEmpty(ipAddress))
                {
                   // entry.IpEntry.IpAddress = ipAddress;
                }
                else
                {
                    entry.IpEntry.IpAddress = "Error: No IP found";
                }
                OnPropertyChanged(nameof(Entries));
            }
        }
    }
}
