﻿using System;
using System.Collections.Generic;
using Dapper.Database.Extensions;
using Xunit;
using System.Linq;
using System.Threading.Tasks;

namespace Dapper.Tests.Database
{
    public abstract partial class TestSuite
    {

        [ProviderFact]
        [Trait("Category", "GetPageListAsync")]
        public async Task GetPageListNoOrderAsync()
        {
            using (var connection = GetSqlDatabase())
            {
                var lst =await connection.GetPageListAsync<Product>(12, 10);
                Assert.Equal(10, lst.Count());
                var item = lst.Single(p => p.ProductID == 816);
                ValidateProduct816(item);
            }
        }


        [ProviderFact]
        [Trait("Category", "GetPageListAsync")]
        public async Task GetPageListWithWhereClauseAsync()
        {
            using (var connection = GetSqlDatabase())
            {
                var lst = await connection.GetPageListAsync<Product>(4, 10, "where Color = 'Black'");
                Assert.Equal(10, lst.Count());
                var item = lst.Single(p => p.ProductID == 816);
                ValidateProduct816(item);
            }
        }


        [ProviderFact]
        [Trait("Category", "GetPageListAsync")]
        public async Task GetPageListWithWhereClauseParameterAsync()
        {
            using (var connection = GetSqlDatabase())
            {
                var lst = await connection.GetPageListAsync<Product>(4, 10, "where Color = @Color", new { Color = "Black" });
                Assert.Equal(10, lst.Count());
                var item = lst.Single(p => p.ProductID == 816);
                ValidateProduct816(item);
            }
        }

        [ProviderFact()]
        [Trait("Category", "GetPageListAsync")]
        public async Task GetPageListOrderAsync()
        {
            using (var connection = GetSqlDatabase())
            {
                var lst = await connection.GetPageListAsync<Product>(15, 10, "order by lower(Name)");
                Assert.Equal(10, lst.Count());
                var item = lst.Single(p => p.ProductID == 816);
                ValidateProduct816(item);
            }
        }


        [ProviderFact]
        [Trait("Category", "GetPageListAsync")]
        public async Task GetPageListWithWhereOrderClauseAsync()
        {
            using (var connection = GetSqlDatabase())
            {
                var lst = await connection.GetPageListAsync<Product>(5, 10, "where Color = 'Black' order by lower(Name)");
                Assert.Equal(10, lst.Count());
                var item = lst.Single(p => p.ProductID == 816);
                Assert.Equal(6, lst.ToList().IndexOf(item));
                ValidateProduct816(item);
            }
        }


        [ProviderFact]
        [Trait("Category", "GetPageListAsync")]
        public async Task GetPageListWithWhereOrderClauseParameterAsync()
        {
            using (var connection = GetSqlDatabase())
            {
                var lst = await connection.GetPageListAsync<Product>(5, 10, "where Color = @Color order by lower(Name)", new { Color = "Black" });
                Assert.Equal(10, lst.Count());
                var item = lst.Single(p => p.ProductID == 816);
                ValidateProduct816(item);
            }
        }

        [ProviderFact]
        [Trait("Category", "GetPageListAsync")]
        public async Task GetPageListWithSelectClauseAsync()
        {
            using (var connection = GetSqlDatabase())
            {
                var lst = await connection.GetPageListAsync<Product>(4,10, "select *, rowguid as GuidId from Product where Color = 'Black'");
                Assert.Equal(10, lst.Count());
                var item = lst.Single(p => p.ProductID == 816);
                ValidateProduct816(item);
            }
        }
        [ProviderFact]
        [Trait("Category", "GetPageListAsync")]
        public async Task GetPageListWithSelectOrderClauseAsync()
        {
            using (var connection = GetSqlDatabase())
            {
                var lst = await connection.GetPageListAsync<Product>(5, 10, "select *, rowguid as GuidId from Product where Color = 'Black' order by lower(Name)");
                Assert.Equal(10, lst.Count());
                var item = lst.Single(p => p.ProductID == 816);
                ValidateProduct816(item);
            }
        }

        [ProviderFact]
        [Trait("Category", "GetPageListAsync")]
        public async Task GetPageListWithSelectClauseParameterAsync()
        {
            using (var connection = GetSqlDatabase())
            {
                var lst = await connection.GetPageListAsync<Product>(4, 10, "select *, rowguid as GuidId from Product where Color = @Color", new { Color = "Black" });
                Assert.Equal(10, lst.Count());
                var item = lst.Single(p => p.ProductID == 816);
                ValidateProduct816(item);
            }
        }

        [ProviderFact]
        [Trait("Category", "GetPageListAsync")]
        public async Task GetPageListWithSelectClauseOrderParameterAsync()
        {
            using (var connection = GetSqlDatabase())
            {
                var lst = await connection.GetPageListAsync<Product>(5, 10, "select *, rowguid as GuidId from Product where Color = @Color order by lower(Name)", new { Color = "Black" });
                Assert.Equal(10, lst.Count());
                var item = lst.Single(p => p.ProductID == 816);
                ValidateProduct816(item);
            }
        }

        [ProviderFact]
        [Trait("Category", "GetPageListAsync")]
        public async Task GetPageListPartialBySelectAsync()
        {
            using (var connection = GetSqlDatabase())
            {
                var lst = await connection.GetPageListAsync<Product>(4,10,"select ProductId, rowguid AS GuidId, Name from Product where Color = @Color", new { Color = "Black" });
                Assert.Equal(10, lst.Count());
                var p = lst.Single(a => a.ProductID == 816);
                Assert.Equal(816, p.ProductID);
                Assert.Equal("ML Mountain Front Wheel", p.Name);
                Assert.Null(p.ProductNumber);
                Assert.Equal(new Guid("5E3E5033-9A77-4DCA-8B7F-DFED78EFA08A"), p.GuidId);
            }
        }

        [ProviderFact]
        [Trait("Category", "GetPageListAsync")]
        public async Task GetPageListOneJoinUnmappedAsync()
        {
            using (var connection = GetConnection())
            {
                var lst = await connection.GetPageListAsync<Product, ProductCategory>(4,10,
                    @"select  P.ProductID, P.Name, P.ProductNumber, P.Color, P.StandardCost, P.ListPrice, P.Size, 
                    P.Weight, P.ProductModelID, P.SellStartDate, P.SellEndDate, P.DiscontinuedDate, 
                    P.ThumbNailPhoto, P.ThumbnailPhotoFileName, P.rowguid, P.ModifiedDate, PC.ProductCategoryID, 
                    PC.ParentProductCategoryID
                    from Product P
                    join ProductCategory PC on PC.ProductCategoryID = P.ProductCategoryID
                    where Color = @Color", new { Color = "Black" });
                Assert.Equal(10, lst.Count());
                var item = lst.Single(p => p.ProductID == 816);

                item.ProductCategoryID = 21;
                item.GuidId = new Guid("5e3e5033-9a77-4dca-8b7f-dfed78efa08a");

                ValidateProduct816(item);
                Assert.NotNull(item.ProductCategory);
            }
        }

        [ProviderFact]
        [Trait("Category", "GetPageListAsync")]
        public async Task GetPageListOneJoinMappedAsync()
        {
            using (var connection = GetConnection())
            {
                var lst = await connection.GetPageListAsync<Product, ProductCategory, Product>(4, 10,
                    (pr, pc) =>
                    {
                        pr.ProductCategory = pc;
                        return pr;
                    },
                    @"select  P.ProductID, P.Name, P.ProductNumber, P.Color, P.StandardCost, P.ListPrice, P.Size, 
                    P.Weight, P.ProductModelID, P.SellStartDate, P.SellEndDate, P.DiscontinuedDate, 
                    P.ThumbNailPhoto, P.ThumbnailPhotoFileName, P.rowguid, P.ModifiedDate, PC.ProductCategoryID, 
                    PC.ParentProductCategoryID
                    from Product P
                    join ProductCategory PC on PC.ProductCategoryID = P.ProductCategoryID
                    where Color = @Color", new { Color = "Black" });
                Assert.Equal(10, lst.Count());
                var item = lst.Single(p => p.ProductID == 816);

                item.ProductCategoryID = 21;
                item.GuidId = new Guid("5e3e5033-9a77-4dca-8b7f-dfed78efa08a");

                ValidateProduct816(item);
                Assert.NotNull(item.ProductCategory);
            }
        }

    }
}
