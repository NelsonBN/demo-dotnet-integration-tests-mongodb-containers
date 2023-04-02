using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);


builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen();


builder.Services
    .AddSingleton(sp =>
        new MongoUrl(sp.GetRequiredService<IConfiguration>().GetConnectionString("MongoDB")))
    .AddSingleton<IMongoClient>(sp =>
        new MongoClient(sp.GetService<MongoUrl>()))
    .AddScoped(sp =>
    {
        var mongoUrl = sp.GetService<MongoUrl>();
        var mongoClient = sp.GetService<IMongoClient>();

        return mongoClient!.GetDatabase(mongoUrl!.DatabaseName);
    })
    .AddScoped(sp =>
        sp.GetRequiredService<IMongoDatabase>().GetCollection<Product>(nameof(Product)));



var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();



app.MapGet("/connection", (IConfiguration configuration) =>
    TypedResults.Ok(configuration.GetConnectionString("MongoDB")));


app.MapGet("/products", async (IMongoCollection<Product> collection) =>
{
    var result = await collection
        .Find(f => true)
        .ToListAsync();

    return TypedResults.Ok(result);
});


app.MapGet("/products/{id:guid}", async (IMongoCollection<Product> collection, Guid id) =>
{
    var result = await collection
        .Find(f => f.Id == id)
        .SingleOrDefaultAsync();

    return TypedResults.Ok(result);
}).WithName("GetProduct");


app.MapPost("/products", async (IMongoCollection<Product> collection, ProductRequest product) =>
{
    Product entity = new()
    {
        Id = Guid.NewGuid(),
        Name = product.Name,
        Quantity = product.Quantity
    };

    await collection.InsertOneAsync(entity);

    return TypedResults.CreatedAtRoute(
        entity.Id,
        "GetProduct",
        new { entity.Id });
});


app.Run();



public class Product
{
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public int Quantity { get; set; }
}

public record ProductRequest
{
    public string? Name { get; init; }
    public int Quantity { get; init; }
};
