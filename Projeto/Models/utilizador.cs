﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Noitcua.Models;

public partial class utilizador
{
    public int id { get; set; }

    public string handle { get; set; }

    public DateOnly birth_date { get; set; }

    public string password { get; set; }

    public string email { get; set; }

    public string nationality { get; set; }

    public virtual ICollection<admin> admin { get; set; } = new List<admin>();

    public virtual ICollection<chat> chat { get; set; } = new List<chat>();

    public virtual ICollection<comprador> comprador { get; set; } = new List<comprador>();

    public virtual ICollection<denuncia> denunciaid_denunciadoNavigation { get; set; } = new List<denuncia>();

    public virtual ICollection<denuncia> denunciaid_denunciadorNavigation { get; set; } = new List<denuncia>();

    public virtual ICollection<vendedor> vendedor { get; set; } = new List<vendedor>();
}