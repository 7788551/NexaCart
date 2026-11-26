using NexaCart.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;


namespace NexaCart.ApiController
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductApiController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public ProductApiController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult GetProducts()
        {
            try
            {
                List<Products> products = new List<Products>();

                string connectionString = _configuration.GetConnectionString("DefaultConnection");

                string query = @"SELECT ProductId,ProductName,
                                 Price,Quatity,CreatedDate FROM Products";

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        con.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                products.Add(new Products
                                {
                                    ProductId = Convert.ToInt32(reader["ProductId"]),
                                    ProductName =  reader["ProductName"].ToString(),
                                    Price = Convert.ToDecimal(reader["Price"]),
                                    Quantity = Convert.ToInt32(reader["Quantity"]),
                                    CreatedDate = Convert.ToDateTime(reader["CreatedDate"])
                                });
                            }
                        }

                    }
                }

                return Ok(products);
            }
            catch (SqlException)
            {
                return StatusCode(500, new
                {
                    message = "Database error occured!"
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    message = "Something went wrong!"
                });
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetProductById(int id)
        {
            try
            {

                Products products = null;

                string connectionString = 
                    _configuration.GetConnectionString("DefaultConnection");

                string query = @"SELECT ProductId,ProductName,Price,Quantity
                                 FROM Products
                                 WHERE ProductId = @ProductId";
                
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@ProductId", id);
                        con.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                products = new Products
                                {
                                    ProductId = Convert.ToInt32(reader["ProductId"]),
                                    ProductName = reader["ProductName"].ToString(),
                                    Price = Convert.ToDecimal(reader["Price"]),
                                    Quantity = Convert.ToInt32(reader["Quantity"])
                                };
                            }
                        }
                    }
                }
                if(products == null)
                {
                    return NotFound(new
                    {
                        message = "Product not found!"
                    });
                }

                return Ok(products);
            }
            catch (SqlException)
            {
                return StatusCode(500, new
                {
                    message = "Database error occured!"
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    message = "Something went wrong"
                });
            }
        }

        [HttpPost]
        public IActionResult AddProduct([FromBody] Products products)
        {
            try
            {
                string connectionString =
                    _configuration.GetConnectionString("DefaultConnection");

                string query = @"INSERT INTO Products(ProductName,Price,Quantity)
                                 VALUES
                                 (@ProductName,@Price,@Quantity)";

                using(SqlConnection con = new SqlConnection(connectionString))
                {
                    using(SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@ProductName", products.ProductName);
                        cmd.Parameters.AddWithValue("@Price",products.Price);
                        cmd.Parameters.AddWithValue("@Quantity", products.Quantity);

                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                return Ok(new
                {
                    message = "Product Added Successfully!"
                });
            }
            catch (SqlException)
            {
                return StatusCode(500, new
                {
                    message = "Database error occured!"
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    message = "Something went wrong !"
                });
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateProduct(int id, [FromBody] Products products)
        {
            try
            {
                string connectionString = _configuration.GetConnectionString("DefaultConnection");

                string query = @"UPDATE Products
                                  SET ProductName = @ProductName,
                                      Price = @Price,
                                      Quantity = @Quantity
                                  WHERE ProductId =@ProductId";
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using(SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@ProductName", products.ProductName);
                        cmd.Parameters.AddWithValue("@Price", products.Price);
                        cmd.Parameters.AddWithValue("@ProductId", id);

                        con.Open();

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected == 0)
                        {
                            return NotFound(new
                            {
                                message = "Product not found!"
                            });
                        }


                    }
                }
                return Ok(new
                {
                    message = "Product Updated Successfully"
                });

            }
            catch (SqlException)
            {
                return StatusCode(500, new
                {
                    message = "Database error occured!"
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    message = "Something went wrong !"
                });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteProduct(int id)
        {
            try
            {
                string connectionString =
                    _configuration.GetConnectionString("DefaultConnection");

                string query = @"DELETE FROM Products WHERE ProductId = @ProductId";

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using(SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@ProductId", id);
                        con.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected == 0)
                        {
                            return NotFound(new
                            {
                                message = "Product not found!"
                            });
                        }
                    }
                }
                return Ok(new
                {
                    message = "Product Deleted Successfully"
                });
            }
            catch (SqlException)
            {
                return StatusCode(500, new
                {
                    message = "Database error occured!"
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    message = "Something went wrong!"
                });
            }
        }
    }
}

