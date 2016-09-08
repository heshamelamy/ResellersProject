﻿using HubManPractices.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HubManPractices.Service.ViewModels
{
    public class ClientSubscriptionViewModel
    {
        public Guid ClientID { get; set; }
        public Guid SubscriptionID { get; set; }
        public int UsersPerSubscription { get; set; }
        public bool IsDeleted { get; set; }
        public virtual OfficeSubscription OfficeSubscription { get; set; }
        public virtual Client Client { get; set; }
    }
}
