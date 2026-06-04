import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { TinyUrlService } from '../../services/tiny-url.service';
import { TinyUrl } from '../../models/tiny-url.model';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  urlForm = new FormGroup({
    originalUrl: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required]
    }),
    isPrivate: new FormControl(false, { nonNullable: true })
  });

  searchControl = new FormControl('', { nonNullable: true });
  isLoading = false;
  errorMsg = '';
  successMsg = '';
  private successMessageTimer: ReturnType<typeof setTimeout> | null = null;

  urls: TinyUrl[] = [];
  filteredUrls: TinyUrl[] = [];
  copiedCode = '';

  constructor(private tinyUrlService: TinyUrlService) {}

  ngOnInit(): void {
    this.loadUrls();
    this.searchControl.valueChanges.subscribe(() => this.applySearch());
  }

  loadUrls(): void {
    this.tinyUrlService.getUrls().subscribe({
      next: (data) => {
        this.urls = data;
        this.applySearch();
      },
      error: (err) => {
        console.error('Error loading URLs', err);
        this.errorMsg = 'Could not load URLs. Make sure the API is running on http://localhost:5000.';
      }
    });
  }

  generateUrl(): void {
    this.errorMsg = '';
    this.successMsg = '';

    const originalUrl = this.urlForm.controls.originalUrl.value.trim();
    if (!originalUrl) {
      this.errorMsg = 'Please enter a URL.';
      this.urlForm.controls.originalUrl.markAsTouched();
      return;
    }

    this.isLoading = true;

    this.tinyUrlService.createUrl({
      originalUrl,
      isPrivate: this.urlForm.controls.isPrivate.value
    }).subscribe({
      next: (created) => {
        this.isLoading = false;
        this.successMsg = `Short URL created: ${created.shortUrl}`;
        this.clearSuccessMessageAfterDelay();
        this.urlForm.reset({
          originalUrl: '',
          isPrivate: false
        });
        this.urls = created.isPrivate ? this.urls : [created, ...this.urls];
        this.applySearch();
      },
      error: (err) => {
        this.isLoading = false;
        this.errorMsg = err?.error?.error || 'Failed to create URL. Use a full URL like https://example.com.';
      }
    });
  }

  deleteUrl(shortCode: string): void {
    if (!confirm(`Delete short URL "${shortCode}"?`)) return;

    this.tinyUrlService.deleteUrl(shortCode).subscribe({
      next: () => {
        this.urls = this.urls.filter((u) => u.shortCode !== shortCode);
        this.applySearch();
      },
      error: (err) => {
        console.error('Delete failed', err);
        this.errorMsg = err?.error?.error || 'Delete failed.';
      }
    });
  }

  copyToClipboard(shortUrl: string, shortCode: string): void {
    navigator.clipboard.writeText(shortUrl).then(() => {
      this.copiedCode = shortCode;
      setTimeout(() => this.copiedCode = '', 2000);
    });
  }

  trackUrlClick(clickedUrl: TinyUrl): void {
    clickedUrl.hitCount++;
  }

  applySearch(): void {
    const q = this.searchControl.value.toLowerCase().trim();
    this.filteredUrls = q
      ? this.urls.filter((u) =>
          u.shortCode.toLowerCase().includes(q) ||
          u.originalUrl.toLowerCase().includes(q) ||
          u.shortUrl.toLowerCase().includes(q)
        )
      : [...this.urls];
  }

  onSearchChange(): void {
    this.applySearch();
  }

  private clearSuccessMessageAfterDelay(): void {
    if (this.successMessageTimer) {
      clearTimeout(this.successMessageTimer);
    }

    this.successMessageTimer = setTimeout(() => {
      this.successMsg = '';
      this.successMessageTimer = null;
    }, 10000);
  }
}
