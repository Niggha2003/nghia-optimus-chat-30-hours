import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PermissionView } from './permission-view';

describe('PermissionView', () => {
  let component: PermissionView;
  let fixture: ComponentFixture<PermissionView>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PermissionView]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PermissionView);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
