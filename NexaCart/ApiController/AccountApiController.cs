
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using NexaCart.Models;

namespace NexaCart.ApiController
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountApiController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AccountApiController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] Users obj)
        {
            try
            {
                string connectionString =
                    _configuration.GetConnectionString("DefaultConnection");

                string query = @"INSERT INTO Users
                                (FullName, Email, PasswordHash, RoleId)
                                VALUES
                                (@FullName, @Email, @PasswordHash, @RoleId)";

                // Generate BCrypt PasswordHash
                string passwordHash =
                    BCrypt.Net.BCrypt.HashPassword(obj.PasswordHash);

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@FullName", obj.FullName);
                        cmd.Parameters.AddWithValue("@Email", obj.Email);
                        cmd.Parameters.AddWithValue("@PasswordHash", passwordHash);
                        cmd.Parameters.AddWithValue("@RoleId", 2);

                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                return Ok(new
                {
                    success = true,
                    message = "User registration successful!"
                });
            }
            catch (SqlException)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Database error occurred!"
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Something went wrong!"
                });
            }
        }


        [HttpPost("login")]
        public IActionResult Login([FromBody] Login obj)
        {
            try
            {
                string connectionString =
                    _configuration.GetConnectionString("DefaultConnection");

                string query = @"SELECT UserId, FullName, Email, PasswordHash, RoleId
                                 FROM Users
                                 WHERE Email = @Email";

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Email", obj.Email);

                        con.Open();

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                // Retrieve stored BCrypt PasswordHash
                                string storedPasswordHash =
                                    dr["PasswordHash"].ToString();

                                // Verify entered password with stored hash
                                bool isPasswordValid =
                                    BCrypt.Net.BCrypt.Verify(
                                        obj.PasswordHash,
                                        storedPasswordHash
                                    );

                                if (isPasswordValid)
                                {
                                    string userId = dr["UserId"].ToString();
                                    string role = dr["RoleId"].ToString();

                                    HttpContext.Session.SetString("UserId", userId);
                                    HttpContext.Session.SetString("RoleId", role);

                                    int roleId = Convert.ToInt32(role);

                                    string redirectUrl;

                                    if (roleId == 1)
                                    {
                                        redirectUrl = "/Dashboard/AdminDashboard";
                                    }
                                    else if (roleId == 2)
                                    {
                                        redirectUrl = "/Dashboard/UserDashboard";
                                    }
                                    else
                                    {
                                        return Unauthorized(new
                                        {
                                            success = false,
                                            message = "Invalid user role."
                                        });
                                    }

                                    return Ok(new
                                    {
                                        success = true,
                                        message = "Login successful!",
                                        redirectUrl = redirectUrl
                                    });
                                }
                            }
                        }
                    }
                }

                return Unauthorized(new
                {
                    success = false,
                    message = "Invalid email or password!"
                });
            }
            catch (SqlException)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Database error occurred!"
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Something went wrong!"
                });
            }
        }


        [HttpPost("logout")]
        public IActionResult Logout()
        {
            try
            {
                HttpContext.Session.Clear();

                return Ok(new
                {
                    success = true,
                    message = "Logout successful!"
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Logout failed!"
                });
            }
        }
    }
}
