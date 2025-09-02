import { AfterViewInit, Component, ViewChild } from '@angular/core';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { MatSort, MatSortModule } from '@angular/material/sort';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatButtonModule } from '@angular/material/button';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { UserService } from '../../services/user.service';
import { MatIconModule } from '@angular/material/icon';
import {MatSelectModule} from '@angular/material/select';
import { RoleService } from '../../services/role.service';

export interface UserData {
  Id: number;
  UserName: string;
  Email: string;
  Role: string;
}

@Component({
  selector: 'app-user-view',
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
  templateUrl: './user-view.html',
  styleUrl: './user-view.scss'
})
export class UserView implements AfterViewInit {
  displayedColumns: string[] = ['Id', 'UserName', 'UserEmail', 'Role', 'Actions'];
  dataSource: MatTableDataSource<UserData> = new MatTableDataSource<UserData>([]);
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;
  @ViewChild('dialogTemplate') dialogTemplate: any;
  @ViewChild('dialogRoleControlTemplate') dialogRoleControlTemplate: any;
  dialogForm: FormGroup;
  dialogRoleControlForm: FormGroup;
  errorMessage: string = '';
  roleList: any[] = [];
  currentUserId: number = 0;

  constructor(
    private userService: UserService,
    private dialog: MatDialog,
    private fb: FormBuilder,
    private roleService: RoleService,
  ) {
    this.fetchUsers();
    this.dialogForm = this.fb.group({
      userName: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', [Validators.required]],
      roles: [[], Validators.required]
    }, { validators: this.passwordMatchValidator });
    this.dialogRoleControlForm = this.fb.group({
      userName: [{value: '', disabled: true}, Validators.required],
      roles: [[], Validators.required]
    });
    this.getRoles();
  }

  passwordMatchValidator(form: FormGroup) {
    return form.get('password')?.value === form.get('confirmPassword')?.value
      ? null : { mismatch: true };
  }

  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
  }

  fetchUsers() {
    this.userService.getAll().subscribe({
      next: (data) => {
        this.dataSource.data = data;
      },
      error: (err) => {
        console.error('Failed to fetch users', err);
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
      var userData = {
        UserName: this.dialogForm.value.userName,
        UserEmail: this.dialogForm.value.email,
        Password: this.dialogForm.value.password,
        RoleIds: this.dialogForm.value.roles
      };
      this.userService.create(userData).subscribe({
        next: () => {
          console.log('User added successfully');
          this.fetchUsers();
          this.closeDialog();
        },
        error: (err) => {
          console.error('Failed to add user', err);
          this.errorMessage = err?.error?.message || 'Failed to add user';
        }
      });
    }
  }

  openRoleControlDialog(userId: number, userName: string) {
    this.currentUserId = userId;
    this.dialogRoleControlForm.get('userName')?.setValue(userName);
    this.userService.getUserRoles(this.currentUserId).subscribe({
        next: (roles) => {
          this.dialogRoleControlForm.get('roles')?.setValue(roles.map((role: any) => role.id));
        },
        error: (err) => {
          console.error('Failed to get role', err);
        }
    });
    const dialogRef = this.dialog.open(this.dialogRoleControlTemplate);
    dialogRef.afterClosed().subscribe(() => {
      this.dialogRoleControlForm.reset();
    });
  }

  submitRoleControlDialog() {
    if (this.dialogRoleControlForm.valid) {
      this.userService.updateUserRoles(this.currentUserId, this.dialogRoleControlForm.value.roles).subscribe({
        next: () => {
          console.log('User change role successfully');
          this.fetchUsers();
          this.closeDialog();
        },
        error: (err) => {
          console.error('Failed to change role', err);
          this.errorMessage = err?.error?.message || 'Failed to change role';
        }
      });
    }
  }

  deleteUser(id: number) {
    this.userService.delete(id).subscribe({
      next: () => this.fetchUsers(),
      error: (err) => console.error('Failed to delete user', err)
    });
  }

  getRoles(): void {
    this.roleService.getAll().subscribe({
        next: (roles) => {
          this.roleList = roles.map((role: any) => ({
            roleId: role.id,
            roleName: role.roleName
          }));
          console.log('Fetched roles:', this.roleList);
        },
        error: (err) => {
          this.errorMessage = err?.error?.message || 'Registration failed';
        }
      });
  }
}
