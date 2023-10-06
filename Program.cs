using WebApplication1.Services;

public class Program
{
    public static void Main(string[] args)
    {
       // DemoDB nhHelper = new DemoDB();
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        //bindingOptions => bindingOptions.AllowEmptyInputInBodyModelBinding = true).AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null

        builder.Services.AddControllers();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddSingleton<DemoDB>();
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
    }
}