import { AfterViewInit, Component, ViewChild } from '@angular/core';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { MatSort, MatSortModule } from '@angular/material/sort';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatButtonModule } from '@angular/material/button';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { RoleService } from '../../services/role.service';
import { PermissionService } from '../../services/permission.service';
import { MatIconModule } from '@angular/material/icon';
import {MatSelectModule} from '@angular/material/select';

export interface RoleData {
  Id: number;
  RoleName: string;
  RoleCode: string;
}

@Component({
  selector: 'app-role-view',
  standalone: true,
  imports: [
    MatFormFieldModule,
    MatInputModule,
    MatTableModule,
    MatSortModule,
    MatPaginatorModule,
    MatButtonModule,
    MatDialogModule,
    ReactiveFormsModule,
    MatIconModule,
    MatSelectModule
  ],
  templateUrl: './role-view.html',
  styleUrl: './role-view.scss'
})
export class RoleView implements AfterViewInit {
  displayedColumns: string[] = ['Id', 'RoleName', 'RoleCode', 'Actions'];
  dataSource: MatTableDataSource<RoleData> = new MatTableDataSource<RoleData>([]);
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;
  @ViewChild('dialogTemplate') dialogTemplate: any;
  @ViewChild('dialogPermissionControlTemplate') dialogPermissionControlTemplate: any;
  dialogForm: FormGroup;
  dialogPermissionControlForm: FormGroup;
  permissionList: any[] = [];
  currentRoleId: number = 0;
  errorMessage: string = '';

  constructor(
    private roleService: RoleService,
    private permissionService: PermissionService,
    private dialog: MatDialog,
    private fb: FormBuilder
  ) {
    this.fetchRoles();
    this.dialogForm = this.fb.group({
      RoleName: ['', Validators.required],
      RoleCode: ['', Validators.required]
    });
    this.dialogPermissionControlForm = this.fb.group({
      permissionName: [{value: '', disabled: true}, Validators.required],
      permissions: [[], Validators.required]
    });
    this.getPermissions();
  }

  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
  }

  fetchRoles() {
    this.roleService.getAll().subscribe({
      next: (data) => {
        this.dataSource.data = data;
      },
      error: (err) => {
        console.error('Failed to fetch roles', err);
      }
    });
  }

  openDialog() {
    const dialogRef = this.dialog.open(this.dialogTemplate);
    dialogRef.afterClosed().subscribe(() => {
      this.dialogForm.reset();
    });
  }

  closeDialog() {
    this.dialog.closeAll();
  }

  submitDialog() {
    if (this.dialogForm.valid) {
      this.roleService.create(this.dialogForm.value).subscribe({
        next: () => {
          this.fetchRoles();
          this.closeDialog();
        },
        error: (err) => {
          console.error('Failed to add role', err);
        }
      });
    }
  }

  openPermissionControlDialog(roleId: number, roleName: string) {
    this.currentRoleId = roleId;
    this.dialogPermissionControlForm.get('permissionName')?.setValue(roleName);
    this.roleService.getRolePermissions(this.currentRoleId).subscribe({
        next: (permissions) => {
          this.dialogPermissionControlForm.get('permissions')?.setValue(permissions.map((permission: any) => permission.id));
        },
        error: (err) => {
          console.error('Failed to get role', err);
        }
    });
    const dialogRef = this.dialog.open(this.dialogPermissionControlTemplate);
    dialogRef.afterClosed().subscribe(() => {
      this.dialogPermissionControlForm.reset();
    });
  }

  submitPermissionControlDialog() {
    if (this.dialogPermissionControlForm.valid) {
      this.roleService.updateRolePermissions(this.currentRoleId, this.dialogPermissionControlForm.value.permissions).subscribe({
        next: () => {
          console.log('User change permission successfully');
          this.fetchRoles();
          this.closeDialog();
        },
        error: (err) => {
          console.error('Failed to change permission', err);
          this.errorMessage = err?.error?.message || 'Failed to change role';
        }
      });
    }
  }

  deleteRole(id: number) {
    this.roleService.delete(id).subscribe({
      next: () => this.fetchRoles(),
      error: (err) => console.error('Failed to delete role', err)
    });
  }

  getPermissions(): void {
    this.permissionService.getAll().subscribe({
        next: (permissions) => {
          this.permissionList = permissions.map((permission: any) => ({
            permissionId: permission.id,
            permissionName: permission.permissionName
          }));
        },
        error: (err) => {
        }
      });
  }
}
