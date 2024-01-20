using System;
using System.Collections.Generic;

namespace Noitcua.Models;

public partial class sala
{
    public int id { get; set; }

    public bool estado { get; set; }

    public string titulo { get; set; }

    public string descricao { get; set; }

    public int id_comprador { get; set; }

    public virtual ICollection<chat> chat { get; set; } = new List<chat>();

    public virtual comprador id_compradorNavigation { get; set; }

    public virtual ICollection<venda> venda { get; set; } = new List<venda>();
}