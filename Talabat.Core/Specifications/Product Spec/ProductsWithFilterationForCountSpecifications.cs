﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Core.Specifications.Product_Spec
{
    public class ProductsWithFiltrationForCountSpecifications : BaseSpecifications<Product>
    {
        public ProductsWithFiltrationForCountSpecifications(ProductSpecParams specParams) : base(P =>
             (string.IsNullOrEmpty(specParams.Search) || P.Name.ToLower().Contains(specParams.Search) &&
             (!specParams.BrandId.HasValue || P.BrandId == specParams.BrandId.Value) &&
             (!specParams.CategoryId.HasValue || P.CategoryId == specParams.CategoryId.Value)
        ))
        {

        }
    }
}