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
        //Create instance of FileDataAccess
        private FileDataAccess fileDataAccess = new FileDataAccess();

        /// <summary>
        /// Method to get the file contents as HostEntryModel
        /// </summary>
        /// <returns>collection of comments and ip entries</returns>
        public List<HostEntryModel> ReadFile()
        {
            //get model from access layer method
            var list = fileDataAccess.GetFile();
            return list;
            

        }

        /// <summary>
        /// Method to take in the collection as model, and return success/fail
        /// </summary>
        /// <param name="entries"></param>
        public bool SaveEntries(ObservableCollection<HostEntryModel> entries)
        {
             return fileDataAccess.SaveFile(entries.ToList());
        }

        
    }
}
