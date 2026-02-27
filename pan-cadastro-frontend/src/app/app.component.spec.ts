import { TestBed } from '@angular/core/testing';
import { AppComponent } from './app.component';
//teste unitário básico para o componente raiz do aplicativo, verificando se ele
//  é criado corretamente. Ele utiliza o TestBed do Angular para configurar o ambiente
//  de teste e criar uma instância do AppComponent, garantindo que a aplicação inicialize
//  sem erros.
describe('AppComponent', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AppComponent],
    }).compileComponents();
  });

  it('should create the app', () => {
    const fixture = TestBed.createComponent(AppComponent);
    const app = fixture.componentInstance;
    expect(app).toBeTruthy();
  });
});