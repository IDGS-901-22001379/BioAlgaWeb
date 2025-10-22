import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';

import { CorteTurnosPage } from './turnos';
import { CajaTurnosService } from '../../../services/caja-turnos.service';
import { AuthService } from '../../../services/auth.service';

// Stubs muy simples para no pegarle a servicios reales
class AuthServiceStub {
  currentUser = { id_Usuario: 1, nombre_Usuario: 'demo' } as any;
  get isLoggedIn() { return true; }
  get currentUser$() { return { subscribe: () => ({ unsubscribe() {} }) } as any; }
}

describe('CorteTurnosPage', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        CorteTurnosPage,               // componente standalone
        HttpClientTestingModule,
        ReactiveFormsModule,
        FormsModule
      ],
      providers: [
        CajaTurnosService,
        { provide: AuthService, useClass: AuthServiceStub }
      ]
    }).compileComponents();
  });

  it('debe crearse', () => {
    const fixture = TestBed.createComponent(CorteTurnosPage);
    const comp = fixture.componentInstance;
    expect(comp).toBeTruthy();
  });

  it('valOr0 debe devolver 0 si es null/undefined', () => {
    const comp = TestBed.createComponent(CorteTurnosPage).componentInstance;
    expect(comp.valOr0(null)).toBe(0);
    expect(comp.valOr0(undefined)).toBe(0);
    expect(comp.valOr0(12.34)).toBe(12.34);
  });
});
