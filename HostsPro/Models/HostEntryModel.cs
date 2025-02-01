using System.Net;

namespace HostsPro.Models
{
    public class HostEntryModel
    {
        public IPEntryModel? IpEntry {  get; set; }
        public string? CommentBlock { get; set; }
        public bool IsCommentBlock { get; set; }
    }
}
