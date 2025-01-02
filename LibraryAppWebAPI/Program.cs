using Hangfire;
using LibraryAppWebAPI.CleanUp;
using LibraryAppWebAPI.DataContext;
using LibraryAppWebAPI.Repository;
using LibraryAppWebAPI.Repository.Interfaces;
using LibraryAppWebAPI.Service;
using LibraryAppWebAPI.Service.IServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Net;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
{
    // Add services to the container.

    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
    builder.Services.AddScoped<IBookRepository, BookRepository>();
    builder.Services.AddScoped<IDvdRepository, DvdRepository>();
    builder.Services.AddScoped<IMemberRepository, MemberRepository>();
    builder.Services.AddScoped<IMessageRepository, MessageRepository>();
    builder.Services.AddScoped<IQueueItemRepository, QueueItemRepository>();
    builder.Services.AddScoped<IRentalEntryRepository, RentalEntryRepository>();
    builder.Services.AddScoped<IMessagingService, MessagingService>();
    builder.Services.AddScoped<IQueueService, QueueService>();
    builder.Services.AddScoped<IRentalEntryService, RentalEntryService>();

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo { Title = "Library App REST Api documentation", Version = "v1" });
        options.EnableAnnotations();
    });

    builder.Services.AddDbContext<LibraryContext>(options =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    });

    // Nastavenie Hangfire
    builder.Services.AddHangfire(config =>
    {
        config.UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection"));
    });

    builder.Services.AddHangfireServer();

    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
    });

    builder.Services.Configure<ForwardedHeadersOptions>(options =>
    {
        options.KnownProxies.Add(IPAddress.Parse("127.0.0.1"));
    });
}

WebApplication app = builder.Build();
{
    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        });
    }

    app.UseHttpsRedirection();

    app.UseCors();

    app.UseRouting();

    app.UseAuthorization();

    app.UseHangfireDashboard();

    RecurringJob.AddOrUpdate<DataCleanupJob>(
        "DataCleanupJob", 
        job => job.ExecuteAsync(), 
        "0 1 * * *"
    );

    app.MapControllers();

    app.Run();
}