import {
  HttpErrorResponse,
  HttpInterceptorFn
} from '@angular/common/http';

import {
  catchError,
  throwError
} from 'rxjs';


export const apiErrorInterceptor: HttpInterceptorFn =
  (request, next) => {

    return next(request).pipe(

      catchError(
        (error: HttpErrorResponse) => {

          let errorMessage =
            'İşlem sırasında bir hata oluştu.';


          // Backend'e hiç ulaşılamadı
          if (error.status === 0) {

            errorMessage =
              'Sunucuya ulaşılamadı.';

          }


          // Bad Request
          else if (error.status === 400) {

            errorMessage =
              error.error?.message ??
              'Gönderilen bilgiler geçersiz.';

          }


          // Yetkilendirme gerekli
          else if (error.status === 401) {

            errorMessage =
              'Bu işlem için giriş yapmanız gerekiyor.';

          }


          // Yetki yok
          else if (error.status === 403) {

            errorMessage =
              'Bu işlem için yetkiniz bulunmuyor.';

          }


          // Kayıt bulunamadı
          else if (error.status === 404) {

            errorMessage =
              error.error?.message ??
              'İstenen kayıt bulunamadı.';

          }


          // Sunucu hatası
          else if (error.status >= 500) {

            errorMessage =
              'Sunucuda bir hata oluştu.';

          }


          console.error(
            'API Hatası:',
            {
              url: request.url,
              method: request.method,
              status: error.status,
              message: errorMessage
            }
          );


          // Hatayı component'e geri gönderiyoruz.
          // Böylece mevcut Toast kodlarımız da
          // çalışmaya devam ediyor.
          return throwError(
            () => error
          );

        }
      )

    );

  };