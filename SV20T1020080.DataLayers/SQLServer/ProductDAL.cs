using Azure;
using Dapper;
using Microsoft.VisualBasic;
using SV20T1020080.DomainModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV20T1020080.DataLayers.SQLServer
{
    public class ProductDAL : _BaseDAL, IProductDAL
    {
        public ProductDAL(string connectionString) : base(connectionString)
        {
        }

        public int Add(Product data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"  insert into Products(ProductName,ProductDescription,SupplierID,CategoryID,Unit,Price,Photo,IsSelling)
                               values(@ProductName,@ProductDescription,@SupplierID,@CategoryID,@Unit,@Price,@Photo,@IsSelling);
                               select @@identity;
                           ";
                var parameters = new
                {
                    ProductName = data.ProductName??"",
                    ProductDescription = data.ProductDescription,
                    SupplierID = data.SupplierID ,
                    CategoryID = data.CategoryID,
                    Unit = data.Unit,
                    Price=data.Price,
                    Photo = data.Photo ?? "",
                    IsSelling = data.IsSelling


                };
                id = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return id;
        }

        public long AddAttribute(ProductAttribute data)
        {
            long id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"  insert into ProductAttributes(ProductID,AttributeName,AttributeValue,DisplayOrder)
                               values(@ProductID,@AttributeName,@AttributeValue,@DisplayOrder);
                               select @@identity";
                var parameters = new
                {
                    ProductID = data.ProductID,
                    AttributeName = data.AttributeName,
                    AttributeValue = data.AttributeValue,
                    DisplayOrder = data.DisplayOrder
                };
                id = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return id;
        }

        public long AddPhoto(ProductPhoto data)
        {
            
            long id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"  insert into ProductPhotos(ProductID,Photo,Description,DisplayOrder,IsHidden)
                               values(@ProductID,@Photo,@Description,@DisplayOrder,@IsHidden);
                               select @@identity";
                var parameters = new
                {
                    ProductID = data.ProductID,
                    Photo = data.Photo,
                    Description = data.Description,
                    DisplayOrder = data.DisplayOrder,
                    IsHidden = data.IsHidden
                };
                id = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return id;
        }

        public int Count(string searchValue = "", int categoryID = 0, int supplierID = 0, decimal minPrice = 0, decimal maxPrice = 0)
        {
            int count = 0;
            if (!string.IsNullOrEmpty(searchValue))
            {
                searchValue = "%" + searchValue + "%";

            }
            using (var connection = OpenConnection())
            {
                var sql = @"select count(*) from Products 
                            where (@searchValue = N'') or (ProductName like @searchValue)";
                var parameters = new
                {
                    searchValue = searchValue ?? "",
                };
                count = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return count;
        }

        public bool Delete(int productID)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"delete from Products where ProductID = @ProductID";
                var parameters = new
                {
                    ProductID = productID
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }

            return result;
        }

        public bool DeleteAttribute(long attributeID)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"delete from ProductAttributes where AttributeID = @AttributeID";
                var parameters = new
                {
                    AttributeID = attributeID
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }

            return result;
        }

        public bool DeletePhoto(long photoID)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"delete from ProductPhotos where PhotoID = @PhotoID";
                var parameters = new
                {
                    PhotoID = photoID
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }

            return result;
        }

        public Product? Get(int productID)
        {
            Product? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"select * from Products where ProductID = @ProductID";
                var parameters = new
                {
                    ProductID = productID

                };
                data = connection.QueryFirstOrDefault<Product>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();

            }

            return data;
        }

        public ProductAttribute? GetAttribute(long attributeID)
        {
            ProductAttribute? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"select * from ProductAttributes where attributeID = @attributeID";
                var parameters = new
                {
                    attributeID = attributeID

                };
                data = connection.QueryFirstOrDefault<ProductAttribute>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();

            }

            return data;
        }

        public ProductPhoto? GetPhoto(long photoID)
        {
            ProductPhoto? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"select * from ProductPhotos where PhotoID = @PhotoID";
                var parameters = new
                {
                    PhotoID = photoID

                };
                data = connection.QueryFirstOrDefault<ProductPhoto>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();

            }

            return data;
        }

        public bool InUsed(int productID)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists(select * from OrderDetails where ProductId = @ProductId)
                                select 1
                            else 
                                select 0";
                var parameters = new { ProductId = productID };
                result = connection.ExecuteScalar<bool>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return result;
        }

        public IList<Product> List(int page = 1, int pageSize = 0, string searchValue = "", int categoryID = 0, int supplierID = 0, decimal minPrice = 0, decimal maxPrice = 0)
        {
            List<Product> data;
            if (!string.IsNullOrEmpty(searchValue))
                searchValue = "%" + searchValue + "%";
            using (var connection = OpenConnection())
            {
                var sql = @"with cte as
                            (
                                select  *,
                                        row_number() over(order by ProductName) as RowNumber
                                from    Products
                                where   (@SearchValue = N'' or ProductName like @SearchValue)
                                    and (@CategoryID = 0 or CategoryID = @CategoryID)
                                    and (@SupplierID = 0 or SupplierId = @SupplierID)
                                    and (Price >= @MinPrice)
                                    and (@MaxPrice <= 0 or Price <= @MaxPrice)
                            )
                            select * from cte
                            where   (@PageSize = 0)
                                or (RowNumber between (@Page - 1)*@PageSize + 1 and @Page * @PageSize)";
                var parameters = new
                {
                    Page = page,
                    PageSize = pageSize,
                    SearchValue = searchValue,
                    CategoryID = categoryID,
                    SupplierID = supplierID,
                    MinPrice = minPrice,
                    MaxPrice = maxPrice
                };
                data = connection.Query<Product>(sql: sql, param: parameters, commandType: CommandType.Text).ToList();
                connection.Close();
            }
            if (data == null)
                data = new List<Product>();
            return data;
        }

        public IList<ProductAttribute> ListAttributes(int productID)
        {
            List<ProductAttribute> data;
            using (var connection = OpenConnection())
            {

                var sql = @"SELECT * FROM ProductAttributes WHERE ProductID = @ProductID ORDER BY DisplayOrder ASC";
                var parameters = new
                {
                    ProductID = productID

                };
                data = connection.Query<ProductAttribute>(sql: sql, param: parameters, commandType: CommandType.Text).ToList();
                connection.Close();
            }
            if (data == null)
                data = new List<ProductAttribute>();
            return data;
        }

        public IList<ProductPhoto> ListPhotos(int productID)
        {
            List<ProductPhoto> data;
            using (var connection = OpenConnection())
            {

                var sql = @"select * from ProductPhotos where ProductID=@ProductID ORDER BY DisplayOrder ASC";
                var parameters = new
                {
                    ProductID = productID

                };
                data = connection.Query<ProductPhoto>(sql: sql, param: parameters, commandType: CommandType.Text).ToList();
                connection.Close();
            }
            if (data == null)
                data = new List<ProductPhoto>();
            return data;
        }

        public bool Update(Product data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"
                               update Products 
                               set ProductName = @ProductName,
                                        ProductDescription = @ProductDescription,
                                        SupplierID = @SupplierID,
                                        CategoryID = @CategoryID,
                                        Unit = @Unit,
                                        Price=@Price,
                                        Photo=@Photo,
                                        IsSelling = @IsSelling
                                where ProductID = @ProductID
                               ";
                var parameter = new
                {
                    ProductID = data.ProductID,
                    ProductName = data.ProductName ?? "",
                    ProductDescription = data.ProductDescription ?? "",
                    SupplierID = data.SupplierID ,
                    CategoryID = data.CategoryID,
                    Unit = data.Unit,
                    Price = data.Price,
                    Photo=data.Photo??"",
                    IsSelling = data.IsSelling

                };
                result = connection.Execute(sql: sql, param: parameter, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public bool UpdateAttribute(ProductAttribute data)
        {
            
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"update ProductAttributes
                               set AttributeName = @AttributeName,
                                        AttributeValue = @AttributeValue,
                                        DisplayOrder = @DisplayOrder
                                where AttributeID = @AttributeID";
                var parameter = new
                {
                    AttributeID = data.AttributeID,
                    AttributeName = data.AttributeName ?? "",
                    AttributeValue = data.AttributeValue ?? "",
                    DisplayOrder = data.DisplayOrder

                };
                result = connection.Execute(sql: sql, param: parameter, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public bool UpdatePhoto(ProductPhoto data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"update ProductPhotos 
                               set Photo = @Photo,
                                        Description = @Description,
                                        DisplayOrder = @DisplayOrder,
                                        IsHidden = @IsHidden
                                where PhotoID = @PhotoID";
                var parameter = new
                {
                    PhotoID = data.PhotoID,
                    Photo = data.Photo ?? "",
                    Description = data.Description ?? "",
                    DisplayOrder = data.DisplayOrder,
                    IsHidden = data.IsHidden,

                };
                result = connection.Execute(sql: sql, param: parameter, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }
    }
}
