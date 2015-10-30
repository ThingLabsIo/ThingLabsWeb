using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ThingLabsWeb.Models
{
    // Models returned by MeController actions.
    public class GetViewModel
    {
        public string IoTHubConnectionString { get; set; }
        public string IoTHubEndpoint { get; set; }
    }
}