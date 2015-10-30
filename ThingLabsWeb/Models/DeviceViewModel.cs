using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ThingLabsWeb.Models
{
    // Models returned by DeviceController actions.
    public class DeviceCreateViewModel
    {
        [Required]
        [Display(Name = "DeviceId")]
        public string DeviceId { get; set; }
    }

}