namespace TodoList.Application.Common.Responses;

public class ApiResponse<T>
{
    public bool Success { get; set; }   // İşlem başarısız gibi

    public string? Message { get; set; }   // Görev başarıyla oluşturuldu. gibi

    public T? Data { get; set; }   // İşlemin sonucunda dönecek veriyi taşır.

    public IEnumerable<string>? Errors { get; set; }   // Bir veya birden fazla hata mesajını taşır. Enumerable ile birden fazla hata gönderebiliriz.
}