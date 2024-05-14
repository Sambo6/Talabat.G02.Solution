﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order_Aggregate;

namespace Talabat.Repository.Data
{
    public class StoreContextSeeding
    {
        public async static Task SeedAsync(StoreContext _dbContext)
        {
            if (_dbContext.ProductBrands.Any())
            {
                var brandsData = File.ReadAllText("../Talabat.Repository/Data/DataSeeding/brands.json");
                var brands = JsonSerializer.Deserialize<List<ProductBrand>>(brandsData);

                if (brands is not null && brands.Count > 0)
                {
                    #region If I diden't need to change JsonFile

                    //brands = brands.Select(b => new ProductBrand()
                    //{
                    //    Name = b.Name
                    //}).ToList(); 
                    #endregion

                    foreach (var brand in brands)
                    {
                        _dbContext.Set<ProductBrand>().Add(brand);
                    }
                    await _dbContext.SaveChangesAsync();
                }
            }

            if (_dbContext.ProductCategories.Any())
            {
                var categoriesData = File.ReadAllText("../Talabat.Repository/Data/DataSeeding/categories.json");
                var categories = JsonSerializer.Deserialize<List<ProductCategory>>(categoriesData);

                if (categories is not null && categories.Count > 0)
                {
                    #region If I diden't need to change JsonFile

                    //brands = brands.Select(b => new ProductBrand()
                    //{
                    //    Name = b.Name
                    //}).ToList(); 
                    #endregion

                    foreach (var category in categories)
                    {
                        _dbContext.Set<ProductCategory>().Add(category);
                    }
                    await _dbContext.SaveChangesAsync();
                }
            }

            if (_dbContext.Products.Any())
            {
                var productsData = File.ReadAllText("../Talabat.Repository/Data/DataSeeding/products.json");
                var products = JsonSerializer.Deserialize<List<Product>>(productsData);

                if (products is not null && products.Count > 0)
                {
                    #region If I diden't need to change JsonFile

                    //brands = brands.Select(b => new ProductBrand()
                    //{
                    //    Name = b.Name
                    //}).ToList(); 
                    #endregion

                    foreach (var product in products)
                    {
                        _dbContext.Set<Product>().Add(product);
                    }
                    await _dbContext.SaveChangesAsync();
                }

            }

			if (_dbContext.Products.Any())
			{
				var deliveryMethodsData = File.ReadAllText("../Talabat.Repository/Data/DataSeeding/delivery.json");
				var deliveryMethods = JsonSerializer.Deserialize<List<DeliveryMethod>>(deliveryMethodsData);

				if (deliveryMethods is not null && deliveryMethods.Count > 0)
				{
					#region If I diden't need to change JsonFile

					//brands = brands.Select(b => new ProductBrand()
					//{
					//    Name = b.Name
					//}).ToList(); 
					#endregion

					foreach (var deliveryMethod in deliveryMethods)
					{
						_dbContext.Set<DeliveryMethod>().Add(deliveryMethod);
					}
					await _dbContext.SaveChangesAsync();
				}

			}




		}
	}
}
