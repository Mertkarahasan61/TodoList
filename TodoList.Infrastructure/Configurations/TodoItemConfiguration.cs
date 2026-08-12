using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TodoList.Domain.Entities;

namespace TodoList.Infrastructure.Configurations;

public class TodoItemConfiguration
    : IEntityTypeConfiguration<TodoItem>    // Bu sınıf, TodoItem entity’sinin veritabanı ayarlarını yapacak.
{
    public void Configure(EntityTypeBuilder<TodoItem> builder)  // EntityTypeBuilder<TodoItem> → TodoItem tablosunu ayarlamamızı sağlayan tür  builder → Bu ayar nesnesinin değişken adı
    {
        builder.ToTable("TodoItems");   // TodoItem sınıfı SQL Server’da TodoItems isimli tabloya dönüşsün.

        builder.HasKey(x => x.Id);  // Id yi primary key yap demektir  x bir todoitemi temsil eder

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.Description)
            .HasMaxLength(1000);

        builder.Property(x => x.Priority)     // tanımlı olmak zorunda
            .IsRequired();

        builder.Property(x => x.IsCompleted)   // deger verilmezse false olsun
            .HasDefaultValue(false);

        builder.Property(x => x.IsDeleted)  
            .HasDefaultValue(false);

        builder.HasOne(x => x.Category)   // Bir TodoItem en fazla bir Categoryye sahip olabilir.
            .WithMany(x => x.TodoItems)     // birden fazla todoitem olsun categirode
            .HasForeignKey(x => x.CategoryId)  // İki tablo arasındaki bağlantıyı CategoryId alanı kuracak.
            .OnDelete(DeleteBehavior.SetNull);     // Bir kategori silinirse ona bağlı görevleri silme; görevlerin CategoryId değerini null yap

        builder.HasQueryFilter(x => !x.IsDeleted);  // silinmemiş görevleri getirir.
    }
}