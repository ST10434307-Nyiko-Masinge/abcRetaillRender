#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCRetailFunctions.Models
{
    public class QueueLogViewModel
    {
        public string? MessageID { get; set; }
        public string? DateTimeOffset { get; set; }
        public string? MessageText { get; set; }
        public DateTimeOffset? InsertionTime { get; internal set; }
        public string MessagesText { get; internal set; }
    }
}
