using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
//using ShoppingListAPI.Models;
using static System.Net.Mime.MediaTypeNames;
using System.Diagnostics.Metrics;
using System;
using System.Data;
using System.Reflection;
using ShoppingListAPI;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Data.Common;

namespace ShoppingListAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductsController : ControllerBase
    {

        private readonly ILogger<ProductsController> _logger;

        public ProductsController(ILogger<ProductsController> logger)
        {
            _logger = logger;
        }
        [Route("GetProducts")]
        [HttpGet]
        public IEnumerable<Product> Get()
        {
            try
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
                builder.DataSource = "(localdb)\\MSSQLLocalDB";
                builder.InitialCatalog = "ShoppingListDB";
                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    connection.Open();
                    String sql = "select * from ShoppingListDB.dbo.Products";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            List<Product> products = DataReaderMapToList<Product>(reader);
                            connection.Close();
                            return products;
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
                return new List<Product>();
            }
        }
        [Route("AddProducts")]
        [HttpPost]
        public void Post(string category, string productName)
        {
            try
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
                builder.DataSource = "(localdb)\\MSSQLLocalDB";
                builder.InitialCatalog = "ShoppingListDB";
                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    bool alreadyExists = false;
                    int productQuantity = 0;

                    connection.Open();

                    String sqlCheckExists = "select * from ShoppingListDB.dbo.Products where productName = " + "'" + productName + "'";

                    using (SqlCommand command = new SqlCommand(sqlCheckExists, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            List<Product> products = DataReaderMapToList<Product>(reader);
                            if(products.Count > 0) 
                            { 
                                alreadyExists = true;
                                productQuantity = products.FirstOrDefault().ProductQuantity;
                            }

                        }
                    }
                    if (alreadyExists)
                    {
                        String sqlAddToQuantity = "UPDATE ShoppingListDB.dbo.Products SET productQuantity = " + (productQuantity + 1) + " where productName = '" + productName + "'";

                        using (SqlCommand command = new SqlCommand(sqlAddToQuantity, connection))
                        {
                            command.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        String sqlCreate = "INSERT INTO ShoppingListDB.dbo.Products (productName,productCategoryId,productQuantity) VALUES ('" + productName + "'," + category + ",1)";

                        using (SqlCommand command = new SqlCommand(sqlCreate, connection))
                        {
                            command.ExecuteNonQuery();
                        }
                    }
                    connection.Close();
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        public static List<T> DataReaderMapToList<T>(IDataReader dr)
        {
            List<T> list = new List<T>();
            T obj = default;
            while (dr.Read())
            {
                obj = Activator.CreateInstance<T>();
                foreach (PropertyInfo prop in obj.GetType().GetProperties())
                {
                    if (!object.Equals(dr[prop.Name], DBNull.Value))
                    {
                        prop.SetValue(obj, dr[prop.Name], null);
                    }
                }
                list.Add(obj);
            }
            return list;
        }
    }
}