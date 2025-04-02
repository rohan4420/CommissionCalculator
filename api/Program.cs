namespace FCamara.CommissionCalculator
{
   
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            
            // Add CORS service
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("ReactAppPolicy",
                    policy => policy
                        .WithOrigins("http://localhost:3000") // react origin
                        .AllowAnyHeader()
                        .AllowAnyMethod());
            });
            
            // Add services to the container with custom validation behavior
            builder.Services.AddControllers()
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.SuppressModelStateInvalidFilter = true;
                });
            
            // Register the Commission Service
            builder.Services.AddScoped<FCamara.CommissionCalculator.Services.Interfaces.ICommissionService, FCamara.CommissionCalculator.Services.CommissionService>();
            
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
        
            var app = builder.Build();
            
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            
            app.UseRouting();
            
            app.UseCors("ReactAppPolicy");

            
            app.UseAuthorization();
    
            app.MapControllers();
            
            app.Run();
        }
    }
}
