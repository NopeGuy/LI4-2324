﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Noitcua.Models;

public partial class admin
{
    public int id { get; set; }

    public int id_user { get; set; }

    public virtual utilizador id_userNavigation { get; set; }
}