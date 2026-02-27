import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { MessageService } from 'primeng/api';

// Este interceptor captura erros HTTP globalmente e exibe mensagens de erro usando
// o MessageService do PrimeNG.
export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const messageService = inject(MessageService);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      let mensagem = 'Erro inesperado. Tente novamente.';

      if (error.error?.mensagem) {
        mensagem = error.error.mensagem;
      } else if (error.status === 0) {
        mensagem = 'Não foi possível conectar ao servidor.';
      } else if (error.status === 404) {
        mensagem = 'Registro não encontrado.';
      }

      messageService.add({
        severity: 'error',
        summary: 'Erro',
        detail: mensagem,
        life: 5000
      });

      return throwError(() => error);
    })
  );
};
