import { Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { ReviewService } from '../../core/services/review.service';
import { BookingService } from '../../core/services/booking.service';
import { AuthService } from '../../core/services/auth.service';
import { Review } from '../../models/review.models';
import { Booking } from '../../models/booking.models';

@Component({
  selector: 'app-reviews',
  standalone: true,
  imports: [FormsModule, RouterLink],
  templateUrl: './reviews.html',
  styleUrl: './reviews.scss'
})
export class ReviewsComponent implements OnInit {
  private reviewService = inject(ReviewService);
  private bookingService = inject(BookingService);
  authService = inject(AuthService);

  allReviews = signal<Review[]>([]);
  myCompletedBookings = signal<Booking[]>([]);
  isLoadingReviews = signal(true);

  showForm = signal(false);
  isSubmitting = signal(false);
  formError = signal('');
  successMsg = signal('');

  hoverRating = signal(0);

  formData = { bookingId: 0, rating: 0, comment: '' };

  stars = [1, 2, 3, 4, 5];

  ngOnInit(): void {
    this.loadAllReviews();
    if (this.authService.isLoggedIn()) {
      this.loadMyCompletedBookings();
    }
  }

  loadAllReviews(): void {
    this.isLoadingReviews.set(true);
    this.reviewService.getAllReviews().subscribe({
      next: (data) => {
        this.allReviews.set(data);
        this.isLoadingReviews.set(false);
      },
      error: () => {
        this.isLoadingReviews.set(false);
      }
    });
  }

  loadMyCompletedBookings(): void {
    this.bookingService.getMyBookings().subscribe({
      next: (data) => {
        this.myCompletedBookings.set(data.filter(b => b.status === 'Completed'));
      }
    });
  }

  // Completed bookings jinhe already review nahi kiya
  get reviewableBookings(): Booking[] {
    const myUserId = this.authService.currentUser()?.userId;
    const reviewedIds = new Set(
      this.allReviews()
        .filter(r => r.userId === myUserId)
        .map(r => r.bookingId)
    );
    return this.myCompletedBookings().filter(b => !reviewedIds.has(b.id));
  }

  // Current user ke reviews
  get myReviews(): Review[] {
    const myUserId = this.authService.currentUser()?.userId;
    return this.allReviews().filter(r => r.userId === myUserId);
  }

  // Public reviews sorted by newest first
  get publicReviews(): Review[] {
    return [...this.allReviews()].sort(
      (a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime()
    );
  }

  get averageRating(): string {
    const reviews = this.allReviews();
    if (reviews.length === 0) return '0.0';
    const sum = reviews.reduce((acc, r) => acc + r.rating, 0);
    return (sum / reviews.length).toFixed(1);
  }

  get averageRatingStars(): string {
    const reviews = this.allReviews();
    if (reviews.length === 0) return '☆☆☆☆☆';
    const sum = reviews.reduce((acc, r) => acc + r.rating, 0);
    const avg = Math.round(sum / reviews.length);
    return this.getStars(avg);
  }

  isMyReview(review: Review): boolean {
    return review.userId === this.authService.currentUser()?.userId;
  }

  openForm(): void {
    this.resetForm();
    this.showForm.set(true);
  }

  closeForm(): void {
    this.showForm.set(false);
    this.resetForm();
  }

  setRating(star: number): void {
    this.formData.rating = star;
  }

  setHover(star: number): void {
    this.hoverRating.set(star);
  }

  clearHover(): void {
    this.hoverRating.set(0);
  }

  // Star filled hai ya nahi — hover state ya selected state check karo
  isStarActive(star: number): boolean {
    const active = this.hoverRating() || this.formData.rating;
    return star <= active;
  }

  // Review card mein static stars display karne ke liye
  getStars(rating: number): string {
    return '★'.repeat(rating) + '☆'.repeat(5 - rating);
  }

  getRatingLabel(rating: number): string {
    switch (rating) {
      case 1: return 'Poor';
      case 2: return 'Fair';
      case 3: return 'Good';
      case 4: return 'Very Good';
      case 5: return 'Excellent';
      default: return '';
    }
  }

  onSubmit(): void {
    if (!this.formData.bookingId) {
      this.formError.set('Please select a booking to review.');
      return;
    }
    if (!this.formData.rating) {
      this.formError.set('Please select a star rating.');
      return;
    }

    this.isSubmitting.set(true);
    this.formError.set('');

    this.reviewService.createReview({
      bookingId: this.formData.bookingId,
      rating: this.formData.rating,
      comment: this.formData.comment.trim()
    }).subscribe({
      next: () => {
        this.successMsg.set('Thank you! Your review has been submitted.');
        this.isSubmitting.set(false);
        this.closeForm();
        this.loadAllReviews();
      },
      error: (err) => {
        this.formError.set(err.error?.message || err.error || 'Unable to submit review. Please try again.');
        this.isSubmitting.set(false);
      }
    });
  }

  formatDate(dateStr: string): string {
    if (!dateStr) return '';
    return new Date(dateStr).toLocaleDateString('en-IN', {
      day: '2-digit', month: 'short', year: 'numeric'
    });
  }

  private resetForm(): void {
    this.formData = { bookingId: 0, rating: 0, comment: '' };
    this.hoverRating.set(0);
    this.formError.set('');
    this.successMsg.set('');
    this.isSubmitting.set(false);
  }
}
