using HostsPro.DataAccessService;
using HostsPro.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HostsPro.BussinessServices
{
    internal class FileManager
    {
        private FileDataAccess fileDataAccess = new FileDataAccess();


        public List<HostEntryModel> ReadFile()
        {
            List<HostEntryModel> list = new List<HostEntryModel>();
            list = fileDataAccess.GetFile();
            return list;
            

        }

        public void SaveEntries(ObservableCollection<HostEntryModel> entries)
        {

        }

        //public List<HostEntryModel> GetAllEntriesHardCoded()
        //{
        //    //return FileDataAccess.GetAllEntries();
        //    List<string> lines = fileDataAccess.GetAllLines();
        //    List<EntryModel> entries = new List<EntryModel>();

        //    var entry1 = new EntryModel();
        //    entry1.IPEntry = new IpEntryModel
        //    {
        //        IpAddress = IPAddress.Parse("1.1.1.1"),
        //        ForwardTo = "mag.az1",
        //        DNS = "localhost",
        //        Comment = "Comemnt goes here",
        //        IsActive = false
        //    };
        //    var entry2 = new EntryModel();
        //    entry1.IPEntry = new IpEntryModel
        //    {
        //        IpAddress = IPAddress.Parse("1.1.1.1"),
        //        ForwardTo = "mag.az1",
        //        DNS = "localhost",
        //        Comment = "Comemnt goes here",
        //        IsActive = false
        //    };
        //    var entry3 = new EntryModel();
        //    entry1.IPEntry = new IpEntryModel
        //    {
        //        IpAddress = IPAddress.Parse("1.1.1.1"),
        //        ForwardTo = "mag.az1",
        //        DNS = "localhost",
        //        Comment = "Comemnt goes here",
        //        IsActive = false

        //    };
        //    entries.Add(entry1);
        //    entries.Add(entry2);
        //    entries.Add(entry3);
        //    return entries;
        //}
    }
}
