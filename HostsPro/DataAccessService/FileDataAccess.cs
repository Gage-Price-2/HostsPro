using HostsPro.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HostsPro.DataAccessService
{
    public class FileDataAccess
    {
        //private string pathToHosts = "C:\\Users\\Gage\\OneDrive - Grand Canyon University\\SeniorYear\\Example-Hosts-file-format.txt";
        //private string pathToHosts = "C:\\Users\\Price\\OneDrive - Grand Canyon University\\SeniorYear\\Example-Hosts-file-format.txt";
        //private string testFile = "C:\\Users\\Gage\\OneDrive - Grand Canyon University\\SeniorYear\\TestFile.txt";
        private string testFile = "C:\\Users\\Price\\HostsPro\\TempFile.txt";
        //private string testFile = "C:\\Windows\\System32\\drivers\\etc\\hosts";
        private List<string> fileLines;
        private const int MaxLineLength = 80;

        public FileDataAccess()
        {
            fileLines = new List<string>();
        }

        #region GET
        public List<string> GetAllLines()
        {
            try
            {
                // Open the text file using a stream reader.
                using (StreamReader reader = new StreamReader(testFile))
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
                var lines = File.ReadAllLines(testFile);
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
                            //string trimmedLine = line.TrimStart('#') + Environment.NewLine;
                            //currentCommentBlock = trimmedLine;

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
            if (string.IsNullOrWhiteSpace(line))
                return null;

            // Trim leading and trailing spaces
            line = line.Trim();

            // Check for a comment
            string comment = string.Empty;
            int commentIndex = line.IndexOf('#');
            if (commentIndex >= 0)
            {
                comment = line.Substring(commentIndex + 1).Trim();
                line = line.Substring(0, commentIndex).Trim();
            }

            // Split the remaining part by spaces
            var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            // Check if we have at least IP Address, DNS,
            if (parts.Length < 2)
                return null;

            string ipAddress = parts[0];
            string dns = parts[1];
            string routesTo = string.Empty;
            if (!string.IsNullOrEmpty(comment))
            {
                var commentParts = comment.Split('+', StringSplitOptions.RemoveEmptyEntries);
                if (commentParts.Length == 2)
                {
                    routesTo = commentParts[0].Trim();
                    comment = commentParts[1].Trim();
                }
                else
                {
                    routesTo = comment.Trim();
                    comment = null;
                }
            }
            return new IPEntryModel
            {
                IpAddress = ipAddress,
                DNS = dns,
                RoutesTo = routesTo,
                IsActive = isActive,
                Comment = comment
            };
        }

        #endregion

        #region PUT
        public bool SaveFile(List<HostEntryModel> list)
        {
            using StreamWriter writer = new StreamWriter(testFile);
            foreach (var entry in list)
            {
                if(entry.IsCommentBlock == true)
                {
                    //parse comment
                    WriteCommentBlock(writer, entry.CommentBlock);
                }
                else
                {
                    //parse IP
                    WriteHostEntry(writer, entry.IpEntry);
                }
            }
            return false;
        }

        private static void WriteCommentBlock(StreamWriter writer, string commentBlock)
        {
            
            var lines = commentBlock.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            if(commentBlock != string.Empty)
            {
                writer.WriteLine();
                writer.WriteLine("###");
                foreach (var line in lines)
                {
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        foreach (var wrappedLine in WrapText(line.Trim(), 80)) // Wrap long lines at 80 characters
                        {
                            writer.WriteLine("## " + wrappedLine);
                        }
                    }
                }

                writer.WriteLine("###");
            }
            
        }

        private static void WriteHostEntry(StreamWriter writer, IPEntryModel hostEntry)
        {
            writer.WriteLine();
            string entryPrefix = hostEntry.IsActive ? "" : "# ";
            string baseEntry = $"{entryPrefix}{hostEntry.IpAddress} {hostEntry.DNS}";

            // RoutesTo is always required
            string commentSection = $"# {hostEntry.RoutesTo}";

            // Append comment if it exists
            if (!string.IsNullOrWhiteSpace(hostEntry.Comment))
            {
                commentSection += $" + {hostEntry.Comment}";
            }

            // Write the formatted entry
            writer.WriteLine($"{baseEntry} {commentSection}");
        }

        private static List<string> WrapText(string text, int maxLength)
        {
            List<string> lines = new();
            while (text.Length > maxLength)
            {
                int splitIndex = text.LastIndexOf(' ', maxLength);
                if (splitIndex == -1 || splitIndex == 0) splitIndex = maxLength; // Avoid infinite loop

                lines.Add(text[..splitIndex].Trim());
                text = text[(splitIndex + 1)..].Trim();
            }

            if (!string.IsNullOrEmpty(text))
            {
                lines.Add(text);
            }

            return lines;
        }

        #endregion


    }
}

