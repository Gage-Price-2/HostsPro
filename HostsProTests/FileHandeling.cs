namespace HostsProTests;
using HostsPro;
using HostsPro.DataAccessService;
using HostsPro.Models;
using HostsPro.ViewModels;

[TestClass]
    public class FileHandeling
    {
        [TestMethod]
        public void WriteToFile()
        {
            var fileManager = new FileDataAccess();
            var testModel = new List<HostEntryModel> { new HostEntryModel
            {
                CommentBlock = null,
                IsCommentBlock = false,
                IpEntry = new IPEntryModel
                {
                    IpAddress = "127.0.0.1",
                    DNS = "example.com",
                    RoutesTo = "localhost",
                    Comment = "test comment",
                    IsActive = true,
                }
            } };

        }
    }
