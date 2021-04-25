using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using static KeqingNiuza.Wish.Const;
using System.IO;

namespace KeqingNiuza.Wish
{
    public class WishEvent
    {
        public WishType WishType { get; set; }
        public string Name { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public List<string> UpStar5 { get; set; }
        public List<string> UpStar4 { get; set; }

    }
}