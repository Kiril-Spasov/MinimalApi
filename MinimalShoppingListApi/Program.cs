using Microsoft.EntityFrameworkCore;
using MinimalShoppingListApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApiDbContext>(opt => opt.UseInMemoryDatabase("ShoppingListApi"));

var app = builder.Build();


app.MapGet("/shoppinglist", async (ApiDbContext db) =>
	await db.Groceries.ToListAsync());

app.MapGet("/shoppinglist/{id}", async (int id, ApiDbContext db) => 
{
	var grocery = await db.Groceries.FindAsync(id);
	return grocery != null ? Results.Ok(grocery) : Results.NotFound();
});


app.MapPost("/shoppinglist", async (Grocery grocery, ApiDbContext db) =>
{
	db.Groceries.Add(grocery);
	await db.SaveChangesAsync();

	return Results.Created($"/shoppinglist/{grocery.Id}", grocery);
});


app.MapDelete("/shoppinglist/{id}", async (int id, ApiDbContext db) =>
{
	var grocery = db.Groceries.Find(id);

	if(grocery == null)
	{
		return Results.NotFound();
	}

	db.Groceries.Remove(grocery);
	await db.SaveChangesAsync();
	return Results.NoContent();
});


app.MapPut("/shoppinglist/{id}", async (int id, Grocery grocery, ApiDbContext db) =>
{
	var groceryDb = await db.Groceries.FindAsync(id);
	if (groceryDb == null)
	{
		return Results.NotFound();
	}

	groceryDb.Name = grocery.Name;
	groceryDb.Purchased = grocery.Purchased;
	await db.SaveChangesAsync();
	return Results.Ok(groceryDb);
});
	


if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();