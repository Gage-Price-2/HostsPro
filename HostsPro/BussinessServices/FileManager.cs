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
            bool success = fileDataAccess.SaveFile(entries.ToList());
        }

        
    }
}
