import { TestBed } from '@angular/core/testing';
import { provideRouter, Router } from '@angular/router';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { App } from './app';
import { routes } from './app.routes';

describe('App', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [App],
      providers: [provideRouter(routes), provideAnimationsAsync()],
    }).compileComponents();

    TestBed.inject(Router).initialNavigation();
  });

  it('should create the app', () => {
    const fixture = TestBed.createComponent(App);
    expect(fixture.componentInstance).toBeTruthy();
  });

  it('should render the shell with Home content and nav links', async () => {
    const fixture = TestBed.createComponent(App);
    fixture.detectChanges();
    await fixture.whenStable();
    fixture.detectChanges();

    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.textContent).toContain('PF Data Pipeline');
    expect(compiled.textContent).toContain('Employees');
    expect(compiled.textContent).toContain('Jobs');
    expect(compiled.textContent).toContain('Data Management');
    expect(compiled.querySelector('h1')?.textContent).toContain('Home');
    expect(compiled.textContent).toContain('Welcome to PF Data Pipeline.');
  });
});
