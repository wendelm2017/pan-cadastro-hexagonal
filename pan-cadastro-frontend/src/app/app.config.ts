import { ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideAnimations } from '@angular/platform-browser/animations';
import { MessageService, ConfirmationService } from 'primeng/api';
import { routes } from './app.routes';
import { errorInterceptor } from './core/interceptors/error.interceptor';
//configuração global do aplicativo, onde são registrados os provedores de serviços, 
// interceptors e rotas. Ele utiliza o provideZoneChangeDetection para otimizar a detecção
//  de mudanças, o provideRouter para configurar as rotas do aplicativo, 
// o provideHttpClient para adicionar interceptors às requisições HTTP, 
// e o provideAnimations para habilitar animações. Além disso, registra os serviços de
//  mensagem e confirmação do PrimeNG para uso em todo o aplicativo.
export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideHttpClient(withInterceptors([errorInterceptor])),
    provideAnimations(),
    MessageService,
    ConfirmationService
  ]
};
