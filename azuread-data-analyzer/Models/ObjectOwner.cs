using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Text;

namespace azuread_data_analyzer.Models
{
    public class ObjectOwner
    {
        public string ParentType { get; set; }
        public string ParentId { get; set; }
        public DirectoryObject Owner { get; set; }
    }
}
