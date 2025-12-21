using App.Data.Repositories.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.

builder.Services.AddControllers();

var connectionString = builder.Configuration.GetConnectionString("SqlServer")
    ?? throw new InvalidOperationException("SqlServer connection string is missing.");

builder.Services.AddData(connectionString);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = "App.Api.Data",
            ValidateAudience = true,
            ValidAudience = "App",
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]))

        };

        options.Events = new JwtBearerEvents
        {
            // her http isteði geldiðinde
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Cookies["access_token"];

                if (!string.IsNullOrWhiteSpace(accessToken))
                {
                    context.Token = accessToken;
                }

                return Task.CompletedTask;
            },

            // Login.Path için;

            OnChallenge = async context =>
            {
                // zorlama davranýþýný engelle
                context.HandleResponse();

                // Þu adrese yönlendir.
                context.Response.Redirect("/Auth/Login");

                await Task.CompletedTask;
            }


        };

        options.MapInboundClaims = false;
        // kendi oluþturduðumuz claim key isimleri olursa, bunlarý soap mimarisine göre map'lemesin 


    });


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
