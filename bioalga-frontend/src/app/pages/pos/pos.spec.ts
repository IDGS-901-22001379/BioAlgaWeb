import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Pos } from './pos';

describe('Pos', () => {
  let component: Pos;
  let fixture: ComponentFixture<Pos>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Pos]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Pos);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
