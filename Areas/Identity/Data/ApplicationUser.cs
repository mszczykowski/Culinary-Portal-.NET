﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;


namespace PortalKulinarny.Areas.Identity.Data
{
    public class ApplicationUser : IdentityUser
    {
        [Column(TypeName = "DateTime2")]
        public DateTime DoJ { get; set; }
        [PersonalData]
        [MaxLength(20)]
        [Column(TypeName = "varchar(20)")]
        public string FirstName { get; set; }
        [PersonalData]
        [MaxLength(20)]
        [Column(TypeName = "varchar(20)")]
        public string LastName { get; set; }

    }
}