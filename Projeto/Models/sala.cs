﻿using System;
using System.Collections.Generic;

namespace Noitcua.Models;

public partial class sala
{
    public int id { get; set; }

    public int estado { get; set; }

    public string titulo { get; set; }

    public string descricao { get; set; }

    public int id_comprador { get; set; }
}