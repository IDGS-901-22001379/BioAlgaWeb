import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Stock } from './stock';

describe('Stock', () => {
  let component: Stock;
  let fixture: ComponentFixture<Stock>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Stock]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Stock);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
