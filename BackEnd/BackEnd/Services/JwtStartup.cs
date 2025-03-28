using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace BackEnd.Services
{
    public static class JwtStartup
    {

       
        public static void ConfigureJwt(this WebApplicationBuilder builder)
        {
            builder.Services.AddAuthentication("Bearer")
                .AddJwtBearer(options =>
                    { 
                        options.SaveToken = true;
                        options.RequireHttpsMetadata = false;
                        options.TokenValidationParameters = new()
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateIssuerSigningKey = true,
                            ValidIssuer = builder.Configuration["Authentication:Issuer"],
                            ValidAudience = builder.Configuration["Authentication:Audience"],
                            IssuerSigningKey = new SymmetricSecurityKey(
                                Encoding.ASCII.GetBytes(builder.Configuration["Authentication:SecretForKey"]))
                        };
                    }
                );
        }
    }
}