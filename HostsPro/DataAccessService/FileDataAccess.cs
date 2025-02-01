using HostsPro.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HostsPro.DataAccessService
{
    public class FileDataAccess
    {
        private string pathToHosts = "C:\\Users\\Gage\\OneDrive - Grand Canyon University\\SeniorYear\\Example-Hosts-file-format.txt";
        private string testFile = "C:\\Users\\Gage\\OneDrive - Grand Canyon University\\SeniorYear\\TestFile.txt";
        private List<string> fileLines;

        public FileDataAccess()
        {
            fileLines = new List<string>();
        }

        public List<string> GetAllLines()
        {
            try
            {
                // Open the text file using a stream reader.
                using (StreamReader reader = new StreamReader(pathToHosts))
                {
                    while (!reader.EndOfStream)
                    {
                        try
                        {
                            string line = reader.ReadLine();
                            if (line != null)
                            {
                                fileLines.Add(line);

                            }
                        }
                        catch (IOException e) { }
                    }
                }
                return fileLines;
            }
            catch (IOException e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
                return new List<string>();
            }
        }

        public List<HostEntryModel> GetFile() 
        {
            List<HostEntryModel> entries = new List<HostEntryModel>();
            try
            {
                var lines = File.ReadAllLines(pathToHosts);
                bool isCommentBlock = false;
                string currentCommentBlock = string.Empty;

                foreach (var line in lines)
                {
                     if (line.StartsWith("###"))
                    {
                        // End of comment block
                        if (isCommentBlock)
                        {
                            entries.Add(new HostEntryModel { CommentBlock = currentCommentBlock, IsCommentBlock = true });
                            currentCommentBlock = string.Empty;
                            isCommentBlock = false;
                        }
                        //Start of block comment
                        else
                        {
                            //Set variables
                            currentCommentBlock = string.Empty;
                            isCommentBlock = true;
                            string trimmedLine = line.TrimStart('#') + Environment.NewLine;
                            currentCommentBlock = trimmedLine;

                        }
                    }
                    else if (line.StartsWith("##"))
                    {
                        // Handle comment block contents
                        //IsCommentBlock should be true here
                        currentCommentBlock += line.TrimStart('#') + Environment.NewLine;
                        
                    }
                    else if (line.StartsWith("#"))
                    {
                        // Handle inactive IP entry
                        var ipEntry = ParseIPEntry(line.TrimStart('#'), false);
                        if (ipEntry != null)
                        {
                            entries.Add(new HostEntryModel { IpEntry = ipEntry, IsCommentBlock = false });
                        }
                    }
                    else if (!string.IsNullOrWhiteSpace(line))
                    {
                        // Handle active IP entry
                        var ipEntry = ParseIPEntry(line, true);
                        if (ipEntry != null)
                        {
                            entries.Add(new HostEntryModel { IpEntry = ipEntry, IsCommentBlock = false });
                        }
                    }
                }

                // Add any remaining comment block
                if (!string.IsNullOrEmpty(currentCommentBlock))
                {
                    entries.Add(new HostEntryModel { CommentBlock = currentCommentBlock.Trim() });
                }
            }
            catch (Exception ex)
            {
                // Handle any parsing or file reading exceptions
                Console.WriteLine($"Error parsing hosts file: {ex.Message}");
            }

            return entries;
        }

        private IPEntryModel ParseIPEntry(string line, bool isActive)
        {
            // Split by spaces to isolate the IPAddress and RoutesTo
            var parts = line.Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 2)
                return null;

            string ipAddress = parts[0];
            string routesToAndComment = parts[1];

            // Split the remaining part by '#' to isolate the DNS and the comment
            var dnsAndComment = routesToAndComment.Split('#', 2);
            if (dnsAndComment.Length < 2)
                return null;

            string routesTo = dnsAndComment[0].Trim();
            string dnsAndCommentPart = dnsAndComment[1];

            // Split the DNS and comment by the first comma
            var dnsCommentSplit = dnsAndCommentPart.Split(new[] { ',' }, 2, StringSplitOptions.None);
            string dns = dnsCommentSplit[0].Trim();
            string comment = dnsCommentSplit.Length > 1 ? dnsCommentSplit[1].Trim() : string.Empty;

            return new IPEntryModel
            {
                IpAddress = ipAddress,
                RoutesTo = routesTo,
                DNS = dns,
                IsActive = isActive,
                Comment = comment
            };
        }

    }
}

