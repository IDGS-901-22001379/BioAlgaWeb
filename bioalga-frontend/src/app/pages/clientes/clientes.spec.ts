import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Clientes } from './clientes';

describe('Clientes', () => {
  let component: Clientes;
  let fixture: ComponentFixture<Clientes>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Clientes]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Clientes);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
