import { Component } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatListModule } from '@angular/material/list';

@Component({
  selector: 'app-layout',
  imports: [RouterOutlet, RouterLink, RouterLinkActive, MatSidenavModule, MatListModule],
  templateUrl: './layout.html',
  styleUrl: './layout.scss',
})
export class Layout {
  readonly navItems = [
    { label: 'Home', path: '/' },
    { label: 'Employees', path: '/employees' },
    { label: 'Jobs', path: '/jobs' },
    { label: 'Data Management', path: '/data-management' },
  ] as const;
}
