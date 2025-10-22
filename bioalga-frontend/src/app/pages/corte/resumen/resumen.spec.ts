import { TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { CorteResumenPage } from './resumen';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { CorteService } from '../../../services/corte.service';
import { CorteResumenDto } from '../../../models/corte-resumen.model';

describe('ResumenCortePage', () => {
  let component: CorteResumenPage;

  const mockResumen: CorteResumenDto = {
    id_Turno: 10,
    id_Caja: 1,
    id_Usuario: 5,
    apertura: '2025-01-01T08:00:00Z',
    cierre_Usado: '2025-01-01T18:00:00Z',
    num_Tickets: 3,
    ventas_Totales: 305,
    ventas_Por_Metodo: [
      { metodo: 'Efectivo', monto: 200 },
      { metodo: 'Tarjeta',  monto: 105 },
    ],
    ventas_Efectivo_Neto: 200,
    entradas_Efectivo: 500,
    salidas_Efectivo: 100,
    devoluciones_Total: 0,
    saldo_Inicial: 0,
    efectivo_Esperado: 600,
  };

  const corteSvcMock = {
    resumenPorTurno: jasmine.createSpy('resumenPorTurno')
                           .and.returnValue(of(mockResumen))
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        // standalone component
        CorteResumenPage,
        HttpClientTestingModule
      ],
      providers: [
        { provide: CorteService, useValue: corteSvcMock }
      ]
    }).compileComponents();

    const fixture = TestBed.createComponent(CorteResumenPage);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('debe crearse', () => {
    expect(component).toBeTruthy();
  });

  it('totalMetodos() debe sumar montos de ventas_Por_Metodo', () => {
    // Simula que ya hay resumen cargado
    component['resumen'].set(mockResumen);
    expect(component.totalMetodos()).toBe(305);
  });
});
