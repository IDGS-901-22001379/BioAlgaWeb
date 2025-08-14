import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { SidebarComponent } from '../../shared/sidebar/sidebar.component';

@Component({
  selector: 'app-inicio',
  standalone: true,
  imports: [CommonModule, RouterModule, SidebarComponent],
  templateUrl: './inicio.html',
  styleUrls: ['./inicio.css']
})
export class InicioComponent { }
