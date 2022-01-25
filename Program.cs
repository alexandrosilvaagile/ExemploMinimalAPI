using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options => 
             options.UseInMemoryDatabase("Clientes"));

builder.Services.AddSingleton<TesteService>(new TesteService());
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseSwagger();

app.MapGet("/clientes/{id}",async (int id, AppDbContext dbContext)=> 
        await dbContext.Clientes.FirstOrDefaultAsync( a => a.Id == id));

app.MapPost("/clientes", async (Cliente cliente, AppDbContext dbContext) =>
{
    dbContext.Clientes.Add(cliente);
    await dbContext.SaveChangesAsync();
    return cliente;
});
app.MapPut("/clientes/{id}", async (int id, Cliente cliente, AppDbContext dbContext) =>
{
    dbContext.Entry(cliente).State = EntityState.Modified;
    await dbContext.SaveChangesAsync();
    return cliente;
});

app.MapDelete("/Clientes/{id}", async (int id, AppDbContext dbContext) =>
{
    var cliente = await dbContext.Clientes.FirstOrDefaultAsync(a => a.Id == id);
    if (cliente != null)
    {
        dbContext.Clientes.Remove(cliente);
        await dbContext.SaveChangesAsync();
    }
    return;
});

app.MapGet("/", () => "Hello World!");
app.MapGet("/cliente", () => new Cliente(1,"Macoratti","alexandro@outlook.com"));
app.MapGet("/albums", ()=> new HttpClient().GetStringAsync("https://jsonplaceholder.typicode.com/albums"));
app.MapGet("/boasvindas",(HttpContext context, TesteService testeService)=> testeService.BoasVindas(context.Request.Query["nome"].ToString()));
app.UseSwaggerUI();

await app.RunAsync();
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions options) : base(options)
    {
    }
    public DbSet<Cliente>? Clientes { get; set; }
}


public class Cliente
{
    public Cliente(int id, string nome, string email)
    {
        Id = id;
        Nome = nome;
        Email = email;
    }

    public int Id { get; set; }
    public string? Nome { get; set; }

    public string Email{get; set;}
}
public class TesteService
{
    public string BoasVindas(string nome)
    {
        return $"Bem-Vindo {nome}";
    }
}