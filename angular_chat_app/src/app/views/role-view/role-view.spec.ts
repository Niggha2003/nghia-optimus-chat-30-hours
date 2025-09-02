import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RoleView } from './role-view';

describe('RoleView', () => {
  let component: RoleView;
  let fixture: ComponentFixture<RoleView>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RoleView]
    })
    .compileComponents();

    fixture = TestBed.createComponent(RoleView);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
