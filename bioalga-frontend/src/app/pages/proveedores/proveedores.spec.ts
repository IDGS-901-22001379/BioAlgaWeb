import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Proveedores } from './proveedores';

describe('Proveedores', () => {
  let component: Proveedores;
  let fixture: ComponentFixture<Proveedores>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Proveedores]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Proveedores);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
