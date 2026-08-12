using Microsoft.EntityFrameworkCore;
using TodoList.Domain.Entities;

namespace TodoList.Infrastructure.Persistence;

public class TodoDbContext : DbContext   // dbcontext sql veri ekleme veri çıkarma tablo oluşturma ... veritabanı ile c# arasındadır
{
    public TodoDbContext(DbContextOptions<TodoDbContext> options)  //  TodoDbContext'e ait ayarları taşıyan tür options değişkendir  base(options) Constructor’a gelen options değişkenini üst sınıfın constructor’ına gönderir. üst sınıf DbContext
        : base(options)  // base üst sınıf olan dbcontexti ifade eder
    {
    }

    public DbSet<TodoItem> TodoItems => Set<TodoItem>();    // DbSet<TodoItem> Birden fazla TodoItem kaydını temsil eden veritabanı kümesi    => Set<TodoItem>();   Entity Framework, bana TodoItem türündeki kayıtları yöneten veritabanı kümesini getir.

    public DbSet<Category> Categories => Set<Category>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)  // override: DbContext’in model oluşturma işlemini Todo projemize özel olarak genişletiyorum.
    {
        base.OnModelCreating(modelBuilder); // Üst sınıf olan DbContext'in kendi model oluşturma işlemlerini de çalıştırır. // EF Core'a bu kuralları öğreten yapı

        modelBuilder.ApplyConfigurationsFromAssembly(  // git bu kuralları bul ve uygula” komutu  // Tablolar oluşturulurken bizim belirlediğimiz özel kuralların uygulanacağı metottur.
            typeof(TodoDbContext).Assembly
        );
    }
}