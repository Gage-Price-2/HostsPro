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
        //Path to the file 
        private string testFile = "C:\\Users\\Price\\HostsPro\\TempFile.txt";
        private string tempFile = Path.GetTempFileName();
        //private string testFile = "C:\\Users\\Price\\OneDrive - Grand Canyon University\\SeniorYear\\CST-452\\UnitTestFile.txt";
        //private string testFile = "C:\\Windows\\System32\\drivers\\etc\\hosts";
        private List<string> fileLines;
        private const int MaxLineLength = 80;

        //Instantiate a new list of lines in the constructor
        public FileDataAccess()
        {
            fileLines = new List<string>();
        }

        #region GET
        /// <summary>
        /// Method to read the file and put it into a list of strings
        /// </summary>
        /// <returns></returns>
        public List<string> GetAllLines()
        {
            //Try catch block to catch a file not found exception
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
                //Log the error
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
                return new List<string>();
            }
        }

        /// <summary>
        /// method to return the list of Entry models back to the view model
        /// </summary>
        /// <returns></returns>
        public List<HostEntryModel> GetFile() 
        {
            List<HostEntryModel> entries = new List<HostEntryModel>();
            //Try catch to catch any errors
            try
            {
                var lines = File.ReadAllLines(testFile);
                bool isCommentBlock = false;
                string currentCommentBlock = string.Empty;

                foreach (var line in lines)
                {
                    //Either start of end of comment block entry
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
            //return the list
            return entries;
        }

        /// <summary>
        /// Method to parse an Ip entry line from the file
        /// </summary>
        /// <param name="line"></param>
        /// <param name="isActive"></param>
        /// <returns></returns>
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

            // Check if we have at least IP Address, and DNS
            if (parts.Length < 2)
                return null;

            string ipAddress = parts[0];
            string dns = parts[1];

            string routesTo = string.Empty;
            //Split find routesTo and comment part of the line and try to assign those variable with the value
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
            //Return populated model
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
        /// <summary>
        /// Method to save all entries to the file
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public bool SaveFile(List<HostEntryModel> list)
        {
            using StreamWriter writer = new StreamWriter(tempFile);
            bool success = true;
            foreach (var entry in list)
            {
                if(entry.IsCommentBlock == true)
                {
                    //parse comment and write it
                     success = WriteCommentBlock(writer, entry.CommentBlock);
                }
                else
                {
                    //parse IP and write it
                    success = WriteHostEntry(writer, entry.IpEntry);
                }
                if(success == false)
                {
                    return false;
                }
            }
            //close the writer
            writer.Close(); 

            //Replate the files
            File.Replace(tempFile, testFile, null);

            //Delete the temp file
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile); 
            }
            return true;
        }

        /// <summary>
        /// Method to write a comment block
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="commentBlock"></param>
        /// <returns></returns>
        private static bool WriteCommentBlock(StreamWriter writer, string commentBlock)
        {
            //Get all lines to write and check if empty
            var lines = commentBlock.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            if(commentBlock != string.Empty)
            {
                //Try catch to avoid errors
                try
                {
                    //Start comment and create padding between entries
                    writer.WriteLine();
                    writer.WriteLine("###");
                    foreach (var line in lines)
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            // Wrap long lines at 80 characters
                            foreach (var wrappedLine in WrapText(line.Trim(), 80)) 
                            {
                                writer.WriteLine("## " + wrappedLine);
                            }
                        }
                    }
                    //Write end of comment identifier
                    writer.WriteLine("###");
                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }
            }
            //Return true if null - wont cause an error
            return true;
            
        }

        /// <summary>
        /// Method to turn model into string structure and write to the file
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="hostEntry"></param>
        /// <returns></returns>
        private static bool WriteHostEntry(StreamWriter writer, IPEntryModel hostEntry)
        {
            try
            {
                //Create padding
                writer.WriteLine();
                string entryPrefix = hostEntry.IsActive ? "" : "# ";
                string baseEntry = $"{entryPrefix}{hostEntry.IpAddress} {hostEntry.DNS}";

                //Start comment section with routes to
                string commentSection = $"# {hostEntry.RoutesTo}";

                // Append comment if it exists
                if (!string.IsNullOrWhiteSpace(hostEntry.Comment))
                {
                    commentSection += $" + {hostEntry.Comment}";
                }

                // Write the formatted entry
                writer.WriteLine($"{baseEntry} {commentSection}");
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
            
        }

        /// <summary>
        /// Splits a string if it is over the maxLength into multiple lines
        /// </summary>
        /// <param name="text"></param>
        /// <param name="maxLength"></param>
        /// <returns></returns>
        private static List<string> WrapText(string text, int maxLength)
        {
            List<string> lines = new();
            //Itterate as many times until it is shorter than max length
            while (text.Length > maxLength)
            {
                //split at maxlength (80)
                int splitIndex = text.LastIndexOf(' ', maxLength);
                if (splitIndex == -1 || splitIndex == 0) splitIndex = maxLength; 

                //Add new line of the text up to the split index
                lines.Add(text[..splitIndex].Trim());
                //Reassign the variable to all text after the split index(max length)
                text = text[(splitIndex + 1)..].Trim();
            }

            if (!string.IsNullOrEmpty(text))
            {
                lines.Add(text);
            }
            //Return new array
            return lines;
        }


        #endregion


    }
}

