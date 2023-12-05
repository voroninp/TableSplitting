// See https://aka.ms/new-console-template for more information
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

var ctx = new AppDbContext();


var identityUserModel = ctx.Model.FindEntityType(typeof(IdentityUser));
var tableMapping = identityUserModel.GetTableMappings().Single();
Console.WriteLine("IdnetityUser mappings:");
foreach (var col in tableMapping.ColumnMappings)
{
    Console.WriteLine($"{col.Property} - {col.Column}");
}
Console.WriteLine();


var userInfoModel = ctx.Model.FindEntityType(typeof(UserInfo));
if (userInfoModel is not null)
{
    Console.WriteLine("UserInfo mappings:");
    tableMapping = identityUserModel.GetTableMappings().Single();
    foreach (var col in tableMapping.ColumnMappings)
    {
        Console.WriteLine($"{col.Property} - {col.Column}");
    }
}
Console.WriteLine();

var fullModel = ctx.Model.FindEntityType(typeof(Full));
Console.WriteLine("Full mappings:");
tableMapping = fullModel.GetTableMappings().Single();
foreach (var col in tableMapping.ColumnMappings)
{
    Console.WriteLine($"{col.Property} - {col.Column}");
}
Console.WriteLine();

var partModel = ctx.Model.FindEntityType(typeof(Part));
Console.WriteLine("Part mappings:");
tableMapping = partModel.GetTableMappings().Single();
foreach (var col in tableMapping.ColumnMappings)
{
    Console.WriteLine($"{col.Property} - {col.Column}");
}
Console.WriteLine();

public sealed record UserInfo(string Id, [property: StringLength(256)]string? UserName);

public sealed record Full(int Id, string Prop1, string Prop2);
public sealed record Part(int Id, string Prop1);

sealed class AppDbContext : IdentityDbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder.UseSqlite("DataSource=:memory:");
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // builder.Entity<IdentityUser>(b => b.Property(_ => _.UserName).HasColumnName("UserName"));

        builder.Entity<UserInfo>(b =>
        {
            var identityUserMetadata = builder.Entity<IdentityUser>().Metadata;
            var table = identityUserMetadata.GetTableName();
            var userNameProperty = identityUserMetadata.GetProperty(nameof(IdentityUser.UserName));

            b.ToTable(table);

            b.HasOne<IdentityUser>().WithOne().HasForeignKey<IdentityUser>(_ => _.Id);

            var columnName = userNameProperty.GetColumnName();
            b.Property(_ => _.UserName).HasColumnName(columnName);
        });

        builder.Entity<Full>();

        builder.Entity<Part>(b =>
        {
            var metadata = builder.Entity<Full>().Metadata;
            var table = metadata.GetTableName();

            b.ToTable(table);

            b.HasOne<Full>().WithOne().HasForeignKey<Part>(_ => _.Id);
        });
    }
}