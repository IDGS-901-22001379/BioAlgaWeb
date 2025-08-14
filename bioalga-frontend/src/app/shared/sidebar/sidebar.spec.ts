import { TestBed } from '@angular/core/testing';
import { SidebarComponent } from './sidebar.component';

describe('SidebarComponent', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SidebarComponent]
    }).compileComponents();
  });

  it('should create', () => {
    const fixture = TestBed.createComponent(SidebarComponent);
    const comp = fixture.componentInstance;
    expect(comp).toBeTruthy();
  });
});
