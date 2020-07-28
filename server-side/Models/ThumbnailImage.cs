using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentExplorer.Models
{
    public class ThumbnailImage
    {
        public string Src { get; set; }
        public int PageNumber { get; set; }

        public ThumbnailImage(int pageNumber, string src)
        {
            PageNumber = pageNumber;
            Src = src;
        }
    }
}
