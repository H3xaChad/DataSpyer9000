using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairDataGetter.Client
{
    public static class NavigationState
    {
        // Global state to track whether the customer is corporate or not
        public static bool IsCorporateCustomer { get; set; }
    }
}
