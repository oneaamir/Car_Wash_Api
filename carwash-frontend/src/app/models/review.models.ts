export interface Review {
  id: number;
  bookingId: number;
  userId: number;
  washerId: number | null;
  rating: number;
  comment: string;
  createdAt: string;
  message: string;
}

export interface CreateReviewRequest {
  bookingId: number;
  rating: number;
  comment: string;
}
