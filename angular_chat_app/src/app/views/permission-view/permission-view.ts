import { AfterViewInit, Component, ViewChild } from '@angular/core';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { MatSort, MatSortModule } from '@angular/material/sort';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { PermissionService } from '../../services/permission.service'; // import service
import { MatButtonModule } from '@angular/material/button';
import { MatDialog } from '@angular/material/dialog';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ReactiveFormsModule } from '@angular/forms';
import { MatIconModule } from '@angular/material/icon';

export interface PermissionData {
  Id: number;
  PermissionName: string;
  PermissionCode: string;
}

@Component({
  selector: 'app-permission-view',
  standalone: true,
  imports: [
    MatFormFieldModule,
    MatInputModule,
    MatTableModule,
    MatSortModule,
    MatPaginatorModule,
    MatButtonModule,
    ReactiveFormsModule,
    MatIconModule
  ],
  templateUrl: './permission-view.html',
  styleUrl: './permission-view.scss'
})
export class PermissionView implements AfterViewInit {
  displayedColumns: string[] = ['Id', 'PermissionName', 'PermissionCode', 'Actions'];
  dataSource: MatTableDataSource<PermissionData> = new MatTableDataSource<PermissionData>([]);
  newPermission: Partial<PermissionData> = {};

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;
  @ViewChild('dialogTemplate') dialogTemplate: any;
  dialogForm: FormGroup;

  constructor(private permissionService: PermissionService, private dialog: MatDialog, private fb: FormBuilder) {
    this.fetchPermissions();
    this.dialogForm = this.fb.group({
      PermissionName: ['', Validators.required],
      PermissionCode: ['', Validators.required]
    });
  }

  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
  }

  fetchPermissions() {
    this.permissionService.getAll().subscribe({
      next: (data) => {
        this.dataSource.data = data;
      },
      error: (err) => {
        console.error('Failed to fetch permissions', err);
      }
    });
  }

  addPermission() {
    if (!this.newPermission.PermissionName || !this.newPermission.PermissionCode) return;
    this.permissionService.create(this.newPermission).subscribe({
      next: () => {
        this.newPermission = {};
        this.fetchPermissions();
      },
      error: (err) => {
        console.error('Failed to add permission', err);
      }
    });
  }

  deletePermission(id: number) {
    this.permissionService.delete(id).subscribe({
      next: () => this.fetchPermissions(),
      error: (err) => console.error('Failed to delete permission', err)
    });
  }

  
  openDialog() {
    const dialogRef = this.dialog.open(this.dialogTemplate);
    dialogRef.afterClosed().subscribe(result => {
      this.dialogForm.reset();
    });
  }

  closeDialog() {
    this.dialog.closeAll();
  }

  submitDialog() {
    if (this.dialogForm.valid) {
      this.permissionService.create(this.dialogForm.value).subscribe({
        next: () => {
          this.fetchPermissions();
          this.closeDialog();
        }
      });
    }
  }

}


