import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { MovimientosListaPage } from './lista';

describe('MovimientosListaPage', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [MovimientosListaPage],
      providers: [provideHttpClient()]
    });
  });

  it('should create', () => {
    const fixture = TestBed.createComponent(MovimientosListaPage);
    const comp = fixture.componentInstance;
    expect(comp).toBeTruthy();
  });
});
