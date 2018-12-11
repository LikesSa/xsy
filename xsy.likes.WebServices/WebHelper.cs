using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xsy.likes.WebServices
{
    public class WebHelper
    {
        public static IWeb Instrance => new WebServices();
    }
}
