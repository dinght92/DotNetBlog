﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetBlog.Web.Areas.Api.Models.Tag
{
    public class DeleteTagModel
    {
        [Required]
        public int[] TagList { get; set; }
    }
}
