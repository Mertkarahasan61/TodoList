using Microsoft.AspNetCore.Mvc;
using TodoList.Application.Common.Responses;
using TodoList.Application.DTOs.Todos;
using TodoList.Application.Interfaces.Services;
using FluentValidation;
namespace TodoList.Api.Controllers;

[ApiController]  // ASP.NET Core'a: Bu sınıf bir API Controller'ı.diyoruz 
[Route("api/todos")]  // Bu Controller'ın ana adresini belirliyor.
public class TodosController : ControllerBase  // ControllerBase ise ASP.NET Core'un hazır sınıfı.  Ok(...) gibi şeyler veriyo bize 
{
    private readonly ITodoService _todoService; // Controller'ın Todo işlemlerini kendi içinde yapmasını istemiyoruz Controller Service'i kullanacak.
    private readonly IValidator<CreateTodoRequestDto> _createTodoValidator;
    private readonly IValidator<UpdateTodoRequestDto> _updateTodoValidator;
    public TodosController(
    ITodoService todoService,
    IValidator<CreateTodoRequestDto> createTodoValidator,
    IValidator<UpdateTodoRequestDto> updateTodoValidator)  // AddScoped sayesinde Itodoservice önce  Program.cs ye gider ordan Todoservice alır
    {   
        _todoService = todoService;
        _createTodoValidator = createTodoValidator;
        _updateTodoValidator = updateTodoValidator;
    }

    [HttpGet]  // Bu metot bir GET isteği karşılayacak. Controller'ın route'u:  api/todos olduğu için bunun adresi:GET /api/todos
    public async Task<ActionResult<ApiResponse<PagedResult<TodoResponseDto>>>> GetAll(   // GetAll(...): Görevleri listelemek   [FromQuery] :  Bilgileri URL'deki query parametrelerinden al.
        [FromQuery] TodoFilterRequestDto filter)
    {
        var result = await _todoService.GetAllAsync(filter);  // Bu filtrelere göre Todo'ları getir Service sonucu Controller'a geri veriyor

        return Ok(new ApiResponse<PagedResult<TodoResponseDto>>  // Sonucu kullanıcıya gönderiyoruz  200 OK gibi cevap alıyoruz http olarak
        {
            Success = true,
            Data = result
        });   // ApiResponse:Success Message  Data  Errors  ortak cevap yapımızdı: 
    }

    [HttpGet("{id:int}")]  // GET /api/todos/5 böle bi yapı oluşturuyor 
    public async Task<ActionResult<ApiResponse<TodoResponseDto>>> GetById(int id)  // buradaki ide gelir 
    {
        var result = await _todoService.GetByIdAsync(id);  // service ile 5 numaralı Todo'yu getir.

        if (result is null)
        {
            return NotFound(new ApiResponse<TodoResponseDto>  // not found döndürür httpde ise 404 not found
            {
                Success = false,
                Message = "Görev bulunamadı."
            });
        }

        return Ok(new ApiResponse<TodoResponseDto>   // 200 OK + TodoResponseDto

        {
            Success = true,
            Data = result
        });
    }

    [HttpPost] // POST /api/todos   // Bu metodu HTTP POST isteği geldiğinde çalıştır. Controller'ımızın üstünde zaten:  [Route("api/todos")] var  Dolayısıyla: POST /api/todos oluyor
    public async Task<ActionResult<ApiResponse<TodoResponseDto>>> Create(  // [FromBody]: Kullanıcının gönderdiği bilgileri HTTP isteğinin body'sinden al  //  CreateTodoRequestDto request kullanıcıdan yeni görev bilgilerini alıyoruz
    [FromBody] CreateTodoRequestDto request)
    {                                                // [FromQuery] kullanmıştık get de Çünkü filtreler URL'den geliyordu: /api/todos?status=active&pageSize=10
        var validationResult =
    await _createTodoValidator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return BadRequest(new ApiResponse<TodoResponseDto>
            {
                Success = false,
                Message = "Gönderilen bilgiler geçersiz.",
                Errors = validationResult.Errors
                    .Select(x => x.ErrorMessage)
            });
        }

        var result = await _todoService.CreateAsync(request);  // Kullanıcıdan aldığım bu requesti sana veriyorum, görevi oluştur. var result artık oluşturulmuş görevin TodoResponseDto halidir
        // POST'ta ise büyük bir veri gönderiyoruz, bu yüzden: [FromBody]  
        return CreatedAtAction(  // CreatedAtAction ASP.NET Core'un hazır metodudur. Yeni bir kayıt başarıyla oluşturulduğunda: HTTP 201 Created
            nameof(GetById),  // GetById(int id) ile aynı  zamanında oluşturduğumuz  [HttpGet("{id:int}")] in adresini getirdik   nameof(GetById),  /api/todos/ıd metodunun adres yapısını kullanıyor.
            new { id = result.Id },   // burada idye resultun idesi gelecek ve adres /api/todos/15 olcak mesela
            new ApiResponse<TodoResponseDto>
            {
                Success = true,
                Message = "Görev başarıyla oluşturuldu.",
                Data = result // Oluşturulan görevin bilgilerini cevap içinde gönder.
            });
    }
    [HttpPut("{id:int}")]  // Bu endpointin adresini oluşturuyor: PUT /api/todos/5 Yani kullanıcı:PUT /api/todos/5 gönderirse:  id = 5; oluyo
    public async Task<ActionResult<ApiResponse<TodoResponseDto>>> Update(
    int id,
    [FromBody] UpdateTodoRequestDto request)  // Yeni bilgileri ise body'den aldık:  Title  Description  Priority  CategoryId
    {
        request.Id = id;  // idi url den aldık  request.Id ile verdik
        var validationResult =
    await _updateTodoValidator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return BadRequest(new ApiResponse<TodoResponseDto>
            {
                Success = false,
                Message = "Gönderilen bilgiler geçersiz.",
                Errors = validationResult.Errors
                    .Select(x => x.ErrorMessage)
            });
        }

        var result = await _todoService.UpdateAsync(request);  // yani requeste urlden id geldi bodyden diğer özellikler geldi bunları resulta attık

        if (result is null)
        {
            return NotFound(new ApiResponse<TodoResponseDto>
            {
                Success = false,
                Message = "Görev bulunamadı."
            });
        }

        return Ok(new ApiResponse<TodoResponseDto>
        {
            Success = true,
            Message = "Görev başarıyla güncellendi.",
            Data = result
        });
    }
    [HttpPatch("{id:int}/status")]  // PATCH /api/todos/5/status  hangi Todo'nun durumunu değiştireceğimizi belirtiyor.  PATCH: Todo'nun sadece belirli bir alanını değiştir PUT TAMAMINI DEĞİŞTİRİR
    public async Task<ActionResult<ApiResponse<object>>> ChangeStatus(    
    int id,
    [FromBody] ChangeTodoStatusRequestDto request)    // URL: PATCH /api/todos/5/status id 5 geliyo  Body'den yani ChangeTodoStatusRequestDto  ise:completed true geliyo 
    {
        bool success =
            await _todoService.ChangeStatusAsync(id, request);

        if (!success)
        {
            return NotFound(new ApiResponse<object>
            {
                Success = false,
                Message = "Görev bulunamadı."
            });
        }

        return Ok(new ApiResponse<object>
        {
            Success = true,
            Message = "Görev durumu başarıyla güncellendi."
        });
    }
    [HttpDelete("{id:int}")]  // DELETE /api/todos/5
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id)  // object “Data kısmında özel bir veri tipi beklemiyorum.”
    {
        bool success = await _todoService.DeleteAsync(id);

        if (!success)
        {
            return NotFound(new ApiResponse<object>
            {
                Success = false,
                Message = "Görev bulunamadı."
            });
        }

        return Ok(new ApiResponse<object>
        {
            Success = true,
            Message = "Görev başarıyla silindi."
        });
    }


}