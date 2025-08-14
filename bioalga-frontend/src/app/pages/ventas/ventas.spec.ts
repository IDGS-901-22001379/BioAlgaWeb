import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Ventas } from './ventas';

describe('Ventas', () => {
  let component: Ventas;
  let fixture: ComponentFixture<Ventas>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Ventas]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Ventas);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
