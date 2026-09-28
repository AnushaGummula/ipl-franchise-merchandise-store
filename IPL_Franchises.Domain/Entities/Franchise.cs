using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPL_Franchises.Domain.Entities;

public class Franchise
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;

    public string? LogoUrl { get; set; }

    public ICollection<Product> Products { get; set; }
        = new List<Product>();
}
