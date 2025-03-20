﻿using Microsoft.EntityFrameworkCore;
using WorkingOnIt.Core;
using WorkingOnIt.Core.InterfaceRepository;
using WorkingOnIt.Core.InterfaceService;
using WorkingOnIt.Data;
using WorkingOnIt.Data.Repository;
using WorkingOnIt.Service.Services;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Recipes.Service.Services;
using DotNetEnv;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        Env.Load(); 
        //string jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET");
        string jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET") ?? throw new Exception("JWT_SECRET is not set");

        string awsAccessKey = Environment.GetEnvironmentVariable("AWS_ACCESS_KEY_ID");
        string awsSecretKey = Environment.GetEnvironmentVariable("AWS_SECRET_ACCESS_KEY");
        string awsRegion = Environment.GetEnvironmentVariable("AWS_REGION");
        string bucketName = Environment.GetEnvironmentVariable("AWS_BUCKET_NAME");
        Console.WriteLine($"✅ JWT_SECRET: {jwtSecret}");
        Console.WriteLine($"✅ AWS_ACCESS_KEY_ID: {awsAccessKey}");
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<IInterviewQuestionsService, InterviewQuestionsService>();
        builder.Services.AddScoped<IInterviewService, InterviewService>();
        builder.Services.AddScoped<IResumeService, ResumeService>();
        builder.Services.AddScoped<IAuthService, AuthService>();

        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IInterviewQuestionsRepository, InterviewQuestionsRepository>();
        builder.Services.AddScoped<IInterviewRepository, InterviewRepository>();
        builder.Services.AddScoped<IResumeRepository, ResumeRepository>();



        builder.Services.AddScoped(typeof(IRepositoryManager), typeof(RepositoryManager));
        builder.Services.AddScoped(typeof(IRepositoryGeneric<>), typeof(RepositoryGeneric<>));

        builder.Services.AddAutoMapper(typeof(MappingProfile));
        builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        builder.Services.AddSingleton<IS3Service, S3Service>(); // רישום ה-S3Service
        builder.Services.AddScoped<JwtService>();
        var connectionString = "Server=bhlraqx5nvyxmpm5cicv-mysql.services.clever-cloud.com;Database=bhlraqx5nvyxmpm5cicv;User=ua67fticoup8ufvo;Password=eB0GEfKtrMmgTdEjQExt; ";
        builder.Services.AddDbContext<DataContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString),
    mysqlOptions => mysqlOptions.CommandTimeout(60)));
      //  builder.Services.AddDbContext<DataContext>(options =>
      //options.UseMySql("Server=bhlraqx5nvyxmpm5cicv-mysql.services.clever-cloud.com;Database=bhlraqx5nvyxmpm5cicv;User=ua67fticoup8ufvo;Password=eB0GEfKtrMmgTdEjQExt; Integrated Security = true;TrustServerCertificate=True; ",
      //  new MySqlServerVersion(new Version(8, 0, 0))));
      //  builder.Services.AddDbContext<DataContext>(
      //options => options.UseSqlServer("Data Source = DESKTOP-SSNMLFD; Initial Catalog = WorkingOnIt; Integrated Security = true;TrustServerCertificate=True; "));
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAnyOrigin", builder =>
            {
                builder.AllowAnyOrigin()  // מאפשר גישה מכל מקור
                       .AllowAnyMethod()  // מאפשר כל סוג בקשה (GET, POST, וכו')
                       .AllowAnyHeader(); // מאפשר כל כותרת בבקשה
            });
        });
        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        //    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        //.AddJwtBearer(options =>
        //{
        //    options.TokenValidationParameters = new TokenValidationParameters
        //    {
        //        ValidateIssuerSigningKey = true,
        //        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"])),
        //        ValidateIssuer = false,
        //        ValidateAudience = false
        //    };
        //});
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)), // ✅ משתמש במפתח מה-ENV
                ValidateIssuer = false,
                ValidateAudience = false
            };
        });

        var app = builder.Build();
        app.UseCors("AllowAnyOrigin"); // החלת ה-CORS
        app.UseAuthentication(); // להוסיף לפני `app.UseAuthorization();`
        app.UseAuthorization();




       

  

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
    }
}