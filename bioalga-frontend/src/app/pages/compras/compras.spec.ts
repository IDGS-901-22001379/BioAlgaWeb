import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Compras } from './compras';

describe('Compras', () => {
  let component: Compras;
  let fixture: ComponentFixture<Compras>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Compras]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Compras);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
