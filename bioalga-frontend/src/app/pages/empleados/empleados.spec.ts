import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Empleados } from './empleados';

describe('Empleados', () => {
  let component: Empleados;
  let fixture: ComponentFixture<Empleados>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Empleados]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Empleados);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
