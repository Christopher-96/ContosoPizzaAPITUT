//what does this do
using Microsoft.OpenApi.Models;
//adding EntityFrameworkCore
using Microsoft.EntityFrameworkCore;
// adding Models
using PizzaStore.Models;

var builder = WebApplication.CreateBuilder(args);
//Database connection string
var connectionString = builder.Configuration.GetConnectionString("Pizzas") ?? "Data Source=Pizzas.db";

builder.Services.AddEndpointsApiExplorer();
//Adding SQLlite connection context to services
builder.Services.AddSqlite<PizzaDb>(connectionString);
//
builder.Services.AddSwaggerGen(c =>
{
     c.SwaggerDoc("v1", new OpenApiInfo {
         Title = "PizzaStore API",
         Description = "Making the Pizzas you love",
         Version = "v1" });
});

var app = builder.Build();
app.UseSwagger();
//
app.UseSwaggerUI(c =>
{
   c.SwaggerEndpoint("/swagger/v1/swagger.json", "PizzaStore API V1");
});
//Get List of Pizzas
app.MapGet("/Pizzas", async (PizzaDb db) => await db.Pizzas.ToListAsync());

app.MapGet("/", () => "Hello World!");
//Add Pizza
app.MapPost("/Pizza", async (PizzaDb db, Pizza pizza) =>
{
    await db.Pizzas.AddAsync(pizza);
    await db.SaveChangesAsync();
    return Results.Created($"/pizza/{pizza.Id}", pizza);
});
//Update Pizza
app.MapPut("/pizza/{id}", async (PizzaDb db, Pizza updatepizza, int id) =>
{
    var pizza = await db.Pizzas.FindAsync(id);
    if (pizza is null) return Results.NotFound();
    pizza.Name = updatepizza.Name;
    pizza.Description = updatepizza.Description;
    await db.SaveChangesAsync();
    return Results.NoContent();
});

//Delete Pizza
app.MapDelete("/pizza/{id}", async (PizzaDb db, int id) =>
{
  var pizza = await db.Pizzas.FindAsync(id);
  if (pizza is null)
  {
    return Results.NotFound();
  }
  db.Pizzas.Remove(pizza);
  await db.SaveChangesAsync();
  return Results.Ok();
});
//For Running the application
app.Run();