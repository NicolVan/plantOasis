using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;

namespace PlantOasis.lib.Models
{
        public class MainNavigationModel
        {
            public MembershipUser User { get; set; }
            public _EshopModel Eshop { get; set; }
        }
    }

